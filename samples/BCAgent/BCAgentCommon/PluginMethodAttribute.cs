// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.Common
{
    using System;
    using System.Composition;

    /// <summary>
    /// Attribute to expose a method through Azure Relay.
    /// </summary>
    /// <remarks>
    /// Attribute constructor
    /// </remarks>
    /// <param name="httpMethod">Http Method</param>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PluginMethodAttribute(string httpMethod) : Attribute
    {
        /// <summary>
        /// Specifies the HttpMethod supported by this method. E.g. GET, PUT, ...
        /// </summary>
        public string HttpMethod { get; } = httpMethod;
    }
}
