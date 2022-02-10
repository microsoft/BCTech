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
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperation environmentOperation = adminCenterClient.CreateEnvironment("BusinessCentral", newEnvironmentName, createEnvironmentRequest);
        Utils.ConsoleWriteLineAsJson(environmentOperation);
    }

    internal static void CopyProductionEnvironmentToSandboxEnvironment(AdminCenterClient adminCenterClient, string sourceEnvironmentName, string targetEnvironmentName)
    {
        var copyEnvironmentRequest = new CopyEnvironmentRequest
        {
            EnvironmentName = targetEnvironmentName,
            Type = "Sandbox",
        };
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperation environmentOperation = adminCenterClient.CopyEnvironment("BusinessCentral", sourceEnvironmentName, copyEnvironmentRequest);
        Utils.ConsoleWriteLineAsJson(environmentOperation);
    }

    internal static void RenameEnvironment(AdminCenterClient adminCenterClient, string currentEnvironmentName, string newEnvironmentName)
    {
        var renameEnvironmentRequest = new RenameEnvironmentRequest
        {
            NewEnvironmentName = newEnvironmentName
         };
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperation environmentOperation = adminCenterClient.RenameEnvironment("BusinessCentral", currentEnvironmentName, renameEnvironmentRequest);
        Utils.ConsoleWriteLineAsJson(environmentOperation);
    }

    internal static void RestartEnvironment(AdminCenterClient adminCenterClient, string environmentName)
    {
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperation environmentOperation = adminCenterClient.RestartEnvironment("BusinessCentral", environmentName);
        Utils.ConsoleWriteLineAsJson(environmentOperation);
    }

    internal static void RemoveEnvironment(AdminCenterClient adminCenterClient, string environmentName)
    {
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperation environmentOperation = adminCenterClient.RemoveEnvironment("BusinessCentral", environmentName);
        Utils.ConsoleWriteLineAsJson(environmentOperation);
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

    internal static void GetEnvironmentOperations(AdminCenterClient adminCenterClient, string environmentName)
    {
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperationListResult operations = adminCenterClient.GetOperations("BusinessCentral", environmentName);
        Utils.ConsoleWriteLineAsJson(operations);
    }

    internal static void GetOperationsForAllEnvironments(AdminCenterClient adminCenterClient)
    {
        Microsoft.Dynamics.BusinessCentral.AdminCenter.Models.EnvironmentOperationListResult operations = adminCenterClient.GetOperationsForAllEnvironments();
        Utils.ConsoleWriteLineAsJson(operations);
    }
}