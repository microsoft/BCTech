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
    TableNo = "Parallel Session Entry";

    trigger OnRun()
    begin
        if Rec.Parameter = 'Milk first' then
            PrepareBreakfastMilkFirst()
        else
            PrepareBreakfastCerealFirst();
    end;

    procedure RunSlowCode()
    var
        FirstPersonEntry: Record "Parallel Session Entry";
        SecondPersonEntry: Record "Parallel Session Entry";
        FoodManagement: Codeunit "Food Management";
        ParallelSessions: Codeunit "Parallel Sessions";
        SessionID: Integer;
        SessionIDs: List of [Integer];
    begin
        FoodManagement.SetupFood();

        FirstPersonEntry.Parameter := 'Milk first';
        SecondPersonEntry.Parameter := 'Cereal First';

        Session.StartSession(SessionID, Codeunit::"What Goes First", CompanyName(), FirstPersonEntry);
        SessionIDs.Add(SessionID);
        Session.StartSession(SessionID, Codeunit::"What Goes First", CompanyName(), SecondPersonEntry);
        SessionIDs.Add(SessionID);

        ParallelSessions.WaitForSessionsToComplete(SessionIDs);
    end;

    procedure GetHint(): Text
    begin
        exit('Try checking the ''Database Locks'' page or long running queries in telemetry.');
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