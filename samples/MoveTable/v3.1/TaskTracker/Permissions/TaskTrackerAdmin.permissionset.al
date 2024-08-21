permissionset 50104 "Task Tracker Admin"
{
    Assignable = true;

    Permissions = tabledata Task = RIMD,
                tabledata "Task Entry" = RIMD,
#pragma warning disable AL0801
    // Table 'Employee Exp. Task Efficiency' is marked to be moved. Reason: moving to Human Worker Efficiency Tracker app.. Tag: 3.1.0.0.    
                tabledata "Employee Exp. Task Efficiency" = RIMD,
                table "Employee Exp. Task Efficiency" = X,
#pragma warning restore AL0801
                table Task = X,
                table "Task Entry" = X,
                page "Emp. Exp. Task Efficiency List" = X,
                page "Task Entries" = X,
                page "Tasks List" = X;
}