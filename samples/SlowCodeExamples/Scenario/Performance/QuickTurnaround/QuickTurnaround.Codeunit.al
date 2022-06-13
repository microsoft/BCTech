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
        Hints.Add('Try using the performance profiler or check the telemetry for long running queries.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;

    local procedure EasyComeEasyGo()
    begin
        RemoveCereal();
        AddCereal(5000);
        RemoveCereal();
    end;

    local procedure AddCereal(Number: Integer)
    var
        Cereal: Record Cereal;
        Iterator: Integer;
    begin
        for Iterator := 1 to Number do begin
            Cereal.Init();
            Cereal."Box No." := Iterator;
            Cereal.Insert();
        end;
    end;

    local procedure RemoveCereal()
    var
        Cereal: Record Cereal;
    begin
        Cereal.DeleteAll();
    end;
}