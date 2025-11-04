namespace BcMCPProxy.Auth
{
    using BcMCPProxy.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AuthenticationServiceFactory : IAuthenticationServiceFactory
    {
        private readonly ConfigOptions configOptions;

        public AuthenticationServiceFactory(IOptions<ConfigOptions> options)
        {
            this.configOptions = options.Value;
        }

        public IAuthenticationService GetAuthenticationService(ILoggerFactory loggerFactory, string[] scopes, string environmentId, string cacheName, string clientId)
        {
            return new AuthenticationService(loggerFactory, scopes, configOptions, environmentId, cacheName, clientId);
        }
    }
}
