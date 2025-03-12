// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

namespace FileSystemPlugin
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Dynamics.BusinessCentral.Agent.Common;
    using Newtonsoft.Json;

    [AgentPlugin("filesystem/V1.0/directoryinfo")]
    public class DirectoryInfoPlugIn : IAgentPlugin
    {
        [PluginMethod("GET")]
        public static string GetFiles(string path, string searchPattern)
        {
            DirectoryInfo directoryInfo = new(path);

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                JsonSerializer.Create().Serialize(sw, directoryInfo.GetFiles(searchPattern).Select(d => d.Name).ToArray());
            }

            return sb.ToString();
        }

        [PluginMethod("GET")]
        public static string GetDirectories(string path, string searchPattern)
        {
            DirectoryInfo directoryInfo = new(path);

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                JsonSerializer.Create().Serialize(sw, directoryInfo.GetDirectories(searchPattern).Select(d => d.Name).ToArray());
            }

            return sb.ToString();
        }


        [PluginMethod("GET")]
        public static string GetDirectoryItems(string path, string searchPattern)
        {
            DirectoryInfo directoryInfo = new(path);

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                JsonSerializer.Create().Serialize(sw, directoryInfo.GetFileSystemInfos(searchPattern)
                    .Select(d => new { d.Name, d.FullName, IsDirectory = d.Attributes.HasFlag(FileAttributes.Directory), Created = d.CreationTime })
                    .ToArray());
            }

            return sb.ToString();
        }

    }
}
