using Microsoft.Dynamics.BusinessCentral.AdminCenter;
using Microsoft.Dynamics.BusinessCentral.AdminCenter.Models;

class NotificationRecipients
{
    internal static void GetNotificationRecipients(AdminCenterClient adminCenterClient)
    {
        NotificationRecipientListResult notificationRecipients = adminCenterClient.GetNotificationRecipients();
        foreach (var notificationRecipient in notificationRecipients.Value)
        {
            Utils.ConsoleWriteLineAsJson(notificationRecipient);
        }
    }

    internal static void AddNotificationRecipient(AdminCenterClient adminCenterClient, string emailAddress, string name)
    {
        var notificationRecipient = new NotificationRecipient
        {
            Email = emailAddress,
            Name = name,
        };
        NotificationRecipient addedNotificationRecipient = adminCenterClient.SetNotificationRecipient(notificationRecipient);
        Utils.ConsoleWriteLineAsJson(addedNotificationRecipient);
    }
}