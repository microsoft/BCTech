namespace Microsoft.GP.MigrationDiagnostic.Analysis;

using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a diagnostic task.
/// </summary>
public interface IDiagnosticTask
{
    /// <summary>
    /// A description of the task.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Specifies if this task has been evaluated.
    /// </summary>
    [JsonIgnore]
    bool IsEvaluated { get; }

    /// <summary>
    /// Specifies if the task evaluation discovered a migration issue.
    /// </summary>
    bool IsIssue { get; }

    /// <summary>
    /// A summarized value of the task.
    /// </summary>
    string SummaryValue { get; }

    /// <summary>
    /// An unique identifier for the task.
    /// </summary>
    string UniqueIdentifier { get; }

    /// <summary>
    /// A raw value for the task.
    /// </summary>
    object? EvaluatedValue { get; }

    /// <summary>
    /// Starts the task.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken);
}
