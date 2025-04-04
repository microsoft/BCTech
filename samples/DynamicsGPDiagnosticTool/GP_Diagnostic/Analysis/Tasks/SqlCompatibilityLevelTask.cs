namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the compatibility level of the current SQL server database.
/// </summary>
public class SqlCompatibilityLevelTask : IDiagnosticTask
{
    private readonly IGpDatabase database;

    /// <inheritdoc/>
    public string Description => Resources.SqlCompatibilityLevelTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated => this.EvaluatedValue != null;

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => "SQLDBCMPL";

    public SqlCompatibilityLevelTask(IGpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        DbCommandBuilder sql = command =>
        {
            var dbNameParam = command.CreateParameter();
            dbNameParam.ParameterName = "dbName";
            dbNameParam.Value = this.database.DatabaseName;
            command.Parameters.Add(dbNameParam);
            return $@"SELECT compatibility_level FROM sys.databases WHERE name = @{dbNameParam.ParameterName}";
        };
        string compatibilityLevel;

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            compatibilityLevel = reader.GetValue(0)?.ToString() ?? string.Empty;
        }

        if (int.TryParse(compatibilityLevel, out var level))
        {
            switch (level)
            {
                case <130:
                    this.IsIssue = true;
                    this.SummaryValue = string.Format(Resources.SqlCompatibilityLevelTaskSummary, compatibilityLevel);
                    this.EvaluatedValue = Resources.SqlCompatibilityLevelTaskDetail;
                    break;
                default:
                    this.IsIssue = false;
                    this.SummaryValue = string.Empty;
                    this.EvaluatedValue = string.Empty;
                    break;
            }
        }
        else
        {
            this.IsIssue = true;
            this.SummaryValue = Resources.SqlCompatibilityLevelTaskUndetermined;
            this.EvaluatedValue = Resources.SqlCompatibilityLevelTaskUndetermined;
        }
    }
}
