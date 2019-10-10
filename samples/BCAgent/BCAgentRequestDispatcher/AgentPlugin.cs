// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher
{
    using System.Collections.Immutable;
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;
    using System.Linq;
    using System.Reflection;

    internal class AgentPlugin
    {
        private AgentPluginMetadata Metadata { get; }

        internal string RootPath { get; }

        private readonly ImmutableDictionary<string, ImmutableDictionary<string, PluginMethod>> methodsByHttpMethod;

        internal static AgentPlugin Create(AgentPluginMetadata metadata, IAgentPlugin plugin, ILogger logger)
        {
            logger.LogMessage(LogLevel.Verbose, $"Loading: {metadata.RootPath}");
            MethodInfo[] methodInfos = plugin.GetType().GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod | BindingFlags.Instance);
            if (methodInfos == null || methodInfos.Length == 0)
            {
                return null;
            }

            ImmutableDictionary<string, ImmutableDictionary<string, PluginMethod>> methodByVerb = methodInfos
                .Select(m => PluginMethod.Create(plugin, m))
                .Where(m => m != null)
                .GroupBy(m => m.HttpMethod)
                .ToImmutableDictionary(g => g.Key, g => g.ToImmutableDictionary(m => m.Name));

            if (methodByVerb.Count == 0)
            {
                logger.LogMessage(LogLevel.Warning, "No valid methods found.");
                return null;
            }

            foreach (var mbv in methodByVerb)
            {
                foreach (var method in mbv.Value)
                {
                    logger.LogMessage(LogLevel.Verbose, $"Exporting: {mbv.Key} {method.Key}");
                }

            }

            return new AgentPlugin(metadata, methodByVerb);
        }

        private AgentPlugin(AgentPluginMetadata metadata, ImmutableDictionary<string, ImmutableDictionary<string, PluginMethod>> methodsByHttpMethod)
        {
            this.Metadata = metadata;
            this.RootPath = metadata.RootPath;
            this.methodsByHttpMethod = methodsByHttpMethod;
        }

        internal PluginMethod FindMethod(string httpMethod, string methodName)
        {
            if (!this.methodsByHttpMethod.TryGetValue(httpMethod, out ImmutableDictionary<string, PluginMethod> methodsByName))
            {
                return null;
            }

            if (!methodsByName.TryGetValue(methodName, out PluginMethod method))
            {
                return null;
            }

            return method;
        }
    }
}
