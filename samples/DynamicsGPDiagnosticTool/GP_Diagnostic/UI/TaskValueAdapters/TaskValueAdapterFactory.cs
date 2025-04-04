namespace Microsoft.GP.MigrationDiagnostic.UI.TaskValueAdapters;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.GP.MigrationDiagnostic.Analysis;

public delegate ITaskValueAdapter? TaskValueViewResolver(IDiagnosticTask task);

/// <summary>
/// DI extensions for the <see cref="TaskValueViewFactory"/>.
/// </summary>
public static class TaskAdapterFactoryServiceCollectionExtensions
{
    public static IServiceCollection AddTaskValueAdapterFactory(this IServiceCollection serviceCollection) => serviceCollection
        .AddSingleton<TaskValueViewResolver>(services => task => GetHandler(task, services));

    private static ITaskValueAdapter? GetHandler(IDiagnosticTask task, IServiceProvider serviceProvider) => serviceProvider
        .GetRequiredService<IEnumerable<ITaskValueAdapter>>()
        .FirstOrDefault(x => x.CanHandleTask(task));
}
