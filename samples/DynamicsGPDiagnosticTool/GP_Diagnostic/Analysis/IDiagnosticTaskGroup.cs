namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using System.Collections.Generic;

/// <summary>
/// A collection of related diagnostic tasks.
/// </summary>
public interface IDiagnosticTaskGroup
{
    /// <summary>
    /// The name of the group of diagnostic tasks.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Name as seen in the application.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Individual diagnostic tasks that are members of the group.
    /// </summary>
    IList<IDiagnosticTask> Tasks { get; }
}
