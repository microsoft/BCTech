namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if there are duplicate document numbers in AP transactions.
/// </summary>
public class GpDuplicateApTransactionDocumentNumberTask : IDiagnosticTask
{
    public class Item
    {
        public required string VENDORID { get; init; }
        public required string DOCNUMBR { get; init; }
        public required int Count { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpDuplicateApTransactionDocumentNumberTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpDuplicateApTransactionDocumentNumberTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "DuplicateApTransactionDocumentNumber";

    public GpDuplicateApTransactionDocumentNumberTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"WITH QUERY AS
(
	SELECT v.VENDORID, trx.DOCNUMBR,
		CASE trx.DOCTYPE
			WHEN 6 THEN 1
			WHEN 1 THEN 2
			WHEN 2 THEN 2
			WHEN 3 THEN 2
			WHEN 7 THEN 2
			WHEN 4 THEN 3
			WHEN 5 THEN 3
			ELSE 0
		END AS CALCDOCTYPE
	FROM PM00200 v
	JOIN PM20000 trx ON trx.VENDORID=v.VENDORID
	WHERE v.VENDSTTS IN (1,3)
		AND trx.VOIDED = 0
		AND trx.DOCTYPE <= 7
		AND CURTRXAM >= 0.01
)

SELECT VENDORID, DOCNUMBR, COUNT('x') [DuplicateCount]
FROM QUERY
GROUP BY VENDORID, DOCNUMBR, CALCDOCTYPE
HAVING COUNT(*) > 1";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    VENDORID = reader.GetString(0),
                    DOCNUMBR = reader.GetString(1),
                    Count = reader.GetInt32(2),
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
