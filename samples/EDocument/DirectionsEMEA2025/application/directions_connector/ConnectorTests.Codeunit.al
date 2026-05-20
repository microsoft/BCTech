// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Format;

using System.TestLibraries.Utilities;
using Microsoft.eServices.EDocument;
using Microsoft.EServices.EDocument.Integration;
using Microsoft.eServices.EDocument.Integration.Send;
using Microsoft.eServices.EDocument.Integration.Receive;
using System.Utilities;

/// <summary>
/// Test suite for Connector Integration exercises.
/// Tests verify that the three main exercises are implemented correctly:
/// - Exercise 2.A: Send (enqueue documents)
/// - Exercise 2.B: ReceiveDocuments (peek at documents)
/// - Exercise 2.C: DownloadDocument (dequeue documents)
/// </summary>
codeunit 50129 "Connector Tests"
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

    // ============================================================================
    // EXERCISE 2.A: Test Send (Enqueue) functionality
    // Verifies that the Send procedure correctly:
    // - Gets the TempBlob from SendContext
    // - Reads JSON content from the blob
    // - Creates POST request to /enqueue endpoint
    // - Sends the request using HttpClient
    // ============================================================================
    [Test]
    procedure TestExercise2A_Send()
    var
        EDocument: Record "E-Document";
        EDocumentService: Record "E-Document Service";
        ConnectorIntegration: Codeunit "Connector Integration";
        SendContext: Codeunit SendContext;
        TempBlob: Codeunit "Temp Blob";
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        HttpHeaders: HttpHeaders;
        JsonText: Text;
        RequestUri: Text;
    begin
        Initialize();
        SetupConnectionSetup();

        // [GIVEN] A JSON document to send
        JsonText := GetTestSalesInvoiceJson();
        WriteJsonToBlob(JsonText, TempBlob);
        SendContext.SetTempBlob(TempBlob);

        // [WHEN] Calling Send to enqueue the document
        asserterror ConnectorIntegration.Send(EDocument, EDocumentService, SendContext);

        // [THEN] The HTTP request should be created correctly
        HttpRequest := SendContext.Http().GetHttpRequestMessage();
        HttpResponse := SendContext.Http().GetHttpResponseMessage();

        // Verify POST method was used
        Assert.AreEqual('POST', HttpRequest.Method(), 'Should use POST method');

        // Verify the endpoint is /enqueue
        RequestUri := HttpRequest.GetRequestUri();
        Assert.IsTrue(RequestUri.EndsWith('/enqueue'), 'Should call /enqueue endpoint');

        // Verify the request has content
        HttpRequest.Content.GetHeaders(HttpHeaders);
        Assert.IsTrue(HttpHeaders.Contains('Content-Type'), 'Should have Content-Type header');
    end;

    // ============================================================================
    // EXERCISE 2.B: Test ReceiveDocuments (Peek) functionality  
    // Verifies that the ReceiveDocuments procedure correctly:
    // - Creates GET request to /peek endpoint
    // - Sends the request using HttpClient
    // - Parses the JSON response array
    // - Adds each document to DocumentsMetadata
    // ============================================================================
    [Test]
    procedure TestExercise2B_ReceiveDocuments()
    var
        EDocumentService: Record "E-Document Service";
        ConnectorIntegration: Codeunit "Connector Integration";
        ReceiveContext: Codeunit ReceiveContext;
        DocumentsMetadata: Codeunit "Temp Blob List";
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        RequestUri: Text;
    begin
        Initialize();
        SetupConnectionSetup();

        // [WHEN] Calling ReceiveDocuments to peek at available documents
        asserterror ConnectorIntegration.ReceiveDocuments(EDocumentService, DocumentsMetadata, ReceiveContext);

        // [THEN] The HTTP request should be created correctly
        HttpRequest := ReceiveContext.Http().GetHttpRequestMessage();
        HttpResponse := ReceiveContext.Http().GetHttpResponseMessage();

        // Verify GET method was used
        Assert.AreEqual('GET', HttpRequest.Method(), 'Should use GET method');

        // Verify the endpoint is /peek
        RequestUri := HttpRequest.GetRequestUri();
        Assert.IsTrue(RequestUri.EndsWith('/peek'), 'Should call /peek endpoint');
    end;

    // ============================================================================
    // EXERCISE 2.C: Test DownloadDocument (Dequeue) functionality
    // Verifies that the DownloadDocument procedure correctly:
    // - Creates GET request to /dequeue endpoint
    // - Sends the request using HttpClient
    // - Parses the document from JSON response
    // - Writes the document to TempBlob in ReceiveContext
    // ============================================================================
    [Test]
    procedure TestExercise2C_DownloadDocument()
    var
        EDocument: Record "E-Document";
        EDocumentService: Record "E-Document Service";
        ConnectorIntegration: Codeunit "Connector Integration";
        ReceiveContext: Codeunit ReceiveContext;
        DocumentMetadata: Codeunit "Temp Blob";
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        RequestUri: Text;
    begin
        Initialize();
        SetupConnectionSetup();

        // [WHEN] Calling DownloadDocument to dequeue a document
        asserterror ConnectorIntegration.DownloadDocument(EDocument, EDocumentService, DocumentMetadata, ReceiveContext);

        // [THEN] The HTTP request should be created correctly
        HttpRequest := ReceiveContext.Http().GetHttpRequestMessage();
        HttpResponse := ReceiveContext.Http().GetHttpResponseMessage();

        // Verify GET method was used
        Assert.AreEqual('GET', HttpRequest.Method(), 'Should use GET method');

        // Verify the endpoint is /dequeue
        RequestUri := HttpRequest.GetRequestUri();
        Assert.IsTrue(RequestUri.EndsWith('/dequeue'), 'Should call /dequeue endpoint');
    end;

    // ============================================================================
    // HELPER: Verify JSON reading and writing helpers work
    // ============================================================================
    [Test]
    procedure TestJsonHelpers()
    var
        ConnectorRequests: Codeunit "Connector Requests";
        TempBlob: Codeunit "Temp Blob";
        JsonText: Text;
        ReadText: Text;
    begin
        Initialize();
        // [GIVEN] Test JSON content
        JsonText := GetTestSalesInvoiceJson();

        // [WHEN] Writing JSON to blob and reading it back
        ConnectorRequests.WriteTextToBlob(JsonText, TempBlob);
        ReadText := ConnectorRequests.ReadJsonFromBlob(TempBlob);

        // [THEN] Content should match
        Assert.AreEqual(JsonText, ReadText, 'JSON content should be preserved');
    end;

    // ============================================================================
    // HELPER METHODS
    // ============================================================================

    local procedure SetupConnectionSetup()
    var
        ConnectorSetup: Record "Connector Connection Setup";
    begin
        if not ConnectorSetup.Get() then begin
            ConnectorSetup.Init();
            ConnectorSetup."API Base URL" := 'https://test.azurewebsites.net/';
            ConnectorSetup.SetAPIKey('12345678-1234-1234-1234-123456789abc');
            ConnectorSetup.Insert();
        end;
    end;

    local procedure GetTestSalesInvoiceJson(): Text
    var
        JsonObject: JsonObject;
        LinesArray: JsonArray;
        LineObject: JsonObject;
        JsonText: Text;
    begin
        // Create test JSON that matches the expected format for sales
        JsonObject.Add('documentType', 'Invoice');
        JsonObject.Add('documentNo', 'TEST-SI-001');
        JsonObject.Add('customerNo', TestCustomerNo);
        JsonObject.Add('customerName', 'Test Customer');
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
