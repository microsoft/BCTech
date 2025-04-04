namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if there are invalid payment terms.
/// </summary>
public class GpInvalidPaymentTermsTask : IDiagnosticTask
{
    public class Item
    {
        public required string PYMTRMID { get; init; }
        public required int DUETYPE { get; init; }
        public required int DUEDTDS { get; init; }
        public required int DueMonth { get; init; }
        public required int DISCTYPE { get; init; }
        public required int DISCDTDS { get; init; }
        public required int DiscountMonth { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpInvalidPaymentTermsTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpInvalidPaymentTermsTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "InvalidPaymentTerms";

    public GpInvalidPaymentTermsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT 
	PYMTRMID,
	DUETYPE,
	DUEDTDS,
	DueMonth,
	DISCTYPE,
	DISCDTDS,
	DiscountMonth
FROM SY03300
WHERE 
	(DUETYPE = 2 AND DUEDTDS > 31) -- Due Type: Date, DUEDTDS out of range
	OR (DISCTYPE = 2 AND DISCDTDS > 31) -- Discount Type: Date, DUEDTDS out of range
	OR (DUETYPE = 7 AND ((DueMonth < 1 OR DueMonth > 12) OR (DUEDTDS < 1 OR DUEDTDS > 31))) -- Due Type: Month/Day, Month or day out of range
	OR (DISCTYPE = 7 AND ((DiscountMonth < 1 OR DiscountMonth > 12) OR (DISCDTDS < 1 OR DISCDTDS > 31))) -- Discount Type: Month/Day, Month or day out of range";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    PYMTRMID = reader.GetString(0),
                    DUETYPE = reader.GetInt16(1),
                    DUEDTDS = reader.GetInt16(2),
                    DueMonth = reader.GetInt16(3),
                    DISCTYPE = reader.GetInt16(4),
                    DISCDTDS = reader.GetInt16(5),
                    DiscountMonth = reader.GetInt16(6),
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
