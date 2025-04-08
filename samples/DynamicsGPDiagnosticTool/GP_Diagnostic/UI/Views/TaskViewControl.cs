namespace Microsoft.GP.MigrationDiagnostic.UI.Views;

using Microsoft.GP.MigrationDiagnostic.Analysis;
using System;
using System.ComponentModel;
using System.Windows.Forms;

/// <summary>
/// A control that is used to display the list of tasks to a user.
/// </summary>
[TypeDescriptionProvider(typeof(AbstractControlDescriptionProvider<TaskViewControl, UserControl>))]
public abstract class TaskViewControl : UserControl, ITaskView
{
    /// <inheritdoc/>
    public abstract string ViewName { get; }

    /// <inheritdoc/>
    public abstract string ViewShortName { get; }

    /// <summary>
    /// A callback that is triggered after each task completes execution.
    /// </summary>
    /// <remarks>Can be used to trigger UI refresh.</remarks>
    public abstract Action<IDiagnosticTaskGroup, IDiagnosticTask> GetOnTaskCompletedCallback();

    /// <summary>
    /// Informs the control of the currently active task engine.
    /// </summary>
    /// <param name="engine">The currently active task engine.</param>
    public abstract void SetEngine(GpEngine engine);

    /// <summary>
    /// Informs the control that the evalution process has begun. Can be overridden in a derived class.
    /// </summary>
    public virtual void EvaluationStarted() { }
}
