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
/// Simple JSON Format
/// </summary>
codeunit 50102 "SimpleJson Format" implements "E-Document"
{
    Access = Internal;

    // ============================================================================
    // OUTGOING DOCUMENTS 
    // Exercise 1
    // Validate that required fields are filled before creating the document.
    // ============================================================================

    procedure Check(var SourceDocumentHeader: RecordRef; EDocumentService: Record "E-Document Service"; EDocumentProcessingPhase: Enum "E-Document Processing Phase")
    var
        SalesInvoiceHeader: Record "Sales Invoice Header";
    begin
        case SourceDocumentHeader.Number of
            Database::"Sales Invoice Header":
                begin
                    // Validation complete
                    SourceDocumentHeader.Field(SalesInvoiceHeader.FieldNo("Sell-to Customer No.")).TestField();

                    // TODO: Exercise 1.A: Validate Posting Date
                    SourceDocumentHeader.Field(SalesInvoiceHeader.FieldNo("Posting Date")).TestField();
                end;
        end;
    end;

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

        // Fields
        RootObject.Add('documentType', 'Invoice');
        RootObject.Add('documentNo', SalesInvoiceHeader."No.");
        RootObject.Add('postingDate', Format(SalesInvoiceHeader."Posting Date", 0, '<Year4>-<Month,2>-<Day,2>'));
        RootObject.Add('currencyCode', SalesInvoiceHeader."Currency Code");
        RootObject.Add('totalAmount', Format(SalesInvoiceHeader."Amount Including VAT", 0, 9));

        // TODO: Exercise 1.B - Fill in customerNo and customerName, vendorNo and VendorName for header
        // It is important that you have a vendor with the same number in your system, since when you receive the data you will have to pick the vendor based on the VendorNo
        RootObject.Add('customerNo', SalesInvoiceHeader."Sell-to Customer No.");
        RootObject.Add('customerName', SalesInvoiceHeader."Sell-to Customer Name");
        
        // Hardcoded for simplicity. Normally this would be company information
        RootObject.Add('vendorNo', '10000');
        RootObject.Add('vendorName', 'Adatum Corporation');

        // Create lines array
        if SalesInvoiceLine.FindSet() then
            repeat
                Clear(LineObject);
                LineObject.Add('lineNo', SalesInvoiceLine."Line No.");
                LineObject.Add('type', Format(SalesInvoiceLine.Type));
                LineObject.Add('no', SalesInvoiceLine."No.");
                LineObject.Add('unitPrice', SalesInvoiceLine."Unit Price");
                LineObject.Add('lineAmount', SalesInvoiceLine."Amount Including VAT");

                // TODO: Exercise 1.B - Fill in description and quantity for line
                LineObject.Add('description', SalesInvoiceLine.Description);
                LineObject.Add('quantity', SalesInvoiceLine.Quantity);

                LinesArray.Add(LineObject);
            until SalesInvoiceLine.Next() = 0;

        RootObject.Add('lines', LinesArray);

        RootObject.WriteTo(JsonText);
        OutStr.WriteText(JsonText);
    end;

    procedure CreateBatch(EDocumentService: Record "E-Document Service"; var EDocuments: Record "E-Document"; var SourceDocumentHeaders: RecordRef; var SourceDocumentsLines: RecordRef; var TempBlob: Codeunit "Temp Blob")
    begin
        Error('Batch creation is not implemented in this workshop version');
    end;

    // ============================================================================
    // INCOMING DOCUMENTS 
    // Exercise 4
    // Parse information from received JSON document.
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
            EDocument."Incoming E-Document No." := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Incoming E-Document No."));
            
        // Extract posting date
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'postingDate', JsonToken) then
            EDocument."Document Date" := SimpleJsonHelper.GetJsonTokenDate(JsonToken);

        // Extract currency code
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'currencyCode', JsonToken) then
            EDocument."Currency Code" := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Currency Code"));


        // TODO: Exercise 2.A - Fill in the vendor information and total amount

        // TODO: Extract vendor number (from "vendorNo" in JSON)
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'vendorNo', JsonToken) then
            EDocument."Bill-to/Pay-to No." := SimpleJsonHelper.getJsonTokenValue(JsonToken);
            
        // TODO: Extract vendor name
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'vendorName', JsonToken) then
            EDocument."Bill-to/Pay-to Name" := SimpleJsonHelper.getJsonTokenValue(JsonToken);

        // TODO: Extract total amount
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'totalAmount', JsonToken) then
            EDocument."Amount Incl. VAT" := SimpleJsonHelper.GetJsonTokenDecimal(JsonToken);
    end;

    procedure GetCompleteInfoFromReceivedDocument(var EDocument: Record "E-Document"; var CreatedDocumentHeader: RecordRef; var CreatedDocumentLines: RecordRef; var TempBlob: Codeunit "Temp Blob")
    var
        PurchaseHeader: Record "Purchase Header" temporary;
        PurchaseLine: Record "Purchase Line" temporary;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        JsonArray: JsonArray;
        JsonLineToken: JsonToken;
        SimpleJsonHelper: Codeunit "SimpleJson Helper";
        LineNo: Integer;
    begin
        if not SimpleJsonHelper.ReadJsonFromBlob(TempBlob, JsonObject) then
            Error('Failed to parse JSON document');

        // Create Purchase Header
        PurchaseHeader."Document Type" := PurchaseHeader."Document Type"::Invoice;
        PurchaseHeader.InitRecord();
        PurchaseHeader.Insert(true);

        // Set vendor from JSON
        if SimpleJsonHelper.SelectJsonToken(JsonObject, 'vendorNo', JsonToken) then
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

                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'no', JsonToken) then
                    PurchaseLine."No." := SimpleJsonHelper.GetJsonTokenValue(JsonToken);

                // TODO: Set description
                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'description', JsonToken) then
                    PurchaseLine.Description := SimpleJsonHelper.GetJsonTokenValue(JsonToken);

                // TODO: Set quantity
                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'quantity', JsonToken) then
                    PurchaseLine.Quantity := SimpleJsonHelper.GetJsonTokenDecimal(JsonToken);

                // TODO: Set line amount
                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'lineAmount', JsonToken) then
                    PurchaseLine.Amount := SimpleJsonHelper.GetJsonTokenDecimal(JsonToken);

                // Set unit cost
                if SimpleJsonHelper.SelectJsonToken(JsonObject, 'unitCost', JsonToken) then
                    PurchaseLine."Direct Unit Cost" := SimpleJsonHelper.GetJsonTokenDecimal(JsonToken);

                PurchaseLine.Insert(true);
                LineNo += 10000;
            end;
        end;

        // Return via RecordRef
        CreatedDocumentHeader.GetTable(PurchaseHeader);
        CreatedDocumentLines.GetTable(PurchaseLine);
    end;
}
