namespace Microsoft.GP.MigrationDiagnostic.Analysis;

/// <summary>
/// Defines a task group that may apply to multiple companies.
/// </summary>
public interface IMultiCompanyDiagnosticTaskGroup : IDiagnosticTaskGroup
{
    /// <summary>
    /// Specifies the company this task group is applicable for.
    /// </summary>
    string CompanyName { get; }
}
