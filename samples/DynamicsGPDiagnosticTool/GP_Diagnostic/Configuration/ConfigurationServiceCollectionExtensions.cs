namespace Microsoft.GP.MigrationDiagnostic.Configuration;

using Microsoft.Extensions.DependencyInjection;

public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddRuntimeConfiguration(this IServiceCollection serviceCollection)
    {
        var sqlConfigurationSource = new RuntimeConfigurationSource<SqlConfiguration>(new SqlConfiguration
        {
            CommandTimeout = "90",
            ConnectionTimeout = "300",
        });
        var gpConfigurationSource = new RuntimeConfigurationSource<GpConfiguration>(new GpConfiguration());

        return serviceCollection
            .AddSingleton<IRuntimeConfigurationSource<SqlConfiguration>>(sqlConfigurationSource)
            .AddSingleton(sqlConfigurationSource.Option)
            .AddSingleton<IRuntimeConfigurationSource<GpConfiguration>>(gpConfigurationSource)
            .AddSingleton(gpConfigurationSource.Option);
    }
}
