// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A data CTF challenge.
/// </summary>
codeunit 50115 "Security Check" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    begin
        Page.RunModal(Page::"Security Check");
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('User security ID is a field on the User record.');
        Hints.Add('The hidden fields on pages can be seen using the page inspector.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Data);
    end;
}