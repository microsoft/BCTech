using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class UpdateSettings
{
     internal static async Task GetUpdateWindowAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/settings/upgrade");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task SetUpdateWindowAsync(string accessToken, string environmentName, DateTime preferredStartTime, DateTime preferredEndTime)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = new {
            PreferredStartTimeUtc = preferredStartTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
            PreferredEndTimeUtc = preferredEndTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PutAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/settings/upgrade", content);

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

       internal static async Task GetScheduledUpdatesAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/upgrade");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }
}