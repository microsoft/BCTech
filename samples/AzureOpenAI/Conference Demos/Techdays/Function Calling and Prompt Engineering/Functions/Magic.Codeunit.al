// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using System.AI;

/// <summary>
/// Magic function for the Order Copilot extension.
/// The magic function is a catch-all function that is called when the user input is not about sales quotation request or order status.
/// </summary>
codeunit 50101 "Magic" implements "AOAI Function"
{

    procedure GetPrompt(): JsonObject
    var
        Object: JsonObject;
        [NonDebuggable]
        FuncJson: Text;
        ErrMsg: Text;
    begin
        // IsolatedStorage.Get('MagicPrompt', FuncJson);
        FuncJson := PromptObjectLbl;
        if FuncJson = '' then begin
            ErrMsg := StrSubstNo(PromptErrorLbl, GetName());
            Error(ErrMsg);
        end;

        Object.ReadFrom(FuncJson);
        exit(Object);
    end;

    procedure Execute(Arguments: JsonObject): Variant
    begin
        Error(MagicFuncErr);
    end;

    procedure GetName(): Text
    begin
        exit('magic_function');
    end;

    procedure SetCustomMessage(Message: Text)
    begin
        CustomErrorMessage := Message;
    end;

    var
        CustomErrorMessage: Text;
        MagicFuncErr: Label 'What you have requested for is unsupported. Please rephrase your request.';
        PromptErrorLbl: Label '%1 Prompt not found', Comment = '%1 is the name of the function.';
        PromptObjectLbl: Label '{"type": "function","function": {"name": "magic_function","description": "If the user input is not about sales quotation request or order status. Call this function.","parameters": {"type": "object","properties": {"intent": {"type": "string","description": "The intent of the customer request."}}}}}';
}