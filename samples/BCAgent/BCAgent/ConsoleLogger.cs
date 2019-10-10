// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace BCAgent
{
    using System;
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;

    /// <summary>
    /// ILogger implementation that writes to console.
    /// </summary>
    class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
        }

        public void LogException(Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        public void LogMessage(LogLevel level, string message)
        {
            Console.WriteLine(message);
        }
    }
}
