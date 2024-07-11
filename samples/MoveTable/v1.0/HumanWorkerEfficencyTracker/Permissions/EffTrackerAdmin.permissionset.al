permissionset 50100 "Eff. Tracker Admin"
{
    Assignable = true;

    Permissions = tabledata "Coffee Consumption Entry" = RIMD,
                tabledata "Employee Energy Level Entry" = RIMD,
                tabledata "Employee Exp. Task Efficiency" = RIMD,
                tabledata "Exp. Coffee Energy Boost" = RIMD,
                tabledata Task = RIMD,
                tabledata "Task Entry" = RIMD,
                table "Coffee Consumption Entry" = X,
                table "Employee Energy Level Entry" = X,
                table "Employee Exp. Task Efficiency" = X,
                table "Exp. Coffee Energy Boost" = X,
                table Task = X,
                table "Task Entry" = X,
                page "Coffee Consumption Entries" = X,
                page "Employee Energy Level Entries" = X,
                page "Exp. Coffee Energy Boost List" = X,
                page "Emp. Exp. Task Efficiency List" = X,
                page "Task Entries" = X,
                page "Tasks List" = X;
}