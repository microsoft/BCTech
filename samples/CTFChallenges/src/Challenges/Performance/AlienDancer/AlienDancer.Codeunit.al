// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50114 "Alien Dancer" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    var
        iterator: Integer;
        alienDancer: Record AlienDancer;
    begin
        for iterator := 1 to 1000 do begin
            alienDancer.Reset();
            alienDancer.Id := 0;
            alienDancer.Name_Flag_7f1d141a := 'thename';
            alienDancer.Insert();
            Commit();
        end;
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Telemetry will give the answer, but maybe the session isn''t logging detailed telemetry at the moment?');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;
}