namespace Microsoft.GP.MigrationDiagnostic.Analysis.TaskGroups;

using Microsoft.Extensions.Logging;
using Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;
using Microsoft.GP.MigrationDiagnostic.Configuration;
using System.Collections.Generic;

/// <summary>
/// A group of Gp System related tasks.
/// </summary>
public class GpSystemTaskGroup : IDiagnosticTaskGroup
{
    /// <inheritdoc/>
    public string Name => "System";

    /// <inheritdoc/>
    public string DisplayName => this.Name;

    /// <inheritdoc/>
    public IList<IDiagnosticTask> Tasks { get; } = new List<IDiagnosticTask>();

    public GpSystemTaskGroup(GpDatabase database, IRuntimeConfiguration<GpConfiguration> gpConfig, ILogger logger)
    {
        this.Tasks.Add(new SqlIncompatibleVersionTask(database));
        this.Tasks.Add(new SqlChangeTrackingDisabledTask(database));
        this.Tasks.Add(new SqlCompatibilityLevelTask(database));
        this.Tasks.Add(new GpSystemVersionTask(database));
    }
}
