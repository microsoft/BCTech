using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Environments
{
     internal static async Task ListEnvironmentsAsync(string accessToken)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync("https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task CreateNewEnvironmentAsync(string accessToken, string newEnvironmentName, string environmentType, string countryCode)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = new {
            EnvironmentType = environmentType,
            CountryCode = countryCode
        };
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PutAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{newEnvironmentName}", content);

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task CopyProductionEnvironmentToSandboxEnvironmentAsync(string accessToken, string sourceEnvironmentName, string targetEnvironmentName, string countryCode)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = new {
            EnvironmentName = targetEnvironmentName,
            Type = "Sandbox"
        };
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{sourceEnvironmentName}", content);

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

     internal static async Task SetAppInsightsKeyAsync(string accessToken, string environmentName, Guid appInsightsKey)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var body = new {
            key = appInsightsKey.ToString()
        };
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/settings/appinsightskey", content);

        Console.WriteLine($"Responded with {(int)response.StatusCode} {response.ReasonPhrase}");
    }

    internal static async Task GetDatabaseSizeAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/{environmentName}/dbsize");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }

    internal static async Task GetSupportSettingsAsync(string accessToken, string environmentName)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await httpClient.GetAsync($"https://api.businesscentral.dynamics.com/admin/v2.1/support/applications/businesscentral/environments/{environmentName}/supportcontact");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented));
    }
}