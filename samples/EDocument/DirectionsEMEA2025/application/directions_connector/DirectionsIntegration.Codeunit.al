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
/// Implements the IDocumentSender and IDocumentReceiver interfaces for DirectionsConnector.
/// This codeunit handles sending and receiving E-Documents via the Directions API.
/// </summary>
codeunit 81100 "Directions Integration Impl." implements IDocumentSender, IDocumentReceiver
{
    Access = Internal;

    // ============================================================================
    // SENDING DOCUMENTS
    // ============================================================================

    // ============================================================================
    // TODO: Exercise 2.B (10 minutes)
    // Send an E-Document to the Directions API.
    //
    // TASK: Uncomment the code below - it's already complete!
    // ============================================================================
    procedure Send(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; SendContext: Codeunit SendContext)
    var
        DirectionsSetup: Record "Directions Connection Setup";
        DirectionsAuth: Codeunit "Directions Auth";
        DirectionsRequests: Codeunit "Directions Requests";
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        TempBlob: Codeunit "Temp Blob";
        JsonContent: Text;
    begin
        // TODO: Uncomment all the code below
        // DirectionsAuth.GetConnectionSetup(DirectionsSetup);
        // SendContext.GetTempBlob(TempBlob);
        // JsonContent := DirectionsRequests.ReadJsonFromBlob(TempBlob);
        // DirectionsRequests.CreatePostRequest(DirectionsSetup."API Base URL" + 'enqueue', JsonContent, HttpRequest);
        // DirectionsAuth.AddAuthHeader(HttpRequest, DirectionsSetup);
        // SendContext.Http().SetHttpRequestMessage(HttpRequest);
        // if not HttpClient.Send(HttpRequest, HttpResponse) then
        //     Error('Failed to send document to API.');
        // SendContext.Http().SetHttpResponseMessage(HttpResponse);
        // DirectionsRequests.CheckResponseSuccess(HttpResponse);
    end;

    // ============================================================================
    // RECEIVING DOCUMENTS
    // ============================================================================

    // ============================================================================
    // TODO: Exercise 2.C (10 minutes)
    // Receive a list of documents from the Directions API.
    //
    // TASK: Uncomment the code below and fill in the ??? with the correct endpoint
    // ============================================================================
    procedure ReceiveDocuments(var EDocumentService: Record "E-Document Service"; DocumentsMetadata: Codeunit "Temp Blob List"; ReceiveContext: Codeunit ReceiveContext)
    var
        DirectionsSetup: Record "Directions Connection Setup";
        DirectionsAuth: Codeunit "Directions Auth";
        DirectionsRequests: Codeunit "Directions Requests";
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        JsonArray: JsonArray;
        TempBlob: Codeunit "Temp Blob";
        ResponseText: Text;
        DocumentJson: Text;
    begin
        // TODO: Uncomment the code below and replace ??? with 'peek'
        // DirectionsAuth.GetConnectionSetup(DirectionsSetup);
        // DirectionsRequests.CreateGetRequest(DirectionsSetup."API Base URL" + '???', HttpRequest);  // TODO: What endpoint shows the queue?
        // DirectionsAuth.AddAuthHeader(HttpRequest, DirectionsSetup);
        // ReceiveContext.Http().SetHttpRequestMessage(HttpRequest);
        // if not HttpClient.Send(HttpRequest, HttpResponse) then
        //     Error('Failed to retrieve documents from API.');
        // ReceiveContext.Http().SetHttpResponseMessage(HttpResponse);
        // DirectionsRequests.CheckResponseSuccess(HttpResponse);
        // ResponseText := DirectionsRequests.GetResponseText(HttpResponse);
        // if JsonObject.ReadFrom(ResponseText) then begin
        //     if JsonObject.Get('items', JsonToken) then begin
        //         JsonArray := JsonToken.AsArray();
        //         foreach JsonToken in JsonArray do begin
        //             Clear(TempBlob);
        //             JsonToken.WriteTo(DocumentJson);
        //             DirectionsRequests.WriteTextToBlob(DocumentJson, TempBlob);
        //             DocumentsMetadata.Add(TempBlob);
        //         end;
        //     end;
        // end;
    end;

    // ============================================================================
    // TODO: Exercise 2.D (5 minutes)
    // Download a single document from the Directions API (dequeue).
    //
    // TASK: Uncomment the code below and fill in ??? with the correct endpoint
    // ============================================================================
    procedure DownloadDocument(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; DocumentMetadata: codeunit "Temp Blob"; ReceiveContext: Codeunit ReceiveContext)
    var
        DirectionsSetup: Record "Directions Connection Setup";
        DirectionsAuth: Codeunit "Directions Auth";
        DirectionsRequests: Codeunit "Directions Requests";
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        TempBlob: Codeunit "Temp Blob";
        ResponseText: Text;
        DocumentJson: Text;
    begin
        // TODO: Uncomment the code below and replace ??? with 'dequeue'
        // DirectionsAuth.GetConnectionSetup(DirectionsSetup);
        // DirectionsRequests.CreateGetRequest(DirectionsSetup."API Base URL" + '???', HttpRequest);  // TODO: What endpoint removes from queue?
        // DirectionsAuth.AddAuthHeader(HttpRequest, DirectionsSetup);
        // ReceiveContext.Http().SetHttpRequestMessage(HttpRequest);
        // if not HttpClient.Send(HttpRequest, HttpResponse) then
        //     Error('Failed to download document from API.');
        // ReceiveContext.Http().SetHttpResponseMessage(HttpResponse);
        // DirectionsRequests.CheckResponseSuccess(HttpResponse);
        // ResponseText := DirectionsRequests.GetResponseText(HttpResponse);
        // if JsonObject.ReadFrom(ResponseText) then begin
        //     if JsonObject.Get('document', JsonToken) then begin
        //         JsonToken.WriteTo(DocumentJson);
        //         ReceiveContext.GetTempBlob(TempBlob);
        //         DirectionsRequests.WriteTextToBlob(DocumentJson, TempBlob);
        //     end else
        //         Error('No document found in response.');
        // end;
    end;

    // ============================================================================
    // Event Subscriber - Opens setup page when configuring the service
    // ============================================================================
    [EventSubscriber(ObjectType::Page, Page::"E-Document Service", OnBeforeOpenServiceIntegrationSetupPage, '', false, false)]
    local procedure OnBeforeOpenServiceIntegrationSetupPage(EDocumentService: Record "E-Document Service"; var IsServiceIntegrationSetupRun: Boolean)
    var
        DirectionsSetup: Page "Directions Connection Setup";
    begin
        if EDocumentService."Service Integration V2" <> EDocumentService."Service Integration V2"::"Directions Connector" then
            exit;

        DirectionsSetup.RunModal();
        IsServiceIntegrationSetupRun := true;
    end;
}
