namespace Microsoft.GP.MigrationDiagnostic.UI;

using Microsoft.Extensions.Logging;
using Microsoft.GP.MigrationDiagnostic.Analysis;
using Microsoft.GP.MigrationDiagnostic.TaskProcessing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using Views;
using Views.TaskGroup;

public partial class MainWindow : Form
{
    private readonly SynchronizationContext? synchContext;
    private readonly GpEngine engine;
    private readonly TaskProcessorFactory taskProcessorFactory;
    private readonly IList<TaskViewControl> taskViews = new List<TaskViewControl>();
    private readonly ILogger logger;
    private readonly object _statusLock = new object();
    private CancellationTokenSource taskExecutionCancellationSource = new CancellationTokenSource();
    private readonly ConcurrentBag<(IDiagnosticTask task, Exception exception)> exceptionTasks = new ConcurrentBag<(IDiagnosticTask task, Exception exception)>();
    private SemaphoreSlim tasksProcessing = new SemaphoreSlim(1, 1);
    private SemaphoreSlim tasksCancelling = new SemaphoreSlim(1, 1);

    private int completedTaskCount = 0;
  

    public MainWindow(
        GpEngine engine,
        TaskProcessorFactory taskProcessorFactory,
        TaskGroupView taskView,
        IssuesView issueView,
        ILogger<MainWindow> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.engine = engine;
        this.taskProcessorFactory = taskProcessorFactory;
        InitializeComponent();
        InitializeMenuEvents();

        this.taskViews.Add(taskView ?? throw new ArgumentNullException(nameof(taskView)));
        this.taskViews.Add(issueView ?? throw new ArgumentNullException(nameof(issueView)));

        foreach (var view in this.taskViews)
        {
            view.Dock = DockStyle.Fill;
        }

        this.tabSummary.Controls.Add(taskView);
        this.tabIssues.Controls.Add(issueView);

        // Set synch context
        this.synchContext = SynchronizationContext.Current;

        // Set the task view
        this.tabTaskViews.Refresh();

        this.UpdateStatusLabel(string.Empty);

        RefreshTaskViewEngines();

        this.tabTaskViews.Refresh();
    }

    private void InitializeMenuEvents()
    {
        // File menu items
        this.printPreviewToolStripMenuItem.Click += PrintPreviewToolStripMenuItem_Click;

        this.runEvaluationMenuItem.Click += runEvaluationMenuItem_Click;

        this.databaseSettings.Click +=
           (object? sender, EventArgs e) =>
           {
                this.ShowConfiguration();
           };
    }

    private void Main_Load(object sender, EventArgs e)
    {
        this.ShowConfiguration();
    }

    private void runEvaluationMenuItem_Click(object? sender, EventArgs e)
    {
        StartTaskProcessing();
    }

    private void btnRunEvaluation_Click(object sender, EventArgs e)
    {
        StartTaskProcessing();
    }

    private async void StartTaskProcessing()
    {
        // If an operation is in progress - ignore the request to run
        if (tasksProcessing.CurrentCount == 0 || tasksCancelling.CurrentCount == 0)
        {
            return;
        }

        await tasksProcessing.WaitAsync();

        try
        {
            var processor = this.taskProcessorFactory();

            // Reset the cancellation token
            this.taskExecutionCancellationSource = new CancellationTokenSource();

            // Reset task group data
            this.engine.ResetTaskGroups();

            this.synchContext?.Post(x =>
            {
                // Reset the view to use the reset task groups
                RefreshTaskViewEngines();
            }, null);

            var taskCount = this.engine.TaskGroups.SelectMany(x => x.Tasks).Count();

            // Task start
            processor.StartProcessing(
                this.engine,
                GetProcessingStartCallback(taskCount),
                GetTaskCompletionCallback(taskCount),
                GetTaskExceptionCallback(),
                GetProcessingCompletedCallback(),
                this.taskExecutionCancellationSource.Token);
        }
        catch
        {
            tasksProcessing.Release();
        }
        
    }

    private Action GetProcessingStartCallback(int taskCount)
    {
        return () =>
        {
            this.synchContext?.Post(x =>
            {
                foreach (var view in this.taskViews)
                {
                    view.EvaluationStarted();
                }

                SetRunEvalControlsEnabled(false);
                ClearStatusRunningTaskCount(taskCount);
            }, null);
        };
    }

    private Action<IDiagnosticTaskGroup, IDiagnosticTask> GetTaskCompletionCallback(int taskCount)
    {
        return (taskGroup, task) =>
        {
            var taskViewCallbacks = this.taskViews.Select(x => x.GetOnTaskCompletedCallback());

            foreach (var callback in taskViewCallbacks)
            {
                // Need to do some hand-waving to get the params packed and passed through cleanly.
                this.synchContext?.Post(param => 
                {
                    if (param is (IDiagnosticTaskGroup taskGroup, IDiagnosticTask task))
                    {
                        callback(taskGroup, task);
                    }
                }, (taskGroup, task));
            }

            this.synchContext?.Post(x =>
            {
                IncrementStatusRunningTaskCount(taskCount);
            }, null);
        };
    }

    private Action<IDiagnosticTask, Exception> GetTaskExceptionCallback()
    {
        return (IDiagnosticTask task, Exception exception) =>
        {
            this.exceptionTasks.Add((task, exception));
        };
    }

    private Action GetProcessingCompletedCallback()
    {
        return () =>
        {
            // process completion
            string resultText = string.Empty;

            if (this.taskExecutionCancellationSource.IsCancellationRequested)
            {
                resultText = "Evaluation was canceled.";
            }
            if (exceptionTasks.Any())
            {
                resultText = $"Exceptions occurred while processing tasks. {exceptionTasks.Count()} exceptions encountered.";
            }
            else
            {
                resultText = "Evaluation complete, ready for reporting.";
            }

            foreach (var exceptionTask in this.exceptionTasks)
            {
                var exception = exceptionTask.exception;
                var diagnosticTask = exceptionTask.task;
                this.logger.LogError(exception, "Exception found while running tasks. ID: {0}, Description: {1}", diagnosticTask.UniqueIdentifier, diagnosticTask.Description);
            }

            this.synchContext?.Post(x =>
            {
                SetRunEvalControlsEnabled(true);

                UpdateStatusLabel(resultText);

                if (!this.taskExecutionCancellationSource.IsCancellationRequested)
                {
                    SetReportControlsEnabledState(true);
                    this.tabTaskViews.SelectTab(this.tabIssues);
                }
            }, null);

            tasksProcessing.Release();
        };
    }

    private async void btnCancelEvaluation_Click(object sender, EventArgs e)
    {
        if (tasksProcessing.CurrentCount == 0)
        {
            await tasksCancelling.WaitAsync();
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var result = MessageBox.Show("Would you like to cancel the running evaluation?", "Cancel evaluation?", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    btnCancelEvaluation.Enabled = false;

                    this.taskExecutionCancellationSource.Cancel();

                    // Wait until the task processing operation gives up its semaphore, this ensures the cancel operation 'blocks'
                    await tasksProcessing.WaitAsync();
                    SetRunEvalControlsEnabled(true);
                }
            }
            finally
            {
                tasksCancelling.Release();
                tasksProcessing.Release();
                this.Cursor = Cursors.Default;
            }
        }
    }

    private void SetRunEvalControlsEnabled(bool enabled)
    {
        this.runEvaluationMenuItem.Enabled = enabled;
        this.btnRunEvaluation.Visible = enabled;
        this.btnRunEvaluation.Enabled = enabled;

        this.btnCancelEvaluation.Visible = !enabled;
        this.btnCancelEvaluation.Enabled = !enabled;
    }

    private void SetReportControlsEnabledState(bool enabled)
    {
        this.printPreviewToolStripMenuItem.Enabled = enabled;
    }

    private void PrintPreviewToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        var jsonString = JsonSerializer.Serialize(this.engine, new JsonSerializerOptions
        {
            IncludeFields = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        var fileDialog = new SaveFileDialog();
        fileDialog.Title = "Please choose a save location for your results preview.";
        fileDialog.FileName = "DiagnosticResultsPreview.json";
        fileDialog.DefaultExt = "json";
        fileDialog.Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*";
        fileDialog.AddExtension = true;

        var result = fileDialog.ShowDialog();

        if (result == DialogResult.OK)
        {
            System.IO.File.WriteAllText($"{fileDialog.FileName}", jsonString);
        }
    }

    private void UpdateStatusLabel(string s)
    {
        this.statusStrip.Text = s;
        currentStateStatusLabel.Text = s;
        this.statusStrip.Refresh();
    }

    private void ClearStatusRunningTaskCount(int maxTaskCount)
    {
        lock (_statusLock)
        {
            completedTaskCount = 0;
            UpdateStatusLabel($"Running... { completedTaskCount } / { maxTaskCount }");
        }
    }

    private void IncrementStatusRunningTaskCount(int maxTaskCount)
    {
        lock (_statusLock)
        {
            completedTaskCount++;
            UpdateStatusLabel($"Running... { completedTaskCount } / { maxTaskCount }");
        }
    }

    private void RefreshTaskViewEngines()
    {
        foreach (var view in this.taskViews)
        {
            view.SetEngine(this.engine);
        }

        this.runEvaluationMenuItem.Enabled = this.engine.IsConfigured;
        this.btnRunEvaluation.Enabled = this.engine.IsConfigured;
    }

    private void ShowConfiguration()
    {
        try
        {
            var configPage = this.engine.GetConfigurationView();
            configPage.ShowConfiguration(this);
        }
        catch (NotImplementedException)
        {
            this.logger.LogWarning("There is no configuration required for the selected task set.");
        }

        this.RefreshTaskViewEngines();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var aboutBox = new AboutBox();
        aboutBox.Show(this);
    }
}
