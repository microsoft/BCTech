namespace Microsoft.GP.MigrationDiagnostic.UI.Views;

using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.GP.MigrationDiagnostic.Analysis;
using Microsoft.GP.MigrationDiagnostic.UI.TaskValueAdapters;

/// <summary>
/// A view of the tasks suitable for debugging or development.
/// </summary>
public partial class IssuesView : TaskViewControl
{
    private class Item(IDiagnosticTaskGroup group, IDiagnosticTask task)
    {
        public string CompanyName => group switch
        {
            IMultiCompanyDiagnosticTaskGroup multiCompanyDiagnosticTaskGroup => multiCompanyDiagnosticTaskGroup.CompanyName,
            _ => string.Empty,
        };

        public string Description => task.Description;

        public string SummaryValue => task.SummaryValue;

        public IDiagnosticTask Task => task;

        public string UniqueIdentifier => task.UniqueIdentifier;

        public object? EvaluatedValue => task.EvaluatedValue;
    }

    private readonly TaskValueViewResolver taskValueViewResolver;
    private readonly BindingList<Item> bindingList = [];

    public override string ViewName => "Issues View";

    public override string ViewShortName => "Issues";

    public IssuesView(TaskValueViewResolver taskValueViewResolver)
    {
        this.taskValueViewResolver = taskValueViewResolver ?? throw new ArgumentNullException(nameof(taskValueViewResolver));

        InitializeComponent();
        InitializeDataGrid();
    }

    private void InitializeDataGrid()
    {
        this.dgvTasks.AutoGenerateColumns = false;
        this.dgvTasks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
        this.dgvTasks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        this.gbxTasks.AutoSize = true;
        this.dgvTasks.AutoSize = true;

        // Add columns
        this.dgvTasks.Columns.Add(new DataGridViewColumn
        {
            Name = "Description",
            HeaderText = "Description",
            DataPropertyName = nameof(Item.Description),
            CellTemplate = new DataGridViewTextBoxCell(),
            Width = 300,
        });
        this.dgvTasks.Columns.Add(new DataGridViewColumn
        {
            CellTemplate = new DataGridViewTextBoxCell(),
            DataPropertyName = nameof(Item.CompanyName),
            HeaderText = "Company",
            Name = "Company",
            Width = 150,
        });
        this.dgvTasks.Columns.Add(new DataGridViewColumn
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            CellTemplate = new DataGridViewTextBoxCell(),
            DataPropertyName = nameof(Item.SummaryValue),
            HeaderText = "Issue",
            Name = "Issue",
        });

        this.dgvTasks.DataSource = this.bindingList;
        this.dgvTasks.SelectionChanged += this.dgvTasks_SelectionChanged;
    }

    /// <inheritdoc />
    public override void SetEngine(GpEngine engine)
    {
        this.bindingList.Clear();

        foreach (var group in engine.TaskGroups)
        {
            foreach (var task in group.Tasks.Where(x => x.IsIssue))
            this.bindingList.Add(new(group, task));
        }
    }

    /// <inheritdoc />
    public override Action<IDiagnosticTaskGroup, IDiagnosticTask> GetOnTaskCompletedCallback()
    {
        return (group, task) => {
            if (task.IsIssue)
            {
                this.bindingList.Add(new(group, task));
            }

            this.dgvTasks.Refresh();
        };
    }

    private async void dgvTasks_SelectionChanged(object? sender, EventArgs e)
    {
        this.splitContainer1.Panel2.Controls.Clear();

        if (this.dgvTasks.SelectedRows.Count == 1
            && this.dgvTasks.SelectedRows[0].DataBoundItem is Item item)
        {
            var adapter = this.taskValueViewResolver(item.Task);

            if (adapter != null)
            {
                var panel = await adapter.GetPanelControlsAsync(item.Task);
                this.splitContainer1.Panel2.Controls.AddRange(panel);
            }
        }

        this.splitContainer1.Panel2.Refresh();
    }
}
