using System;
using System.Threading.Tasks;

namespace CSharp
{
    class Program
    {
        // Authentication parameters
        const string aadAppId = "a19cb26a-2e4c-408b-82e1-6311742ecc50";         // partner's AAD app id
        const string aadAppRedirectUri = "http://localhost";                    // partner's AAD app redirect URI
        const string aadTenantId = "8c8dbccd-c171-4937-a134-e3c5a5dd0470";      // customer's tenant id

        static async Task Main(string[] args)
        {
            // Get an access token that we can use for making calls to the Business Central Admin Center APIs
            string accessToken = await Authenticate.GetAccessTokenAsync(aadAppId, aadAppRedirectUri, aadTenantId);

            // Manage environments
            await Environments.ListEnvironmentsAsync(accessToken);
            await Environments.CreateNewEnvironmentAsync(accessToken, "MyNewSandbox2", "Sandbox", "DK");
            await Environments.CopyProductionEnvironmentToSandboxEnvironmentAsync(accessToken, "MyProd", "MyNewSandboxAsACopy", "DK");
            await Environments.SetAppInsightsKeyAsync(accessToken, "MyProd", new Guid("0da21b54-841e-4a64-a117-6092784245f9"));
            await Environments.GetDatabaseSizeAsync(accessToken, "MyProd");
            await Environments.GetSupportSettingsAsync(accessToken, "MyProd");

            // Manage support settings
            await NotificationRecipients.GetNotificationRecipientsAsync(accessToken);
            await NotificationRecipients.AddNotificationRecipientAsync(accessToken, "partnernotifications@partnerdomain.com", "Partner Notifications Mail Group");

            // Manage apps
            await Apps.GetInstalledAppsAsync(accessToken, "MyProd");
            await Apps.GetAvailableAppUpdatesAsync(accessToken, "MyProd");
            await Apps.UpdateAppAsync(accessToken, "MyProd", "334ef79e-547e-4631-8ba1-7a7f18e14de6", "16.0.11240.12188");
            await Apps.GetAppOperationsAsync(accessToken, "MyProd", "334ef79e-547e-4631-8ba1-7a7f18e14de6");
        }
    }
}
