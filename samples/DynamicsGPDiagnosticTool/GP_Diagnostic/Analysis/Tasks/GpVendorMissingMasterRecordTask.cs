namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to detect vendor classes missing master records.
/// </summary>
public class GpVendorMissingMasterRecordTask : IDiagnosticTask
{
    public class Item
    {
        public required string VNDCLSID { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpCustomerMissingMasterRecordTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpCustomerMissingMasterRecordTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "VendorMissingMasterRecord";

    public GpVendorMissingMasterRecordTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = @"SELECT
    DISTINCT x.[VNDCLSID]
FROM [PM00200] x
LEFT OUTER JOIN [PM00100] y
    ON y.[VNDCLSID] = x.[VNDCLSID]
WHERE
    x.[VNDCLSID] <> ''
    AND y.[VNDCLSID] IS NULL";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    VNDCLSID = reader.GetString(0),
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
