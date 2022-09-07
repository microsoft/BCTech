codeunit 50144 ChallengingActionOnItems implements "CTF Challenge"
{
    Access = Internal;

    trigger OnRun()
    begin

    end;

    [InherentPermissions(PermissionObjectType::TableData, Database::TableToRead, 'I')]
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

        // TableToRead.ID := TableToRead;
        // Treasure.Insert();
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
    begin
        for Iterator := 1 to 200 do begin
            TableToRead.ID := CreateGuid();
            TableToRead.Value := 'SomeData';
            TableToRead.Insert();
        end;

        // Treasure.ID := TableToRead;
        // Treasure.Value := 'Flag_5b9237c0';
        // Treasure.Insert();
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Check the telemetry.';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Permissions);
    end;

    var
        AreYouReadyQst: Label 'Are you ready to start the challenge?';
}