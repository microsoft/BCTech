// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Web;
    using Azure.Relay;
    using Common;

    internal class PluginMethod
    {
        internal string HttpMethod { get; }

        internal IAgentPlugin Plugin { get; }

        internal MethodInfo MethodInfo { get; }

        internal string Name => this.MethodInfo.Name;

        private readonly ParameterInfo[] parameters;
        private readonly Func<string, object>[] parameterConverters;

        private readonly ParameterInfo returnParameter;

        internal static PluginMethod Create(IAgentPlugin plugin, MethodInfo methodInfo)
        {
            PluginMethodAttribute methodAttribute = methodInfo.GetCustomAttribute<PluginMethodAttribute>();
            if (!IsValidMethodAttribute(methodAttribute))
            {
                return null;
            }

            if (!IsValidSignature(methodInfo))
            {
                return null;
            }

            return new PluginMethod(plugin, methodAttribute.HttpMethod, methodInfo);
        }

        private PluginMethod(IAgentPlugin plugin, string httpMethod, MethodInfo methodInfo)
        {
            this.HttpMethod = httpMethod;
            this.MethodInfo = methodInfo;
            this.parameters = this.MethodInfo.GetParameters();
            this.parameterConverters = CreateParameterConverters();

            this.returnParameter = this.MethodInfo.ReturnParameter;
            this.Plugin = plugin;
        }

        private Func<string, object>[] CreateParameterConverters()
        {
            var parameterConverters = new Func<string, object>[this.parameters.Length];
            for (int i = 0; i < this.parameters.Length; i++)
            {
                Type parameterType = this.parameters[i].ParameterType;
                if (parameterType == typeof(string))
                {
                    parameterConverters[i] = ToString;
                }
                else if (parameterType == typeof(decimal))
                {
                    parameterConverters[i] = ToDecimal;
                }
                else if (parameterType == typeof(int))
                {
                    parameterConverters[i] = ToInt;
                }
                else if (parameterType == typeof(long))
                {
                    parameterConverters[i] = ToInt64;
                }
            }

            return parameterConverters;
        }

        /// <summary>
        /// Invoke the method using reflection.
        /// </summary>
        /// <param name="context">The listener context</param>
        internal void Invoke(RelayedHttpListenerContext context)
        {
            var result = this.MethodInfo.Invoke(this.Plugin, this.PrepareArguments(context.Request.Url.Query));
            if (result != null)
            {
                using var sw = new StreamWriter(context.Response.OutputStream);
                sw.WriteLine(result.ToString());
            }
        }

        private static readonly object[] EmptyParameters = [];

        /// <summary>
        /// Create an object array with converted parameters.
        /// </summary>
        /// <param name="query">The request QueryString</param>
        /// <returns>Object array with converted parameters</returns>
        private object[] PrepareArguments(string query)
        {
            NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(query);
            if (this.parameters.Length == 0)
            {
                if (query.Length > 0)
                {
                    // TODO: Warn for excess parameters
                }

                return EmptyParameters;
            }

            List<Object> arguments = [];
            for (int i = 0; i < this.parameters.Length; i++)
            {
                ParameterInfo parameter = this.parameters[i];
                string value = nameValueCollection.Get(parameter.Name) ?? throw new ArgumentException($"Parameter '{parameter.Name}' not found.");
                arguments.Add(this.parameterConverters[i](value));
            }

            return [.. arguments];
        }

        /// <summary>
        /// Validate the method attribute for supported Http Method
        /// </summary>
        /// <param name="methodAttribute">The plugin method attribute</param>
        /// <returns>true if the attribute is valid and supported,</returns>
        private static bool IsValidMethodAttribute(PluginMethodAttribute methodAttribute)
        {
            if (methodAttribute == null)
            {
                return false;
            }

            switch (methodAttribute.HttpMethod)
            {
                case "GET":
                case "PUT":
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the method signature is valid and supported by the runtime.
        /// </summary>
        /// <param name="methodInfo">Method Info</param>
        /// <returns>true - if the method is supported by the runtime.</returns>
        private static bool IsValidSignature(MethodInfo methodInfo)
        {
            if (methodInfo.IsGenericMethod)
            {
                return false;
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo.IsOut)
                {
                    return false;
                }

                if (parameterInfo.ParameterType != typeof(string) && 
                    parameterInfo.ParameterType != typeof(decimal) &&
                    parameterInfo.ParameterType != typeof(long) &&
                    parameterInfo.ParameterType != typeof(int))
                {
                    return false;
                }
            }

            return true;
        }

        #region Parameter value converters

        private static object ToInt(string value)
        {
            if (!int.TryParse(value, out int i))
            {
                return null;
            }

            return i;
        }

        private static object ToInt64(string value)
        {
            if (!long.TryParse(value, out long l))
            {
                return null;
            }

            return l;
        }

        private static object ToDecimal(string value)
        {
            if (!decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal d))
            {
                return null;
            }

            return d;
        }

        private static object ToString(string value)
        {
            return value;
        }

        #endregion

        public override string ToString()
        {
            return this.MethodInfo.ToString();
        }
    }
}
