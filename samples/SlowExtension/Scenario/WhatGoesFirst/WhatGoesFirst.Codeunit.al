// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50104 "What Goes First" implements "Slow Code Example"
{
    Access = Internal;
    TableNo = "Job Queue Entry";

    trigger OnRun()
    begin
        if Rec."Parameter String" = 'Milk first' then
            PrepareBreakfastMilkFirst()
        else
            PrepareBreakfastCerealFirst();
    end;

    procedure RunSlowCode()
    var
        FirstPersonJobQueue: Record "Job Queue Entry";
        SecondPersonJobQueue: Record "Job Queue Entry";
        FoodManagement: Codeunit "Food Management";
        SessionId: Integer;
    begin
        FoodManagement.SetupFood();

        FirstPersonJobQueue."Parameter String" := 'Milk first';
        SecondPersonJobQueue."Parameter String" := 'Cereal First';

        Session.StartSession(SessionId, Codeunit::"What Goes First", CompanyName(), FirstPersonJobQueue);
        Session.StartSession(SessionId, Codeunit::"What Goes First", CompanyName(), SecondPersonJobQueue);
    end;

    procedure GetHint(): Text
    begin
        exit('Try checking lock timeout telemetry or the ''Database Locks'' page.');
    end;

    procedure IsBackground(): Boolean
    begin
        exit(true);
    end;

    local procedure PrepareBreakfastMilkFirst()
    begin
        AddMilk();
        FetchCereal();
        AddCereal();
    end;

    local procedure PrepareBreakfastCerealFirst()
    begin
        AddCereal();
        FetchMilk();
        AddMilk();
    end;

    local procedure AddMilk()
    var
        Milk: Record Milk;
    begin
        Milk.SetFilter("Amount Left", '>%1', 0.2);
        Milk.LockTable();
        if Milk.FindFirst() then begin
            Milk."Amount Left" := Milk."Amount Left" - 0.2;
            Milk.Modify();
        end;
    end;

    local procedure AddCereal()
    var
        Cereal: Record Cereal;
    begin
        Cereal.SetFilter("Amount Left", '>%1', 0.2);
        Cereal.LockTable();
        if Cereal.FindFirst() then begin
            Cereal."Amount Left" := Cereal."Amount Left" - 0.2;
            Cereal.Modify();
        end;
    end;

    local procedure FetchCereal()
    begin
        Sleep(5000);
    end;

    local procedure FetchMilk()
    begin
        Sleep(5000);
    end;
}