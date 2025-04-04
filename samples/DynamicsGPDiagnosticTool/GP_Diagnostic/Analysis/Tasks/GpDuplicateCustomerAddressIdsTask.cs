namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine duplicate customer address IDs when numbers are truncated to 10 characters.
/// </summary>
public class GpDuplicateCustomerAddressIdsTask : IDiagnosticTask
{
    public class Item
    {
        public required string CUSTNMBR { get; init; }
        public required string ADRSCODE_TO_BE_TRUNCATED { get; init; }
        public required string TRUNCATED_RESULT_MATCHES_ADRSCODE { get; init; }
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
    public string UniqueIdentifier => "DuplicateCustomerAddressIds";

    public GpDuplicateCustomerAddressIdsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = @"WITH QUERY1 AS (
    SELECT
        A.[CUSTNMBR],
        B.[ADRSCODE] AS [ADRSCODE_TO_BE_TRUNCATED],
        A.[ADRSCODE] AS [TRUNCATED_RESULT_MATCHES_ADRSCODE]
    FROM
        [RM00102] A
    JOIN (
        SELECT
            [CUSTNMBR],
            [ADRSCODE]
        FROM
            [RM00102]
        WHERE
            LEN([ADRSCODE]) > 10
        GROUP BY
            [CUSTNMBR],
        ADRSCODE
    ) B ON A.[CUSTNMBR] = B.[CUSTNMBR]
        AND A.[ADRSCODE] = SUBSTRING(B.[ADRSCODE], 1, 10)
),
QUERY2 AS (
    SELECT
        [CUSTNMBR],
        [ADRSCODE] AS [ADRSCODE_TO_BE_TRUNCATED],
        SUBSTRING([ADRSCODE], 1, 10) AS [TRUNCATED_RESULT_MATCHES_ADRSCODE]
    FROM
        [RM00102]
    WHERE 
        LEN(ADRSCODE) > 10
        AND SUBSTRING([ADRSCODE], 1, 10) NOT IN (
            SELECT
                [TRUNCATED_RESULT_MATCHES_ADRSCODE]
            FROM
                QUERY1
        )
    GROUP BY
        [CUSTNMBR],
        [ADRSCODE]
),
QUERY3 AS (
    SELECT
        [CUSTNMBR],
        [TRUNCATED_RESULT_MATCHES_ADRSCODE]
    FROM
        QUERY2
    GROUP BY
        [CUSTNMBR],
        [TRUNCATED_RESULT_MATCHES_ADRSCODE]
    HAVING
        COUNT([TRUNCATED_RESULT_MATCHES_ADRSCODE]) > 1
)
SELECT
    [CUSTNMBR], [ADRSCODE_TO_BE_TRUNCATED], [TRUNCATED_RESULT_MATCHES_ADRSCODE]
FROM
    QUERY1
UNION
SELECT
    [CUSTNMBR], [ADRSCODE_TO_BE_TRUNCATED], [TRUNCATED_RESULT_MATCHES_ADRSCODE]
FROM
    QUERY2
WHERE
    [TRUNCATED_RESULT_MATCHES_ADRSCODE] IN (
        SELECT
            [TRUNCATED_RESULT_MATCHES_ADRSCODE]
        FROM
            QUERY3
        WHERE
            [CUSTNMBR] = QUERY2.[CUSTNMBR]
    )";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    CUSTNMBR = reader.GetString(0),
                    ADRSCODE_TO_BE_TRUNCATED = reader.GetString(1),
                    TRUNCATED_RESULT_MATCHES_ADRSCODE = reader.GetString(2),
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
