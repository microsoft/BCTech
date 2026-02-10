// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

using Microsoft.eServices.EDocument;
using Microsoft.eServices.EDocument.Integration.Send;
using Microsoft.eServices.EDocument.Integration.Receive;
using Microsoft.eServices.EDocument.Integration.Interfaces;
using System.Utilities;

/// <summary>
/// Implements the IDocumentSender and IDocumentReceiver interfaces for Connector.
/// This codeunit handles sending and receiving E-Documents via the Connector API.
/// </summary>
codeunit 50123 "Connector Integration" implements IDocumentSender, IDocumentReceiver
{
    Access = Internal;

    // ============================================================================
    // SENDING DOCUMENTS
    // ============================================================================

    // ============================================================================
    // TODO: Exercise 2 (10 minutes)
    // Now send the SimpleJSON to the endpoint, using the Connector API.
    //
    // TASK: Do the TODOs in the Send procedure below
    // ============================================================================
    procedure Send(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; SendContext: Codeunit SendContext)
    var
        ConnectorSetup: Record "Connector Connection Setup";
        ConnectorAuth: Codeunit "Connector Auth";
        ConnectorRequests: Codeunit "Connector Requests";
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        TempBlob: Codeunit "Temp Blob";
        JsonContent, APIEndpoint : Text;
    begin
        ConnectorAuth.GetConnectionSetup(ConnectorSetup);

        // TODO: Get temp blob with json from SendContext
        // - Tips: Use SendContext.GetTempBlob()
        // - Tips: Use ConnectorRequests.ReadJsonFromBlob(TempBlob) to read json text from blob


        TempBlob := SendContext.GetTempBlob();
        JsonContent := ConnectorRequests.ReadJsonFromBlob(TempBlob);
        // <Add code here>

        // TODO: Create POST request to 'enqueue' endpoint
        // - Tips: Add enqueue to the base URL from ConnectorSetup

        // <Add code here>
        APIEndpoint := ConnectorSetup."API Base URL" + 'enqueue';

        ConnectorRequests.CreatePostRequest(APIEndpoint, JsonContent, HttpRequest);
        ConnectorAuth.AddAuthHeader(HttpRequest, ConnectorSetup);

        // TODO: Send the HTTP request and handle the response using HttpClient

        if not HttpClient.Send(HttpRequest, HttpResponse) then
            Error('Failed to connect to the API server.');
        // <Add code here>

        SendContext.Http().SetHttpRequestMessage(HttpRequest);
        SendContext.Http().SetHttpResponseMessage(HttpResponse);
        ConnectorRequests.CheckResponseSuccess(HttpResponse);
    end;

    // ============================================================================
    // RECEIVING DOCUMENTS
    // ============================================================================

    // ============================================================================
    // TODO: Exercise 3,A (10 minutes)
    // Receive a list of documents from the Connector API.
    //
    // TASK: Do the todos 
    // ============================================================================
    procedure ReceiveDocuments(var EDocumentService: Record "E-Document Service"; DocumentsMetadata: Codeunit "Temp Blob List"; ReceiveContext: Codeunit ReceiveContext)
    var
        ConnectorSetup: Record "Connector Connection Setup";
        ConnectorAuth: Codeunit "Connector Auth";
        ConnectorRequests: Codeunit "Connector Requests";
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        JsonArray: JsonArray;
        TempBlob: Codeunit "Temp Blob";
        ResponseText: Text;
        DocumentJson: Text;
        APIEndpoint: Text;
    begin
        ConnectorAuth.GetConnectionSetup(ConnectorSetup);


        // TODO: Create Get request to 'peek' endpoint
        // - Tips: Add peek to the base URL from ConnectorSetup
        APIEndpoint := ConnectorSetup."API Base URL" + 'peek';

        // <Add code here>

        ConnectorRequests.CreateGetRequest(APIEndpoint, HttpRequest);
        ConnectorAuth.AddAuthHeader(HttpRequest, ConnectorSetup);


        // TODO: Send the HTTP request and handle the response using HttpClient

        if not HttpClient.Send(HttpRequest, HttpResponse) then
            Error('Failed to connect to the API server.');
        // <Add code here>

        ReceiveContext.Http().SetHttpRequestMessage(HttpRequest);
        ReceiveContext.Http().SetHttpResponseMessage(HttpResponse);
        ConnectorRequests.CheckResponseSuccess(HttpResponse);

        ResponseText := ConnectorRequests.GetResponseText(HttpResponse);

        if JsonObject.ReadFrom(ResponseText) then begin
            if JsonObject.Get('items', JsonToken) then begin
                JsonArray := JsonToken.AsArray();
                foreach JsonToken in JsonArray do begin
                    Clear(TempBlob);
                    JsonToken.WriteTo(DocumentJson);
                    ConnectorRequests.WriteTextToBlob(DocumentJson, TempBlob);

                    // TODO: Add TempBlob to DocumentsMetadata so we can process it later in DownloadDocument
                    DocumentsMetadata.Add(TempBlob);
                    // <Add code here>
                end;
            end;
        end;
    end;

    // ============================================================================
    // TODO: Exercise 3.B (5 minutes)
    // Download a single document from the Connector API (dequeue).
    //
    // TASK: Do the todos 
    // ============================================================================
    procedure DownloadDocument(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; DocumentMetadata: codeunit "Temp Blob"; ReceiveContext: Codeunit ReceiveContext)
    var
        ConnectorSetup: Record "Connector Connection Setup";
        ConnectorAuth: Codeunit "Connector Auth";
        ConnectorRequests: Codeunit "Connector Requests";
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        TempBlob: Codeunit "Temp Blob";
        ResponseText: Text;
        DocumentJson: Text;
        APIEndpoint: Text;
    begin
        ConnectorAuth.GetConnectionSetup(ConnectorSetup);

        // TODO: Create Get request to 'dequeue' endpoint
        // - Tips: Add dequeue to the base URL from ConnectorSetup

        APIEndpoint := ConnectorSetup."API Base URL" + 'dequeue';
        // <Add code here>

        ConnectorRequests.CreateGetRequest(APIEndpoint, HttpRequest);
        ConnectorAuth.AddAuthHeader(HttpRequest, ConnectorSetup);

        // TODO: Send the HTTP request and handle the response using HttpClient
        // <Add code here>
        if not HttpClient.Send(HttpRequest, HttpResponse) then
            Error('Failed to connect to the API server.');

        ReceiveContext.Http().SetHttpRequestMessage(HttpRequest);
        ReceiveContext.Http().SetHttpResponseMessage(HttpResponse);
        ConnectorRequests.CheckResponseSuccess(HttpResponse);

        ResponseText := ConnectorRequests.GetResponseText(HttpResponse);
        if JsonObject.ReadFrom(ResponseText) then begin
            if JsonObject.Get('document', JsonToken) then begin
                JsonToken.WriteTo(DocumentJson);
                TempBlob := ReceiveContext.GetTempBlob();
                ConnectorRequests.WriteTextToBlob(DocumentJson, TempBlob);
            end else
                Error('No document found in response.');
        end;
    end;

    // ============================================================================
    // Event Subscriber - Opens setup page when configuring the service
    // ============================================================================
    [EventSubscriber(ObjectType::Page, Page::"E-Document Service", OnBeforeOpenServiceIntegrationSetupPage, '', false, false)]
    local procedure OnBeforeOpenServiceIntegrationSetupPage(EDocumentService: Record "E-Document Service"; var IsServiceIntegrationSetupRun: Boolean)
    var
        ConnectorSetup: Page "Connector Connection Setup";
    begin
        if EDocumentService."Service Integration V2" <> EDocumentService."Service Integration V2"::Connector then
            exit;

        ConnectorSetup.RunModal();
        IsServiceIntegrationSetupRun := true;
    end;
}
