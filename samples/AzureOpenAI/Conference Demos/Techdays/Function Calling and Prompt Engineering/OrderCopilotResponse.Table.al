// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using Microsoft.Sales.Document;

/// <summary>
/// The Order Copilot Response table to store the response of the Copilot.
/// </summary>
table 50100 "Order Copilot Response"
{
    TableType = Temporary;
    InherentEntitlements = X;
    InherentPermissions = X;

    fields
    {
        field(1; Type; Enum "Order Copilot Type")
        {
            DataClassification = ToBeClassified;
        }
        field(2; DocumentNo; Code[20])
        {
            DataClassification = ToBeClassified;
        }
        field(3; Arguments; Text[2048])
        {
            DataClassification = ToBeClassified;
        }
    }

    local procedure GetSourceHeader(): RecordId
    var
        SalesHeader: Record "Sales Header";
    begin
        case
            Rec.Type of
            Enum::"Order Copilot Type"::"Order Status":
                begin
                    SalesHeader.Get("Sales Document Type"::Order, Rec.DocumentNo);
                    exit(SalesHeader.RecordId);
                end;
            Enum::"Order Copilot Type"::"Create Quote":
                begin
                    SalesHeader.Get("Sales Document Type"::Quote, Rec.DocumentNo);
                    exit(SalesHeader.RecordId);
                end;
        end;
    end;

    procedure DeleteSalesQuote()
    begin
        //ToDo: Implement the logic to delete the sales quote.
    end;

    procedure GetStatus(): Enum "Sales Document Status"
    var
        SalesHeader: Record "Sales Header";
    begin
        case
            Rec.Type of
            Enum::"Order Copilot Type"::"Order Status":
                begin
                    SalesHeader.Get("Sales Document Type"::Order, Rec.DocumentNo);
                    exit(SalesHeader.Status);
                end;
            Enum::"Order Copilot Type"::"Create Quote":
                begin
                    SalesHeader.Get("Sales Document Type"::Quote, Rec.DocumentNo);
                    exit(SalesHeader.Status);
                end;
        end;
    end;

    procedure GetStatusStyleText(Status: Enum "Sales Document Status") StatusStyleText: Text
    begin
        if Status = Status::Open then
            StatusStyleText := 'Favorable'
        else
            StatusStyleText := 'Strong';
    end;

    internal procedure ShowSourceHeaderDocument()
    var
        SalesHeader: Record "Sales Header";
        SourceLineRecId: RecordId;
    begin
        SourceLineRecId := GetSourceHeader();
        case SourceLineRecId.TableNo of
            Database::"Sales Header":
                begin
                    SourceLineRecId.GetRecord().SetTable(SalesHeader);
                    RunSalesHeaderPage(SalesHeader);
                end;
        end;
    end;

    local procedure RunSalesHeaderPage(var SalesHeader: Record "Sales Header")
    begin
        case SalesHeader."Document Type" of
            SalesHeader."Document Type"::Order:
                PAGE.RunModal(Page::"Sales Order", SalesHeader);
            SalesHeader."Document Type"::Invoice:
                PAGE.RunModal(Page::"Sales Invoice", SalesHeader);
            SalesHeader."Document Type"::"Credit Memo":
                PAGE.RunModal(Page::"Sales Credit Memo", SalesHeader);
            SalesHeader."Document Type"::"Blanket Order":
                PAGE.RunModal(Page::"Blanket Sales Order", SalesHeader);
            SalesHeader."Document Type"::"Return Order":
                PAGE.RunModal(Page::"Sales Return Order", SalesHeader);
            SalesHeader."Document Type"::Quote:
                PAGE.RunModal(Page::"Sales Quote", SalesHeader);
        end;
    end;

}