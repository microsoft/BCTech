using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class NotificationRecipients
{
     internal static async Task GetNotificationRecipientsAsync(string accessToken)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync("https://api.businesscentral.dynamics.com/admin/v2.1/settings/notification/recipients");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task AddNotificationRecipientAsync(string accessToken, string emailAddress, string name)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = new {
            Email = emailAddress,
            Name = name
        };
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PutAsync("https://api.businesscentral.dynamics.com/admin/v2.1/settings/notification/recipients", content);

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }
}