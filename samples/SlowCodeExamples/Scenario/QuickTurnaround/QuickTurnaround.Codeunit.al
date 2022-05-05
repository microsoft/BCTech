// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license inFormation.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50103 "Quick Turnaround" implements "Slow Code Example"
{
    Access = Internal;

    procedure RunSlowCode()
    begin
        EasyComeEasyGo();
    end;

    procedure GetHint(): Text
    begin
        exit('Try using the performance profiler or check the telemetry for long running queries.');
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