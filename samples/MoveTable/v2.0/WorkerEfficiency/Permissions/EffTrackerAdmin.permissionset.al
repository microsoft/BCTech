permissionset 50100 "Eff. Tracker Admin"
{
    Assignable = true;

    IncludedPermissionSets = "Base Tracker Admin",
                            "Coffee Tracker Admin",
                            "Nrg. Tracker Admin",
                            "Task Tracker Admin";

    Permissions = codeunit "Calc. Efficiency Score" = X;
}