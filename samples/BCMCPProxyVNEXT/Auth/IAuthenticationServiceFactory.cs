namespace BcMCPProxy.Auth;

using Microsoft.Extensions.Logging;

/// <summary>
/// Factory for creating authentication service instances.
/// </summary>
public interface IAuthenticationServiceFactory
{
    /// <summary>
    /// Creates an authentication service with the specified configuration.
    /// </summary>
    IAuthenticationService GetAuthenticationService(ILoggerFactory loggerFactory, string[] scopes, string environmentId, string cacheName, string clientId);
}
