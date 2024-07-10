permissionset 50102 "Coffee Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin";

    Permissions = tabledata "Coffee Consumption Entry" = RIMD,
                tabledata "Exp. Coffee Energy Boost" = RIMD,
                table "Coffee Consumption Entry" = X,
                table "Exp. Coffee Energy Boost" = X,
                page "Coffee Consumption Entries" = X,
                page "Exp. Coffee Energy Boost List" = X;
}