permissionset 50102 "Coffee Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin";

    Permissions = tabledata "Coffee Consumption Entry" = RIMD,
                table "Coffee Consumption Entry" = X,
                page "Coffee Consumption Entries" = X;
}