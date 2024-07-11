codeunit 50201 Install
{
    Access = Internal;
    Subtype = Install;

    trigger OnInstallAppPerCompany()
    var
        Demodata: Codeunit Demodata;
    begin
        Demodata.CreateDemodata();
    end;
}