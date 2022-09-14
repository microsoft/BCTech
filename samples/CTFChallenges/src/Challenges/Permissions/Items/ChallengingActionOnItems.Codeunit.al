codeunit 50144 ChallengingActionOnItems implements "CTF Challenge"
{
    Access = Internal;

    trigger OnRun()
    begin

    end;

    [InherentPermissions(PermissionObjectType::TableData, Database::TableToRead, 'RI')]
    procedure RunChallenge();
    var
        ScenarioLabel: Label 'Please go to the ''Items'' page, find a ''Challenging Action'' and invoke it. What is wrong here?';

        TableToRead: Record TableToRead;
    begin
        Message(ScenarioLabel);

        if not IsInitialized() then
            Initialize();

        if not Confirm(AreYouReadyQst) then
            exit;
    end;

    local procedure IsInitialized(): Boolean
    var
        TableToRead: Record TableToRead;
    begin
        exit(not TableToRead.IsEmpty());
    end;

    local procedure Initialize()
    var
        TableToRead: Record TableToRead;
        Iterator: Integer;

        TenantPermissionSet: Record "Tenant Permission Set";
    begin
        for Iterator := 1 to 200 do begin
            TableToRead.ID := CreateGuid();
            TableToRead.Value := 'SomeData';
            TableToRead.Insert();
        end;
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Check the Business Central telemetry from challenging_action_query_data_demo.csv. Pay attention to what permission set operations have been recently performed on the tenant.';
        HintLine2: Label 'Has any of the permission sets been removed from the user?';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1);
        Hints.Add(HintLine2);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Permissions);
    end;

    var
        AreYouReadyQst: Label 'Are you ready to start the challenge?';
}