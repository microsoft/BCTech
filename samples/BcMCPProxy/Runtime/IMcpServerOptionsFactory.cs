namespace BcMCPProxy.Runtime
{
    using BcMCPProxy.Models;
    using ModelContextProtocol.Client;
    using ModelContextProtocol.Server;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMcpServerOptionsFactory
    {
        public McpServerOptions GetMcpServerOptions(ConfigOptions configOptions, IMcpClient mcpClient);
    }
}
