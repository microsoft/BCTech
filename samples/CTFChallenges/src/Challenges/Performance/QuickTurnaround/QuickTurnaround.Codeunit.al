// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license inFormation.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50103 "Quick Turnaround" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    begin
        EasyComeEasyGo();
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Try checking the telemetry for long running queries.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;

    local procedure EasyComeEasyGo()
    begin
        RemoveItems();
        AddItems(5000);
        RemoveItems();
    end;

    local procedure AddItems(Number: Integer)
    var
        QuickItem: Record "Quick Item Flag_6e5b1753";
        Iterator: Integer;
    begin
        for Iterator := 1 to Number do begin
            QuickItem."No." := Iterator;
            QuickItem.Insert();
        end;
    end;

    local procedure RemoveItems()
    var
        QuickItem: Record "Quick Item Flag_6e5b1753";
    begin
        QuickItem.DeleteAll();
    end;
}