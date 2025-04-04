namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine the unbalanced journal entries.
/// </summary>
public class GpUnbalancedJournalEntriesTask : IDiagnosticTask
{
    public class Item
    {
        public required int JRNENTRY { get; init; }

        public required decimal TotalDebits { get; init; }

        public required decimal TotalCredits { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly string table;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => string.Format(Resources.GpUnbalancedJournalEntriesTaskDescription, this.table);

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpUnbalancedJournalEntriesTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => $"UJEC{this.table}";

    public GpUnbalancedJournalEntriesTask(IGpDatabase database, string table)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
        this.table = table ?? throw new ArgumentNullException(nameof(table), "No table specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT 
    A.JRNENTRY, 
    SUM(A.DEBITAMT) AS TOTALDEBITS,
    SUM(A.CRDTAMNT) AS TOTALCREDITS 
FROM 
    {this.table.AsSqlBracketedValue()} A,
    [GL00100] B
WHERE 
    A.ACTINDX = B.ACTINDX 
    AND B.ACCTTYPE = 1 
    AND B.ACCATNUM <> 0
GROUP BY 
    A.JRNENTRY 
HAVING 
    SUM(ABS(A.CRDTAMNT)) <> SUM(ABS(A.DEBITAMT))";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    JRNENTRY = reader.GetInt32(0),
                    TotalDebits = reader.GetDecimal(1),
                    TotalCredits = reader.GetDecimal(2),
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
