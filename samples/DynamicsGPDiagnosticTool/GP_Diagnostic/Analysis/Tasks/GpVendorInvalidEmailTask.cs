namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to detect invalid vendor email addresses.
/// </summary>
public class GpVendorInvalidEmailTask : IDiagnosticTask
{
    public class Item
    {
        public required string Master_ID { get; init; }
        public required string ADRSCODE { get; init; }
        public required string INET1 { get; init; }
        public required string EmailToAddress { get; init; }
        public required string EmailCcAddress { get; init; }
        public required string EmailBccAddress { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpVendorInvalidEmailTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpVendorInvalidEmailTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "VendorInvalidEmail";

    public GpVendorInvalidEmailTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = SqlQueryBuilder.EmailFormatValidation(this.database.DatabaseName, "dbo", "VEN", "VEN");

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    Master_ID = reader.GetString(0),
                    ADRSCODE = reader.GetString(1),
                    INET1 = reader.GetString(2),
                    EmailToAddress = reader.GetString(3),
                    EmailCcAddress = reader.GetString(4),
                    EmailBccAddress = reader.GetString(5),
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
