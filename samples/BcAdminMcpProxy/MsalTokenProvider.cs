// Copyright (c) Microsoft Corporation. All rights reserved.

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace BcAdminMcpProxy;

/// <summary>
/// Acquires Entra ID tokens via MSAL with DPAPI-encrypted persistent cache.
/// Supports multiple tenants — each tenant gets its own authority but shares the cache.
/// First call triggers interactive browser login; subsequent calls use silent auth.
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
    private readonly string clientId;
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private readonly Dictionary<string, IPublicClientApplication> apps = new(StringComparer.OrdinalIgnoreCase);
    private MsalCacheHelper? cacheHelper;

    public MsalTokenProvider(ProxyConfiguration config)
    {
        this.config = config;
        this.clientId = string.IsNullOrWhiteSpace(config.ClientId) ? DefaultClientId : config.ClientId;
    }

    /// <summary>
    /// Gets an access token for the specified Entra tenant.
    /// Returns (accessToken, resolvedTenantId).
    /// </summary>
    public async Task<(string AccessToken, string TenantId)> GetTokenAsync(string tenantId, CancellationToken ct = default)
    {
        var app = await this.GetOrCreateAppAsync(tenantId);

        // Try silent first (from cache)
        var accounts = await app.GetAccountsAsync();
        var account = accounts.FirstOrDefault();

        if (account is not null)
        {
            try
            {
                var silentResult = await app.AcquireTokenSilent(this.config.Scopes, account)
                    .WithForceRefresh(false)
                    .ExecuteAsync(ct);
                Log($"Token acquired silently for tenant {silentResult.TenantId}");
                return (silentResult.AccessToken, silentResult.TenantId);
            }
            catch (MsalUiRequiredException)
            {
                Log($"Silent auth failed for tenant {tenantId}, launching browser.");
            }
        }

        // Interactive — opens system browser
        var interactiveResult = await app.AcquireTokenInteractive(this.config.Scopes)
            .WithPrompt(Prompt.SelectAccount)
            .WithUseEmbeddedWebView(false)
            .ExecuteAsync(ct);

        Log($"Token acquired interactively for tenant {interactiveResult.TenantId}");
        return (interactiveResult.AccessToken, interactiveResult.TenantId);
    }

    private async Task<IPublicClientApplication> GetOrCreateAppAsync(string tenantId)
    {
        await this.semaphore.WaitAsync();
        try
        {
            if (this.apps.TryGetValue(tenantId, out var existing))
            {
                return existing;
            }

            // Build app with tenant-specific authority so all token requests target the right tenant
            var authority = $"{this.config.LoginAuthority.TrimEnd('/')}/{tenantId}";
            var app = PublicClientApplicationBuilder
                .Create(this.clientId)
                .WithAuthority(authority)
                .WithDefaultRedirectUri()
                .Build();

            var helper = await this.GetCacheHelperAsync();
            helper.RegisterCache(app.UserTokenCache);

            this.apps[tenantId] = app;
            Log($"Created MSAL app for tenant {tenantId} (clientId: {this.clientId[..8]}...)");
            return app;
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

        Log($"Token cache initialized at {cacheDir}");
        return this.cacheHelper;
    }

    private static void Log(string message)
    {
        Console.Error.WriteLine($"[BcMcpProxy:Auth] {message}");
    }
}
