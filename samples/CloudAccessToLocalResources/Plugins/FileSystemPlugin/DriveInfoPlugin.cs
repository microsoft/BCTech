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

    [AgentPlugin("filesystem/V1.0/driveinfo")]
    public class DriveInfoPlugIn: IAgentPlugin
    {
        [PluginMethod("GET")]
        public string GetDrives()
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                JsonSerializer.Create().Serialize(sw, DriveInfo.GetDrives().Select(d => d.Name).ToArray());
            }

            return sb.ToString();
        }

        [PluginMethod("GET")]
        public long GetAvailableFreeSpace(string driveName)
        {
            DriveInfo driveInfo = new DriveInfo(driveName);
            return driveInfo.AvailableFreeSpace;
        }
    }
}
