permissionset 59100 "Sales Order Perm"
{
    Assignable = true;
    Permissions = table "Sales Orders" = X,
        tabledata "Sales Orders" = RIMD,
        table "Sales Order Lines" = X,
        tabledata "Sales Order Lines" = RIMD,
        table "Sales Order Additional Info" = X,
        tabledata "Sales Order Additional Info" = RIMD,
        codeunit "Sales Order" = X,
        page "Sales Order Data" = X,
        report "Sales Order" = X,
        report "Sales Order - List" = X;
}