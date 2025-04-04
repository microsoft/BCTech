namespace Microsoft.GP.MigrationDiagnostic.TaskProcessing;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

public delegate TaskProcessor TaskProcessorFactory();

public static class TaskProcessorFactoryServiceCollectionExtensions
{
    public static IServiceCollection AddTaskProcessorFactory(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<TaskProcessorFactory>(
            (IServiceProvider services) =>
            {
                return () => new TaskProcessor(services.GetRequiredService<ILogger<TaskProcessor>>());
            });
    }
}
