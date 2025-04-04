namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Gets the records with same main segment but different account categories.
/// </summary>
public class GpAccountCategoryMismatchTask : IDiagnosticTask
{
    public class Item
    {
        public required string MNACSGMT { get; init; }

        public required string ACCATDSC { get; init; }

        public required string ACTDESCR { get; init; }
    }

    private readonly GpCompanyDatabase database;
    private readonly IList<Item> items = [];

    /// <inheritdoc />
    public string Description => Resources.GpAccountCategoryMismatchTaskDescription;

    /// <inheritdoc />
    public bool IsEvaluated { get; private set; }

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc />
    public object? EvaluatedValue => this.items.AsReadOnly();

    /// <inheritdoc />
    public string SummaryValue => this.IsEvaluated ? string.Format(Resources.GpAccountCategoryMismatchTaskSummary, this.items.Count) : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "MSMAC";

    public GpAccountCategoryMismatchTask(GpCompanyDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = $@"SELECT 
  Z.MNACSGMT, 
  Y.ACCATDSC, 
  Z.ACTDESCR 
FROM 
  GL00100 Z 
  JOIN GL00102 Y ON Z.ACCATNUM = Y.ACCATNUM 
  JOIN GL00105 X ON Z.ACTINDX = X.ACTINDX 
WHERE 
  Z.MNACSGMT IN (
    SELECT 
	  D.MNACSGMT
    FROM 
      (
        SELECT 
          A.MNACSGMT, 
          B.ACCATDSC 
        FROM 
          GL00100 A 
          JOIN GL00102 B ON A.ACCATNUM = B.ACCATNUM 
          JOIN GL00105 C ON A.ACTINDX = C.ACTINDX 
        GROUP BY 
          A.MNACSGMT, 
          B.ACCATDSC
      ) D 
    GROUP BY 
      D.MNACSGMT 
    HAVING 
      COUNT(D.MNACSGMT) > 1
  )";

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            while (reader.Read())
            {
                this.items.Add(new Item
                {
                    MNACSGMT = reader.GetString(0),
                    ACCATDSC = reader.GetString(1),
                    ACTDESCR = reader.GetString(2),
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
