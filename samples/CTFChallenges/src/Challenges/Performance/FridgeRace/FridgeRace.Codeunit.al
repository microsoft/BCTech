// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A performance CTF challenge.
/// </summary>
codeunit 50105 "Fridge Race" implements "CTF Challenge"
{
    Access = Internal;

    trigger OnRun()
    var
        Milk: Record Milk;
    begin
        if FetchMilk(Milk) then begin
            GoToAnotherRoom();
            DrinkMilk(Milk);
        end;
    end;

    procedure RunChallenge()
    var
        FoodManagement: Codeunit "Food Management";
        ParallelSessions: Codeunit "Parallel Sessions";
        SessionIDs: List of [Integer];
        SessionId: Integer;
    begin
        FoodManagement.SetupFood();

        Session.StartSession(SessionId, Codeunit::"Fridge Race");
        SessionIDs.Add(SessionID);

        Session.StartSession(SessionId, Codeunit::"Fridge Race");
        SessionIDs.Add(SessionID);

        ParallelSessions.WaitForSessionsToComplete(SessionIDs);
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Try checking lock timeout telemetry.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Performance);
    end;

    local procedure FetchMilk(var Milk: Record Milk): Boolean
    begin
        Milk.SetFilter("Amount Left", '>%1', 0.2);
        Milk.LockTable();
        exit(Milk.FindFirst());
    end;

    local procedure DrinkMilk(var Milk: Record Milk): Boolean
    begin
        Milk."Amount Left" := Milk."Amount Left" - 0.2;
        Milk.Modify();
    end;

    local procedure GoToAnotherRoom()
    begin
        Wait();
    end;

    local procedure Wait()
    begin
        Flag_38dfbb00();
    end;

    local procedure Flag_38dfbb00()
    begin
        Sleep(32000);
    end;
}