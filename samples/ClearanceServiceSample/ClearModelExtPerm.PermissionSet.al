permissionset 50104 "ClearModelExtPerm"
{
    Assignable = true;
    Caption = 'Clear Model Ext Permissions';

    Permissions =
        tabledata "ClearModelExtConnectionSetup" = rmid;
}
