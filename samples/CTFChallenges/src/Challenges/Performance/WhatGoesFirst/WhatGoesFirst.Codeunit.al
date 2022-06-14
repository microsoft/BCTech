// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50104 "What Goes First" implements "CTF Challenge"
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

    procedure RunChallenge()
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

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Try checking the ''Database Locks'' page or long running queries in telemetry.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;

    local procedure PrepareBreakfastMilkFirst()
    begin
        AddMilk();
        FetchCereal_Flag_a7e2b268();
        AddCereal();
    end;

    local procedure PrepareBreakfastCerealFirst()
    begin
        AddCereal();
        FetchMilk_Flag_a7e2b268();
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

    local procedure FetchCereal_Flag_a7e2b268()
    begin
        Sleep(5000);
    end;

    local procedure FetchMilk_Flag_a7e2b268()
    begin
        Sleep(5000);
    end;
}