namespace BcMCPProxy.Auth;

using BcMCPProxy.Logging;
using BcMCPProxy.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Runtime.InteropServices;

/// <summary>
/// Service for acquiring authentication tokens using MSAL with cross-platform broker support.
/// </summary>
internal class AuthenticationService : IAuthenticationService
{
    private const string CacheDirectoryName = "BcMCPProxy";

    private readonly ILogger<AuthenticationService> logger;
    private readonly string[] scopes;
    private readonly ILoggerFactory loggerFactory;
    private readonly ConfigOptions configOptions;
    private readonly string cacheName;
    private readonly string clientId;
    private readonly string environmentId;
    
    private IPublicClientApplication? msalClient;
    private MsalCacheHelper? cacheHelper;
    private readonly SemaphoreSlim initializationLock = new(1, 1);

    public AuthenticationService(ILoggerFactory loggerFactory, string[] scopes, ConfigOptions configOptions, string environmentId, string cacheName, string clientId)
    {
        this.loggerFactory = loggerFactory;
        this.scopes = scopes;
        this.configOptions = configOptions;
        this.cacheName = cacheName;
        this.clientId = clientId;
        this.environmentId = environmentId;

        logger = loggerFactory.CreateLogger<AuthenticationService>();
    }

    public async Task<string> AcquireTokenAsync()
    {
        // Lazy initialization of MSAL client to avoid sync-over-async in constructor
        await EnsureInitializedAsync();

        try
        {
            var accounts = await msalClient!.GetAccountsAsync();
            var account = accounts.FirstOrDefault();

            AuthenticationResult? result = null;

            try
            {
                if (account != null)
                {
                    logger.LogDebug("Attempting silent token acquisition for account {AccountId}", account.Username);
                    result = await msalClient.AcquireTokenSilent(scopes, account).ExecuteAsync();
                }
                else
                {
                    logger.LogDebug("Attempting silent token acquisition using operating system account");
                    result = await msalClient.AcquireTokenSilent(scopes, PublicClientApplication.OperatingSystemAccount)
                                        .ExecuteAsync();
                }
            }
            catch (MsalUiRequiredException ex)
            {
                logger.LogInformation("Silent token acquisition failed, prompting for interactive login. Reason: {Reason}", ex.Message);
                
                // Use Device Code Flow (no redirect URI required, works with existing Azure app registration)
                var platform = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : 
                               RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" : "Linux";
                logger.LogInformation("Using Device Code Flow for authentication on {Platform}", platform);
                
                result = await msalClient.AcquireTokenWithDeviceCode(scopes, async deviceCodeResult =>
                    {
                        // Platform-specific device code handling
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            // Windows: Copy to clipboard and open browser
                            try
                            {
                                var psi = new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = "cmd.exe",
                                    Arguments = $"/c echo {deviceCodeResult.UserCode} | clip",
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                };
                                using var clipProcess = System.Diagnostics.Process.Start(psi);
                                clipProcess?.WaitForExit();
                                logger.LogInformation("Device code {Code} copied to clipboard", deviceCodeResult.UserCode);
                            }
                            catch (Exception clipEx)
                            {
                                logger.LogWarning("Could not copy to clipboard: {Error}", clipEx.Message);
                            }

                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = deviceCodeResult.VerificationUrl,
                                    UseShellExecute = true
                                });
                                logger.LogInformation("Browser opened to {Url}", deviceCodeResult.VerificationUrl);
                            }
                            catch (Exception openEx)
                            {
                                logger.LogWarning("Could not open browser: {Error}", openEx.Message);
                            }

                            logger.LogInformation("AUTHENTICATION REQUIRED: Go to {Url} and enter code: {Code}", 
                                deviceCodeResult.VerificationUrl, deviceCodeResult.UserCode);
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            // macOS: Copy to clipboard and open browser
                            try
                            {
                                using var clipboardProcess = new System.Diagnostics.Process
                                {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo
                                    {
                                        FileName = "pbcopy",
                                        RedirectStandardInput = true,
                                        UseShellExecute = false,
                                        CreateNoWindow = true
                                    }
                                };
                                clipboardProcess.Start();
                                await clipboardProcess.StandardInput.WriteAsync(deviceCodeResult.UserCode);
                                clipboardProcess.StandardInput.Close();
                                await clipboardProcess.WaitForExitAsync();
                                logger.LogInformation("Device code {Code} copied to clipboard", deviceCodeResult.UserCode);
                            }
                            catch (Exception clipEx)
                            {
                                logger.LogWarning("Could not copy to clipboard: {Error}", clipEx.Message);
                            }

                            try
                            {
                                using var browserProcess = System.Diagnostics.Process.Start("open", deviceCodeResult.VerificationUrl);
                            }
                            catch (Exception openEx)
                            {
                                logger.LogWarning("Could not open browser automatically: {Error}", openEx.Message);
                            }

                            try
                            {
                                using var notificationProcess = new System.Diagnostics.Process
                                {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo
                                    {
                                        FileName = "osascript",
                                        Arguments = $"-e 'display notification \"Code {deviceCodeResult.UserCode} copied to clipboard. Just paste it in the browser!\" with title \"BC MCP - Authentication Required\"'",
                                        UseShellExecute = false,
                                        CreateNoWindow = true
                                    }
                                };
                                notificationProcess.Start();
                                await notificationProcess.WaitForExitAsync();
                            }
                            catch (Exception notifEx)
                            {
                                logger.LogWarning("Could not show notification: {Error}", notifEx.Message);
                            }

                            logger.LogInformation("Device Code {Code} - Browser opened and code copied to clipboard", deviceCodeResult.UserCode);
                        }
                        else
                        {
                            // Linux: Just log the information
                            logger.LogInformation("AUTHENTICATION REQUIRED: Go to {Url} and enter code: {Code}", 
                                deviceCodeResult.VerificationUrl, deviceCodeResult.UserCode);
                        }
                    }).ExecuteAsync();
            }

            logger.LogInformation("Successfully acquired access token for environment {EnvironmentId}", environmentId);
            return result.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Authentication failed for environment {EnvironmentId} with client {ClientId}", environmentId, clientId);
            throw;
        }
    }

    private async Task EnsureInitializedAsync()
    {
        if (msalClient != null)
            return;

        await initializationLock.WaitAsync();
        try
        {
            if (msalClient != null)
                return;

            await InitializeMsalClientAsync();
        }
        finally
        {
            initializationLock.Release();
        }
    }

    private async Task InitializeMsalClientAsync()
    {
        var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CacheDirectoryName);
        var cacheFilename = $"{environmentId}_{cacheName}.bin";

        var storagePropertiesBuilder = new StorageCreationPropertiesBuilder(cacheFilename, cachePath);

        // Configure platform-specific storage
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS requires keychain service and account names
            storagePropertiesBuilder = storagePropertiesBuilder
                .WithMacKeyChain(
                    serviceName: $"BcMCPProxy_{environmentId}",
                    accountName: cacheName);
            logger.LogDebug("Configured macOS keychain storage for service: BcMCPProxy_{EnvironmentId}", environmentId);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Linux uses libsecret
            storagePropertiesBuilder = storagePropertiesBuilder
                .WithLinuxKeyring(
                    schemaName: "msal.cache",
                    collection: "default",
                    secretLabel: $"BcMCPProxy token cache",
                    attribute1: new KeyValuePair<string, string>("Version", "1"),
                    attribute2: new KeyValuePair<string, string>("Environment", environmentId));
            logger.LogDebug("Configured Linux keyring storage");
        }

        var storageProperties = storagePropertiesBuilder.Build();

        var publicClientApplicationBuilder = PublicClientApplicationBuilder.Create(clientId);

        if (configOptions.EnableMsalLogging)
        {
            publicClientApplicationBuilder = publicClientApplicationBuilder.WithLogging(new IdentityLogger(loggerFactory));
        }

        publicClientApplicationBuilder = publicClientApplicationBuilder
            .WithAuthority(AadAuthorityAudience.AzureAdMyOrg)
            .WithTenantId(configOptions.TenantId);

        // Device Code Flow is used for all platforms (no broker)
        // This provides a consistent authentication experience across Windows, macOS, and Linux
        logger.LogDebug("Using Device Code Flow authentication (no broker)");

        msalClient = publicClientApplicationBuilder.Build();

        // Async cache helper initialization
        cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
        cacheHelper.RegisterCache(msalClient.UserTokenCache);

        logger.LogInformation("MSAL client initialized for environment {EnvironmentId}", environmentId);
    }
}