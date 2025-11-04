namespace BcMCPProxy.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAuthenticationService
    {
        public Task<string> AcquireTokenAsync();
    }
}
