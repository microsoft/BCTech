namespace BcMCPProxy.Auth;

using BcMCPProxy.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Factory implementation for creating authentication service instances.
/// </summary>
public class AuthenticationServiceFactory : IAuthenticationServiceFactory
{
    private readonly ConfigOptions configOptions;

    public AuthenticationServiceFactory(IOptions<ConfigOptions> options)
    {
        configOptions = options.Value;
    }

    public IAuthenticationService GetAuthenticationService(ILoggerFactory loggerFactory, string[] scopes, string environmentId, string cacheName, string clientId)
    {
        return new AuthenticationService(loggerFactory, scopes, configOptions, environmentId, cacheName, clientId);
    }
}
