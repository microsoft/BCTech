using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class Sessions
{
    internal static void GetActiveSessions(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentSessionListResult environmentSessions = adminCenterClient.GetSessions("BusinessCentral", environmentName);
        foreach (var environmentSession in environmentSessions.Value)
        {
            Utils.ConsoleWriteLineAsJson(environmentSession);
        }
    }

    internal static void CancelSession(AdminCenterClient adminCenterClient, string environmentName, int sessionId)
    {
        adminCenterClient.RemoveSession("BusinessCentral", environmentName, sessionId.ToString());
    }
}