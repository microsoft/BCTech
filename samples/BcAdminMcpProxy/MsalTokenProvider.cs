// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Diagnostics;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace BcAdminMcpProxy;

/// <summary>
/// Acquires Entra ID tokens via MSAL with DPAPI-encrypted persistent cache.
/// Uses a single /common app so refresh tokens work across tenants (GDAP partner scenarios).
/// After initial interactive login, silently acquires tokens for any tenant the user has access to.
/// </summary>
public sealed class MsalTokenProvider
{
    // Falls back to InteractiveBrowserCredentialOptions default client ID (Azure development application).
    // It is recommended that developers register their applications and assign appropriate roles.
    // For more information, visit https://aka.ms/azsdk/identity/AppRegistrationAndRoleAssignment.
    // If not specified, users will authenticate to an Azure development application,
    // which is not recommended for production scenarios.
    private static readonly string DefaultClientId =
        new Azure.Identity.InteractiveBrowserCredentialOptions().ClientId;

    private const string CacheFileName = "bc_mcp_proxy_token_cache.bin";
    private const string CacheDirectoryName = "BcMcpProxy";
    private const string KeyChainServiceName = "BcMcpProxy";
    private const string KeyChainAccountName = "BcMcpProxyTokenCache";
    private const string LinuxKeyRingSchemaName = "com.microsoft.bcmcpproxy";
    private const string LinuxKeyRingCollection = MsalCacheHelper.LinuxKeyRingDefaultCollection;
    private const string LinuxKeyRingLabel = "BC MCP Proxy token cache";
    private static readonly KeyValuePair<string, string> LinuxKeyRingAttribute1 =
        new("Version", "1");
    private static readonly KeyValuePair<string, string> LinuxKeyRingAttribute2 =
        new("ProductGroup", "BcMcpProxy");

    private readonly ProxyConfiguration config;
    private readonly TelemetryLogger logger;
    private readonly string clientId;
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private IPublicClientApplication? app;
    private MsalCacheHelper? cacheHelper;

    // The account from the initial interactive login — used for silent acquisition across tenants
    private IAccount? cachedAccount;

    public MsalTokenProvider(ProxyConfiguration config, TelemetryLogger logger)
    {
        this.config = config;
        this.logger = logger;
        this.clientId = string.IsNullOrWhiteSpace(config.ClientId) ? DefaultClientId : config.ClientId;
    }

    /// <summary>
    /// Returns true if the MSAL cache has at least one account (i.e., user has logged in before).
    /// </summary>
    public async Task<bool> HasCachedAccountsAsync()
    {
        var msalApp = await this.GetOrCreateAppAsync();
        var accounts = await msalApp.GetAccountsAsync();
        return accounts.Any();
    }

    /// <summary>
    /// Gets an access token for the specified tenant.
    /// Uses the cached account from initial login to silently acquire tokens for other tenants (GDAP).
    /// Returns (accessToken, resolvedTenantId, homeTenantId).
    /// </summary>
    public async Task<(string AccessToken, string TenantId, string HomeTenantId)> GetTokenAsync(
        string tenantId, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var msalApp = await this.GetOrCreateAppAsync();

        // Try silent first using the cached account
        var account = this.cachedAccount ?? (await msalApp.GetAccountsAsync()).FirstOrDefault();

        if (account is not null)
        {
            try
            {
                var silentBuilder = msalApp.AcquireTokenSilent(this.config.Scopes, account);

                // Target the specific tenant — this is the key for GDAP:
                // the refresh token from the home tenant is used to get an access token
                // for the customer tenant that the user has delegated access to.
                if (tenantId != "common")
                {
                    silentBuilder = silentBuilder.WithTenantId(tenantId);
                }

                var silentResult = await silentBuilder.ExecuteAsync(ct);
                sw.Stop();

                this.cachedAccount = silentResult.Account;
                this.logger.TokenAcquired(silentResult.TenantId, "silent", sw.ElapsedMilliseconds);
                this.LogTokenDebugInfo(silentResult, "silent");

                return (silentResult.AccessToken, silentResult.TenantId,
                    silentResult.Account.HomeAccountId.TenantId);
            }
            catch (MsalUiRequiredException ex)
            {
                this.logger.Info("SilentAuthFailed", new { tenantId, reason = "UiRequired", detail = ex.Message });
            }
            catch (MsalServiceException ex)
            {
                this.logger.Info("SilentAuthFailed", new { tenantId, reason = ex.ErrorCode, detail = ex.Message });
            }
        }
        else
        {
            this.logger.Info("NoAccountInCache", new { tenantId });
        }

        // Interactive — opens system browser
        var interactiveBuilder = msalApp.AcquireTokenInteractive(this.config.Scopes)
            .WithPrompt(Prompt.SelectAccount)
            .WithUseEmbeddedWebView(false);

        if (tenantId != "common")
        {
            interactiveBuilder = interactiveBuilder.WithTenantId(tenantId);
        }

        var interactiveResult = await interactiveBuilder.ExecuteAsync(ct);
        sw.Stop();

        this.cachedAccount = interactiveResult.Account;
        this.logger.TokenAcquired(interactiveResult.TenantId, "interactive", sw.ElapsedMilliseconds);
        this.LogTokenDebugInfo(interactiveResult, "interactive");

        return (interactiveResult.AccessToken, interactiveResult.TenantId,
            interactiveResult.Account.HomeAccountId.TenantId);
    }

    /// <summary>
    /// Logs non-sensitive token metadata when debug mode is enabled.
    /// Never logs the actual access token or refresh token.
    /// </summary>
    private void LogTokenDebugInfo(AuthenticationResult result, string method)
    {
        if (!this.config.Debug)
        {
            return;
        }

        this.logger.Info("TokenDebugInfo", new
        {
            method,
            tenantId = result.TenantId,
            homeTenantId = result.Account.HomeAccountId.TenantId,
            homeAccountId = result.Account.HomeAccountId.Identifier,
            username = result.Account.Username,
            expiresOn = result.ExpiresOn.ToString("o"),
            scopes = string.Join(" ", result.Scopes),
            correlationId = result.CorrelationId,
            authenticationResultMetadata = new
            {
                tokenSource = result.AuthenticationResultMetadata.TokenSource.ToString(),
                durationTotalInMs = result.AuthenticationResultMetadata.DurationTotalInMs,
                durationInHttpInMs = result.AuthenticationResultMetadata.DurationInHttpInMs,
            },
        });
    }

    private async Task<IPublicClientApplication> GetOrCreateAppAsync()
    {
        await this.semaphore.WaitAsync();
        try
        {
            if (this.app is not null)
            {
                return this.app;
            }

            // Use /common authority so the refresh token works across tenants.
            // Per-request .WithTenantId() overrides the target tenant.
            var authority = $"{this.config.LoginAuthority.TrimEnd('/')}/common";
            this.app = PublicClientApplicationBuilder
                .Create(this.clientId)
                .WithAuthority(authority)
                .WithDefaultRedirectUri()
                .Build();

            var helper = await this.GetCacheHelperAsync();
            helper.RegisterCache(this.app.UserTokenCache);

            this.logger.Info("MsalAppCreated", new { authority, clientId = this.clientId });
            return this.app;
        }
        finally
        {
            this.semaphore.Release();
        }
    }

    private async Task<MsalCacheHelper> GetCacheHelperAsync()
    {
        if (this.cacheHelper is not null)
        {
            return this.cacheHelper;
        }

        var cacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            CacheDirectoryName);

        Directory.CreateDirectory(cacheDir);

        // DPAPI-encrypted storage on Windows; KeyChain on macOS; KeyRing on Linux
        var storageProperties = new StorageCreationPropertiesBuilder(CacheFileName, cacheDir)
            .WithMacKeyChain(KeyChainServiceName, KeyChainAccountName)
            .WithLinuxKeyring(
                LinuxKeyRingSchemaName,
                LinuxKeyRingCollection,
                LinuxKeyRingLabel,
                LinuxKeyRingAttribute1,
                LinuxKeyRingAttribute2)
            .Build();

        this.cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);

        // Verify the cache is working (DPAPI available, etc.)
        this.cacheHelper.VerifyPersistence();

        this.logger.Info("TokenCacheInitialized", new { cacheDir });
        return this.cacheHelper;
    }
}
