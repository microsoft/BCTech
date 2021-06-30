using System;
using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class Environments
{
    internal static void ListEnvironments(AdminCenterClient adminCenterClient)
    {
        EnvironmentListResult environments = adminCenterClient.GetEnvironments();
        foreach (var environment in environments.Value)
        {
            Utils.ConsoleWriteLineAsJson(environment);
        }
    }

    internal static void CreateNewEnvironment(AdminCenterClient adminCenterClient, string newEnvironmentName, string environmentType, string countryCode)
    {
        var createEnvironmentRequest = new CreateEnvironmentRequest
        {
            CountryCode = countryCode,
            EnvironmentType = environmentType
        };
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.Environment newEnvironment = adminCenterClient.CreateEnvironment("BusinessCentral", newEnvironmentName, createEnvironmentRequest);
        Utils.ConsoleWriteLineAsJson(newEnvironment);
    }

    internal static void CopyProductionEnvironmentToSandboxEnvironment(AdminCenterClient adminCenterClient, string sourceEnvironmentName, string targetEnvironmentName)
    {
        var copyEnvironmentRequest = new CopyEnvironmentRequest
        {
            EnvironmentName = targetEnvironmentName,
            Type = "Sandbox",
        };
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.Environment newEnvironment = adminCenterClient.CopyEnvironment("BusinessCentral", sourceEnvironmentName, copyEnvironmentRequest);
        Utils.ConsoleWriteLineAsJson(newEnvironment);
    }

    internal static void SetAppInsightsKey(AdminCenterClient adminCenterClient, string environmentName, Guid appInsightsKey)
    {
        var applicationInsights = new ApplicationInsights
        {
            Key = appInsightsKey.ToString(),
        };
        adminCenterClient.SetApplicationInsightsInstrumentationKey("BusinessCentral", environmentName, applicationInsights);
    }

    internal static void GetDatabaseSize(AdminCenterClient adminCenterClient, string environmentName)
    {
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.Environment environment = adminCenterClient.GetEnvironment("BusinessCentral", environmentName, skipDbSize: false);
        Utils.ConsoleWriteLineAsJson(environment.DatabaseSize);
    }

    internal static void GetSupportSettings(AdminCenterClient adminCenterClient, string environmentName)
    {
        SupportContact supportContact = adminCenterClient.GetSupportContactInformation("BusinessCentral", environmentName);
        Utils.ConsoleWriteLineAsJson(supportContact);
    }
}