namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the specific version string of the current SQL server.
/// </summary>
public class GpSystemVersionTask : IDiagnosticTask
{
    private readonly GpDatabase database;

    /// <inheritdoc/>
    public string Description => Resources.GpSystemVersionTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated => this.EvaluatedValue != null;

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? $"{this.EvaluatedValue}" : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => $"SystemVersion";

    public GpSystemVersionTask(GpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = SqlQueryBuilder.GetGpDatabaseVersion(this.database.DatabaseName, this.database.DatabaseName, "version");

        var versionString = string.Empty;

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            versionString = reader.GetString(0);
        }

        if (Version.TryParse(versionString, out Version? version))
        {
            switch (version.Major)
            {
                case >= 14:
                    this.IsIssue = false;
                    this.EvaluatedValue = string.Empty;
                    break;
                default:
                    this.IsIssue = true;
                    this.EvaluatedValue = string.Format(Resources.GpSystemVersionTaskIncompatible, version.Major);
                    break;
            }
        }
        else
        {
            this.IsIssue = true;
            this.EvaluatedValue = Resources.GpSystemVersionTaskUndetermined;
        }
    }
}
