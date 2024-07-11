permissionset 50100 "Eff. Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin",
                            "Coffee Tracker Admin",
                            "Nrg. Tracker Admin",
                            "Task Tracker Admin";

    Permissions = tabledata "Employee Exp. Task Efficiency" = RIMD,
                table "Employee Exp. Task Efficiency" = X,
                page "Emp. Exp. Task Efficiency List" = X,
                codeunit "Calc. Efficiency Score" = X;

}