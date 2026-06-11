namespace BcMCPProxy.Runtime;

using BcMCPProxy.Auth;
using BcMCPProxy.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;
using System.Net.Http.Headers;
using System.Web;

/// <summary>
/// Main proxy server that bridges MCP clients with Business Central OData services.
/// </summary>
public class MCPServerProxy
{
    // Constants for authentication and configuration
    private const string DefaultEnvironmentId = "Production";
    private const string DefaultCacheName = "prodTest";
    private const string ClientApplicationHeaderName = "X-Client-Application";
    private const string ClientApplicationHeaderValue = "BcMCPProxy";
    private const string CompanyHeaderName = "Company";
    private const string ConfigurationNameHeaderName = "ConfigurationName";
    private const string McpEndpointPath = "/v2.0/{0}/mcp";
    private const string HttpClientName = "McpClient";

    private readonly ConfigOptions configOptions;
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<MCPServerProxy> logger;
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
        logger = loggerFactory.CreateLogger<MCPServerProxy>();
        this.httpClientFactory = httpClientFactory;
        this.authenticationServiceFactory = authenticationServiceFactory;
        this.mcpServerOptionsFactory = mcpServerOptionsFactory;
    }

    public async Task RunAsync()
    {
        if (configOptions.Debug)
        {
            logger.LogInformation("Debug mode enabled, launching debugger");
            System.Diagnostics.Debugger.Launch();
        }

        var endpoint = BuildEndpointUri();
        logger.LogInformation("Initializing MCP server proxy for endpoint: {Endpoint}", endpoint);

        var transportOptions = new HttpClientTransportOptions
        {
            Name = configOptions.ServerName,
            Endpoint = endpoint,
            TransportMode = HttpTransportMode.StreamableHttp,
            AdditionalHeaders = BuildAdditionalHeaders()
        };

        HttpClient httpClient;
        bool ownsHttpClient = false;

        if (string.IsNullOrEmpty(configOptions.CustomAuthHeader))
        {
            logger.LogInformation("Using MSAL authentication for environment {Environment}", configOptions.Environment);
            
            // Create authentication service
            var authService = authenticationServiceFactory.GetAuthenticationService(
                loggerFactory,
                [configOptions.TokenScope],
                DefaultEnvironmentId,
                DefaultCacheName,
                configOptions.ClientId
            );

            // Pre-authenticate to avoid blocking during MCP client initialization
            logger.LogInformation("Acquiring authentication token (this may prompt for login)");
            var authStart = System.Diagnostics.Stopwatch.StartNew();
            await authService.AcquireTokenAsync();
            authStart.Stop();
            logger.LogInformation("Authentication successful, token acquired in {ElapsedMs}ms", authStart.ElapsedMilliseconds);

            // Use our custom authentication handler
            httpClient = new HttpClient(new AuthenticationHandler(authService, loggerFactory)
            {
                InnerHandler = new HttpClientHandler()
            })
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            ownsHttpClient = true;
            logger.LogInformation("HTTP client configured with 30 second timeout");
        }
        else
        {
            logger.LogInformation("Using custom authentication header");
            
            // Use the regular HTTP client with a static header for custom auth
            httpClient = httpClientFactory.CreateClient(HttpClientName);
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", configOptions.CustomAuthHeader);
        }

        try
        {
            var serverConfig = new HttpClientTransport(transportOptions, httpClient);

            logger.LogInformation("Creating MCP client connection to {Endpoint}", endpoint);
            var mcpClientStart = System.Diagnostics.Stopwatch.StartNew();
            await using var mcpClient = await McpClient.CreateAsync(serverConfig);
            mcpClientStart.Stop();
            logger.LogInformation("MCP client connected in {ElapsedMs}ms", mcpClientStart.ElapsedMilliseconds);

            var options = mcpServerOptionsFactory.GetMcpServerOptions(configOptions, mcpClient);

            logger.LogInformation("Starting MCP server {ServerName} version {ServerVersion}", 
                configOptions.ServerName, configOptions.ServerVersion);
            
            await using McpServer server = McpServer.Create(
                new StdioServerTransport(configOptions.ServerName),
                options,
                loggerFactory);

            await server.RunAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fatal error in MCP server proxy");
            throw;
        }
        finally
        {
            // Only dispose HttpClient if we created it ourselves
            if (ownsHttpClient)
            {
                httpClient?.Dispose();
                logger.LogDebug("Disposed owned HTTP client");
            }
        }
    }

    private Uri BuildEndpointUri()
    {
        var baseUrl = configOptions.Url.TrimEnd('/');
        var path = string.Format(McpEndpointPath, configOptions.Environment);
        return new Uri(baseUrl + path);
    }

    private Dictionary<string, string> BuildAdditionalHeaders()
    {
        var headers = new Dictionary<string, string>
        {
            { CompanyHeaderName, HttpUtility.UrlDecode(configOptions.Company) },
            { ClientApplicationHeaderName, ClientApplicationHeaderValue }
        };

        if (!string.IsNullOrEmpty(configOptions.ConfigurationName))
        {
            headers.Add(ConfigurationNameHeaderName, HttpUtility.UrlDecode(configOptions.ConfigurationName));
        }

        return headers;
    }

    /// <summary>
    /// DelegatingHandler that adds a fresh authorization token to each request.
    /// </summary>
    private class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthenticationService authService;
        private readonly ILogger<AuthenticationHandler> logger;

        public AuthenticationHandler(IAuthenticationService authService, ILoggerFactory loggerFactory)
        {
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            logger = loggerFactory.CreateLogger<AuthenticationHandler>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Get a fresh token for each request
                var token = await authService.AcquireTokenAsync();

                // Set the Authorization header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                logger.LogDebug("Added authentication token to request for {RequestUri}", request.RequestUri);

                // Continue processing the request
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to acquire authentication token for request to {RequestUri}", request.RequestUri);
                throw;
            }
        }
    }
}
