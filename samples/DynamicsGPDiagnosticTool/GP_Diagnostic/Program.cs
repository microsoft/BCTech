namespace Microsoft.GP.MigrationDiagnostic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.GP.MigrationDiagnostic.Analysis;
using Microsoft.GP.MigrationDiagnostic.Configuration;
using Microsoft.GP.MigrationDiagnostic.TaskProcessing;
using Microsoft.GP.MigrationDiagnostic.UI;
using Microsoft.GP.MigrationDiagnostic.UI.TaskValueAdapters;
using Microsoft.GP.MigrationDiagnostic.UI.Views;
using Microsoft.GP.MigrationDiagnostic.UI.Views.TaskGroup;
using System;
using System.Windows.Forms;

public static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(builder =>
            {
            })
            .ConfigureLogging(logging => 
            {
                logging
                    .ClearProviders()
                    .AddDebug()
                    .AddEventLog();
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddScoped<MainWindow>()
                    .AddRuntimeConfiguration()
                    .AddAnalysisServices()
                    .AddTaskValueAdapterFactory()
                    .AddTaskProcessorFactory()
                    .AddTransient<IssuesView>()
                    .AddTransient<TaskGroupView>()
                    .AddSingleton<ITaskValueAdapter, ParagraphValueTaskAdapter>()
                    .AddSingleton<ITaskValueAdapter, TableValueTaskAdapter>();
            })
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
            Application.Run(mainWindow);
        }
    }
}
