// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using Microsoft.Sales.Document;
pageextension 50102 "Sales Quote List Ext" extends "Sales Quotes"
{
    actions
    {
        addfirst(Prompting)
        {
            action("Order Copilot")
            {
                ApplicationArea = All;
                Caption = 'Order Copilot';
                Image = SparkleFilled;
                ToolTip = 'Order Copilot';

                trigger OnAction()
                begin
                    OrderCopilotImpl.GetDetailsOrderCopilot();
                end;
            }
        }
        addfirst(processing)
        {
            action("Order Copilot Processing")
            {
                ApplicationArea = All;
                Caption = 'Order Copilot';
                Image = SparkleFilled;
                ToolTip = 'Order Copilot';
                Promoted = true;

                trigger OnAction()
                begin
                    OrderCopilotImpl.GetDetailsOrderCopilot();
                end;
            }
        }
    }

    var
        OrderCopilotImpl: Codeunit "Order Copilot Impl";
}