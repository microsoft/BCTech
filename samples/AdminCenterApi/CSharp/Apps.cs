using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Apps
{
     internal static async Task GetInstalledAppsAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/apps");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

     internal static async Task GetAvailableAppUpdatesAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/apps/availableUpdates");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task UpdateAppAsync(string accessToken, string environmentName, string appId, string newAppVersion)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = new {
            TargetVersion = newAppVersion
        };
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/apps/{appId}/update", content);

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

      internal static async Task GetAppOperationsAsync(string accessToken, string environmentName, string appId)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/apps/{appId}/operations");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }
}