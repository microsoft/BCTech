// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50101 "Ping Pong" implements "Slow Code Example"
{
    Access = Internal;

    procedure RunSlowCode()
    begin
        Ping(0);
    end;

    procedure GetHint(): Text
    begin
        exit('Try using the performance profiler.');
    end;

    procedure IsBackground(): Boolean
    begin
        exit(false);
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

        if HitCount > 10 then
            exit(false);

        exit(true);
    end;
}