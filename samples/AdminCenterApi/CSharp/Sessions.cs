using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Sessions
{
     internal static async Task GetActiveSessionsAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/sessions");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task CancelSessionAsync(string accessToken, string environmentName, int sessionId)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.DeleteAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/sessions/{sessionId}");

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
    }
}