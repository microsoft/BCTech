// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace BCLocalService
{
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            if (!EventLog.SourceExists(ServiceLogger.EventSource))
            {
                EventLog.CreateEventSource(ServiceLogger.EventSource, ServiceLogger.LogName);
            }

            base.OnBeforeInstall(savedState);
        }
    }
}
