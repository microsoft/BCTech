// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50109 "Quiet Widow" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    var
        httpClient: HttpClient;
        response: HttpResponseMessage;
    begin
        httpClient.Get('https://slowhttpcall.azurewebsites.net/api/Flag_9e8af1e3', response);
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('What does telemetry say?');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;
}