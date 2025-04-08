namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine vendor address IDs with lengths greater than 10 characters.
/// </summary>
public class GpVendorAddressIdLengthTask : IDiagnosticTask
{
    public class Item
    {
        public required string VENDORID { get; init; }
        public required string VENDNAME { get; init; }
        public required string ADRSCODE { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpCustomerAddressIdLengthTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpCustomerAddressIdLengthTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "VendorAddressIdLength";

    public GpVendorAddressIdLengthTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    B.[VENDORID], B.[VENDNAME], A.[ADRSCODE]
FROM
    [PM00300] A
    JOIN [PM00200] B on B.[VENDORID] = A.[VENDORID]
WHERE
    LEN(A.[ADRSCODE]) > 10
    AND B.[VENDSTTS] = 1 --Remove to include all Vendors";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    VENDORID = reader.GetString(0),
                    VENDNAME = reader.GetString(1),
                    ADRSCODE = reader.GetString(2),
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
