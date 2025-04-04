namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine transactions that were posted directly to Retained Earnings account.
/// </summary>
public class GpDirectTransactionsPostingToRetainedEarningsAccountTask : IDiagnosticTask
{
    public class Item
    {
        public required int DEX_ROW_ID { get; init; }

        public required int JRNENTRY { get; init; }

        public required int HSTYEAR { get; init; }

        public required DateTime TRXDATE { get; init; }

        public required decimal RCTRXSEQ { get; init; }
    }

    private readonly GpCompanyDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc />
    public string Description => Resources.GpDirectTransactionsPostingToRetainedEarningsAccountTaskDescription;

    /// <inheritdoc />
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc />
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc />
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpDirectTransactionsPostingToRetainedEarningsAccountTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc />
    public string UniqueIdentifier => "DirectPostingRetainedEarnings";

    public GpDirectTransactionsPostingToRetainedEarningsAccountTask(GpCompanyDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = @"SELECT
    [GL30000].[DEX_ROW_ID],
    [GL30000].[JRNENTRY],
    [GL30000].[HSTYEAR],
    [GL30000].[TRXDATE],
    [GL30000].[RCTRXSEQ]
FROM
    [GL30000]
INNER JOIN
    [GL40000]
ON
    [GL30000].[ACTINDX] = [GL40000].[RERINDX]
WHERE
    [GL30000].[SOURCDOC] <> 'BBF' AND [GL30000].[SOURCDOC] <> 'P/L'";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    DEX_ROW_ID = reader.GetInt32(0),
                    JRNENTRY = reader.GetInt32(1),
                    HSTYEAR = reader.GetInt16(2),
                    TRXDATE = reader.GetDateTime(3),
                    RCTRXSEQ = reader.GetDecimal(4),
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
