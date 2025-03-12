// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace CalculatorPlugin
{
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;

    [AgentPlugin("calculator/V1.0")]
    public class Calculator : IAgentPlugin
    {
        [PluginMethod("GET")]
        public static decimal Add(decimal a, decimal b)
        {
            return a + b;
        }

        [PluginMethod("GET")]
        public static decimal Subtract(decimal a, decimal b)
        {
            return a - b;
        }
    }
}
