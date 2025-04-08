namespace Microsoft.GP.MigrationDiagnostic.UI.Views.TaskGroup;

using Microsoft.GP.MigrationDiagnostic.Analysis;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// A view of the task groups suitable for end users.
/// </summary>
public partial class TaskGroupView : TaskViewControl
{
    private GpEngine? currentEngine;
    
    private readonly BindingList<GridBoundGroup> bindingList = new BindingList<GridBoundGroup>();

    private const string ImageColumnName = "Status";
    private const string GroupNameColumnName = "Group";
    private const string TaskCountColumnName = "Tasks Completed";
    private const string LeftSpacingColumnName = "LSpacing";
    private const string RightSpacingColumnName = "RSpacing";

    private const int targetImageWidth = 24;

    public override string ViewName => "Summary View";

    public override string ViewShortName => "Summary";

    public TaskGroupView()
    {
        InitializeComponent();

        this.DoubleBuffered = true;

        // Grid view properties
        this.dgvGroups.ReadOnly = true;
        this.dgvGroups.RowTemplate.Height = 30;
        this.dgvGroups.RowHeadersVisible = false;
        this.dgvGroups.ColumnHeadersVisible = true;
        this.dgvGroups.AutoGenerateColumns = false;

        // For some reason, DataGridView doesn't expose the DoubleBuffered property directly, unlike other controls.
        Type dgvType = this.dgvGroups.GetType();
        System.Reflection.PropertyInfo? pi = dgvType.GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        pi?.SetValue(this.dgvGroups, true, null);

        // Set up events
        this.dgvGroups.Resize += this.DgvGroups_Resize;

        // Set the header cell style
        var headerStyle = new DataGridViewCellStyle()
        {
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            BackColor = SystemColors.Control,
            ForeColor = SystemColors.Control,
            Font = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Point)
        };

        this.dgvGroups.ColumnHeadersDefaultCellStyle = headerStyle;

        // An empty column, used to help control the image column's sizing            
        var leftSpacingColumnTemplate = new DataGridViewTextBoxCell();
        leftSpacingColumnTemplate.Style.BackColor = SystemColors.Control;
        this.dgvGroups.Columns.Add(new DataGridViewColumn()
        {
            Name = LeftSpacingColumnName,
            DataPropertyName = nameof(GridBoundGroup.Spacer),
            HeaderText = string.Empty,
            CellTemplate = leftSpacingColumnTemplate
        });

        // The status image
        var imageTemplate = new DataGridViewImageCell()
        {
            ImageLayout = DataGridViewImageCellLayout.Zoom,
        };
        imageTemplate.Style.BackColor = SystemColors.Control;
        imageTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        this.dgvGroups.Columns.Add(new DataGridViewColumn()
        {
            Name = ImageColumnName,
            HeaderText = string.Empty,
            DataPropertyName = nameof(GridBoundGroup.Image),
            Width = targetImageWidth,
            CellTemplate = imageTemplate
        });

        // The name of the group
        var groupTemplate = new DataGridViewTextBoxCell();
        groupTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        groupTemplate.Style.BackColor = SystemColors.Control;
        groupTemplate.Style.Font = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Point);
        
        this.dgvGroups.Columns.Add(new DataGridViewColumn()
        {
            Name = GroupNameColumnName,
            HeaderText = "Task",
            DataPropertyName = nameof(GridBoundGroup.Name),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            CellTemplate = groupTemplate
        });

        // The number of tasks
        var tasksTemplate = new DataGridViewTextBoxCell();
        tasksTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
        tasksTemplate.Style.BackColor = SystemColors.Control;
        tasksTemplate.Style.Font = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Point);

        this.dgvGroups.Columns.Add(new DataGridViewColumn()
        {
            Name = TaskCountColumnName,
            HeaderText = "Progress",
            DataPropertyName = nameof(GridBoundGroup.Count),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            CellTemplate = tasksTemplate
        });

        // An empty column, used to help control the other column's sizing            
        var rightSpacingColumnTemplate = new DataGridViewTextBoxCell();
        rightSpacingColumnTemplate.Style.BackColor = SystemColors.Control;
        this.dgvGroups.Columns.Add(new DataGridViewColumn()
        {
            Name = RightSpacingColumnName,
            DataPropertyName = nameof(GridBoundGroup.Spacer),
            HeaderText = "",
            CellTemplate = rightSpacingColumnTemplate
        });
    }

    /// <inheritdoc />
    public override Action<IDiagnosticTaskGroup, IDiagnosticTask> GetOnTaskCompletedCallback()
    {
        return (taskGroup, task) => 
        {
            if (this.currentEngine is not null)
            {
                UpdateGridViewRows(this.currentEngine, taskGroup);
            }
        };
    }

    /// <inheritdoc />
    public override void SetEngine(GpEngine engine)
    {
        this.currentEngine = engine;
        UpdateGridView(this.currentEngine);
        this.ResizeColumnsBasedOnContent();
    }

    private void ResizeColumnsBasedOnContent()
    {
        // Adjust column width to "center" the columns
        var fullWidth = this.dgvGroups.Width;
        var groupNameWidth = this.dgvGroups.Columns[GroupNameColumnName].Width;
        var imageWidth = this.dgvGroups.Columns[ImageColumnName].Width;
        var taskCountWidth = this.dgvGroups.Columns[TaskCountColumnName].Width;

        // Adjust spacing and task columns to fill the rest of the control. 
        var availableSpace = fullWidth - groupNameWidth - imageWidth - taskCountWidth;

        this.dgvGroups.SuspendLayout();
        this.dgvGroups.Columns[LeftSpacingColumnName].Width = (availableSpace / 2) - 16;
        this.dgvGroups.Columns[RightSpacingColumnName].Width = (availableSpace / 2) - 16;
        this.dgvGroups.ResumeLayout();
        this.dgvGroups.Refresh();
    }

    private void UpdateGridView(GpEngine engine)
    {
        this.dgvGroups.SuspendLayout();
        this.bindingList.Clear();

        var taskGroups = engine?.TaskGroups ?? Enumerable.Empty<IDiagnosticTaskGroup>();

        // Order the groups, placing general task groups first, and multicompany groups next.
        var orderedTaskGroups = taskGroups.OrderBy(x => x switch
        {
            IMultiCompanyDiagnosticTaskGroup => 2,
            IDiagnosticTaskGroup => 1,
            _ => int.MaxValue,
        });
        
        foreach (var group in orderedTaskGroups)
        {
            var evaluatedTasks = group.Tasks.Where(x => x.IsEvaluated);

            var gridBoundItem = new GridBoundGroup()
            {
                Name = string.Format("{0}: {1}", group.DisplayName, group.Name),
                Count = $"{evaluatedTasks.Count()} / {group.Tasks.Count()}",
                TaskGroup = group,
            };

            this.bindingList.Add(gridBoundItem);
        }

        this.dgvGroups.DataSource = this.bindingList;
        this.dgvGroups.ClearSelection();
        this.dgvGroups.ResumeLayout();
        this.dgvGroups.Refresh();
    }

    private void UpdateGridViewRows(GpEngine engine, IDiagnosticTaskGroup taskGroup)
    {
        var boundGroup = this.bindingList.FirstOrDefault(x => x.TaskGroup == taskGroup);
        if (boundGroup != null)
        {
            boundGroup.UpdateCount();
            this.dgvGroups.InvalidateRow(this.bindingList.IndexOf(boundGroup));
        }
    }

    public override void EvaluationStarted()
    {
        foreach (var group in bindingList)
        {
            group.Image = TaskViewImages.failedImage;
        }
    }

    private void DgvGroups_Resize(object? sender, EventArgs e)
    {
        this.ResizeColumnsBasedOnContent();
    }

    /// <summary>
    /// Represents a record of the data set that is bound to the grid view.
    /// </summary>
    private class GridBoundGroup: INotifyPropertyChanged
    {
        private Image image = TaskViewImages.notStartedImage;
        private string status = string.Empty;
        private TaskGroupState executionState = TaskGroupState.NotStarted;
       

        public required IDiagnosticTaskGroup TaskGroup { get; init; }
        public string Spacer => string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Count {
            get => status; 
            set
            {
                status = value;
                RaisePropertyEventChanged(nameof(Count));
            }
        }
        public Image Image { 
            get => this.image; 
            set
            {
                this.image = value;
                RaisePropertyEventChanged(nameof(Image));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyEventChanged(string name) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void UpdateCount()
        {
            var evalutedTaskCount = this.TaskGroup.Tasks.Count(x => x.IsEvaluated);
            var allTaskCount = this.TaskGroup.Tasks.Count();
            this.Count = $"{evalutedTaskCount} / {allTaskCount}";

            if (evalutedTaskCount == allTaskCount)
            {
                this.executionState = TaskGroupState.Completed;
                this.image = TaskViewImages.completedImage;
            }
            else if (this.executionState == TaskGroupState.NotStarted && evalutedTaskCount > 0)
            {
                this.executionState = TaskGroupState.Running;
                this.Image = TaskViewImages.failedImage;
            }
        }
    }
}
