namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine vendor phone numbers with alpha characters.
/// </summary>
public class GpVendorPhoneNumberAlphaTask : IDiagnosticTask
{
    public class Item
    {
        public required string VENDORID { get; init; }
        public required string VENDNAME { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpVendorPhoneNumberAlphaTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpVendorPhoneNumberAlphaTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "VendorPhoneNumberAlpha";

    public GpVendorPhoneNumberAlphaTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    [VENDORID], [VENDNAME]
FROM
    [PM00200]
WHERE
    [PHNUMBR1] LIKE '%[^0-9 ]%'
    AND VENDSTTS = 1 --Remove to include all Vendors";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    VENDORID = reader.GetString(0),
                    VENDNAME = reader.GetString(1),
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
