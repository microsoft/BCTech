// Copyright (c) Microsoft Corporation. All rights reserved.

namespace BcAdminMcpProxy;

/// <summary>
/// Configuration for the MCP proxy, loaded from appsettings.json / env vars / CLI args.
/// </summary>
public sealed class ProxyConfiguration
{
    public required string RemoteUrl { get; set; }
    public required string ClientId { get; set; }
    public required string[] Scopes { get; set; }
    public required string LoginAuthority { get; set; }

    /// <summary>
    /// Optional allowlist of Entra tenant IDs. If non-empty, only these tenants can be used.
    /// Empty array means any tenant is allowed.
    /// </summary>
    public string[] EntraTenantIds { get; set; } = [];

    /// <summary>
    /// When true, logs additional diagnostic info (token claims, HTTP headers, etc.)
    /// to the telemetry log file. Never logs secrets or access tokens.
    /// </summary>
    public bool Debug { get; set; }
}
