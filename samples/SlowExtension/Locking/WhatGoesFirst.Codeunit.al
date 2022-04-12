// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50104 "What Goes First"
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