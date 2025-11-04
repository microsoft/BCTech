namespace BcMCPProxy.Runtime;

using BcMCPProxy.Auth;
using BcMCPProxy.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

public class MCPServerProxy
{
    private readonly ConfigOptions configOptions;
    private readonly ILoggerFactory loggerFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IAuthenticationServiceFactory authenticationServiceFactory;
    private readonly IMcpServerOptionsFactory mcpServerOptionsFactory;

    public MCPServerProxy(
        IOptions<ConfigOptions> options,
        ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory,
        IAuthenticationServiceFactory authenticationServiceFactory,
        IMcpServerOptionsFactory mcpServerOptionsFactory)
    {
        configOptions = options.Value;
        this.loggerFactory = loggerFactory;
        this.httpClientFactory = httpClientFactory;
        this.authenticationServiceFactory = authenticationServiceFactory;
        this.mcpServerOptionsFactory = mcpServerOptionsFactory;
    }

    public async Task RunAsync()
    {
        if (this.configOptions.Debug)
        {
            System.Diagnostics.Debugger.Launch();
        }

        var transportOptions = new HttpClientTransportOptions
        {
            Name = "Test Server",
            Endpoint = new Uri(this.configOptions.Url.TrimEnd('/') + "/v2.0/" + this.configOptions.Environment + "/mcp"),
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = new Dictionary<string, string>
            {
                { "Company",  HttpUtility.UrlDecode(this.configOptions.Company) },
                { "X-Client-Application", "BcMCPProxy" }
            }
        };

        if (!string.IsNullOrEmpty(this.configOptions.ConfigurationName))
        {
            transportOptions.AdditionalHeaders.Add("ConfigurationName", HttpUtility.UrlDecode(this.configOptions.ConfigurationName));
        }

        HttpClient httpClient;

        if (string.IsNullOrEmpty(this.configOptions.CustomAuthHeader))
        {
            // Create authentication service
            var authService = this.authenticationServiceFactory.GetAuthenticationService(
                this.loggerFactory,
                [this.configOptions.TokenScope],
                "Production",
                "prodTest",
                this.configOptions.ClientId
            );

            // Use our custom authentication handler
            httpClient = new HttpClient(new AuthenticationHandler(authService)
            {
                InnerHandler = new HttpClientHandler()
            });
        }
        else
        {
            // Use the regular HTTP client with a static header for custom auth
            httpClient = this.httpClientFactory.CreateClient("McpClient");
                new AuthenticationHeaderValue("Bearer", this.configOptions.CustomAuthHeader);
        }

        var serverConfig = new HttpClientTransport(transportOptions, httpClient);

        await using var mcpClient = await McpClient.CreateAsync(serverConfig);

        var options = mcpServerOptionsFactory.GetMcpServerOptions(configOptions, mcpClient);

        await using McpServer server = McpServer.Create(
            new StdioServerTransport(configOptions.ServerName),
            options,
            loggerFactory);

        await server.RunAsync();
    }

    /// <summary>
    /// DelegatingHandler that adds a fresh authorization token to each request
    /// </summary>
    private class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthenticationService authService;

        public AuthenticationHandler(IAuthenticationService authService)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Get a fresh token for each request
            var token = await authService.AcquireTokenAsync();

            // Set the Authorization header
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Continue processing the request
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
