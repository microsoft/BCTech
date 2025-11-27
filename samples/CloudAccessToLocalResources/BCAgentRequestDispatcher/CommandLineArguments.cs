// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher
{
    using Common;

    public class CommandLineArguments
    {
        public string RelayNamespace { get; set; }
        public string HybridConnectionName { get; set; }
        public string KeyName { get; set; }
        public string SharedAccessKey { get; set; }
        public string PluginFolder { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
