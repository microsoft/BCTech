// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace BCTech.EServices.EDocumentConnector;

using Microsoft.eServices.EDocument;
using Microsoft.Sales.History;
using Microsoft.eServices.EDocument.Integration.Send;
using Microsoft.Sales.Customer;
using Microsoft.Foundation.Company;
using System.Utilities;
using System.Text;

codeunit 50103 "Clearance Service Processing"
{
    Access = Internal;
    Permissions = tabledata "Sales Invoice Header" = rimd,
    tabledata ClearModelExtConnectionSetup = rimd;
    local procedure InitRequest(var ExternalConnectionSetup: Record ClearModelExtConnectionSetup;

    var
        HttpRequestMessage: HttpRequestMessage;

    var
        HttpResponseMessage: HttpResponseMessage)
    begin
        Clear(HttpRequestMessage);
        Clear(HttpResponseMessage);
        if not ExternalConnectionSetup.Get() then
            Error(MissingSetupErr);
        ExternalConnectionSetup.TestField("Company Id");
    end;

    internal procedure SendEDocument(var TempBlob: Codeunit "Temp Blob"; var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; HttpRequest: HttpRequestMessage; HttpResponse: HttpResponseMessage): Boolean
    var
        ExternalConnectionSetup: Record ClearModelExtConnectionSetup;
        SendContext: Codeunit SendContext;
        HttpClient: HttpClient;
        DocumentContent: Text;
        RequestContent: Text;
        InStream: InStream;
        HttpHeaders: HttpHeaders;
    begin

        InitRequest(ExternalConnectionSetup, HttpRequest, HttpResponse);

        // Get document content
        TempBlob := SendContext.GetTempBlob();
        TempBlob.CreateInStream(InStream);
        InStream.ReadText(DocumentContent);

        // Build clearance request
        RequestContent := BuildClearanceRequest(EDocument, EDocumentService, DocumentContent);

        // Set up HTTP request
        HttpRequest := SendContext.Http().GetHttpRequestMessage();
        HttpRequest.Method := 'POST';
        HttpRequest.SetRequestUri(ExternalConnectionSetup."FileAPI URL" + '/validate_invoice');
        HttpRequest.Content.WriteFrom(RequestContent);

        // Set content type header
        HttpRequest.Content.GetHeaders(HttpHeaders);
        HttpHeaders.Remove('Content-Type');
        HttpHeaders.Add('Content-Type', 'application/json');

        // Send request
        HttpClient.Send(HttpRequest, HttpResponse);

        SendContext.Http().SetHttpResponseMessage(HttpResponse);

        // Process response
        if HttpResponse.IsSuccessStatusCode() then begin
            EDocument."Last Clearance Request Time" := CurrentDateTime();
            EDocument.Modify();
            exit(true);
        end else
            exit(false);
    end;

    internal procedure BuildClearanceRequest(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; DocumentContent: Text): Text
    var
        CompanyInfo: Record "Company Information";
        Customer: Record Customer;
        JsonRequest: JsonObject;
    begin
        CompanyInfo.Get();
        Customer.Get(EDocument."Bill-to/Pay-to No.");
        // Build clearance request JSON
        JsonRequest.Add('documentType', Format(EDocument."Document Type"));
        JsonRequest.Add('invoice_number', EDocument."Document No.");
        JsonRequest.Add('date', Format(EDocument."Document Date", 0, '<Year4>-<Month,2>-<Day,2>'));
        JsonRequest.Add('issuerTaxId', CompanyInfo."VAT Registration No.");
        JsonRequest.Add('issuerName', CompanyInfo.Name);
        JsonRequest.Add('customerTaxId', Customer."VAT Registration No.");
        JsonRequest.Add('customerName', Customer.Name);
        JsonRequest.Add('totalAmount', EDocument."Amount Incl. VAT");
        JsonRequest.Add('currency', EDocument."Currency Code");
        JsonRequest.Add('timestamp', Format(CurrentDateTime, 0, '<Year4>-<Month,2>-<Day,2>T<Hours24,2>:<Minutes,2>:<Seconds,2>'));

        exit(Format(JsonRequest));
    end;

    internal procedure GetDocumentResponse(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; var HttpRequest: HttpRequestMessage; var HttpResponse: HttpResponseMessage; var SendContext: Codeunit SendContext): Boolean
    var
        ExternalConnectionSetup: Record ClearModelExtConnectionSetup;
        TempBlob: Codeunit "Temp Blob";
        ResponseContent: Text;
        HttpClient: HttpClient;
        DocumentContent: Text;
        RequestContent: Text;
        InStream: InStream;
        HttpHeaders: HttpHeaders;

    begin
        InitRequest(ExternalConnectionSetup, HttpRequest, HttpResponse);

        // Get document content
        TempBlob := SendContext.GetTempBlob();
        TempBlob.CreateInStream(InStream);
        InStream.ReadText(DocumentContent);

        // Build clearance request
        RequestContent := BuildClearanceRequest(EDocument, EDocumentService, DocumentContent);

        // Set up HTTP request
        HttpRequest := SendContext.Http().GetHttpRequestMessage();
        HttpRequest.Method := 'POST';
        HttpRequest.SetRequestUri(ExternalConnectionSetup."FileAPI URL" + '/validate_invoice');
        HttpRequest.Content.WriteFrom(RequestContent);

        // Set content type header
        HttpRequest.Content.GetHeaders(HttpHeaders);
        HttpHeaders.Remove('Content-Type');
        HttpHeaders.Add('Content-Type', 'application/json');

        // Send request

        HttpClient.Send(HttpRequest, HttpResponse);
        SendContext.Http().SetHttpResponseMessage(HttpResponse);

        // Process response
        if HttpResponse.IsSuccessStatusCode() then begin
            EDocument."Last Clearance Request Time" := CurrentDateTime();
            EDocument.Modify();

            HttpResponse.Content().ReadAs(ResponseContent);
            ProcessClearanceSubmissionResponse(EDocument, EDocumentService, ResponseContent, SendContext);
        end else
            Error(FailToSubmitErr, HttpResponse.HttpStatusCode());

        exit(true);
    end;

    internal procedure ProcessClearanceSubmissionResponse(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; ResponseContent: Text; var SendContext: Codeunit SendContext)
    var
        EDocumentServiceStatus: Record "E-Document Service Status";
        SalesInvoiceHeader: Record "Sales Invoice Header";
        Base64Convert: Codeunit "Base64 Convert";
        TempBlob: Codeunit "Temp Blob";
        ClearanceStatus: Enum "E-Document Service Status";
        JsonResponse: JsonObject;
        JsonToken: JsonToken;
        SubmissionId: Text;
        Status: Text;
        QRBase64Txt: Text;
        BlobOutStr: OutStream;
        OutStream: OutStream;
        InStream: InStream;
    begin
        EDocumentServiceStatus.Get(EDocument."Entry No", EDocumentService."Code");

        if not JsonResponse.ReadFrom(ResponseContent) then
            exit;

        // Extract submission ID
        if JsonResponse.Get('certified_id', JsonToken) then
            SubmissionId := JsonToken.AsValue().AsText();

        // Check immediate status
        if JsonResponse.Get('status', JsonToken) then begin
            Status := JsonToken.AsValue().AsText();
            case Status of
                'ACCEPTED':
                    ClearanceStatus := Enum::"E-Document Service Status"::Cleared;
                'REJECTED':
                    ClearanceStatus := Enum::"E-Document Service Status"::Rejected;
                else
                    ClearanceStatus := Enum::"E-Document Service Status"::"Sending Error";
            end;
        end;

        SendContext.Status().SetStatus(ClearanceStatus);

        EDocument."Clearance Date" := CurrentDateTime();
        EDocument.Modify();

        if not SalesInvoiceHeader.Get(EDocument."Document No.") then
            Error(SalesInvNotFoundErr);

        // Extract QR code if it exists
        if JsonResponse.Get('qr_code', JsonToken) then begin
            QRBase64Txt := JsonToken.AsValue().AsText();

            Clear(SalesInvoiceHeader."QR Code Base64");
            SalesInvoiceHeader."QR Code Base64".CreateOutStream(BlobOutStr, TextEncoding::UTF8);
            BlobOutStr.WriteText(QRBase64Txt);

            TempBlob.CreateOutStream(OutStream);
            Base64Convert.FromBase64(QRBase64Txt, OutStream);

            TempBlob.CreateInStream(InStream);
            SalesInvoiceHeader."QR Code Image".ImportStream(InStream, 'image/png');
            SalesInvoiceHeader.Modify();
        end;
    end;

    var
        FailToSubmitErr: Label 'Failed to submit document for clearance. Status: %1', Comment = '%1=Status of Sales Invoice';
        MissingSetupErr: Label 'Missing setup for clearance process.';
        SalesInvNotFoundErr: Label 'Sales Invoice Header not found.';
}