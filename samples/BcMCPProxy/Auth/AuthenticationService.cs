namespace BcMCPProxy.Auth;

using BcMCPProxy.Logging;
using BcMCPProxy.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Linq;
using System.Threading.Tasks;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IPublicClientApplication msalClient;
    private readonly ILogger<AuthenticationService> logger;
    private readonly string[] scopes;
    private readonly ILoggerFactory loggerFactory;
    private readonly ConfigOptions configOptions;
    private readonly string cacheName;
    private readonly string clientId;
    private readonly string environmentId;

    public AuthenticationService(ILoggerFactory loggerFactory, string[] scopes, ConfigOptions configOptions, string environmentId, string cacheName, string clientId)
    {
        this.loggerFactory = loggerFactory;
        this.scopes = scopes;
        this.configOptions = configOptions;
        this.cacheName = cacheName;
        this.clientId = clientId;
        this.environmentId = environmentId;

        logger = loggerFactory.CreateLogger<AuthenticationService>();
        msalClient = GetMsalClient();
    }

    public async Task<string> AcquireTokenAsync()
    {
        try
        {
            var accounts = await msalClient.GetAccountsAsync();
            var account = accounts.FirstOrDefault();

            AuthenticationResult result = null;

            try
            {
                if (account != null)
                {
                    result = await msalClient.AcquireTokenSilent(scopes, account).ExecuteAsync();
                }
                else
                {
                    result = await msalClient.AcquireTokenSilent(scopes, PublicClientApplication.OperatingSystemAccount)
                                        .ExecuteAsync();
                }
            }
            catch (MsalUiRequiredException)
            {
                result = await msalClient.AcquireTokenInteractive(scopes).ExecuteAsync();
            }

            return result.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogError($"Authentication failed: {ex}");
            throw;
        }
    }

    private IPublicClientApplication GetMsalClient()
    {
        var cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BcMCPProxy");
        var cacheFilename = $"{this.environmentId}_{this.cacheName}.bin";

        var storageProperties =
            new StorageCreationPropertiesBuilder(cacheFilename, cachePath)
            .Build();

        var publicClientApplicationBuilder = PublicClientApplicationBuilder.Create(clientId);

        if (configOptions.EnableMsalLogging)
        {
            publicClientApplicationBuilder = publicClientApplicationBuilder.WithLogging(new IdentityLogger(this.loggerFactory));
        }

        var msalClient = publicClientApplicationBuilder.WithAuthority(AadAuthorityAudience.AzureAdMyOrg)
            .WithTenantId(configOptions.TenantId)
            .WithParentActivityOrWindow(MCPHostMainWindowHandleProvider.GetMCPHostWindow)
            .WithBroker(new BrokerOptions(BrokerOptions.OperatingSystems.Windows))
            .Build();

        var cacheHelper = MsalCacheHelper.CreateAsync(storageProperties).GetAwaiter().GetResult();
        cacheHelper.RegisterCache(msalClient.UserTokenCache);

        return msalClient;
    }
}