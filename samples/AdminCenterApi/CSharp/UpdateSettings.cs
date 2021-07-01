using System;
using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class UpdateSettings
{
    internal static void GetUpdateWindow(AdminCenterClient adminCenterClient, string environmentName)
    {
        UpgradeSettings upgradeSettings = adminCenterClient.GetUpgradeSettings("BusinessCentral", environmentName);
        Utils.ConsoleWriteLineAsJson(upgradeSettings);
    }

    internal static void SetUpdateWindow(AdminCenterClient adminCenterClient, string environmentName, DateTime preferredStartTime, DateTime preferredEndTime)
    {
        var upgradeSettings = new UpgradeSettings
        {
            PreferredStartTimeUtc = preferredStartTime,
            PreferredEndTimeUtc = preferredEndTime,
        };
        UpgradeSettings newUpgradeSettings = adminCenterClient.SetUpgradeSettings("BusinessCentral", environmentName, upgradeSettings);
        Utils.ConsoleWriteLineAsJson(newUpgradeSettings);
    }

    internal static void GetScheduledUpdates(AdminCenterClient adminCenterClient, string environmentName)
    {
        ScheduledUpgrade scheduledUpgrade = adminCenterClient.GetScheduledUpgrade("BusinessCentral", environmentName);
        Utils.ConsoleWriteLineAsJson(scheduledUpgrade);
    }
}