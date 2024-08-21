codeunit 50202 Upgrade
{
    Access = Internal;
    Subtype = Upgrade;

    trigger OnUpgradePerCompany()
    var
        Demodata: Codeunit Demodata;
    begin
        Demodata.CreateDemodata();
    end;
}