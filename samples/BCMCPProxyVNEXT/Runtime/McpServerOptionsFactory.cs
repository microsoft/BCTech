namespace BcMCPProxy.Runtime;

using BcMCPProxy.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Text.Json;

/// <summary>
/// Factory implementation for creating MCP server options with configured handlers.
/// </summary>
internal class McpServerOptionsFactory : IMcpServerOptionsFactory
{
    private readonly ILogger<McpServerOptionsFactory> logger;

    public McpServerOptionsFactory(ILogger<McpServerOptionsFactory> logger)
    {
        this.logger = logger;
    }

    public McpServerOptions GetMcpServerOptions(ConfigOptions configOptions, McpClient mcpClient)
    {
        return new McpServerOptions()
        {
            ServerInfo = new Implementation() { Name = configOptions.ServerName, Version = configOptions.ServerVersion },
            Handlers = new McpServerHandlers()
            {
                ListToolsHandler = async (request, cancellationToken) =>
                {
                    logger.LogInformation("ListToolsHandler: Starting tools list request");
                    var sw = System.Diagnostics.Stopwatch.StartNew();
                    try
                    {
                        var tools = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);
                        sw.Stop();
                        logger.LogInformation("ListToolsHandler: Received {Count} tools from Business Central in {ElapsedMs}ms", tools.Count(), sw.ElapsedMilliseconds);

                        var result = new ListToolsResult();
                        // Add protocol tools to result
                        foreach (var tool in tools.Select(t => t.ProtocolTool))
                        {
                            result.Tools.Add(tool);
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        logger.LogError(ex, "ListToolsHandler: Failed after {ElapsedMs}ms", sw.ElapsedMilliseconds);
                        throw;
                    }
                },
                CallToolHandler = async (request, cancellationToken) =>
                {
                    var arguments = request?.Params?.Arguments?
                      .Where(kv => kv.Value.ValueKind != JsonValueKind.Null)
                      .ToDictionary(kv => kv.Key, kv => ConvertJsonElement(kv.Value));

                    var toolName = request?.Params?.Name ?? throw new ArgumentException("Tool name cannot be null", nameof(request));

                    var result = await mcpClient.CallToolAsync(
                        toolName,
                        arguments,
                        cancellationToken: cancellationToken
                    );

                    return result;
                },
            },
        };
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out long l) ? l : element.GetDouble(),
            JsonValueKind.True or JsonValueKind.False => element.GetBoolean(),
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.Array or JsonValueKind.Object => element,
            _ => null
        };
    }
}
