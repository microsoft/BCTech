// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Format;

using Microsoft.eServices.EDocument;
using Microsoft.Sales.History;
using Microsoft.Purchases.Document;
using System.Utilities;

/// <summary>
/// Implements the "E-Document" interface for SimpleJson format.
/// This codeunit converts Business Central documents to/from JSON format.
/// </summary>
codeunit 81000 "SimpleJson Format Impl." implements "E-Document"
{
    Access = Internal;

    // ============================================================================
    // OUTGOING DOCUMENTS - Convert BC documents to JSON
    // ============================================================================

    // ============================================================================
    // TODO: Exercise 1.A (5 minutes)
    // Validate that required fields are filled before creating the document.
    // 
    // TASK: Uncomment the two validation lines below
    // ============================================================================
    procedure Check(var SourceDocumentHeader: RecordRef; EDocumentService: Record "E-Document Service"; EDocumentProcessingPhase: Enum "E-Document Processing Phase")
    var
        SalesInvoiceHeader: Record "Sales Invoice Header";
    begin
        case SourceDocumentHeader.Number of
            Database::"Sales Invoice Header":
                begin
                    // TODO: Uncomment these two lines to validate required fields:
                    // SourceDocumentHeader.Field(SalesInvoiceHeader.FieldNo("Sell-to Customer No.")).TestField();
                    // SourceDocumentHeader.Field(SalesInvoiceHeader.FieldNo("Posting Date")).TestField();
                end;
        end;
    end;

    // ============================================================================
    // TODO: Exercise 1.B (15 minutes)
    // Create JSON representation of a Sales Invoice.
    //
    // TASK: Fill in the missing values marked with ??? in the code below
    // ============================================================================
    procedure Create(EDocumentService: Record "E-Document Service"; var EDocument: Record "E-Document"; var SourceDocumentHeader: RecordRef; var SourceDocumentLines: RecordRef; var TempBlob: Codeunit "Temp Blob")
    var
        OutStr: OutStream;
    begin
        TempBlob.CreateOutStream(OutStr, TextEncoding::UTF8);

        case EDocument."Document Type" of
            EDocument."Document Type"::"Sales Invoice":
                CreateSalesInvoiceJson(SourceDocumentHeader, SourceDocumentLines, OutStr);
            else
                Error('Document type %1 is not supported', EDocument."Document Type");
        end;
    end;

    local procedure CreateSalesInvoiceJson(var SourceDocumentHeader: RecordRef; var SourceDocumentLines: RecordRef; var OutStr: OutStream)
    var
        SalesInvoiceHeader: Record "Sales Invoice Header";
        SalesInvoiceLine: Record "Sales Invoice Line";
        RootObject: JsonObject;
        LinesArray: JsonArray;
        LineObject: JsonObject;
        JsonText: Text;
    begin
        // Get the actual records from RecordRef
        SourceDocumentHeader.SetTable(SalesInvoiceHeader);
        SourceDocumentLines.SetTable(SalesInvoiceLine);

        // TODO: Add header fields - Replace ??? with the correct field names
        RootObject.Add('documentType', 'Invoice');
        RootObject.Add('documentNo', SalesInvoiceHeader."No.");
        RootObject.Add('customerNo', SalesInvoiceHeader."Sell-to Customer No.");
        RootObject.Add('customerName', SalesInvoiceHeader."???");  // TODO: What field contains customer name?
        RootObject.Add('postingDate', Format(SalesInvoiceHeader."Posting Date", 0, '<Year4>-<Month,2>-<Day,2>'));
        RootObject.Add('currencyCode', SalesInvoiceHeader."Currency Code");
        RootObject.Add('totalAmount', SalesInvoiceHeader."???");  // TODO: What field has the total amount?

        // Create lines array
        if SalesInvoiceLine.FindSet() then
            repeat
                // TODO: Add line item - Replace ??? with correct field names
                Clear(LineObject);
                LineObject.Add('lineNo', SalesInvoiceLine."Line No.");
                LineObject.Add('type', Format(SalesInvoiceLine.Type));
                LineObject.Add('no', SalesInvoiceLine."No.");
                LineObject.Add('description', SalesInvoiceLine."???");  // TODO: What field has the description?
                LineObject.Add('quantity', SalesInvoiceLine."???");  // TODO: What field has quantity?
                LineObject.Add('unitPrice', SalesInvoiceLine."Unit Price");
                LineObject.Add('lineAmount', SalesInvoiceLine."Amount Including VAT");
                LinesArray.Add(LineObject);
            until SalesInvoiceLine.Next() = 0;

        // Add lines array to root object
        RootObject.Add('lines', LinesArray);

        // Write JSON to stream
        RootObject.WriteTo(JsonText);
        OutStr.WriteText(JsonText);
    end;

    // This method is for batch processing (optional/advanced)
    procedure CreateBatch(EDocumentService: Record "E-Document Service"; var EDocuments: Record "E-Document"; var SourceDocumentHeaders: RecordRef; var SourceDocumentsLines: RecordRef; var TempBlob: Codeunit "Temp Blob")
    begin
        // Not implemented for workshop - can be homework
        Error('Batch creation is not implemented in this workshop version');
    end;

    // ============================================================================
    // INCOMING DOCUMENTS - Parse JSON to BC documents
    // ============================================================================

    // ============================================================================
    // TODO: Exercise 1.C (5 minutes)
    // Parse basic information from received JSON document.
    //
    // TASK: Fill in the missing JSON field names marked with ???
    // ============================================================================
    procedure GetBasicInfoFromReceivedDocument(var EDocument: Record "E-Document"; var TempBlob: Codeunit "Temp Blob")
    var
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        SimpleJsonHelper: Codeunit "SimpleJson Helper";
    begin
        if not SimpleJsonHelper.ReadJsonFromBlob(TempBlob, JsonObject) then
            Error('Failed to parse JSON document');

        // Set document type to Purchase Invoice
        EDocument."Document Type" := EDocument."Document Type"::"Purchase Invoice";

        // Extract document number
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'documentNo', JsonToken) then
            EDocument."Document No." := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Document No."));

        // TODO: Extract vendor number (from "customerNo" in JSON) - Replace ??? with the JSON field name
        if SimpleJsonHelper.SelectJsonToken(JsonObject, '???', JsonToken) then
            EDocument."Bill-to/Pay-to No." := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Bill-to/Pay-to No."));

        // TODO: Extract vendor name - Replace ??? with the JSON field name
        if SimpleJsonHelper.SelectJsonToken(JsonObject, '???', JsonToken) then
            EDocument."Bill-to/Pay-to Name" := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Bill-to/Pay-to Name"));

        // Extract posting date
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'postingDate', JsonToken) then
            EDocument."Document Date" := SimpleJsonHelper.GetJsonTokenDate(JsonToken);

        // Extract currency code
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'currencyCode', JsonToken) then
            EDocument."Currency Code" := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Currency Code"));
    end;

    // ============================================================================
    // TODO: Exercise 1.D (5 minutes)
    // Create a Purchase Invoice from received JSON document.
    //
    // TASK: Fill in the missing field names marked with ???
    // ============================================================================
    procedure GetCompleteInfoFromReceivedDocument(var EDocument: Record "E-Document"; var CreatedDocumentHeader: RecordRef; var CreatedDocumentLines: RecordRef; var TempBlob: Codeunit "Temp Blob")
    var
        PurchaseHeader: Record "Purchase Header";
        PurchaseLine: Record "Purchase Line";
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        JsonArray: JsonArray;
        JsonLineToken: JsonToken;
        SimpleJsonHelper: Codeunit "SimpleJson Helper";
        LineNo: Integer;
    begin
        TODO: Uncomment all the code below
        if not SimpleJsonHelper.ReadJsonFromBlob(TempBlob, JsonObject) then
            Error('Failed to parse JSON document');

        // Create Purchase Header
        PurchaseHeader.Init();
        PurchaseHeader."Document Type" := PurchaseHeader."Document Type"::Invoice;
        PurchaseHeader.Insert(true);

        // Set vendor from JSON
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'customerNo', JsonToken) then
            PurchaseHeader.Validate("Buy-from Vendor No.", SimpleJsonHelper.GetJsonTokenValue(JsonToken));

        // Set posting date
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'postingDate', JsonToken) then
            PurchaseHeader.Validate("Posting Date", SimpleJsonHelper.GetJsonTokenDate(JsonToken));

        // Set currency code (if not blank)
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'currencyCode', JsonToken) then begin
            if SimpleJsonHelper.GetJsonTokenValue(JsonToken) <> '' then
                PurchaseHeader.Validate("Currency Code", SimpleJsonHelper.GetJsonTokenValue(JsonToken));
        end;

        PurchaseHeader.Modify(true);

        // Create Purchase Lines from JSON array
        if JsonObject.Get('lines', JsonToken) then begin
            JsonArray := JsonToken.AsArray();
            LineNo := 10000;

            foreach JsonLineToken in JsonArray do begin
                JsonObject := JsonLineToken.AsObject();

                PurchaseLine.Init();
                PurchaseLine."Document Type" := PurchaseHeader."Document Type";
                PurchaseLine."Document No." := PurchaseHeader."No.";
                PurchaseLine."Line No." := LineNo;
                PurchaseLine.Type := PurchaseLine.Type::Item;

                // TODO: Set item number - Replace ??? with the JSON field name for item number
                if SimpleJsonHelper.SelectJsonToken(JsonObject, '???', JsonToken) then
                    PurchaseLine.Validate("No.", SimpleJsonHelper.GetJsonTokenValue(JsonToken));

                // Set description
                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'description', JsonToken) then
                    PurchaseLine.Description := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(PurchaseLine.Description));

                // TODO: Set quantity - Replace ??? with the JSON field name for quantity
                if SimpleJsonHelper.SelectJsonToken(JsonObject, '???', JsonToken) then
                    PurchaseLine.Validate(Quantity, SimpleJsonHelper.GetJsonTokenDecimal(JsonToken));

                // Set unit cost
                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'unitPrice', JsonToken) then
                    PurchaseLine.Validate("Direct Unit Cost", SimpleJsonHelper.GetJsonTokenDecimal(JsonToken));

                PurchaseLine.Insert(true);
                LineNo += 10000;
            end;
        end;

        // Return via RecordRef
        CreatedDocumentHeader.GetTable(PurchaseHeader);
        CreatedDocumentLines.GetTable(PurchaseLine);
    end;
}
