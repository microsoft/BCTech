namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

public interface IGpDatabase
{
    string DatabaseName { get; }

    Task<DbDataReader> ExecuteSqlAsync(DbCommandBuilder commandBuilder, CancellationToken cancellationToken);

    Task<DbDataReader> ExecuteSqlAsync(string sql, CancellationToken cancellationToken);
}
