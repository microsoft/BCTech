// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using Microsoft.Sales.Document;
using System.AI;

/// <summary>
/// Get Order Status function for the Order Copilot extension.
/// The get order status function is used to get the status of an order.
/// </summary>
codeunit 50102 "Get Order Status" implements "AOAI Function"
{
    procedure GetPrompt(): JsonObject
    var
        Object: JsonObject;
        [NonDebuggable]
        FuncJson: Text;
        ErrMsg: Text;
    begin
        // IsolatedStorage.Get('GetOrderStatusPrompt', FuncJson);
        FuncJson := PromptObjectLbl;
        if FuncJson = '' then begin
            ErrMsg := StrSubstNo(PromptErrorLbl, GetName());
            Error(ErrMsg);
        end;

        Object.ReadFrom(FuncJson);
        exit(Object);
    end;

    procedure Execute(Arguments: JsonObject): Variant
    var
        TempResult: Record "Order Copilot Response" temporary;
        OrderCopilotType: Enum "Order Copilot Type";
        OrderNo: Code[20];
        JsonToken: JsonToken;
    begin
        if not Arguments.Get('order-no', JsonToken) then
            Error(OrderNoParameterErr);
        OrderNo := CopyStr(JsonToken.AsValue().AsText(), 1, MaxStrLen(OrderNo));

        if OrderNo = '' then
            Error(OrderNoEmptyErr);

        CheckOrderExists(OrderNo);

        TempResult.Type := OrderCopilotType::"Order Status";
        TempResult.DocumentNo := OrderNo;
        TempResult.Insert(true);
        exit(TempResult);
    end;

    procedure GetName(): Text
    begin
        exit('get_order_status');
    end;

    procedure CheckOrderExists(OrderNo: Code[20])
    var
        SalesHeader: Record "Sales Header";
    begin
        if not SalesHeader.Get(OrderType, OrderNo) then
            Error(OrderNotFoundErr);
    end;

    procedure SetOrderType(Type: Enum "Sales Document Type")
    begin
        OrderType := Type;
    end;

    var
        OrderType: Enum "Sales Document Type";
        OrderNoParameterErr: Label 'Order No parameter not found';
        OrderNoEmptyErr: Label 'Order No parameter is empty';
        OrderNotFoundErr: Label 'Order not found';
        PromptErrorLbl: Label '%1 Prompt not found', Comment = '%1 is the name of the function.';
        PromptObjectLbl: Label '{"type": "function","function": {"name": "get_order_status","description": "If the user input is to enquire about an order call this function.","parameters": {"type": "object","properties": {"order-no": {"type": "string"}}}}}';
}