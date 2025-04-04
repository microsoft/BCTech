namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using Microsoft.Extensions.DependencyInjection;

public delegate GpCompanyDatabase GpCompanyDatabaseFactory(string companyName);

public static class AnalysisServiceCollectionExtensions
{
    public static IServiceCollection AddAnalysisServices(this IServiceCollection services) => services
        .AddSingleton<GpDatabase>()
        .AddSingleton<ManagementReporterDatabase>()
        .AddTransient<GpCompanyDatabase>()
        .AddSingleton<GpCompanyDatabaseFactory>((services) =>  (string companyName) => ActivatorUtilities.CreateInstance<GpCompanyDatabase>(services, companyName))
        .AddSingleton<GpEngine>();
}
