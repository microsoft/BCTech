// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

using System.Utilities;

/// <summary>
/// Helper codeunit for building HTTP requests for the Directions API.
/// Pre-written to save time during the workshop.
/// </summary>
codeunit 81102 "Directions Requests"
{
    Access = Internal;

    /// <summary>
    /// Creates an HTTP POST request with JSON content.
    /// </summary>
    procedure CreatePostRequest(Url: Text; JsonContent: Text; var HttpRequest: HttpRequestMessage)
    var
        HttpContent: HttpContent;
    begin
        HttpContent.WriteFrom(JsonContent);
        HttpContent.GetHeaders().Remove('Content-Type');
        HttpContent.GetHeaders().Add('Content-Type', 'application/json');

        HttpRequest.Method := 'POST';
        HttpRequest.SetRequestUri(Url);
        HttpRequest.Content := HttpContent;
    end;

    /// <summary>
    /// Creates an HTTP GET request.
    /// </summary>
    procedure CreateGetRequest(Url: Text; var HttpRequest: HttpRequestMessage)
    begin
        HttpRequest.Method := 'GET';
        HttpRequest.SetRequestUri(Url);
    end;

    /// <summary>
    /// Reads the response content as text.
    /// </summary>
    procedure GetResponseText(HttpResponse: HttpResponseMessage): Text
    var
        ResponseText: Text;
    begin
        HttpResponse.Content.ReadAs(ResponseText);
        exit(ResponseText);
    end;

    /// <summary>
    /// Checks if the response is successful and throws an error if not.
    /// </summary>
    procedure CheckResponseSuccess(HttpResponse: HttpResponseMessage)
    var
        ResponseText: Text;
    begin
        if not HttpResponse.IsSuccessStatusCode() then begin
            ResponseText := GetResponseText(HttpResponse);
            Error('API request failed with status %1: %2', HttpResponse.HttpStatusCode(), ResponseText);
        end;
    end;

    /// <summary>
    /// Reads JSON content from a TempBlob.
    /// </summary>
    procedure ReadJsonFromBlob(var TempBlob: Codeunit "Temp Blob"): Text
    var
        InStr: InStream;
        JsonText: Text;
    begin
        TempBlob.CreateInStream(InStr, TextEncoding::UTF8);
        InStr.ReadText(JsonText);
        exit(JsonText);
    end;

    /// <summary>
    /// Writes text content to a TempBlob.
    /// </summary>
    procedure WriteTextToBlob(TextContent: Text; var TempBlob: Codeunit "Temp Blob")
    var
        OutStr: OutStream;
    begin
        TempBlob.CreateOutStream(OutStr, TextEncoding::UTF8);
        OutStr.WriteText(TextContent);
    end;
}
