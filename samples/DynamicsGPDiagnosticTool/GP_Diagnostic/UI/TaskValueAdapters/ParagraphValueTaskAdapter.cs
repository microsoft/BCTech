namespace Microsoft.GP.MigrationDiagnostic.UI.TaskValueAdapters;

using Microsoft.GP.MigrationDiagnostic.Analysis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

public class ParagraphValueTaskAdapter : ITaskValueAdapter
{
    private static readonly HashSet<string> TaskIds =
    [
        "CompanyVersion",
        "RCSY00300",
        "SFYNWCSY00300ST01",
        "SQLDBCMPL",
        "SQLDBCT",
        "SystemVersion",
    ];

    public bool CanHandleTask(IDiagnosticTask task) => TaskIds.Contains(task.UniqueIdentifier);

    public Task<Control[]> GetPanelControlsAsync(IDiagnosticTask task)
    {
        if (!this.CanHandleTask(task))
        {
            return Task.FromResult<Control[]>([]);
        }

        return Task.FromResult<Control[]>([
            new TextBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Text = task.EvaluatedValue as string,
            },
        ]);
    }
}
