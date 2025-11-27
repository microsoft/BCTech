// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Dynamics.BusinessCentral.Agent.RequestDispatcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Common;

    public class CommandLineParser
    {
        public static CommandLineArguments Parse(IEnumerable<string> args, string baseDirectory)
        {
            string relayNamespace = null;
            string hybridConnectionName = null;
            string keyName = null;
            string key = null;
            string pluginFolder = Path.Combine(baseDirectory, "plugins");
            LogLevel logLevel = LogLevel.Normal;

            foreach (string arg in args)
            {
                if (!TryParseOption(arg, out string name, out string value))
                {
                    // Warn?
                    continue;
                }

                // TODO: Missing error handling for missing values.
                switch (name.ToLowerInvariant())
                {
                    case "namespace":
                        relayNamespace = value;
                        break;
                    case "connectionname":
                        hybridConnectionName = value;
                        break;
                    case "keyname":
                        keyName = value;
                        break;
                    case "key":
                        key = value;
                        break;
                    case "loglevel":
                        if (value != null && Enum.TryParse(value, out LogLevel temp))
                        {
                            logLevel = temp;
                        }
                        break;
                    case "pluginfolder":
                        pluginFolder = value;
                        break;
                }
            }

            return new CommandLineArguments()
            {
                RelayNamespace = relayNamespace,
                HybridConnectionName = hybridConnectionName,
                KeyName = keyName,
                SharedAccessKey = key,
                PluginFolder = pluginFolder,
                LogLevel = logLevel
            };
        }

        private static bool IsOption(string arg) => !string.IsNullOrEmpty(arg) && (arg[0] == '/' || arg[0] == '-');

        internal static bool TryParseOption(string arg, out string name, out string value)
        {
            if (!IsOption(arg))
            {
                name = null;
                value = null;
                return false;
            }

            int colon = arg.IndexOf(':');

            if (colon >= 0)
            {
                name = arg.Substring(1, colon - 1);
                value = arg.Substring(colon + 1);
            }
            else
            {
                name = arg.Substring(1);
                value = null;
            }

            name = name.ToLowerInvariant();
            return true;
        }

    }
}
