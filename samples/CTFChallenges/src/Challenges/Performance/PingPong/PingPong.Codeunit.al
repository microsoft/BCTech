// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50101 "Ping Pong" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    begin
        Ping(0);
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Try using the performance profiler.');
        Hints.Add('Record the scenario with the performance profiler, enable the ''Show technical information'' switch and check the ''Call Tree'' section.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;

    local procedure Ping(HitCount: Integer)
    begin
        if HandleTheBall(HitCount) then
            Pong(HitCount + 1);
    end;

    local procedure Pong(HitCount: Integer)
    begin
        if HandleTheBall(HitCount) then
            Ping(HitCount + 1);
    end;

    local procedure HandleTheBall(HitCount: Integer): Boolean
    begin
        Sleep(500);

        if HitCount > 5 then begin
            Flag_f4509017();
            exit(false);
        end;

        exit(true);
    end;

    local procedure Flag_f4509017()
    begin
        Sleep(500);
    end;
}