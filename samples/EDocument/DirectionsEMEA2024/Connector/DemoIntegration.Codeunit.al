namespace DefaultPublisher.EDocDemo;

using Microsoft.eServices.EDocument;
using System.Utilities;

codeunit 50101 "Demo Integration" implements "E-Document Integration"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure Send(var EDocument: Record "E-Document"; var TempBlob: codeunit System.Utilities."Temp Blob"; var IsAsync: Boolean; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage)
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequest, 'send', 'POST');
        IntegrationHelper.WriteBlobToRequestMessage(HttpRequest, TempBlob);
        HttpClient.Send(HttpRequest, HttpResponse);

        IsAsync := true;
    end;

    procedure GetResponse(var EDocument: Record "E-Document"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
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


    procedure SendBatch(var EDocuments: Record "E-Document"; var TempBlob: codeunit "Temp Blob"; var IsAsync: Boolean; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage)
    begin

    end;

    procedure GetApproval(var EDocument: Record "E-Document"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequest, 'approve', 'GET');
        HttpClient.Send(HttpRequest, HttpResponse);
        exit(true);
    end;

    procedure Cancel(var EDocument: Record "E-Document"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage): Boolean
    begin

    end;

    procedure ReceiveDocument(var TempBlob: codeunit "Temp Blob"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage)
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpClient: HttpClient;
        OutStream: OutStream;
        Content: Text;
    begin
        IntegrationHelper.PrepareRequestMessage(HttpRequest, 'receive', 'get');
        HttpClient.Send(HttpRequest, HttpResponse);
        HttpResponse.Content.ReadAs(Content);
        TempBlob.CreateOutStream(OutStream, TextEncoding::UTF8);
        OutStream.WriteText(Content);
    end;

    procedure GetDocumentCountInBatch(var TempBlob: codeunit "Temp Blob"): Integer
    begin
        // How many documents have we received?
        exit(1);
    end;

    procedure GetIntegrationSetup(var SetupPage: Integer; var SetupTable: Integer)
    begin
        SetupPage := page::"Demo Setup";
    end;

}