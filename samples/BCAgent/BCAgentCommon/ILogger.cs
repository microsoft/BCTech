// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.Common
{
    using System;

    /// <summary>
    /// Interface for a Logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log message 
        /// </summary>
        /// <param name="level">LogLevel for message</param>
        /// <param name="message">Message to log</param>
        void LogMessage(LogLevel level, string message);

        /// <summary>
        /// Log exception.
        /// </summary>
        /// <param name="e">Exception to log.</param>
        void LogException(Exception e);
    }
}
