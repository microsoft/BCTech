namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine customer address IDs with lengths greater than 10 characters.
/// </summary>
public class GpCustomerAddressIdLengthTask : IDiagnosticTask
{
    public class Item
    {
        public required string CUSTNMBR { get; init; }
        public required string CUSTNAME { get; init; }
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
    public string UniqueIdentifier => "CustomerAddressIdLength";

    public GpCustomerAddressIdLengthTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    B.[CUSTNMBR], B.[CUSTNAME], A.[ADRSCODE]
FROM
    [RM00102] A
    JOIN [RM00101] B on B.[CUSTNMBR] = A.[CUSTNMBR]
WHERE
    LEN(A.[ADRSCODE]) > 10
    AND B.[INACTIVE] = 0 --Remove to include all Customers";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    CUSTNMBR = reader.GetString(0),
                    CUSTNAME = reader.GetString(1),
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
