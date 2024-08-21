permissionset 50102 "Coffee Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin";

    Permissions = tabledata "Coffee Consumption Entry" = RIMD,
#pragma warning disable AL0801
                // Table 'Exp. Coffee Energy Boost' is marked to be moved. Reason: moving to Energy Level Tracker app.. Tag: 2.0.0.0.
                tabledata "Exp. Coffee Energy Boost" = RIMD,
                table "Exp. Coffee Energy Boost" = X,
#pragma warning restore AL0801
                table "Coffee Consumption Entry" = X,
                page "Coffee Consumption Entries" = X,
                page "Exp. Coffee Energy Boost List" = X;
}