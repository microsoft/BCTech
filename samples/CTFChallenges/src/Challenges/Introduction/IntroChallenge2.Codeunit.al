// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50111 IntroChallenge2 implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    begin
        Message('Can you guess the flag? No? Maybe you need a hint?');
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('This is a hint. It normally doesn''t give you answer straight away, but we will make an exception: the flag is Flag_f0147182');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Introduction);
    end;
}