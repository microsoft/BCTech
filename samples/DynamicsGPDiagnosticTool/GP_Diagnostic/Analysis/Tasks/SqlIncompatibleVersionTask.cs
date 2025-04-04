namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the specific version string of the current SQL server.
/// </summary>
public class SqlIncompatibleVersionTask : IDiagnosticTask
{
    private readonly GpDatabase database;

    /// <inheritdoc/>
    public string Description => Resources.SqlIncompatibleVersionTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated => this.EvaluatedValue != null;

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? $"{this.EvaluatedValue}" : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => $"SQLPV";

    public SqlIncompatibleVersionTask(GpDatabase database)
    {
        this.database = database ?? throw new ArgumentNullException(nameof(database), "No database provider specified.");
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = @"SELECT SERVERPROPERTY('productversion')";
        var versionString = string.Empty;

        using (var reader = await this.database.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            versionString = reader.GetString(0);
        }

        if (Version.TryParse(versionString, out Version? version))
        {
            switch (version.Major)
            {
                case 13 when version.Build >= 4001:
                    this.IsIssue = false;
                    this.EvaluatedValue = string.Empty;
                    break;
                case > 13:
                    this.IsIssue = false;
                    this.EvaluatedValue = string.Empty;
                    break;
                default:
                    this.IsIssue = true;
                    this.EvaluatedValue = Resources.SqlIncompatibleVersionTaskIncompatible;
                    break;
            }
        }
        else
        {
            this.IsIssue = true;
            this.EvaluatedValue = Resources.SqlIncompatibleVersionTaskUndetermined;
        }
    }
}
