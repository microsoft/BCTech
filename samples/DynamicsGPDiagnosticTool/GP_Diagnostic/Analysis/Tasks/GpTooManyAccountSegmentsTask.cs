namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if there are too many account segments for Business Central.
/// </summary>
public class GpTooManyAccountSegmentsTask : IDiagnosticTask
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
    public string SummaryValue => this.EvaluatedValue?.ToString() ?? string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "RCSY00300";

    public GpTooManyAccountSegmentsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = SqlQueryBuilder.RecordCount(this.database.DatabaseName, "dbo", "SY00300");
        var count = 0;

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            count = reader.GetInt32(0);
        }

        if (count <= 9)
        {
            this.IsIssue = false;
            this.EvaluatedValue = string.Empty;
        }
        else
        {
            this.IsIssue = true;
            this.EvaluatedValue = string.Format(Resources.GpTooManyAccountSegmentsTaskSummary, count);
        }

        this.IsEvaluated = true;
    }
}
