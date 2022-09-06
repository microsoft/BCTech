// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A data CTF challenge.
/// </summary>
codeunit 50112 "Blank Space" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    begin
        Page.RunModal(Page::"Flag Display");
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('The flag will reveal itself when the editable field is validated to be correct.');
        Hints.Add('The editable field has whitespace characters at the end.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Data);
    end;
}