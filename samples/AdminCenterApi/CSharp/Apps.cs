using System;
using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class Apps
{
    internal static void GetInstalledApps(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentAppListResult environmentApps = adminCenterClient.GetInstalledApps("BusinessCentral", environmentName);
        foreach (var environmentApp in environmentApps.Value)
        {
            Utils.ConsoleWriteLineAsJson(environmentApp);
        }
    }

    internal static void GetAvailableAppUpdates(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentAppUpdateListResult environmentAppUpdates = adminCenterClient.GetAvailableAppUpdates("BusinessCentral", environmentName);
        foreach (var environmentAppUpdate in environmentAppUpdates.Value)
        {
            Utils.ConsoleWriteLineAsJson(environmentAppUpdate);
        }
    }

    internal static void UpdateApp(AdminCenterClient adminCenterClient, string environmentName, Guid appId, string newAppVersion)
    {
        var scheduleEnvironmentAppInstallRequest = new ScheduleEnvironmentAppInstallRequest
        {
            TargetVersion = newAppVersion,
            UseEnvironmentUpdateWindow = false,
            AcceptIsvEula = true,
            InstallOrUpdateNeededDependencies = true,
            AllowPreviewVersion = false,
        };
        EnvironmentAppOperation environmentAppOperation = adminCenterClient.ScheduleAppInstall("BusinessCentral", environmentName, appId, scheduleEnvironmentAppInstallRequest);
        Utils.ConsoleWriteLineAsJson(environmentAppOperation);
    }

    internal static void GetAppOperations(AdminCenterClient adminCenterClient, string environmentName, Guid appId)
    {
        EnvironmentAppOperationListResult environmentAppOperations = adminCenterClient.GetAppOperations("BusinessCentral", environmentName, appId);
        foreach (var environmentAppOperation in environmentAppOperations.Value)
        {
            Utils.ConsoleWriteLineAsJson(environmentAppOperation);
        }
    }
}