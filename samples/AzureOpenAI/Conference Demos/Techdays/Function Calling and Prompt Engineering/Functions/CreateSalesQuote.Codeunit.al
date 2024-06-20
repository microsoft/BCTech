// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using System.AI;
using Microsoft.Sales.Document;
using Microsoft.Inventory.Item;
using Microsoft.Sales.Customer;

/// <summary>
/// Create Sales Quote function for the Order Copilot extension.
/// The create sales quote function is used to create a sales quote based on the customer's request.
/// </summary>
codeunit 50103 "Create Sales Quote" implements "AOAI Function"
{
    procedure GetPrompt(): JsonObject
    var
        Object: JsonObject;
        [NonDebuggable]
        FuncJson: Text;
        ErrMsg: Text;
    begin
        // IsolatedStorage.Get('CreateSalesQuotePrompt', FuncJson);
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
    begin
        TempResult.Type := OrderCopilotType::"Create Quote";
        TempResult.Arguments := Format(Arguments);
        TempResult.Insert(true);
        exit(TempResult);
    end;

    procedure CreateSalesQuote(ArgumentsText: Text): Code[20]
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
        CustomerNo: Code[20];
        Arguments: JsonObject;
        CompanyName: JsonToken;
        ItemsArrayToken: JsonToken;
        ItemsArray: JsonArray;
        ItemToken: JsonToken;
        ItemName: JsonToken;
        ItemQty: JsonToken;
        LineNo: Integer;
        Counter: Integer;
    begin
        Arguments.ReadFrom(ArgumentsText);
        if not Arguments.Get('company-name', CompanyName) then
            Error(CustomerNameNotFoundErr);

        CustomerNo := FindCustomerNo(CompanyName.AsValue().AsText());
        CreateSalesHeader(SalesHeader, SalesHeader."Document Type"::Quote, CustomerNo);

        if not Arguments.Get('items-array', ItemsArrayToken) then
            Error(ItemsArrayNotFoundErr);

        SalesLine.FindLast();
        LineNo := SalesLine."Line No.";
        Counter := 0;

        ItemsArray := ItemsArrayToken.AsArray();
        foreach ItemToken in ItemsArray do begin
            Counter += 1;
            ItemToken.AsObject().Get('item-name', ItemName);
            ItemToken.AsObject().Get('quantity', ItemQty);
            CreateSalesLine(SalesHeader, FindItemNo(ItemName.AsValue().AsText()), ItemQty.AsValue().AsDecimal(), LineNo + Counter);
        end;

        exit(SalesHeader."No.");
    end;

    procedure CreateSalesHeader(var SalesHeader: Record "Sales Header"; DocumentType: Enum "Sales Document Type"; SellToCustomerNo: Code[20])
    begin
        Clear(SalesHeader);
        SalesHeader.Validate("Document Type", DocumentType);
        SalesHeader.Insert(true);
        SalesHeader.Validate("Sell-to Customer No.", SellToCustomerNo);
        SalesHeader.Modify(true);
    end;

    procedure CreateSalesLine(SalesHeader: Record "Sales Header"; ItenNo: Code[20]; Quantity: Decimal; LineNo: Integer)
    var
        SalesLine: Record "Sales Line";
    begin
        SalesLine.Init();
        SalesLine.Validate("Document Type", SalesHeader."Document Type");
        SalesLine.Validate("Document No.", SalesHeader."No.");
        SalesLine."Line No." := LineNo;
        SalesLine.Validate("Type", SalesLine."Type"::Item);
        SalesLine.Validate("No.", ItenNo);
        SalesLine.Validate("Quantity", Quantity);
        SalesLine.Insert(true);
    end;

    procedure FindItemNo(ItemName: Text): Code[20]
    var
        Item: Record "Item";
    begin
        Item.SetFilter("Description", ItemName + '*');
        Item.FindFirst();
        if Item.Description <> '' then
            exit(Item."No.")
        else
            exit('');
    end;

    procedure FindCustomerNo(CompanyName: Text): Code[20]
    var
        Customer: Record "Customer";
    begin
        Customer.SetFilter("Name", '*' + CompanyName + '*');
        Customer.FindFirst();
        if Customer.Name <> '' then
            exit(Customer."No.")
        else
            exit('');
    end;

    procedure GetName(): Text
    begin
        exit('create_sales_quote');
    end;

    var
        CustomerNameNotFoundErr: Label 'Company name parameter not found';
        ItemsArrayNotFoundErr: Label 'Items Array parameter not found';
        PromptErrorLbl: Label '%1 Prompt not found', Comment = '%1 is the name of the function.';
        PromptObjectLbl: Label '{"type": "function","function": {"name": "create_sales_quote","description": "","parameters": {"type": "object","properties": {"customer-name": {"type": "string","description": "The name of the contact person"}, "company-name": {"type": "string","description": "The name of the company"},"customer-email": {"type": "string"},"items-array": {"type": "array","description": "An array contains all the information of the items from customer requests.","items": {"type": "object","properties": {"item-name": {"type": "string","description": "The name of the item"},"quantity": {"type": "number","description": "The quantity they need for that item"},"features": {"type": "array","description": "A string array stores all the features such as color, size, weight, type etc that the customer has specified.","items": {"type": "string","description": "A string stores one feature, such as ''red'',''25 kg'',''middle size''"}}},"required": ["name"]}}},"required": ["items-array"]}}}';
}