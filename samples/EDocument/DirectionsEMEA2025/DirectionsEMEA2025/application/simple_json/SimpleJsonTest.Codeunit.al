// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Format;

using System.TestLibraries.Utilities;
using Microsoft.eServices.EDocument;
using Microsoft.Sales.History;
using System.Utilities;
using Microsoft.Purchases.Document;

/// <summary>
/// Simple test runner for workshop participants.
/// Run this codeunit to validate your exercise implementations.
/// </summary>
codeunit 50113 "SimpleJson Test"
{

    Subtype = Test;

    var
        Assert: Codeunit "Library Assert";
        LibraryLowerPermission: Codeunit "Library - Lower Permissions";
        TestCustomerNo: Code[20];
        TestVendorNo: Code[20];

    local procedure Initialize()
    begin
        LibraryLowerPermission.SetOutsideO365Scope();
        TestCustomerNo := 'CUST1';
        TestVendorNo := 'VEND1';
    end;


    [Test]
    procedure TestExercise1_CheckValidation()
    var
        EDocumentService: Record "E-Document Service";
        SimpleJsonFormat: Codeunit "SimpleJson Format";
        SalesInvoiceHeader: Record "Sales Invoice Header";
        SalesInvoiceLine: Record "Sales Invoice Line";
        SourceDocumentHeader: RecordRef;
        EDocProcessingPhase: Enum "E-Document Processing Phase";
    begin
        Initialize();
        // [GIVEN] A Sales Invoice Header with missing required fields
        CreateTestSalesInvoiceWithLines(SalesInvoiceHeader, SalesInvoiceLine);
        Clear(SalesInvoiceHeader."Posting Date"); // Invalidate required field
        SourceDocumentHeader.GetTable(SalesInvoiceHeader);

        // [WHEN] Calling Check method, [THEN] it should fail validation
        asserterror SimpleJsonFormat.Check(SourceDocumentHeader, EDocumentService, EDocProcessingPhase::Create);
        Assert.ExpectedError('Sell-to Customer No.');
    end;


    [Test]
    procedure TestExercise1_CheckCreate()
    var
        EDocument: Record "E-Document";
        EDocumentService: Record "E-Document Service";
        SimpleJsonFormat: Codeunit "SimpleJson Format";
        SalesInvoiceHeader: Record "Sales Invoice Header";
        SalesInvoiceLine: Record "Sales Invoice Line";
        TempBlob: Codeunit "Temp Blob";
        SourceDocumentHeader: RecordRef;
        SourceDocumentLines: RecordRef;
        EDocProcessingPhase: Enum "E-Document Processing Phase";
    begin
        Initialize();
        // [GIVEN] A Sales Invoice Header with missing required fields
        CreateTestSalesInvoiceWithLines(SalesInvoiceHeader, SalesInvoiceLine);
        SourceDocumentHeader.GetTable(SalesInvoiceHeader);
        SourceDocumentLines.GetTable(SalesInvoiceLine);
        EDocument."Document Type" := EDocument."Document Type"::"Sales Invoice";

        // [WHEN] Calling create method, [THEN] it should succeed
        SimpleJsonFormat.Create(EDocumentService, EDocument, SourceDocumentHeader, SourceDocumentLines, TempBlob);

        // [THEN] JSON should be created successfully
        VerifyJsonContent(TempBlob, SalesInvoiceHeader, SalesInvoiceLine);
    end;

    [Test]
    procedure TestExercise2_OutgoingMethodsWork()
    var
        EDocumentService: Record "E-Document Service";
        EDocument: Record "E-Document";
        SimpleJsonFormat: Codeunit "SimpleJson Format";
        SalesInvoiceHeader: Record "Sales Invoice Header";
        SalesInvoiceLine: Record "Sales Invoice Line";
        SourceDocumentHeader: RecordRef;
        SourceDocumentLines: RecordRef;
        TempBlob: Codeunit "Temp Blob";
        EDocProcessingPhase: Enum "E-Document Processing Phase";
    begin
        Initialize();
        // [GIVEN] A complete Sales Invoice
        CreateTestSalesInvoiceWithLines(SalesInvoiceHeader, SalesInvoiceLine);
        SourceDocumentHeader.GetTable(SalesInvoiceHeader);
        SourceDocumentLines.GetTable(SalesInvoiceLine);
        EDocument."Document Type" := EDocument."Document Type"::"Sales Invoice";

        // [WHEN/THEN] Check should work (no error)
        SimpleJsonFormat.Check(SourceDocumentHeader, EDocumentService, EDocProcessingPhase::Create);

        // [WHEN/THEN] Create should work (no error)
        SimpleJsonFormat.Create(EDocumentService, EDocument, SourceDocumentHeader, SourceDocumentLines, TempBlob);

        // [THEN] JSON should be created successfully
        VerifyJsonContent(TempBlob, SalesInvoiceHeader, SalesInvoiceLine);
    end;

    [Test]
    procedure TestExercise2_GetBasicInfoFromJSON()
    var
        EDocument: Record "E-Document";
        SimpleJsonFormat: Codeunit "SimpleJson Format";
        TempBlob: Codeunit "Temp Blob";
        JsonText: Text;
    begin
        Initialize();
        // [GIVEN] A JSON document with customer information
        JsonText := GetTestPurchaseInvoiceJson();
        WriteJsonToBlob(JsonText, TempBlob);

        // [WHEN] Parsing basic info from JSON
        SimpleJsonFormat.GetBasicInfoFromReceivedDocument(EDocument, TempBlob);

        // [THEN] EDocument should have correct basic information
        Assert.AreEqual(EDocument."Document Type"::"Purchase Invoice", EDocument."Document Type", 'Should be Purchase Invoice');
        Assert.AreEqual('TEST-PI-001', EDocument."Document No.", 'Document No should match');
        Assert.AreEqual(TestVendorNo, EDocument."Bill-to/Pay-to No.", 'Vendor No should match');
        Assert.AreEqual('Test Vendor', EDocument."Bill-to/Pay-to Name", 'Vendor Name should match');
        Assert.AreEqual(Today(), EDocument."Document Date", 'Document Date should match');
        Assert.AreEqual('USD', EDocument."Currency Code", 'Currency Code should match');
    end;

    [Test]
    procedure TestExercise2_CreatePurchaseInvoiceFromJSON()
    var
        EDocument: Record "E-Document";
        SimpleJsonFormat: Codeunit "SimpleJson Format";
        PurchaseHeader: Record "Purchase Header";
        PurchaseLine: Record "Purchase Line";
        CreatedDocumentHeader: RecordRef;
        CreatedDocumentLines: RecordRef;
        TempBlob: Codeunit "Temp Blob";
        JsonText: Text;
    begin
        Initialize();
        // [GIVEN] A JSON document with complete invoice data
        JsonText := GetTestPurchaseInvoiceJson();
        WriteJsonToBlob(JsonText, TempBlob);

        // [WHEN] Creating Purchase Invoice from JSON
        SimpleJsonFormat.GetCompleteInfoFromReceivedDocument(EDocument, CreatedDocumentHeader, CreatedDocumentLines, TempBlob);

        // [THEN] Purchase Header should be created correctly
        CreatedDocumentHeader.SetTable(PurchaseHeader);
        Assert.AreEqual(PurchaseHeader."Document Type"::Invoice, PurchaseHeader."Document Type", 'Should be Invoice type');
        Assert.AreEqual(TestVendorNo, PurchaseHeader."Buy-from Vendor No.", 'Vendor should match');
        Assert.AreEqual(Today(), PurchaseHeader."Posting Date", 'Posting Date should match');
        Assert.AreEqual('USD', PurchaseHeader."Currency Code", 'Currency should match');

        // [THEN] Purchase Lines should be created correctly
        CreatedDocumentLines.SetTable(PurchaseLine);
        PurchaseLine.SetRange("Document No.", PurchaseHeader."No.");
        Assert.IsTrue(PurchaseLine.FindFirst(), 'Should have purchase lines');
        Assert.AreEqual('ITEM-001', PurchaseLine."No.", 'Item No should match');
        Assert.AreEqual('Test Item', PurchaseLine.Description, 'Description should match');
        Assert.AreEqual(5, PurchaseLine.Quantity, 'Quantity should match');
        Assert.AreEqual(200, PurchaseLine."Direct Unit Cost", 'Unit Cost should match');
    end;

    local procedure CreateTestSalesInvoice(var SalesInvoiceHeader: Record "Sales Invoice Header"; CustomerNo: Code[20]; PostingDate: Date)
    begin
        SalesInvoiceHeader.Init();
        SalesInvoiceHeader."No." := 'TEST-SI-001';
        SalesInvoiceHeader."Sell-to Customer No." := CustomerNo;
        SalesInvoiceHeader."Sell-to Customer Name" := 'Test Customer';
        SalesInvoiceHeader."Posting Date" := PostingDate;
        SalesInvoiceHeader."Currency Code" := 'USD';
        SalesInvoiceHeader."Amount Including VAT" := 1000;
        SalesInvoiceHeader.Insert();
    end;

    local procedure CreateTestSalesInvoiceWithLines(var SalesInvoiceHeader: Record "Sales Invoice Header"; var SalesInvoiceLine: Record "Sales Invoice Line")
    begin
        CreateTestSalesInvoice(SalesInvoiceHeader, TestCustomerNo, Today());

        SalesInvoiceLine.Init();
        SalesInvoiceLine."Document No." := SalesInvoiceHeader."No.";
        SalesInvoiceLine."Line No." := 10000;
        SalesInvoiceLine.Type := SalesInvoiceLine.Type::Item;
        SalesInvoiceLine."No." := 'ITEM-001';
        SalesInvoiceLine.Description := 'Test Item';
        SalesInvoiceLine.Quantity := 5;
        SalesInvoiceLine."Unit Price" := 200;
        SalesInvoiceLine."Amount Including VAT" := 1000;
        SalesInvoiceLine.Insert();
    end;

    local procedure VerifyJsonContent(var TempBlob: Codeunit "Temp Blob"; SalesInvoiceHeader: Record "Sales Invoice Header"; SalesInvoiceLine: Record "Sales Invoice Line")
    var
        InStr: InStream;
        JsonText: Text;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        JsonArray: JsonArray;
        LineObject: JsonObject;
    begin
        TempBlob.CreateInStream(InStr, TextEncoding::UTF8);
        InStr.ReadText(JsonText);

        Assert.IsTrue(JsonObject.ReadFrom(JsonText), 'Should be valid JSON');

        // Verify header
        Assert.IsTrue(JsonObject.Get('documentNo', JsonToken), 'Should have documentNo');
        Assert.AreEqual(SalesInvoiceHeader."No.", JsonToken.AsValue().AsText(), 'DocumentNo should match');

        Assert.IsTrue(JsonObject.Get('customerName', JsonToken), 'Should have customerName');
        Assert.AreEqual(SalesInvoiceHeader."Sell-to Customer Name", JsonToken.AsValue().AsText(), 'CustomerName should match');

        Assert.IsTrue(JsonObject.Get('totalAmount', JsonToken), 'Should have totalAmount');
        Assert.AreEqual(SalesInvoiceHeader."Amount Including VAT", JsonToken.AsValue().AsDecimal(), 'TotalAmount should match');

        // Verify lines
        Assert.IsTrue(JsonObject.Get('lines', JsonToken), 'Should have lines');
        JsonArray := JsonToken.AsArray();
        Assert.AreEqual(1, JsonArray.Count(), 'Should have one line');

        JsonArray.Get(0, JsonToken);
        LineObject := JsonToken.AsObject();

        Assert.IsTrue(LineObject.Get('description', JsonToken), 'Line should have description');
        Assert.AreEqual(SalesInvoiceLine.Description, JsonToken.AsValue().AsText(), 'Line description should match');

        Assert.IsTrue(LineObject.Get('quantity', JsonToken), 'Line should have quantity');
        Assert.AreEqual(SalesInvoiceLine.Quantity, JsonToken.AsValue().AsDecimal(), 'Line quantity should match');
    end;

    local procedure GetTestPurchaseInvoiceJson(): Text
    var
        JsonObject: JsonObject;
        LinesArray: JsonArray;
        LineObject: JsonObject;
        JsonText: Text;
    begin
        // Create test JSON that matches the expected format
        JsonObject.Add('documentType', 'Invoice');
        JsonObject.Add('documentNo', 'TEST-PI-001');
        JsonObject.Add('customerNo', TestVendorNo); // Note: In JSON it's customerNo, but we map to vendor
        JsonObject.Add('customerName', 'Test Vendor');
        JsonObject.Add('postingDate', Format(Today(), 0, '<Year4>-<Month,2>-<Day,2>'));
        JsonObject.Add('currencyCode', 'USD');
        JsonObject.Add('totalAmount', 1000);

        // Add line
        LineObject.Add('lineNo', 10000);
        LineObject.Add('type', 'Item');
        LineObject.Add('no', 'ITEM-001');
        LineObject.Add('description', 'Test Item');
        LineObject.Add('quantity', 5);
        LineObject.Add('unitPrice', 200);
        LineObject.Add('lineAmount', 1000);
        LinesArray.Add(LineObject);
        JsonObject.Add('lines', LinesArray);

        JsonObject.WriteTo(JsonText);
        exit(JsonText);
    end;

    local procedure WriteJsonToBlob(JsonText: Text; var TempBlob: Codeunit "Temp Blob")
    var
        OutStr: OutStream;
    begin
        TempBlob.CreateOutStream(OutStr, TextEncoding::UTF8);
        OutStr.WriteText(JsonText);
    end;

}