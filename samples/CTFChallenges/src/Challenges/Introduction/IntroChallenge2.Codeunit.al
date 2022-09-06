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
        // This has nothing to do with this intro, but we want the user to say "ok to make HTTP requests" now, rather than in the challenge where it's going to be used (because it will give the solution away)
        UnblockHttpCalls();

        Message('Can you guess the flag? No? Maybe you need a hint?');
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('This is a hint. It normally doesn''t give you answer straight away, but we will make an exception: the flag is Flag_f0147182');
        exit(Hints);
    end;

    procedure UnblockHttpCalls()
    var
        httpClient: HttpClient;
        response: HttpResponseMessage;
    begin
        httpClient.Get('https://www.microsoft.com', response);
    end;


    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Introduction);
    end;
}