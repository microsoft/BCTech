namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to detect item classes missing master records.
/// </summary>
public class GpItemMissingMasterRecordTask : IDiagnosticTask
{
    public class Item
    {
        public required string ITMCLSCD { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpItemMissingMasterRecordTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpItemMissingMasterRecordTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "ItemMissingMasterRecord";

    public GpItemMissingMasterRecordTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = @"SELECT
    DISTINCT x.[ITMCLSCD]
FROM [IV00101] x
LEFT OUTER JOIN [IV40400] y
    ON y.[ITMCLSCD] = x.[ITMCLSCD]
WHERE
    x.[ITMCLSCD] <> ''
    AND y.[ITMCLSCD] IS NULL";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    ITMCLSCD = reader.GetString(0),
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
