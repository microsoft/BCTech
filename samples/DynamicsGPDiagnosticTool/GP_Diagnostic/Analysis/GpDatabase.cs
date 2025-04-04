namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using Microsoft.Data.SqlClient;
using Microsoft.GP.MigrationDiagnostic.Configuration;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

public delegate string DbCommandBuilder(DbCommand command);

public class GpDatabase : IGpDatabase
{
    private readonly SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
    private readonly IRuntimeConfiguration<GpConfiguration> config;
    private readonly int connectionTimeout;
    private readonly int commandTimeout;

    /// <summary>
    /// GP system database name.
    /// </summary>
    public virtual string DatabaseName => this.config.CurrentValue.SystemDatabase ?? string.Empty;

    public GpDatabase(IRuntimeConfiguration<GpConfiguration> config, IRuntimeConfiguration<SqlConfiguration> sqlConfigurationConfig)
    {
        this.config = config ?? throw new ArgumentNullException(nameof(config));

        connectionTimeout = int.TryParse(sqlConfigurationConfig.CurrentValue.ConnectionTimeout, out connectionTimeout) ? connectionTimeout : 300;
        commandTimeout = int.TryParse(sqlConfigurationConfig.CurrentValue.CommandTimeout, out commandTimeout) ? commandTimeout : 90;
    }

    /// <summary>
    /// Executes a SQL statement against the database.
    /// </summary>
    /// <param name="sql">The statement to execute.</param>
    /// <param name="cancellationToken">A cancellation token used to signal cancellation of the operation.</param>
    /// <returns>A <see cref="SqlDataReader"/> used to read the result set.</returns>
    public virtual Task<DbDataReader> ExecuteSqlAsync(string sql, CancellationToken cancellationToken) => this.ExecuteSqlAsync(command => sql, cancellationToken);

    /// <summary>
    /// Executes a SQL statement against the database.
    /// </summary>
    /// <param name="sql">The statement to execute.</param>
    /// <param name="cancellationToken">A cancellation token used to signal cancellation of the operation.</param>
    /// <returns>A <see cref="SqlDataReader"/> used to read the result set.</returns>
    public virtual async Task<DbDataReader> ExecuteSqlAsync(DbCommandBuilder commandBuilder, CancellationToken cancellationToken)
    {
        var dbConfig = this.config.CurrentValue;

        this.builder.DataSource = dbConfig.Server;
        this.builder.InitialCatalog = this.DatabaseName;
        this.builder.UserID = dbConfig.UserName;
        this.builder.Password = dbConfig.Password;
        this.builder.IntegratedSecurity = dbConfig.IntegratedSecurity;
        this.builder.ConnectTimeout = connectionTimeout;
        this.builder.TrustServerCertificate = true;

        // The reader controls the connection closing (CommandBehavior.CloseConnection)
        SqlConnection connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = commandBuilder(command);
            command.CommandTimeout = commandTimeout;
            return await command.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection, cancellationToken);
        }
    }
}
