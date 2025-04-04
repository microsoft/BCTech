namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine customer phone numbers with alpha characters.
/// </summary>
public class GpCustomerPhoneNumberAlphaTask : IDiagnosticTask
{
    public class Item
    {
        public required string CUSTNMBR { get; init; }
        public required string CUSTNAME { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpCustomerPhoneNumberAlphaTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpCustomerPhoneNumberAlphaTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "CustomerPhoneNumberAlpha";

    public GpCustomerPhoneNumberAlphaTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    [CUSTNMBR], [CUSTNAME]
FROM
    [RM00101]
WHERE
    [PHONE1] LIKE '%[^0-9 ]%'
    AND [INACTIVE] = 0 --Remove to include all Customers";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    CUSTNMBR = reader.GetString(0),
                    CUSTNAME = reader.GetString(1),
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
