namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if there are GL summary records with values beyond two decimal places.
/// </summary>
public class GpGlSummaryValuesBeyondTwoDecimalsTask : IDiagnosticTask
{
    public class Item
    {
        public required string Table { get; init; }
        public required int DEX_ROW_ID { get; init; }
        public required int ACTINDX { get; init; }
        public required int YEAR1 { get; init; }
        public required int PERIODID {  get; init; }
        public required decimal DEBITAMT { get; init; }
        public required decimal CRDTAMNT { get; init; }
        public required decimal PERDBLNC { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpGlSummaryValuesBeyondTwoDecimalsTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpGlSummaryValuesBeyondTwoDecimalsTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "GlSummaryValuesBeyondTwoDecimals";

    public GpGlSummaryValuesBeyondTwoDecimalsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    'GL10110' AS [Table],
    DEX_ROW_ID,
    ACTINDX,
    YEAR1,
    PERIODID,
    DEBITAMT,
    CRDTAMNT,
    PERDBLNC
FROM [GL10110]
WHERE
    RIGHT(CRDTAMNT,3) <> 0 OR 
    RIGHT(DEBITAMT,3) <> 0 OR 
    RIGHT(PERDBLNC,3) <> 0
UNION
SELECT
    'GL10111' AS [Table],
    DEX_ROW_ID,
    ACTINDX,
    YEAR1,
    PERIODID,
    DEBITAMT,
    CRDTAMNT,
    PERDBLNC 
FROM [GL10111]
WHERE 
    RIGHT(CRDTAMNT,3) <> 0 OR
    RIGHT(DEBITAMT,3) <> 0 OR
    RIGHT(PERDBLNC,3) <> 0";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    Table = reader.GetString(0),
                    DEX_ROW_ID = reader.GetInt32(1),
                    ACTINDX = reader.GetInt32(2),
                    YEAR1 = reader.GetInt16(3),
                    PERIODID = reader.GetInt16(4),
                    DEBITAMT = reader.GetDecimal(5),
                    CRDTAMNT = reader.GetDecimal(6),
                    PERDBLNC = reader.GetDecimal(7),
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
