namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine purchase receipts without a valuation method.
/// </summary>
public class GpPurchaseReceiptsWithoutValuationMethodTask : IDiagnosticTask
{
    public class Item
    {
        public required string ITEMNMBR { get; init; }
        public required string TRXLOCTN { get; init; }
        public required int QTYTYPE { get; init; }
        public required DateTime DATERECD { get; init; }
        public required int RCTSEQNM { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpPurchaseReceiptsWithoutValuationMethodTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpPurchaseReceiptsWithoutValuationMethodTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "PurchaseReceiptsWithoutValuationMethod";

    public GpPurchaseReceiptsWithoutValuationMethodTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    [ITEMNMBR], [TRXLOCTN], [QTYTYPE], [DATERECD], [RCTSEQNM]
FROM
    [IV10200]
WHERE
    [VCTNMTHD] = 0";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    ITEMNMBR = reader.GetString(0),
                    TRXLOCTN = reader.GetString(1),
                    QTYTYPE = reader.GetInt16(2),
                    DATERECD = reader.GetDateTime(3),
                    RCTSEQNM = reader.GetInt32(4),
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
