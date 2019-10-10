// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace BCAgent
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Business Central Agent");
            CommandLineArguments arguments = CommandLineParser.Parse(args, AppContext.BaseDirectory);
            
            // TODO: argument validating and exit on invalid.

            Console.WriteLine("Press 'q' to quit.");
            var cts = new CancellationTokenSource();
            Task.Run(() => RequestDispatcher.Start(
                arguments.RelayNamespace,
                arguments.HybridConnectionName,
                arguments.KeyName,
                arguments.SharedAccessKey,
                arguments.PluginFolder,
                new ConsoleLogger(), // TODO: LogLevel support 
                cts.Token
            ), cts.Token);

            while (Console.ReadKey().KeyChar != 'q')
            {
                   
            }

            cts.Cancel();
        }
    }
}
