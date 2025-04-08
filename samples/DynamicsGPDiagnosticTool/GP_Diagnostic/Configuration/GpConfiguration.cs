namespace Microsoft.GP.MigrationDiagnostic.Configuration;

using System.Collections.Generic;

/// <summary>
/// A set of configuration values for a GP engine.
/// </summary>
public class GpConfiguration
{
    /// <summary>
    /// The server that hosts the GP databases.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// GP system database name.
    /// </summary>
    public string? SystemDatabase { get; set; }

    /// <summary>
    /// A set of company database and display names.
    /// </summary>
    public List<(string DatabaseName, string DisplayName, bool Selected)> CompanyNames { get; set; } = new List<(string, string, bool)>();

    /// <summary>
    /// The username to use for connecting to the database.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The password to use for connecting to the database.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Specifies whether to use integrated security mode.
    /// </summary>
    public bool IntegratedSecurity { get; set; } = false;

    /// <summary>
    /// Specifies whether the configuration is complete and valid.
    /// </summary>
    public bool IsValidConfig { get; set; } = false;
}
