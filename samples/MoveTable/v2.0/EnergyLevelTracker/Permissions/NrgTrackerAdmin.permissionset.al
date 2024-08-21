permissionset 50103 "Nrg. Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin";

    Permissions = tabledata "Employee Energy Level Entry" = RIMD,
                table "Employee Energy Level Entry" = X,
                page "Employee Energy Level Entries" = X;
}