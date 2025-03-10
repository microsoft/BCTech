// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.Common
{
    using System;
    using System.Composition;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AgentPluginAttribute(string rootPath) : ExportAttribute(typeof(IAgentPlugin))
    {
        public string RootPath { get; } = rootPath;
    }
}
