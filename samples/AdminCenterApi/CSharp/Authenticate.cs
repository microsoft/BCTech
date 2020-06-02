using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

class Authenticate
{
    // This sample authenticates to Azure Active Directory (AAD) an obtains an access token.
    // The access token can be used for authenticating to Business Central APIs.

    internal static async Task<string> GetAccessTokenAsync(string aadAppId, string aadAppRedirectUri, string aadTenantId)
    {
         // Get access token
        IPublicClientApplication app = PublicClientApplicationBuilder.Create(aadAppId)
            .WithAuthority(AzureCloudInstance.AzurePublic, aadTenantId)
            .WithRedirectUri(aadAppRedirectUri)
            .Build();

        AuthenticationResult authResult = await app.AcquireTokenInteractive(new string[] { "https://api.businesscentral.dynamics.com/.default" }).ExecuteAsync();
        string accessToken = authResult.AccessToken;

        Console.WriteLine("Authentication complete. Access token is:");
        Console.WriteLine(accessToken);

        // Peek inside the access token (this is just for education purposes; in actual API calls we'll just pass it as one long string)
        string middlePart = accessToken.Split('.')[1];
        string middlePartPadded = middlePart + "".PadLeft(4-middlePart.Length%4, '=');
        string middlePartDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(middlePartPadded));
        string middlePartDecodedPretty = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(middlePartDecoded), Formatting.Indented);
        Console.WriteLine("Contents of the access token:");
        Console.WriteLine(middlePartDecodedPretty);

        return accessToken;
    }
}