// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------
pageextension 50100 CustomerListX extends "Customer List"
{
    actions
    {
        addbefore(NewSalesQuote)
        {
            action(CreateCustomerDirectoryWordDocumentNoAddin)
            {
                ApplicationArea = All;
                Caption = 'Create customer directory Word document from service';
                Image = Document;
                Promoted = true;
                PromotedCategory = Category5;
                PromotedIsBig = true;
                PromotedOnly = true;

                trigger OnAction();
                var
                    WordDocManagement: Codeunit WordDocManagement;
                begin
                    WordDocManagement.CreateCustomerDirectoryWordDocument();
                end;
            }
        }
    }

}
