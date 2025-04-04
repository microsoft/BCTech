namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if the main segment has not been identified.
/// </summary>
public class GpMainSegmentNotIdentifiedTask : IDiagnosticTask
{
    private readonly IGpDatabase database;

    /// <inheritdoc/>
    public string Description => Resources.GpMainSegmentNotIdentifiedTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue { get; private set; }

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.EvaluatedValue?.ToString() ?? string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "SFYNWCSY00300ST01";

    public GpMainSegmentNotIdentifiedTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = SqlQueryBuilder.RecordCountWhere(this.database.DatabaseName, "dbo", "SY00300", "MNSEGIND = 1");
        var count = 0;

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            count = reader.GetInt32(0);
        }

        if (count > 0)
        {
            this.IsIssue = false;
            this.EvaluatedValue = string.Empty;
        }
        else
        {
            this.IsIssue = true;
            this.EvaluatedValue = Resources.GpMainSegmentNotIdentifiedTaskSummary;
        }

        this.IsEvaluated = true;
    }
}
