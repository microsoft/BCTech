// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

using System.Utilities;

/// <summary>
/// Helper codeunit for authentication and registration with the Connector API.
/// Pre-written to save time during the workshop.
/// </summary>
codeunit 50121 "Connector Auth"
{
    Access = Internal;

    /// <summary>
    /// Registers a new user with the Connector API and stores the API key.
    /// </summary>
    procedure RegisterUser(var ConnectorSetup: Record "Connector Connection Setup")
    var
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        HttpContent: HttpContent;
        HttpHeaders: HttpHeaders;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        ResponseText: Text;
        RequestBody: Text;
    begin
        if ConnectorSetup."API Base URL" = '' then
            Error('Please specify the API Base URL before registering.');

        if ConnectorSetup."User Name" = '' then
            Error('Please specify a User Name before registering.');

        // Create request body
        JsonObject.Add('name', ConnectorSetup."User Name");
        JsonObject.WriteTo(RequestBody);

        // Prepare HTTP request
        HttpRequest.Content.WriteFrom(RequestBody);
        HttpRequest.Content.GetHeaders(HttpHeaders);
        if HttpHeaders.Contains('Content-Type') then
            HttpHeaders.Remove('Content-Type');
        HttpHeaders.Add('Content-Type', 'application/json');

        HttpRequest.Method := 'POST';
        HttpRequest.SetRequestUri(ConnectorSetup."API Base URL" + 'register');

        // Send request
        if not HttpClient.Send(HttpRequest, HttpResponse) then
            Error('Failed to connect to the API server.');

        // Parse response
        HttpResponse.Content.ReadAs(ResponseText);

        if not HttpResponse.IsSuccessStatusCode() then
            Error('Registration failed: %1', ResponseText);

        // Extract API key from response
        if JsonObject.ReadFrom(ResponseText) then begin
            if JsonObject.Get('key', JsonToken) then begin
                ConnectorSetup.SetAPIKey(JsonToken.AsValue().AsText());
                ConnectorSetup.Registered := true;
                ConnectorSetup.Modify();
            end else
                Error('API key not found in response.');
        end else
            Error('Invalid response format from API.');
    end;

    /// <summary>
    /// Tests the connection to the API by calling the /peek endpoint.
    /// </summary>
    procedure TestConnection(ConnectorSetup: Record "Connector Connection Setup")
    var
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
    begin
        if ConnectorSetup."API Base URL" = '' then
            Error('Please specify the API Base URL.');

        if not ConnectorSetup.Registered then
            Error('Please register first to get an API key.');

        // Prepare HTTP request
        HttpRequest.Method := 'GET';
        HttpRequest.SetRequestUri(ConnectorSetup."API Base URL" + 'peek');
        AddAuthHeader(HttpRequest, ConnectorSetup);

        // Send request
        if not HttpClient.Send(HttpRequest, HttpResponse) then
            Error('Failed to connect to the API server.');

        if not HttpResponse.IsSuccessStatusCode() then
            Error('Connection test failed with status: %1', HttpResponse.HttpStatusCode());
    end;

    /// <summary>
    /// Adds the authentication header to an HTTP request.
    /// </summary>
    procedure AddAuthHeader(var HttpRequest: HttpRequestMessage; ConnectorSetup: Record "Connector Connection Setup")
    var
        HttpHeaders: HttpHeaders;
    begin
        // Prepare HTTP request
        HttpRequest.GetHeaders(HttpHeaders);
        HttpHeaders.Add('X-Service-Key', ConnectorSetup.GetAPIKeyText());
    end;

    /// <summary>
    /// Gets the connection setup record, ensuring it exists.
    /// </summary>
    procedure GetConnectionSetup(var ConnectorSetup: Record "Connector Connection Setup")
    begin
        if not ConnectorSetup.Get() then
            Error('Connector is not configured. Please open the Connector Connection Setup page.');

        if ConnectorSetup."API Base URL" = '' then
            Error('API Base URL is not configured.');

        if not ConnectorSetup.Registered then
            Error('Not registered with the API. Please register first.');
    end;
}
