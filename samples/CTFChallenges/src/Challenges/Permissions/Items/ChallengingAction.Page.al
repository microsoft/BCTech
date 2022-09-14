// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A permissions CTF challenge.
/// </summary>
pageextension 50149 "Items Challenging Action" extends "Item List"
{
    actions
    {
        addafter("Item Tracing")
        {
            action(CTFChallenge)
            {
                ApplicationArea = All;
                Caption = 'Challenging Action';
                Promoted = true;
                PromotedCategory = Category7;
                Scope = Repeater;

                trigger OnAction()
                var
                    TableToRead: Record TableToRead;
                begin
                    // Codeunit.Run(50149);
                    TableToRead.FindFirst();
                end;
            }
        }
    }
}