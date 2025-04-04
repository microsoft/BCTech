namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the list of posting accounts that have not been set up.
/// </summary>
public class GpPostingAccountsNotSetupTask : IDiagnosticTask
{
    public class Item
    {
        public required int SERIES { get; init; }

        public required int SEQNUMBR { get; init; }

        public required int ACTINDX { get; init; }

        public required string PTGACDSC { get; init; }

        public required int DEX_ROW_ID { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpPostingAccountsNotSetupTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpPostingAccountsNotSetupTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "PostingAccountsNotSetup";

    public GpPostingAccountsNotSetupTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT 
    [SERIES],
    [SEQNUMBR],
    [ACTINDX],
    [PTGACDSC],
    [DEX_ROW_ID]
FROM
    {this.database.DatabaseName.AsSqlBracketedValue()}.[dbo].[SY01100]
WHERE
    (
        ((PTGACDSC IN ('Accounts Receivable')) AND SERIES = 3) -- SALES
        OR ((PTGACDSC IN ('Accounts Payable')) AND SERIES = 4) -- PURCHASING
        OR ((PTGACDSC IN ('Inventory Control')) AND SERIES = 5) -- INVENTORY
    )
    AND ACTINDX = 0";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    SERIES = reader.GetInt16(0),
                    SEQNUMBR = reader.GetInt32(1),
                    ACTINDX = reader.GetInt32(2),
                    PTGACDSC = reader.GetString(3),
                    DEX_ROW_ID = reader.GetInt32(4),
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
