// Copyright (c) Microsoft Corporation. All rights reserved.

using BcAdminMcpProxy;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables("BCMCP_")
    .AddCommandLine(args)
    .Build();

var proxyConfig = new ProxyConfiguration
{
    RemoteUrl = config["McpProxy:RemoteUrl"] ?? "https://mcp.businesscentral.dynamics.com/admin/v1",
    // No default — falls back to Azure development application client ID in MsalTokenProvider.
    // Set a custom client ID to use your own app registration.
    ClientId = config["McpProxy:ClientId"] ?? string.Empty,
    Scopes = config.GetSection("McpProxy:Scopes").GetChildren().Select(c => c.Value!).ToArray() is { Length: > 0 } scopes
        ? scopes
        : ["https://api.businesscentral.dynamics.com/.default"],
    LoginAuthority = config["McpProxy:LoginAuthority"] ?? "https://login.microsoftonline.com",
    EntraTenantIds = config.GetSection("McpProxy:EntraTenantIds").GetChildren().Select(c => c.Value!).ToArray(),
};

Log($"BC Admin Center MCP Proxy starting...");
Log($"Remote: {proxyConfig.RemoteUrl}");
Log($"Auth: MSAL (ClientId: {(string.IsNullOrWhiteSpace(proxyConfig.ClientId) ? "default Azure dev app" : proxyConfig.ClientId)})");if (proxyConfig.EntraTenantIds.Length > 0)
{
    Log($"Allowed tenants: {string.Join(", ", proxyConfig.EntraTenantIds)}");
}

var tokenProvider = new MsalTokenProvider(proxyConfig);
var proxy = new McpStdioProxy(proxyConfig, tokenProvider);

await proxy.RunAsync(CancellationToken.None);

static void Log(string message)
{
    Console.Error.WriteLine($"[BcMcpProxy] {message}");
}
