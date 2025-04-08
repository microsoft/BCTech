namespace Microsoft.GP.MigrationDiagnostic.Analysis.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A task to get the specific version string of the current SQL server.
/// </summary>
public class GpCompanyVersionTask : IDiagnosticTask
{
    private readonly GpDatabase systemDatabase;
    private readonly GpCompanyDatabase companyDatabase;

    /// <inheritdoc/>
    public string Description => Resources.GpSystemVersionTaskDescription;

    /// <inheritdoc/>
    public bool IsEvaluated => this.EvaluatedValue != null;

    /// <inheritdoc/>
    public bool IsIssue { get; private set; }

    /// <inheritdoc/>
    public object? EvaluatedValue { get; private set; }

    /// <inheritdoc/>
    public string SummaryValue => this.IsEvaluated ? $"{this.EvaluatedValue}" : string.Empty;

    /// <inheritdoc/>
    public string UniqueIdentifier => $"CompanyVersion";

    public GpCompanyVersionTask(GpDatabase systemDatabase, GpCompanyDatabase companyDatabase)
    {
        this.systemDatabase = systemDatabase ?? throw new ArgumentNullException(nameof(systemDatabase), "No database provider specified.");
        this.companyDatabase = companyDatabase;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var sql = SqlQueryBuilder.GetGpDatabaseVersion(this.systemDatabase.DatabaseName, this.systemDatabase.DatabaseName, "version");

        var systemVersionString = string.Empty;

        using (var reader = await this.systemDatabase.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            systemVersionString = reader.GetString(0);
        }

        sql = SqlQueryBuilder.GetGpDatabaseVersion(this.systemDatabase.DatabaseName, this.companyDatabase.DatabaseName, "version");

        var companyVersionString = string.Empty;

        using (var reader = await this.systemDatabase.ExecuteSqlAsync(sql, cancellationToken))
        {
            reader.Read();
            companyVersionString = reader.GetString(0);
        }

        if (Version.TryParse(systemVersionString, out Version? systemVersion)
            && Version.TryParse(companyVersionString, out Version? companyVersion))
        {
            if (companyVersion == systemVersion)
            {
                if (companyVersion.Major >= 14)
                {
                    this.IsIssue = false;
                    this.EvaluatedValue = string.Empty;
                }
                else
                {
                    this.IsIssue = true;
                    this.EvaluatedValue = string.Format(Resources.GpCompanyVersionTaskIncompatible, companyVersion);
                }
            }
            else
            {
                this.IsIssue = true;
                this.EvaluatedValue = string.Format(Resources.GpCompanyVersionTaskNotMatching, companyVersion, systemVersion);
            }
        }
        else
        {
            this.IsIssue = true;
            this.EvaluatedValue = Resources.GpCompanyVersionTaskUndetermined;
        }
    }
}
