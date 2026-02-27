namespace BcMCPProxy.Runtime;

using BcMCPProxy.Models;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;

/// <summary>
/// Factory for creating MCP server options.
/// </summary>
public interface IMcpServerOptionsFactory
{
    /// <summary>
    /// Creates MCP server options configured with handlers for the specified client.
    /// </summary>
    McpServerOptions GetMcpServerOptions(ConfigOptions configOptions, McpClient mcpClient);
}
