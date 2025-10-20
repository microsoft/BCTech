namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if checkbook numbers have spaces as a prefix.
/// </summary>
public class GpCheckbookNumberSpacePrefixTask : IDiagnosticTask
{
    public class Item
    {
        public required string CHEKBKID { get; init; }
        public required string DSCRIPTN { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpCheckbookNumberSpacePrefixTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpCheckbookNumberSpacePrefixTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "CheckbookNumberSpacePrefix";

    public GpCheckbookNumberSpacePrefixTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
	CHEKBKID,
	DSCRIPTN
FROM CM00100
WHERE LEFT(CHEKBKID, 1)=' '";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    CHEKBKID = reader.GetString(0),
                    DSCRIPTN = reader.GetString(1),
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
