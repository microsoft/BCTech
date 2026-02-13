namespace BcMCPProxy.Auth;

/// <summary>
/// Service for acquiring authentication tokens.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Acquires an access token for the configured scopes.
    /// </summary>
    /// <returns>The access token.</returns>
    Task<string> AcquireTokenAsync();
}
