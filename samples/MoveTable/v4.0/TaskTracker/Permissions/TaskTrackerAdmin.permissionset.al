permissionset 50104 "Task Tracker Admin"
{
    Assignable = true;

    Permissions = tabledata Task = RIMD,
                tabledata "Task Entry" = RIMD,
                table Task = X,
                table "Task Entry" = X,
                page "Task Entries" = X,
                page "Tasks List" = X;
}