namespace BcMCPProxy.Runtime
{
    using BcMCPProxy.Models;
    using ModelContextProtocol.Client;
    using ModelContextProtocol.Protocol;
    using ModelContextProtocol.Server;
    using System;
    using System.Linq;
    using System.Text.Json;

    internal class McpServerOptionsFactory : IMcpServerOptionsFactory
    {
        public McpServerOptions GetMcpServerOptions(ConfigOptions configOptions, IMcpClient mcpClient)
        {
            return new McpServerOptions()
            {
                ServerInfo = new Implementation() { Name = configOptions.ServerName, Version = configOptions.ServerVersion },
                Handlers = new McpServerHandlers()
                {
                    ListToolsHandler = async (request, cancellationToken) =>
                    {
                        var tools = await mcpClient.ListToolsAsync();

                        var result = new ListToolsResult();
                        foreach (var tool in tools.Select(t => t.ProtocolTool))
                        {
                            result.Tools.Add(tool);
                        }

                        return result;
                    },
                    CallToolHandler = async (request, cancellationToken) =>
                    {
                        var arguments = request?.Params?.Arguments?
                          .Where(kv => kv.Value.ValueKind != JsonValueKind.Null)
                          .ToDictionary(kv => kv.Key, kv => this.ConvertJsonElement(kv.Value));


                        var result = await mcpClient.CallToolAsync(
                            request?.Params?.Name,
                            arguments
                        );

                        return result;
                    },
                },
            };
        }

        private object? ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long l)) return l;
                    if (element.TryGetDouble(out double d)) return d;
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.Array:
                case JsonValueKind.Object:
                    return element; // or element.Deserialize<object>() if you want a full object tree
            }
            return null;
        }
    }
}
