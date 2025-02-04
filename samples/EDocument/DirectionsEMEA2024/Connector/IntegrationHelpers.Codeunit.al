namespace DefaultPublisher.EDocDemo;

using System.Xml;
using System.Utilities;

codeunit 50142 "Integration Helpers"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure TempBlobToTxt(var TempBlob: Codeunit "Temp Blob"): Text
    var
        XMLDOMManagement: Codeunit "XML DOM Management";
        InStr: InStream;
        Content: Text;
    begin
        TempBlob.CreateInStream(InStr, TextEncoding::UTF8);
        XMLDOMManagement.TryGetXMLAsText(InStr, Content);
        exit(Content);
    end;

    procedure ReadJsonFrom(var HttpContentResponse: HttpResponseMessage): JsonObject
    var
        Result: Text;
        ResponseJObject: JsonObject;
    begin
        HttpContentResponse.Content.ReadAs(Result);
        ResponseJObject.ReadFrom(Result);
        exit(ResponseJObject);
    end;

    procedure PrepareRequestMessage(var HttpRequestMessage: HttpRequestMessage; ApiPath: Text; Method: Text)
    var
        Setup: Record "Demo Setup";
        HttpHeaders: HttpHeaders;
    begin
        Setup.FindFirst();
        HttpRequestMessage.GetHeaders(HttpHeaders);
        HttpHeaders.Add('Authorization', 'Bearer ' + Setup."API Key");
        HttpHeaders.Add('Service', Setup."Service Name");
        HttpHeaders.Add('Accept', '*/*');
        HttpRequestMessage.Method(Method);
        HttpRequestMessage.SetRequestUri(Setup."API Url" + ApiPath);
    end;

    procedure WriteBlobToRequestMessage(var HttpRequestMessage: HttpRequestMessage; var TempBlob: codeunit System.Utilities."Temp Blob")
    var
        IntegrationHelper: Codeunit "Integration Helpers";
        HttpContent: HttpContent;
        HttpHeaders: HttpHeaders;
        Payload: Text;
    begin
        Payload := IntegrationHelper.TempBlobToTxt(TempBlob);
        HttpContent.WriteFrom(Payload);
        HttpContent.GetHeaders(HttpHeaders);
        if HttpHeaders.Contains('Content-Type') then
            HttpHeaders.Remove('Content-Type');
        HttpHeaders.Add('Content-Type', 'application/xml');
        HttpRequestMessage.Content := HttpContent;
    end;
}