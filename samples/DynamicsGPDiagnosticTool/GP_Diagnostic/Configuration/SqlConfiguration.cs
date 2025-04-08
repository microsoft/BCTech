namespace Microsoft.GP.MigrationDiagnostic.Configuration;

/// <summary>
/// Configuration values related to the Sql configuration.
/// </summary>
public class SqlConfiguration
{
    /// <summary>
    /// The Sql connection timeout value
    /// </summary>
    public string? ConnectionTimeout { get; set; }
    
    /// <summary>
    /// The Sql command timeout value
    /// </summary>
    public string? CommandTimeout { get; set; }
}
