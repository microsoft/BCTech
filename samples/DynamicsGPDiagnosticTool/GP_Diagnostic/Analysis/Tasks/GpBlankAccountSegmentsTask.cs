namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the list of accounts with blank segments setup for a database.
/// </summary>
public class GpBlankAccountSegmentsTask : IDiagnosticTask
{
    public class Item
    {
        public required string ACTNUMST { get; init; }

        public required int ACTINDX { get; init; }

        public required int YEAR1 { get; init; }

        public required int PERIODID { get; init; }

        public required string Table { get; init; }
    }

    private readonly IGpDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc/>
    public string Description => Resources.GpBlankAccountSegmentsTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpBlankAccountSegmentsTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "BLANKACCOUNTS";

    public GpBlankAccountSegmentsTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var getMainSegmentNumber = $@"SELECT 
  SGMTNUMB
FROM
  {this.database.DatabaseName.AsSqlBracketedValue()}.[dbo].[SY00300]
WHERE
  MNSEGIND = 1";

        int mainSegmentNumber;

        using (var reader = await this.database.ExecuteSqlAsync(getMainSegmentNumber, cancellationToken))
        {
            reader.Read();
            mainSegmentNumber = reader.GetInt16(0);
        }

        var sql = $@"SELECT 
  *
FROM 
  (
    SELECT 
      GL00105.ACTNUMST, GL00105.ACTINDX, GL10110.YEAR1, GL10110.PERIODID, 'GL10110' as 'Table'
    FROM 
      GL10110 
	  JOIN GL00105 on GL00105.ACTINDX = GL10110.ACTINDX
    UNION 
    SELECT 
      GL00105.ACTNUMST, GL00105.ACTINDX, GL10111.YEAR1, GL10111.PERIODID, 'GL10111' as 'Table'
    FROM 
      GL10111
	  JOIN GL00105 on GL00105.ACTINDX = GL10111.ACTINDX
  ) X 
WHERE 
  X.ACTINDX in (
    SELECT 
      ACTINDX 
    FROM 
      GL00105 
    WHERE 
      ACTNUMBR_{mainSegmentNumber} = ''
  )";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    ACTNUMST = reader.GetString(0),
                    ACTINDX = reader.GetInt32(1),
                    YEAR1 = reader.GetInt16(2),
                    PERIODID = reader.GetInt16(3),
                    Table = reader.GetString(4),
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
