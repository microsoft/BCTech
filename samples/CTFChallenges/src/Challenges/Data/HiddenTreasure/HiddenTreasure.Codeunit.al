// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A data CTF challenge.
/// </summary>
codeunit 50116 "Hidden Treasure" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    var
        Treasure: Record Treasure;
    begin
        if not IsInitializaed() then
            Initialize();

        if not Confirm(AreYouReadyQst) then
            exit;

        Treasure.ID := TreasureId;
        Treasure.Insert();
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Can the ID in the exception message point to the treasure?');
        Hints.Add('It is possible to find the table ID of a given table on the e. g. Table Information page.');
        Hints.Add('Adding ?table=<table ID> in the URL will open a read-only view of a table.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Data);
    end;

    local procedure IsInitializaed(): Boolean
    var
        Treasure: Record Treasure;
    begin
        exit(not Treasure.IsEmpty());
    end;

    local procedure Initialize()
    var
        Treasure: Record Treasure;
        Iterator: Integer;
    begin
        for Iterator := 1 to 200 do begin
            Treasure.ID := CreateGuid();
            Treasure.Value := 'Rubbish';
            Treasure.Insert();
        end;

        Treasure.ID := TreasureId;
        Treasure.Value := 'Flag_5b9237c0';
        Treasure.Insert();
    end;

    var
        AreYouReadyQst: Label 'Are you ready to insert the shovel in the ground to search for treasure?';
        TreasureId: Label 'F0000000-0000-0000-0000-000000000042';
}