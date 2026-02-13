namespace BcMCPProxy.Models;

/// <summary>
/// Configuration options for the BcMCPProxy application.
/// </summary>
public class ConfigOptions
{
    // Default configuration constants
    public const string DefaultServerName = "BcMCPProxy";
    public const string DefaultServerVersion = "2.0.0";
    public const string DefaultTokenScope = "https://api.businesscentral.dynamics.com/.default";
    public const string DefaultUrl = "https://api.businesscentral.dynamics.com";
    public const string DefaultEnvironment = "Production";

    public string ServerName { get; set; } = DefaultServerName;
    public string ServerVersion { get; set; } = DefaultServerVersion;
    public required string TenantId { get; set; }
    public required string ClientId { get; set; }
    public string TokenScope { get; set; } = DefaultTokenScope;
    public string Url { get; set; } = DefaultUrl;
    public string Environment { get; set; } = DefaultEnvironment;

    public required string Company { get; set; }
    public string? ConfigurationName { get; set; }
    public string? CustomAuthHeader { get; set; }

    public bool Debug { get; set; }
    public bool EnableHttpLogging { get; set; }
    public bool EnableMsalLogging { get; set; }
}
