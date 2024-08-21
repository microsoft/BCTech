// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

/// <summary>
/// The Order Copilot Setup page to store the prompts used by the functions and copilot.
/// </summary>
page 50101 "Order Copilot Setup"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    Extensible = false;

    layout
    {
        area(Content)
        {
            group(Functions)
            {
                Caption = 'Function Prompts';
                field(CreateSalesQuotePrompt; CreateSalesQuotePrompt)
                {
                    ApplicationArea = All;
                    Caption = 'Create Sales Quote Prompt';
                    ToolTip = 'Specifies the Create Sales Quote json.';
                    MultiLine = true;
                }
                field(GetOrderStatusPrompt; GetOrderStatusPrompt)
                {
                    ApplicationArea = All;
                    Caption = 'Get Order Status Prompt';
                    ToolTip = 'Specifies the Get Order Status json.';
                    MultiLine = true;
                }
                field(MagicPrompt; MagicPrompt)
                {
                    ApplicationArea = All;
                    Caption = 'Magic Prompt';
                    ToolTip = 'Specifies the Magic function json.';
                    MultiLine = true;
                }
            }
            group(OrderCopilot)
            {
                Caption = 'Order Copilot';
                field(OrderCopilotSystemPrompt; OrderCopilotSystemPrompt)
                {
                    ApplicationArea = All;
                    Caption = 'System Prompt';
                    ToolTip = 'Specifies the system/metaprompt that will be used to control the Order Copilot.';
                    MultiLine = true;
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        if IsolatedStorage.Contains('CreateSalesQuotePrompt') then
            CreateSalesQuotePrompt := '***';
        if IsolatedStorage.Contains('GetOrderStatusPrompt') then
            GetOrderStatusPrompt := '***';
        if IsolatedStorage.Contains('MagicPrompt') then
            MagicPrompt := '***';
        if IsolatedStorage.Contains('OrderCopilotPrompt') then
            OrderCopilotSystemPrompt := '***';
    end;

    var
        [NonDebuggable]
        CreateSalesQuotePrompt: Text;
        [NonDebuggable]
        GetOrderStatusPrompt: Text;
        [NonDebuggable]
        MagicPrompt: Text;
        [NonDebuggable]
        OrderCopilotSystemPrompt: Text;
}