namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the list of posting accounts that have not been set up.
/// </summary>
public class GpVerifyPostingTypesTask : IDiagnosticTask
{
    public class Item
    {
        public required int ACTINDX { get; init; }
        public required string MNACSGMT { get; init; }
        public required string ACTNUMBR_1 { get; init; }
        public required string ACTNUMBR_2 { get; init; }
        public required string ACTNUMBR_3 { get; init; }
        public required string ACTNUMBR_4 { get; init; }
        public required string ACTNUMBR_5 { get; init; }
        public required string ACTDESCR { get; init; }
        public required int PSTNGTYP { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpVerifyPostingTypesTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpVerifyPostingTypesTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "VerifyPostingTypes";

    public GpVerifyPostingTypesTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT
    [ACTINDX],
    [MNACSGMT],
    [ACTNUMBR_1],
    [ACTNUMBR_2],
    [ACTNUMBR_3],
    [ACTNUMBR_4],
    [ACTNUMBR_5],
    [ACTDESCR],
    [PSTNGTYP]
FROM
    {this.database.DatabaseName.AsSqlBracketedValue()}.[dbo].[GL00100]
WHERE
    MNACSGMT IN (
        SELECT
            [MNACSGMT]
        FROM
            {this.database.DatabaseName.AsSqlBracketedValue()}.[dbo].[GL00100]
        GROUP BY
            [MNACSGMT],
            [ACCTTYPE]
        HAVING
            COUNT([MNACSGMT]) >= 2
            AND COUNT(DISTINCT [PSTNGTYP]) = 2
    )
    AND [ACCTTYPE] = 1";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    ACTINDX = reader.GetInt32(0),
                    MNACSGMT = reader.GetString(1),
                    ACTNUMBR_1 = reader.GetString(2),
                    ACTNUMBR_2 = reader.GetString(3),
                    ACTNUMBR_3 = reader.GetString(4),
                    ACTNUMBR_4 = reader.GetString(5),
                    ACTNUMBR_5 = reader.GetString(6),
                    ACTDESCR = reader.GetString(7),
                    PSTNGTYP = reader.GetInt16(8),
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
