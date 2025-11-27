// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace HelloWorldPlugin
{
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;

    [AgentPlugin("hello")]
    public class HelloWorld : IAgentPlugin
    {
        [PluginMethod("GET")]
        public string SayHello(string name)
        {
            return $"Hello, {name}";
        }
    }
}
