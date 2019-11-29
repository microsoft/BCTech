// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50111 "AutoRefresh Repeater Part"
{
    PageType = CardPart;
    UsageCategory = None;
    SourceTable = AutoRefresh;
    SourceTableTemporary = true;
    ModifyAllowed = false;
    InsertAllowed = false;
    DeleteAllowed = false;
    SourceTableView = sorting(Id) order(descending);

    layout
    {
        area(Content)
        {
            repeater(MainRepeater)
            {
                field(Id; Id)
                {
                    ApplicationArea = Basic, Suite;
                }

                field(Date; Date)
                {
                    ApplicationArea = Basic, Suite;
                }

                field(CreatedBySessionId; CreatedBySessionId)
                {
                    ApplicationArea = Basic, Suite;
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        ResetTempTable();
    end;

    procedure ResetTempTable()
    begin
        DeleteAll(false);
        CurrPage.Update(false);
    end;

    procedure UpdateSourceTable(Results: Dictionary of [Text, Text])
    var
        IdKey: Text;
        Value: Text;
        Modified: Boolean;
    begin
        // Any modification to the source table may introduce flickering.
        // Therefor we need to make sure we perform the least possible modifications on the source table.
        // For this sample, Results contains only newly added records.
        Modified := false;
        foreach IdKey in Results.Keys do begin
            Evaluate(Id, IdKey);
            Value := Results.Get(IdKey);
            Evaluate(Date, SelectStr(1, Value));
            Evaluate(CreatedBySessionId, SelectStr(2, Value));
            if not Modify() then
                Insert();

            Modified := true;
        end;

        if Modified then
            CurrPage.Update(false);
    end;
}