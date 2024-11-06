namespace DefaultPublisher.EDocDemo;

using Microsoft.eServices.EDocument;
using Microsoft.eServices.EDocument.Integration.Interfaces;
using System.Utilities;

codeunit 50101 "Demo Integration" implements "E-Document Integration"
{
    InherentEntitlements = X;
    InherentPermissions = X;

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
        Error('Not Implemented');
    end;

}