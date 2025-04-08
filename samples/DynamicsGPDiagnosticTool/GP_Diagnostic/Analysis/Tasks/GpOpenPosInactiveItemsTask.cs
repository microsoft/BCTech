namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to detect open purchase orders with inactive items.
/// </summary>
public class GpOpenPosInactiveItemsTask : IDiagnosticTask
{
    public class Item
    {
        public required string PONUMBER { get; init; }
        public required string ITEMNMBR { get; init; }
        public required int ORD { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpOpenPosInactiveItemsTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpOpenPosInactiveItemsTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "OpenPosInactiveItems";

    public GpOpenPosInactiveItemsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = @"SELECT 
	po.[PONUMBER], 
	line.[ITEMNMBR], 
	line.[ORD]
FROM
    [POP10100] po
JOIN
    [POP10110] line ON line.[PONUMBER] = po.[PONUMBER]
JOIN
    [IV00101] item ON item.[ITEMNMBR] = line.[ITEMNMBR]
WHERE
    po.[POTYPE] = 1
    AND po.[POSTATUS] BETWEEN 1 AND 4
	AND (
        item.[INACTIVE] = 1
        OR item.[ITEMTYPE] = 2
    )";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    PONUMBER = reader.GetString(0),
                    ITEMNMBR = reader.GetString(1),
                    ORD = reader.GetInt32(2),
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
