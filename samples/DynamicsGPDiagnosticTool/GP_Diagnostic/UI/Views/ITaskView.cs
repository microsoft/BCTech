namespace Microsoft.GP.MigrationDiagnostic.UI.Views;

using Microsoft.GP.MigrationDiagnostic.Analysis;
using System;

/// <summary>
/// Common actions for interaction between a form and something that represents a view of the active tasks.
/// </summary>
public interface ITaskView
{
    /// <summary>
    /// Sets the engine state of the object.
    /// </summary>
    /// <param name="engine">The currently active engine.</param>
    public void SetEngine(GpEngine engine);

    /// <summary>
    /// A callback to be called after completion of a task.
    /// </summary>
    public Action<IDiagnosticTaskGroup, IDiagnosticTask> GetOnTaskCompletedCallback();

    /// <summary>
    /// Full name of the view.
    /// </summary>
    public string ViewName { get; }

    /// <summary>
    /// Partial name of the view, to be inserted into other strings.
    /// </summary>
    /// <remarks>Use lowercase, single-word names.</remarks>
    public string ViewShortName { get; }
}
