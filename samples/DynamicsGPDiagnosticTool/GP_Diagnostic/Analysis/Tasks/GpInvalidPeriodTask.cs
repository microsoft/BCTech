namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if there are too many account segments for Business Central.
/// </summary>
public class GpInvalidPeriodTask : IDiagnosticTask
{
    public class Item
    {
        public required int FORIGIN { get; init; }

        public required int YEAR1 { get; init; }

        public required int PERIODID { get; init; }

        public required int SERIES { get; init; }

        public required string ODESCTN { get; init; }

        public required DateTime PERIODDT { get; init; }

        public required DateTime PERDENDT { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpInvalidPeriodTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpInvalidPeriodTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "RCWCSY40100F9";

    public GpInvalidPeriodTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    [FORIGIN],
    [YEAR1],
    [PERIODID],
    [SERIES],
    [ODESCTN],
    [PERIODDT],
    [PERDENDT]
FROM {this.database.DatabaseName.AsSqlBracketedValue()}.[dbo].[SY40100]
WHERE
    ([PERIODDT] = '1900-01-01' OR [PERDENDT] = '1900-01-01')
    AND [SERIES] = 0";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    FORIGIN = reader.GetByte(0),
                    YEAR1 = reader.GetInt16(1),
                    PERIODID = reader.GetInt16(2),
                    SERIES = reader.GetInt16(3),
                    ODESCTN = reader.GetString(4).Trim(),
                    PERIODDT = reader.GetDateTime(5),
                    PERDENDT = reader.GetDateTime(6),
                });
            }
        }

        if (this.items.Count > 0)
        {
            this.IsIssue = true;
        }

        this.IsEvaluated = true;
    }
}
