using System;
using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class Apps
{
    internal static void GetInstalledApps(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentAppListResult installedApps = adminCenterClient.GetInstalledApps("BusinessCentral", environmentName);
        foreach (var installedApp in installedApps.Value)
        {
            Utils.ConsoleWriteLineAsJson(installedApp);
        }
    }

    internal static void GetAvailableAppUpdates(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentAppUpdateListResult appUpdates = adminCenterClient.GetAvailableAppUpdates("BusinessCentral", environmentName);
        foreach (var appUpdate in appUpdates.Value)
        {
            Utils.ConsoleWriteLineAsJson(appUpdate);
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
        EnvironmentAppOperation appOperation = adminCenterClient.ScheduleAppInstall("BusinessCentral", environmentName, appId, scheduleEnvironmentAppInstallRequest);
        Utils.ConsoleWriteLineAsJson(appOperation);
    }

    internal static void GetAppOperations(AdminCenterClient adminCenterClient, string environmentName, Guid appId)
    {
        EnvironmentAppOperationListResult appOperations = adminCenterClient.GetAppOperations("BusinessCentral", environmentName, appId);
        foreach (var environmentAppOperation in appOperations.Value)
        {
            Utils.ConsoleWriteLineAsJson(environmentAppOperation);
        }
    }
}