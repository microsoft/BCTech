using Microsoft.Dynamics.BusinessCentral.Agent.Common;
using Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace BCLocalService
{
    public partial class BCLocalService : ServiceBase
    {
        private CancellationTokenSource cancellationTokenSource;
        private ServiceLogger serviceLogger;

        public BCLocalService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serviceLogger = new ServiceLogger();

            serviceLogger.LogMessage(LogLevel.Normal, "Business Central local agent service started");
            serviceLogger.LogMessage(LogLevel.Normal, "Starting BCLocalService with arguments: " + ((args != null && args.Length !=0)  ? String.Join(",", args) : "No args"));
            cancellationTokenSource = new CancellationTokenSource();

            // TODO: Validate arguments
            CommandLineArguments arguments = CommandLineParser.Parse(args, AppContext.BaseDirectory);

            // TODO: handle errors when dispatcher does not start and stop the service
            var displatcherTask = Task.Run(() => RequestDispatcher.Start(
                arguments.RelayNamespace,
                arguments.HybridConnectionName,
                arguments.KeyName,
                arguments.SharedAccessKey,
                arguments.PluginFolder,
                serviceLogger, // TODO: LogLevel support 
                cancellationTokenSource.Token
            ), cancellationTokenSource.Token);

        }

        protected override void OnStop()
        {
            serviceLogger.LogMessage(LogLevel.Normal, "Business Central local agent service stopped");
            cancellationTokenSource.Cancel();
        }
    }
}
