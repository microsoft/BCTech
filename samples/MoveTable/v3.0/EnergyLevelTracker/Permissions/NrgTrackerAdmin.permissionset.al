permissionset 50103 "Nrg. Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin";

    Permissions = tabledata "Employee Energy Level Entry" = RIMD,
                tabledata "Exp. Coffee Energy Boost" = RIMD,
                table "Employee Energy Level Entry" = X,
                table "Exp. Coffee Energy Boost" = X,
                page "Employee Energy Level Entries" = X,
                page "Exp. Coffee Energy Boost List" = X;
}