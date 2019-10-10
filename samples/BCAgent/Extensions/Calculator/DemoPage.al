// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

pageextension 50144 CustListExt extends "Customer List"
{
    actions
    {
        addlast("&Customer")
        {
            action(Push)
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    Calculator: codeunit Calculator;
                begin
                    Message('2 + 2 = %1', Calculator.Add(2, 2));
                end;

            }
        }
    }

    trigger OnOpenPage()
    var
        Calculator: codeunit Calculator;
    begin
        Message('2 + 2 = %1', Calculator.Add(2, 2));
    end;
}