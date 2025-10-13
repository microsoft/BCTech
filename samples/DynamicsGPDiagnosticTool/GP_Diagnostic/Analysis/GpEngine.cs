namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using Microsoft.Extensions.Logging;
using Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;
using Microsoft.GP.MigrationDiagnostic.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

/// <summary>
/// A task engine, defined by its individual task groups.
/// </summary>
[JsonConverter(typeof(GpEngineJsonConverter))]
public class GpEngine
{
    private readonly GpDatabase systemDatabase;
    private readonly ManagementReporterDatabase managementReporterDatabase;
    private readonly IRuntimeConfigurationSource<GpConfiguration> gpConfigStore;
    private readonly IRuntimeConfiguration<GpConfiguration> gpConfig;
    private readonly IRuntimeConfiguration<SqlConfiguration> sqlConfig;
    private readonly GpCompanyDatabaseFactory companyDatabaseFactory;

    private readonly ILogger<GpEngine> logger;

    public GpEngine(
        GpCompanyDatabaseFactory gpCompanyDatabaseFactory,
        GpDatabase systemDatabase,
        ManagementReporterDatabase managementReporterDatabase,
        IRuntimeConfigurationSource<GpConfiguration> gpConfigurationStore,
        IRuntimeConfiguration<SqlConfiguration> sqlConfig,
        ILogger<GpEngine> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this.Name = "GPEngine";

        this.companyDatabaseFactory = gpCompanyDatabaseFactory ?? throw new ArgumentNullException(nameof(gpCompanyDatabaseFactory));
        this.systemDatabase = systemDatabase ?? throw new ArgumentNullException(nameof(systemDatabase));
        this.managementReporterDatabase = managementReporterDatabase ?? throw new ArgumentNullException(nameof(managementReporterDatabase));
        this.gpConfigStore = gpConfigurationStore;
        this.gpConfig = this.gpConfigStore.Option;
        this.gpConfig.OnChange += this.GpConfiguration_OnChange;
        this.sqlConfig = sqlConfig;
    }

    /// <summary>
    /// Gets whether the engine is configured and ready to execute.
    /// </summary>
    public bool IsConfigured => this.gpConfig.CurrentValue.IsValidConfig;

    /// <summary>
    /// The name of the task engine.
    /// </summary>
    public string Name { get; protected set; } = string.Empty;

    /// <summary>
    /// The groups that make up the engine.
    /// </summary>
    public IList<IDiagnosticTaskGroup> TaskGroups { get; } = new List<IDiagnosticTaskGroup>();

    /// <summary>
    /// Gets the associated configuration view.
    /// </summary>
    /// <param name="gpConfigStore">A configuration store to hold product-specific configs.</param>
    /// <returns>A configuration view that corresponds to this engine.</returns>

    public GpConfigurationView GetConfigurationView()
    {
        return new GpConfigurationView(this.gpConfigStore, this.logger);
    }

    /// <summary>
    /// Resets task groups to a ready-to-run state.
    /// </summary>
    public void ResetTaskGroups()
    {
        this.TaskGroups.Clear();

        if (this.gpConfig.CurrentValue.IsValidConfig)
        {
            this.AddSystemGroup();

            foreach (var company in this.gpConfig.CurrentValue.CompanyNames.Where(x => x.Selected))
            {
                this.AddCompanyTaskGroups(company);
            }
        }
    }

    private void AddSystemGroup()
    {
        var existing = this.TaskGroups.FirstOrDefault(x => x is GpSystemTaskGroup);

        if (existing != null)
        {
            this.TaskGroups.Remove(existing);
        }

        var taskGroup = new GpSystemTaskGroup(this.systemDatabase, this.gpConfig, this.logger);
        this.TaskGroups.Insert(0, taskGroup);
    }

    private void AddCompanyTaskGroups((string databaseName, string displayName, bool) company)
    {
        var companyDatabase = this.companyDatabaseFactory(company.databaseName);

        this.logger.LogInformation("Adding task groups for company {companyName}", company.databaseName);
        this.TaskGroups.Add(new GpCompanyFinanceTaskGroup(companyDatabase, company.databaseName, company.displayName));
        this.TaskGroups.Add(new GpCompanyInventoryTaskGroup(companyDatabase, company.databaseName, company.displayName));
        this.TaskGroups.Add(new GpCompanyPurchasingTaskGroup(companyDatabase, company.databaseName, company.displayName));
        this.TaskGroups.Add(new GpCompanySalesTaskGroup(this.systemDatabase, companyDatabase, company.databaseName, company.displayName));
        this.TaskGroups.Add(new GpCompanySetupTaskGroup(this.systemDatabase, companyDatabase, company.databaseName, company.displayName));
    }

    private void GpConfiguration_OnChange(object? sender, EventArgs e)
    {
        this.ResetTaskGroups();
    }
}
