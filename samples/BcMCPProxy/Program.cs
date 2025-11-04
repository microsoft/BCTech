using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BcMCPProxy.Models;
using BcMCPProxy.Runtime;
using BcMCPProxy.Auth;
using Microsoft.Extensions.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<ConfigOptions>(context.Configuration);
        services.AddSingleton<MCPServerProxy>();
        services.AddHttpClient("McpClient");
        services.AddSingleton<IAuthenticationServiceFactory, AuthenticationServiceFactory>();
        services.AddSingleton<IMcpServerOptionsFactory, McpServerOptionsFactory>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

var app = host.Services.GetRequiredService<MCPServerProxy>();
await app.RunAsync();