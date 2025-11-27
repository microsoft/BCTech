// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace BCLocalService
{
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;
    using System;
    using System.Diagnostics;

    internal class ServiceLogger : ILogger
    {
        public const string EventSource = "BC local agent";
        public const string LogName = "Business Central local agent";
        private readonly EventLog eventLog;

        internal ServiceLogger()
        {
            eventLog = new EventLog();
            if (!EventLog.SourceExists(EventSource))
            {
                EventLog.CreateEventSource(EventSource, LogName);
            }
            eventLog.Source = EventSource;
            eventLog.Log = LogName;
        }

        public void LogException(Exception e)
        {
            eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
        }

        public void LogMessage(LogLevel level, string message)
        {
            // TODO: Implement mapping between LogLevel and EventEntryType
            eventLog.WriteEntry(message /*, eventEntryType */);
        }
    }
}
