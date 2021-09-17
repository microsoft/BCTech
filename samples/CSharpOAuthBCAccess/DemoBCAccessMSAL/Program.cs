using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBCAccessMSAL
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RunAsync().GetAwaiter().GetResult();
        }

        public async Task RunAsync()
        {
            Console.WriteLine("START");

            var ClientId = "<Client Id>";
            var TenantId = "<Tenant Id>";
            string[] Scopes = new[] { "https://api.businesscentral.dynamics.com/Financials.ReadWrite.All" };

            var app = PublicClientApplicationBuilder.Create(ClientId)
                                                    .WithDefaultRedirectUri()
                                                    .WithTenantId(TenantId)
                                                    .Build();

            //    OPTIONAL: Create cache helper to persist token cache

            var CacheFileName = "msal_cache.dat";
            var CacheDir = @"MSAL_CACHE";
            var storageProperties = new StorageCreationPropertiesBuilder(CacheFileName, CacheDir).Build();

            var cacheHelper = await MsalCacheHelper.CreateAsync(storageProperties);
            cacheHelper.RegisterCache(app.UserTokenCache);

            //    Acquire token interactively

            var result = await app.AcquireTokenInteractive(Scopes).ExecuteAsync();

            Console.WriteLine($"ACCESS TOKEN (INTERACTIVE): [{result.ExpiresOn}] {result.AccessToken}");
            Console.WriteLine();

            await Task.Delay(TimeSpan.FromSeconds(5));

            //    Acquire token silently

            var accounts = await app.GetAccountsAsync();
            var silentResult = await app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault()).WithForceRefresh(true).ExecuteAsync();

            Console.WriteLine($"ACCESS TOKEN (SILENT): [{silentResult.ExpiresOn}] {silentResult.AccessToken}");
            Console.WriteLine();

            Console.WriteLine("FINISH");
        }
    }
}
