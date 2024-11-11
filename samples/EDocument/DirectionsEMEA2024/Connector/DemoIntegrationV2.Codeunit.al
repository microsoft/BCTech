namespace DefaultPublisher.EDocDemo;

using Microsoft.eServices.EDocument;
using Microsoft.eServices.EDocument.Integration.Interfaces;
using System.Utilities;

codeunit 50105 "Demo Integration V2" implements Sender, Receiver, "Default Int. Actions"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure Send(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var TempBlob: codeunit System.Utilities."Temp Blob"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage; var IsAsync: Boolean)
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequest, 'send', 'POST');
        IntegrationHelper.WriteBlobToRequestMessage(HttpRequest, TempBlob);
        HttpClient.Send(HttpRequest, HttpResponse);

        IsAsync := true;
    end;

    procedure SendBatch(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var TempBlob: codeunit System.Utilities."Temp Blob"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage; var IsAsync: Boolean)
    begin

    end;

    procedure GetResponse(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
        JsonObject: JsonObject;
        Token: JsonToken;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequest, 'getresponse?invoice_id=' + EDocument."Document No.", 'GET');
        HttpClient.Send(HttpRequest, HttpResponse);

        JsonObject := IntegrationHelper.ReadJsonFrom(HttpResponse);
        JsonObject.Get('message', Token);
        exit(Token.AsValue().AsText() = 'Success');
    end;

    procedure ReceiveDocuments(var EDocumentService: Record "E-Document Service"; var TempBlob: codeunit "Temp Blob"; var HttpRequestMessage: HttpRequestMessage; var HttpResponseMessage: HttpResponseMessage; var Count: Integer)
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
        OutStream: OutStream;
        Content: Text;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequestMessage, 'receive', 'get');
        HttpClient.Send(HttpRequestMessage, HttpResponseMessage);
        HttpResponseMessage.Content.ReadAs(Content);
        TempBlob.CreateOutStream(OutStream, TextEncoding::UTF8);
        OutStream.WriteText(Content);

        Count := 1;
    end;

    procedure DownloadDocument(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var DocumentsBlob: codeunit System.Utilities."Temp Blob"; var DocumentBlob: codeunit "Temp Blob"; var HttpRequestMessage: HttpRequestMessage; var HttpResponseMessage: HttpResponseMessage)
    begin
        DocumentBlob := DocumentsBlob;
    end;

    procedure GetSentDocumentApprovalStatus(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage; var Status: Enum "E-Document Service Status"): Boolean
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequest, 'approve', 'GET');
        Status := Enum::"E-Document Service Status"::Approved;
        exit(HttpClient.Send(HttpRequest, HttpResponse));
    end;

    procedure GetSentDocumentCancellationStatus(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage; var Status: Enum "E-Document Service Status"): Boolean
    begin

    end;

    procedure OpenServiceIntegrationSetupPage(var EDocumentService: Record "E-Document Service"): Boolean
    begin
        Page.Run(Page::"Demo Setup");
        exit(true);
    end;

    procedure Send(var EDocument: Record "E-Document"; var TempBlob: codeunit System.Utilities."Temp Blob"; var IsAsync: Boolean; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage)
    begin
        Error('Not Implemented');
    end;

    procedure GetResponse(var EDocument: Record "E-Document"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
    begin
        Error('Not Implemented');
    end;


    procedure SendBatch(var EDocuments: Record "E-Document"; var TempBlob: codeunit "Temp Blob"; var IsAsync: Boolean; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage)
    begin
        Error('Not Implemented');
    end;

    procedure GetApproval(var EDocument: Record "E-Document"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
    begin
        Error('Not Implemented');
    end;

    procedure Cancel(var EDocument: Record "E-Document"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
    begin
        Error('Not Implemented');
    end;

    procedure ReceiveDocument(var TempBlob: codeunit "Temp Blob"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage)
    begin
        Error('Not Implemented');
    end;

    procedure GetDocumentCountInBatch(var TempBlob: codeunit "Temp Blob"): Integer
    begin
        Error('Not Implemented');
    end;

    procedure GetIntegrationSetup(var SetupPage: Integer; var SetupTable: Integer)
    begin
        SetupPage := page::"Demo Setup";
    end;


}