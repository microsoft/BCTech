namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to determine if any accounts have the incorrect main segment.
/// </summary>
public class GpIncorrectMainSegmentTask : IDiagnosticTask
{
    public class Item
    {
        public required int ACTINDX { get; init; }
        public required string ACTDESCR { get; init; }
        public required string ACTNUMBR_1 { get; init; }
        public required string ACTNUMBR_2 { get; init; }
        public required string ACTNUMBR_3 { get; init; }
        public required string ACTNUMBR_4 { get; init; }
        public required string ACTNUMBR_5 { get; init; }
        public required string MNACSGMT { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpIncorrectMainSegmentTaskDescription;

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpIncorrectMainSegmentTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "IncorrectMainSegment";

    public GpIncorrectMainSegmentTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var getMainSegmentNumber = $@"SELECT 
  SGMTNUMB
FROM
  [{this.database.DatabaseName}].[dbo].[SY00300]
WHERE
  MNSEGIND = 1";

        int mainSegmentNumber;

        using (var reader = await this.database.ExecuteSqlAsync(getMainSegmentNumber, cancellationToken))
        {
            reader.Read();
            mainSegmentNumber = reader.GetInt16(0);
        }

        var sql = $@"SELECT
    [ACTINDX], [ACTDESCR], [ACTNUMBR_1], [ACTNUMBR_2], [ACTNUMBR_3], [ACTNUMBR_4], [ACTNUMBR_5], [MNACSGMT]
FROM
    [GL00100]
WHERE
    [ACTNUMBR_{mainSegmentNumber}] <> [MNACSGMT]";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    ACTINDX = reader.GetInt32(0),
                    ACTDESCR = reader.GetString(1),
                    ACTNUMBR_1 = reader.GetString(2),
                    ACTNUMBR_2 = reader.GetString(3),
                    ACTNUMBR_3 = reader.GetString(4),
                    ACTNUMBR_4 = reader.GetString(5),
                    ACTNUMBR_5 = reader.GetString(6),
                    MNACSGMT = reader.GetString(7),
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
