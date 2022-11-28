permissionset 50132 PTEBCTPermissionSet
{
    Assignable = true;
    Caption = 'BC Tech permission set Calc', MaxLength = 30;
    Permissions =
        table Drives = X,
        tabledata Drives = RMID,
        table DirectoryItems = X,
        tabledata DirectoryItems = RMID,
        codeunit DriveInfo = X,
        codeunit DirectoryInfo = X;
}
