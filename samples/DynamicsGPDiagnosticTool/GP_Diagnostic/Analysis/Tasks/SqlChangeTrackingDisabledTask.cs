namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// A task to determine if change tracking is enabled for a database.
/// </summary>
public class SqlChangeTrackingDisabledTask : IDiagnosticTask
{
    private readonly IGpDatabase database;

    /// <inheritdoc/>
    public string Description => Resources.SqlChangeTrackingDisabledTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue { get; private set; }

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "SQLDBCT";

    public SqlChangeTrackingDisabledTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT COUNT(database_id) FROM sys.change_tracking_databases WHERE database_id=DB_ID('{this.database.DatabaseName}')";
        var count = 0;

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            count = reader.GetInt32(0);
        }

        if (count == 0)
        {
            this.IsIssue = false;
            this.SummaryValue = string.Empty;
            this.EvaluatedValue = string.Empty;
        }
        else
        {
            this.IsIssue = true;
            this.SummaryValue = Resources.SqlChangeTrackingDisabledTaskNotEnabled;
            this.EvaluatedValue = string.Format(Resources.SqlChangeTrackingDisabledTaskNotEnabledDetail, this.database.DatabaseName);
        }

        this.IsEvaluated = true;
    }
}
