namespace Microsoft.GP.MigrationDiagnostic.UI.TaskValueAdapters;

using Microsoft.GP.MigrationDiagnostic.Analysis;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// A generalized adapter used to map an analysis task to a set of controls that can display the resulting value(s) of the task's execution.
/// </summary>
public interface ITaskValueAdapter
{
    /// <summary>
    /// Determines if the view can handle the given task.
    /// </summary>
    bool CanHandleTask(IDiagnosticTask task);

    /// <summary>
    /// Gets the controls that can be used in a panel to display the results of a task.
    /// </summary>
    /// <returns></returns>
    Task<Control[]> GetPanelControlsAsync(IDiagnosticTask task);
}
