namespace BcMCPProxy.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Net.WebRequestMethods;

    public  class ConfigOptions
    {
        public string ServerName = "BcMCPProxy";
        public string ServerVersion = "1.0.0";

        public string TenantId { get; set; } = "9c4a03c7-2908-4bfc-9258-af63220f534a";

        public string ClientId { get; set; } = "3acde393-18cc-4b12-803c-4c85fa111c21";

        public string TokenScope { get; set; } = "https://api.businesscentral.dynamics.com/.default";

        public string Url { get; set; } = "https://api.businesscentral.dynamics.com";

        public string Environment { get; set; } = "Production";

        public string Company { get; set; }

        public string ConfigurationName { get; set; }

        public string CustomAuthHeader { get; set;}

        public bool Debug { get; set; }
        public bool EnableHttpLogging { get; set; }
        public bool EnableMsalLogging { get; set; }
    }
}
