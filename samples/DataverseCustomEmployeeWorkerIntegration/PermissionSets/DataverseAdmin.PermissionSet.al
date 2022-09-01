permissionset 50100 "Dataverse Admin"
{
    Assignable = true;
    Permissions =
        codeunit "Dataverse Int. Management" = X,
        page "Dataverse Worker List" = X,
        table "Dataverse cdm_Worker" = X,
        tabledata "Dataverse cdm_Worker" = RIMD;
}