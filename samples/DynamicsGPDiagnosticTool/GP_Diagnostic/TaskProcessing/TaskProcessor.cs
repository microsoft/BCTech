namespace Microsoft.GP.MigrationDiagnostic.TaskProcessing;

using Microsoft.Extensions.Logging;
using Microsoft.GP.MigrationDiagnostic.Analysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class TaskProcessor
{
    private const int MaximumConcurrentTaskThreads = 25;
    private const int TaskTimeout = 5 * 60 * 1000;

    private readonly SemaphoreSlim taskRunLock = new SemaphoreSlim(1, 1);
    private readonly ILogger<TaskProcessor> logger;
    private BackgroundWorker worker = new BackgroundWorker();

    public TaskProcessor(ILogger<TaskProcessor> logger)
    {
        this.worker.DoWork += worker_DoWork;
        this.worker.RunWorkerCompleted += (s, e) => { this.taskRunLock.Release(1); };
        this.logger = logger;
    }

    public void StartProcessing(
        GpEngine engine,
        Action executionStartCallback,
        Action<IDiagnosticTaskGroup, IDiagnosticTask> completedTaskCallback,
        Action<IDiagnosticTask, Exception> exceptionTaskCallback,
        Action executionCompleteCallback,
        CancellationToken cancellationToken)
    {
        if (taskRunLock.Wait(0))
        {
            if (!this.worker.IsBusy)
            {
                this.worker.RunWorkerAsync(new TaskExecutionWorkerArguments()
                {
                    cancellationToken = cancellationToken,
                    completedTaskCallback = completedTaskCallback,
                    executionStartCallback = executionStartCallback,
                    exceptionTaskCallback = exceptionTaskCallback,
                    executionCompleteCallback = executionCompleteCallback,
                    engine = engine,
                    logger = this.logger,
                });
            }
        }
    }

    private static async void worker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (sender is BackgroundWorker
            && e.Argument is TaskExecutionWorkerArguments execArgs)
        {
            await RunEvaluation(
                execArgs.engine,
                execArgs.logger,
                execArgs.executionStartCallback,
                execArgs.completedTaskCallback,
                execArgs.exceptionTaskCallback,
                execArgs.executionCompleteCallback,
                execArgs.cancellationToken);
        }
    }

    private static async Task RunEvaluation(
        GpEngine engine,
        ILogger<TaskProcessor> logger,
        Action executionStartCallback,
        Action<IDiagnosticTaskGroup, IDiagnosticTask> completedTaskCallback,
        Action<IDiagnosticTask, Exception> exceptionTaskCallback,
        Action executionCompleteCallback,
        CancellationToken cancellationToken)
    {
        var exceptionTasks = new ConcurrentBag<(IDiagnosticTask analysisTask, Task task)>();
        var allTaskCount = engine.TaskGroups.Aggregate(0, (total, group) => total + group.Tasks.Count());

        executionStartCallback();

        foreach (var group in engine.TaskGroups)
        {
            var groupTasks = group.Tasks;
            var taskQueue = new ConcurrentQueue<IDiagnosticTask>(groupTasks);
            var runningTasks = new List<Task>();

            // While any tasks are in the queue or are running, process them.
            while ((taskQueue.Count > 0 || runningTasks.Any()) && !cancellationToken.IsCancellationRequested)
            {
                var runningTaskCount = runningTasks.Where(x => !x.IsCompleted).Count();

                // While tasks are not yet complete, more exist in the queue, start another task
                if (runningTaskCount < MaximumConcurrentTaskThreads && taskQueue.Count() > 0)
                {
                    var tasksToQueue = Math.Min(taskQueue.Count, MaximumConcurrentTaskThreads - runningTaskCount);
                    for (int i = 0; i < tasksToQueue; i++)
                    {
                        await Task.Delay(10);
                        if (taskQueue.TryDequeue(out var task))
                        {
                            // Use a new linked token, so we can add a timeout condition to cancellation.
                            var taskCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                            taskCancellationTokenSource.CancelAfter(TaskTimeout);
                            var timeoutToken = taskCancellationTokenSource.Token;

                            var runningTask = task.StartAsync(timeoutToken)
                                .ContinueWith(x =>
                                {
                                    if (x.IsFaulted && x.Exception != null)
                                    {
                                        exceptionTaskCallback(task, x.Exception);
                                        logger.LogError(x.Exception, "Task exception.");
                                    }
                                    else
                                    {
                                        completedTaskCallback(group, task);
                                    }
                                });

                            runningTasks.Add(runningTask);
                        }
                    }

                    await Task.Delay(10);
                }
                else
                {
                    // Multiple tasks may be finished, remove them:
                    runningTasks.RemoveAll(x => x.IsCompleted);
                }
            }
        }

        executionCompleteCallback();
    }

    private struct TaskExecutionWorkerArguments
    {
        internal GpEngine engine;
        internal Action executionStartCallback;
        internal Action<IDiagnosticTaskGroup, IDiagnosticTask> completedTaskCallback;
        internal Action<IDiagnosticTask, Exception> exceptionTaskCallback;
        internal Action executionCompleteCallback;
        internal CancellationToken cancellationToken;
        internal ILogger<TaskProcessor> logger;
    }
}
