// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50110 IntroChallenge1 implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    begin
        Message('You found the flag: Flag_f6119402');
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Introduction);
    end;
}