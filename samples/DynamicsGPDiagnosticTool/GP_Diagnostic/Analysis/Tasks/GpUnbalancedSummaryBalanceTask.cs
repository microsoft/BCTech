namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine the unbalanced summary balances.
/// </summary>
public class GpUnbalancedSummaryBalanceTask : IDiagnosticTask
{
    public class Item
    {
        public required int YEAR1 { get; init; }

        public required int PERIODID { get; init; }

        public required decimal TotalDebits { get; init; }

        public required decimal TotalCredits { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly string table;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => string.Format(Resources.GpUnbalancedSummaryBalanceTaskDescription, this.table);

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpUnbalancedSummaryBalanceTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => $"USBC{this.table}";

    public GpUnbalancedSummaryBalanceTask(IGpDatabase database, string table)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
        this.table = table ?? throw new ArgumentNullException(nameof(table), "No table specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT 
    YEAR1, 
    PERIODID, 
    SUM(DEBITAMT) AS TOTALDEBITS, 
    SUM(CRDTAMNT) AS TOTALCREDITS 
FROM 
    {this.table.AsSqlBracketedValue()}
WHERE ACCATNUM <> 0
GROUP BY 
    YEAR1, 
    PERIODID 
HAVING 
    SUM(ABS(DEBITAMT)) <> SUM(ABS(CRDTAMNT)) 
ORDER BY 
    YEAR1, 
    PERIODID";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    YEAR1 = reader.GetInt16(0),
                    PERIODID = reader.GetInt16(0),
                    TotalDebits = reader.GetDecimal(2),
                    TotalCredits = reader.GetDecimal(3),
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
