namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine duplicate item IDs when numbers are truncated to 20 characters.
/// </summary>
public class GpDuplicateItemIdsTask : IDiagnosticTask
{
    public class Item
    {
        public required string ITEMNMBR_TO_BE_TRUNCATED { get; init; }
        public required string TRUNCATED_RESULT_MATCHES_ITEMNMBR { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpDuplicateItemIdsTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpDuplicateItemIdsTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "DuplicateItemIds";

    public GpDuplicateItemIdsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"WITH QUERY1 AS (
    SELECT 
        B.[ITEMNMBR] AS [ITEMNMBR_TO_BE_TRUNCATED],
        A.[ITEMNMBR] AS [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
    FROM
        [IV00101] A
    JOIN (
        SELECT
            [ITEMNMBR]
        FROM
            [IV00101]
        WHERE
            LEN([ITEMNMBR]) > 20
            AND [INACTIVE] = 0 --Remove to include all Items
    ) B ON A.[ITEMNMBR] = SUBSTRING(B.[ITEMNMBR], 1, 20)
),
QUERY2 AS (
    SELECT
        [ITEMNMBR] AS [ITEMNMBR_TO_BE_TRUNCATED],
        SUBSTRING([ITEMNMBR], 1, 20) AS [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
    FROM
        [IV00101]
    WHERE
        LEN([ITEMNMBR]) > 20
        AND [INACTIVE] = 0 --Remove to include all Items
        AND SUBSTRING([ITEMNMBR], 1, 20) NOT IN (
            SELECT
                [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
            FROM
                QUERY1
        )
),
QUERY3 AS (
    SELECT
        [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
    FROM
        QUERY2
    GROUP BY
        [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
    HAVING
        COUNT([TRUNCATED_RESULT_MATCHES_ITEMNMBR]) > 1
)
SELECT
    [ITEMNMBR_TO_BE_TRUNCATED], [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
FROM
    QUERY1
UNION
SELECT
    [ITEMNMBR_TO_BE_TRUNCATED], [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
FROM
    QUERY2
WHERE
    [TRUNCATED_RESULT_MATCHES_ITEMNMBR] IN (
        SELECT
            [TRUNCATED_RESULT_MATCHES_ITEMNMBR]
        FROM
            QUERY3
    )";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    ITEMNMBR_TO_BE_TRUNCATED = reader.GetString(0),
                    TRUNCATED_RESULT_MATCHES_ITEMNMBR = reader.GetString(1),
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
