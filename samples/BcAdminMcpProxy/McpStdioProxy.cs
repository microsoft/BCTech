// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;

namespace BcAdminMcpProxy;

/// <summary>
/// A thin MCP proxy that reads JSON-RPC from stdin, injects Entra ID auth,
/// and forwards to the remote BC Admin Center MCP over HTTP.
/// </summary>
public sealed class McpStdioProxy
{
    private const string TenantIdParam = "tenant_id";
    private const string AuthenticateToolName = "authenticate";
    private const string AuthStatusToolName = "auth_status";
    private const string ServerName = "bc-admin-center-mcp-proxy";
    private const string ServerVersion = "1.0.0";

    private readonly ProxyConfiguration config;
    private readonly MsalTokenProvider tokenProvider;
    private readonly TelemetryLogger logger;
    private readonly HttpClient httpClient;

    // Cached remote tools (populated after first successful tools/list with auth)
    private JsonArray? cachedRemoteTools;

    public McpStdioProxy(ProxyConfiguration config, MsalTokenProvider tokenProvider, TelemetryLogger logger)
    {
        this.config = config;
        this.tokenProvider = tokenProvider;
        this.logger = logger;
        this.httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
    }

    public async Task RunAsync(CancellationToken ct)
    {
        using var stdin = Console.OpenStandardInput();
        using var reader = new StreamReader(stdin, Encoding.UTF8);

        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null)
            {
                break; // EOF
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                var message = JsonNode.Parse(line);
                if (message is null)
                {
                    continue;
                }

                var method = message["method"]?.GetValue<string>();
                var id = message["id"];

                this.logger.Info("MessageReceived", new { method = method ?? "(notification)", id = id?.ToString() });
                JsonNode? response = method switch
                {
                    "initialize" => this.HandleInitialize(message),
                    "notifications/initialized" => null, // notification, no response
                    "tools/list" => await this.HandleToolsList(message, ct),
                    "tools/call" => await this.HandleToolsCall(message, ct),
                    "ping" => CreateResult(id, new JsonObject()),
                    _ => CreateError(id, -32603, $"Unknown method: {method}"),
                };

                if (response is not null)
                {
                    WriteResponse(response);
                }
            }
            catch (Exception ex)
            {
                this.logger.Error("MessageProcessingError", ex);
                try
                {
                    var parsed = JsonNode.Parse(line);
                    var id = parsed?["id"];
                    if (id is not null)
                    {
                        WriteResponse(CreateError(id, -32603, $"Internal proxy error: {ex.Message}"));
                    }
                }
                catch { /* unable to respond */ }
            }
        }
    }

    private JsonNode HandleInitialize(JsonNode request)
    {
        var id = request["id"];
        var result = new JsonObject
        {
            ["protocolVersion"] = "2025-03-26",
            ["capabilities"] = new JsonObject
            {
                ["tools"] = new JsonObject
                {
                    ["listChanged"] = true,
                },
            },
            ["serverInfo"] = new JsonObject
            {
                ["name"] = ServerName,
                ["version"] = ServerVersion,
            },
        };
        return CreateResult(id, result);
    }

    /// <summary>
    /// Returns tools list. Before authentication, only the authenticate/auth_status tools are returned.
    /// After authentication, lazily fetches remote tools if not yet cached, then returns all tools.
    /// </summary>
    private async Task<JsonNode> HandleToolsList(JsonNode request, CancellationToken ct)
    {
        var id = request["id"];
        var tools = new JsonArray
        {
            CreateAuthenticateTool(this.config),
            CreateAuthStatusTool(),
        };

        // If we have cached accounts in MSAL, show remote tools
        var hasCachedAccounts = await this.tokenProvider.HasCachedAccountsAsync();
        if (hasCachedAccounts && this.cachedRemoteTools is null)
        {
            try
            {
                await this.RefreshRemoteToolsAsync(ct);
            }
            catch (Exception ex)
            {
                this.logger.Error("RemoteToolDiscoveryFailed", ex);
            }
        }

        if (this.cachedRemoteTools is not null)
        {
            foreach (var tool in this.cachedRemoteTools)
            {
                if (tool is not null)
                {
                    tools.Add(tool.DeepClone());
                }
            }
        }

        var result = new JsonObject { ["tools"] = tools };
        return CreateResult(id, result);
    }

    private async Task<JsonNode> HandleToolsCall(JsonNode request, CancellationToken ct)
    {
        var id = request["id"];
        var toolName = request["params"]?["name"]?.GetValue<string>();
        var arguments = request["params"]?["arguments"]?.AsObject();

        // The Authenticate and AuthStatus tools are always available
        if (toolName == AuthenticateToolName)
        {
            return await this.HandleAuthenticate(id, arguments, ct);
        }

        if (toolName == AuthStatusToolName)
        {
            return await this.HandleAuthStatus(id);
        }

        // Extract tenant_id from arguments — required for all remote tool calls
        string? tenantId = null;
        if (arguments is not null && arguments.ContainsKey(TenantIdParam))
        {
            tenantId = arguments[TenantIdParam]?.GetValue<string>();
            arguments.Remove(TenantIdParam);
        }

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return CreateToolResult(id, isError: true,
                "The 'tenant_id' parameter is required. Provide the Entra tenant ID (GUID) for the target tenant.");
        }

        // Validate against allowlist
        if (this.config.EntraTenantIds.Length > 0 &&
            !this.config.EntraTenantIds.Contains(tenantId, StringComparer.OrdinalIgnoreCase))
        {
            var allowed = string.Join(", ", this.config.EntraTenantIds);
            return CreateToolResult(id, isError: true,
                $"Tenant '{tenantId}' is not in the allowed tenant list. Allowed tenants: {allowed}");
        }

        return await this.ForwardToRemote(request, tenantId, ct)
            ?? CreateError(id, -32603, "No response from remote MCP server.");
    }

    private async Task<JsonNode> HandleAuthStatus(JsonNode? id)
    {
        var hasCachedAccounts = await this.tokenProvider.HasCachedAccountsAsync();
        if (hasCachedAccounts)
        {
            var toolCount = this.cachedRemoteTools?.Count ?? 0;
            return CreateToolResult(id, isError: false,
                $"Authenticated (cached credentials available). {toolCount} remote tools loaded.");
        }

        return CreateToolResult(id, isError: false,
            "Not authenticated. Call the 'authenticate' tool to sign in.");
    }

    private async Task<JsonNode> HandleAuthenticate(JsonNode? id, JsonObject? arguments, CancellationToken ct)
    {
        var tenantId = arguments?[TenantIdParam]?.GetValue<string>();

        // Validate against allowlist (only if tenant was explicitly provided)
        if (!string.IsNullOrWhiteSpace(tenantId) &&
            this.config.EntraTenantIds.Length > 0 &&
            !this.config.EntraTenantIds.Contains(tenantId, StringComparer.OrdinalIgnoreCase))
        {
            var allowed = string.Join(", ", this.config.EntraTenantIds);
            return CreateToolResult(id, isError: true,
                $"Tenant '{tenantId}' is not in the allowed tenant list. Allowed tenants: {allowed}");
        }

        try
        {
            var targetTenantId = string.IsNullOrWhiteSpace(tenantId) ? "common" : tenantId;
            this.logger.Info("Authenticating", new { tenantId = targetTenantId });
            var (token, resolvedTenantId, resolvedHomeTenantId) = await this.tokenProvider.GetTokenAsync(targetTenantId, ct);

            // Validate resolved tenant against allowlist
            if (this.config.EntraTenantIds.Length > 0 &&
                !this.config.EntraTenantIds.Contains(resolvedTenantId, StringComparer.OrdinalIgnoreCase))
            {
                var allowed = string.Join(", ", this.config.EntraTenantIds);
                return CreateToolResult(id, isError: true,
                    $"Authenticated tenant '{resolvedTenantId}' is not in the allowed tenant list. Allowed tenants: {allowed}");
            }

            this.logger.Info("Authenticated", new { tenantId = resolvedTenantId, homeTenantId = resolvedHomeTenantId });

            // Fetch remote tools so they appear in subsequent tools/list calls
            try
            {
                await this.RefreshRemoteToolsAsync(resolvedTenantId, ct);
            }
            catch (Exception ex)
            {
                this.logger.Error("RemoteToolDiscoveryFailed", ex);
            }

            // Notify client that the tool list has changed
            WriteResponse(new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["method"] = "notifications/tools/list_changed",
            });

            var toolCount = this.cachedRemoteTools?.Count ?? 0;
            return CreateToolResult(id, isError: false,
                $"Authenticated successfully for tenant {resolvedTenantId} (home: {resolvedHomeTenantId}). {toolCount} remote tools are now available.");
        }
        catch (Exception ex)
        {
            this.logger.Error("AuthenticationFailed", ex);
            return CreateToolResult(id, isError: true,
                $"Authentication failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Fetches tools from the remote MCP server and caches them (with tenant_id injected).
    /// Tool definitions are tenant-agnostic — any valid tenant works for discovery.
    /// </summary>
    private async Task RefreshRemoteToolsAsync(CancellationToken ct)
    {
        // Use any cached account to get a token for tool discovery
        var tenantId = "common";
        await this.RefreshRemoteToolsAsync(tenantId, ct);
    }

    private async Task RefreshRemoteToolsAsync(string tenantId, CancellationToken ct)
    {
        var listRequest = new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = "proxy-tool-discovery",
            ["method"] = "tools/list",
            ["params"] = new JsonObject(),
        };

        var response = await this.ForwardToRemote(listRequest, tenantId, ct);
        this.logger.Info("RemoteToolsListResponse", new { hasResult = response?["result"] is not null });
        var tools = response?["result"]?["tools"]?.AsArray();

        if (tools is not null)
        {
            foreach (var tool in tools)
            {
                InjectEntraTenantIdParam(tool);
            }

            this.cachedRemoteTools = tools;
            this.logger.Info("RemoteToolsDiscovered", new { count = tools.Count });
        }
    }

    private async Task<JsonNode?> ForwardToRemote(JsonNode request, string tenantId, CancellationToken ct)
    {
        string? token = null;
        try
        {
            token = (await this.tokenProvider.GetTokenAsync(tenantId, ct)).AccessToken;
        }
        catch (Exception ex)
        {
            this.logger.Error("TokenAcquisitionFailed", ex, new { tenantId });
            var id = request["id"];
            if (id is not null)
            {
                return CreateError(id, -32603,
                    $"Token acquisition failed for tenant {tenantId}. Run the 'authenticate' tool. Error: {ex.Message}");
            }

            return null;
        }

        var body = request.ToJsonString();
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, this.config.RemoteUrl)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        httpRequest.Headers.TryAddWithoutValidation("TenantId", tenantId);

        this.logger.Info("RemoteRequest", new { url = this.config.RemoteUrl, tenantId });

        using var httpResponse = await this.httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, ct);
        var contentType = httpResponse.Content.Headers.ContentType?.MediaType;

        if (contentType == "text/event-stream")
        {
            return await ReadSseResponse(httpResponse, ct);
        }

        var responseBody = await httpResponse.Content.ReadAsStringAsync(ct);

        if (!httpResponse.IsSuccessStatusCode)
        {
            this.logger.Warning("RemoteErrorResponse", new { statusCode = (int)httpResponse.StatusCode, body = responseBody });
            var id = request["id"];
            if (id is not null)
            {
                return CreateError(id, -32603,
                    $"Remote MCP server returned HTTP {(int)httpResponse.StatusCode}: {responseBody}");
            }

            return null;
        }

        return JsonNode.Parse(responseBody);
    }

    private static async Task<JsonNode?> ReadSseResponse(HttpResponseMessage httpResponse, CancellationToken ct)
    {
        using var stream = await httpResponse.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        JsonNode? lastMessage = null;

        while (!ct.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(ct);
            if (line is null)
            {
                break;
            }

            if (line.StartsWith("data: ", StringComparison.Ordinal))
            {
                var data = line["data: ".Length..];
                if (!string.IsNullOrWhiteSpace(data))
                {
                    lastMessage = JsonNode.Parse(data);
                }
            }
        }

        return lastMessage;
    }

    private static void InjectEntraTenantIdParam(JsonNode? tool)
    {
        if (tool is null)
        {
            return;
        }

        var inputSchema = tool["inputSchema"]?.AsObject();
        if (inputSchema is null)
        {
            inputSchema = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject(),
            };
            tool["inputSchema"] = inputSchema;
        }

        var properties = inputSchema["properties"]?.AsObject();
        if (properties is null)
        {
            properties = new JsonObject();
            inputSchema["properties"] = properties;
        }

        // Add entraTenantId as the first property
        var newProperties = new JsonObject
        {
            [TenantIdParam] = new JsonObject
            {
                ["type"] = "string",
                ["description"] = "Optional. Override the Entra tenant ID for this call (switches tenant context).",
            },
        };

        foreach (var kvp in properties.ToArray())
        {
            properties.Remove(kvp.Key);
            newProperties[kvp.Key] = kvp.Value?.DeepClone();
        }

        inputSchema["properties"] = newProperties;
    }

    private static JsonObject CreateAuthenticateTool(ProxyConfiguration config)
    {
        var description = "Authenticate to the BC Admin Center MCP service. You MUST call this tool before using any other tools. Opens a browser window for interactive login on first use; subsequent calls use cached credentials. Optionally provide an Entra tenant ID to scope the login to a specific tenant.";

        if (config.EntraTenantIds.Length > 0)
        {
            description += $" Allowed tenant IDs: {string.Join(", ", config.EntraTenantIds)}";
        }

        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                [TenantIdParam] = new JsonObject
                {
                    ["type"] = "string",
                    ["description"] = "Optional. Entra tenant ID (Azure AD tenant GUID) to scope the login. If omitted, you can sign in with any account.",
                },
            },
        };

        // If exactly one tenant is allowed, set it as enum so the LLM auto-fills it
        if (config.EntraTenantIds.Length == 1)
        {
            schema["properties"]![TenantIdParam]!["enum"] = new JsonArray { config.EntraTenantIds[0] };
        }
        else if (config.EntraTenantIds.Length > 1)
        {
            var enumValues = new JsonArray();
            foreach (var t in config.EntraTenantIds)
            {
                enumValues.Add(t);
            }

            schema["properties"]![TenantIdParam]!["enum"] = enumValues;
        }

        return new JsonObject
        {
            ["name"] = AuthenticateToolName,
            ["description"] = description,
            ["inputSchema"] = schema,
        };
    }

    private static JsonObject CreateAuthStatusTool()
    {
        return new JsonObject
        {
            ["name"] = AuthStatusToolName,
            ["description"] = "Check whether you are currently authenticated to the BC Admin Center MCP service, and which tenant is active.",
            ["inputSchema"] = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject(),
            },
        };
    }

    private static JsonNode CreateResult(JsonNode? id, JsonNode result)
    {
        return new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id?.DeepClone(),
            ["result"] = result,
        };
    }

    private static JsonNode CreateToolResult(JsonNode? id, bool isError, string text)
    {
        return CreateResult(id, new JsonObject
        {
            ["content"] = new JsonArray
            {
                new JsonObject
                {
                    ["type"] = "text",
                    ["text"] = text,
                },
            },
            ["isError"] = isError,
        });
    }

    private static JsonNode CreateError(JsonNode? id, int code, string message)
    {
        return new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id?.DeepClone(),
            ["error"] = new JsonObject
            {
                ["code"] = code,
                ["message"] = message,
            },
        };
    }

    private static void WriteResponse(JsonNode response)
    {
        var json = response.ToJsonString();
        Console.Out.WriteLine(json);
        Console.Out.Flush();
    }
}
