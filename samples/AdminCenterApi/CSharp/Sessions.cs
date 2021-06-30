using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class Sessions
{
    internal static void GetActiveSessions(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentSessionListResult environmentSessions = adminCenterClient.GetActiveSessions("BusinessCentral", environmentName);
        foreach (var environmentSession in environmentSessions.Value)
        {
            Utils.ConsoleWriteLineAsJson(environmentSession);
        }
    }

    internal static void CancelSession(AdminCenterClient adminCenterClient, string environmentName, int sessionId)
    {
        adminCenterClient.RemoveActiveSession("BusinessCentral", environmentName, sessionId.ToString());
    }
}