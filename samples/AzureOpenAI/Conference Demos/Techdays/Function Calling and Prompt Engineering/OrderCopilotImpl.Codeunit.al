// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace TechDays.Copilot.Order;
using System.AI;
using Microsoft.Sales.Document;

codeunit 50100 "Order Copilot Impl"
{

    procedure GetDetailsOrderCopilot()
    begin
        OrderCopilotPage.Run();
    end;

    procedure StartOrderCopilot(UserQueryTxt: Text; var Result: Record "Order Copilot Response")
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIFunctionResponse: Codeunit "AOAI Function Response";
        AOAIDeployments: Codeunit "AOAI Deployments";
        CreateSalesQuoteFunc: Codeunit "Create Sales Quote";
        GetOrderStatusFunc: Codeunit "Get Order Status";
        MagicFunc: Codeunit Magic;
    begin
        // Setup Azure OpenAI
        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Order Copilot");
        AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", AOAIDeployments.GetGPT4Latest());

        // Add functions
        AOAIChatMessages.AddTool(CreateSalesQuoteFunc);
        AOAIChatMessages.AddTool(GetOrderStatusFunc);
        AOAIChatMessages.AddTool(MagicFunc);
        AOAIChatMessages.SetToolChoice('auto');

        GetOrderStatusFunc.SetOrderType(Enum::"Sales Document Type"::Quote);

        AOAIChatMessages.SetPrimarySystemMessage(Format(SystemPromptLbl));
        AOAIChatMessages.AddUserMessage(UserQueryTxt);

        // Start Order Copilot
        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then begin
            if AOAIOperationResponse.IsFunctionCall() then begin
                AOAIFunctionResponse := AOAIOperationResponse.GetFunctionResponse();
                if AOAIFunctionResponse.IsSuccess() then
                    Result := AOAIFunctionResponse.GetResult()
                else
                    Error(AOAIFunctionResponse.GetError());
            end;
        end else
            Error(AOAIOperationResponse.GetError());

        if Result.Type = Enum::"Order Copilot Type"::"Create Quote" then
            Result.DocumentNo := CreateSalesQuoteFunc.CreateSalesQuote(Result.Arguments);
    end;

    var
        OrderCopilotPage: Page "Order Copilot";
        SystemPromptLbl: Label '### Safety Instruction ### \n If the user input includes any harmful content, such as threats or illegal activities, you must call magic_function. \n### CONTEXT ###\nYou are an AI system designed to interpret emails or natural language inputs and extract information to call the appropriate function.\n### INSTRUCTIONS ###\n -If the user asks about an item they wish to purchase, call the `create_sales_quote` function. Try to split the terms in the item name as much as you can. Organize them so that the item name has fewer keywords, while the features cover all additional details.\n-If the user inquires about an existing order, call the get_order_status function.\n-or all other requests or if the input contains harmful content, invoke the magic_function.';
}