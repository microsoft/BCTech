namespace BcMCPProxy.Auth
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAuthenticationServiceFactory
    {
        public IAuthenticationService GetAuthenticationService(ILoggerFactory loggerFactory, string[] scopes, string environmentId, string cacheName, string clientId);
    }
}
