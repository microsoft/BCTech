using System;
using Azure.Identity;
using Microsoft.Dynamics.BusinessCentral.AdminCenter;

namespace CSharp
{
    class Program
    {
        static void Main()
        {
            // Create a token credential, which enables us to authenticate to the Business Central Admin Center APIs.
            //   Note 1: This will open the AAD login page in a browser window.
            //   Note 2: You can also skip passing in options altogether if you want to log into your own Business Central admin center, i.e., not a delegated admin scenario
            var interactiveBrowserCredentialOptions = new InteractiveBrowserCredentialOptions
            {
                ClientId = "a19cb26a-2e4c-408b-82e1-6311742ecc50",  // partner's AAD app id
                RedirectUri = new Uri("http://localhost"),          // partner's AAD app redirect URI
                TenantId = "f5b6b245-5dd2-4bf5-94d4-35ef04d73c6d",  // customer's tenant id
            };
            var tokenCredential = new InteractiveBrowserCredential(interactiveBrowserCredentialOptions);

            // Create the Admin Center client
            var adminCenterClient = new AdminCenterClient(tokenCredential);

            // Manage environments
            Environments.ListEnvironments(adminCenterClient);
            Environments.CreateNewEnvironment(adminCenterClient, "MySandbox", "Sandbox", "DK");
            Environments.CopyProductionEnvironmentToSandboxEnvironment(adminCenterClient, "MyProd", "MySandboxAsACopy");
            Environments.SetAppInsightsKey(adminCenterClient, "MyProd", new Guid("0da21b54-841e-4a64-a117-6092784245f9"));
            Environments.GetDatabaseSize(adminCenterClient, "MyProd");
            Environments.GetSupportSettings(adminCenterClient, "MyProd");

            // Manage support settings
            NotificationRecipients.GetNotificationRecipients(adminCenterClient);
            NotificationRecipients.AddNotificationRecipient(adminCenterClient, "partnernotifications@partnerdomain.com", "Partner Notifications Mail Group");

            // Manage apps
            Apps.GetInstalledApps(adminCenterClient, "MyProd");
            Apps.GetAvailableAppUpdates(adminCenterClient, "MyProd");
            Apps.UpdateApp(adminCenterClient, "MyProd", new Guid("334ef79e-547e-4631-8ba1-7a7f18e14de6"), "16.0.11240.12188");
            Apps.GetAppOperations(adminCenterClient, "MyProd", new Guid("334ef79e-547e-4631-8ba1-7a7f18e14de6"));

            // Manage active sessions
            Sessions.GetActiveSessions(adminCenterClient, "MyProd");
            Sessions.CancelSession(adminCenterClient, "MyProd", 196719);

            // Manage update settings
            UpdateSettings.GetUpdateWindow(adminCenterClient, "MyProd");
            UpdateSettings.SetUpdateWindow(adminCenterClient, "MyProd", new DateTime(2020, 06, 01, 4, 15, 0), new DateTime(2020, 06, 01, 11, 30, 0));
            UpdateSettings.GetScheduledUpdates(adminCenterClient, "MyProd");
        }
    }
}
