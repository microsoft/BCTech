using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class Sessions
{
    internal static void GetActiveSessions(AdminCenterClient adminCenterClient, string environmentName)
    {
        EnvironmentSessionListResult sessions = adminCenterClient.GetSessions("BusinessCentral", environmentName);
        foreach (var session in sessions.Value)
        {
            Utils.ConsoleWriteLineAsJson(session);
        }
    }

    internal static void CancelSession(AdminCenterClient adminCenterClient, string environmentName, int sessionId)
    {
        adminCenterClient.RemoveSession("BusinessCentral", environmentName, sessionId.ToString());
    }
}