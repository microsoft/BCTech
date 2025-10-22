// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

using System.Utilities;

/// <summary>
/// Helper codeunit for authentication and registration with the Directions API.
/// Pre-written to save time during the workshop.
/// </summary>
codeunit 81101 "Directions Auth"
{
    Access = Internal;

    /// <summary>
    /// Registers a new user with the Directions API and stores the API key.
    /// </summary>
    procedure RegisterUser(var DirectionsSetup: Record "Directions Connection Setup")
    var
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
        HttpContent: HttpContent;
        JsonObject: JsonObject;
        JsonToken: JsonToken;
        ResponseText: Text;
        RequestBody: Text;
    begin
        if DirectionsSetup."API Base URL" = '' then
            Error('Please specify the API Base URL before registering.');

        if DirectionsSetup."User Name" = '' then
            Error('Please specify a User Name before registering.');

        // Create request body
        JsonObject.Add('name', DirectionsSetup."User Name");
        JsonObject.WriteTo(RequestBody);

        // Prepare HTTP request
        HttpContent.WriteFrom(RequestBody);
        HttpContent.GetHeaders().Remove('Content-Type');
        HttpContent.GetHeaders().Add('Content-Type', 'application/json');

        HttpRequest.Method := 'POST';
        HttpRequest.SetRequestUri(DirectionsSetup."API Base URL" + 'register');
        HttpRequest.Content := HttpContent;

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
                DirectionsSetup.SetAPIKey(JsonToken.AsValue().AsText());
                DirectionsSetup.Registered := true;
                DirectionsSetup.Modify();
            end else
                Error('API key not found in response.');
        end else
            Error('Invalid response format from API.');
    end;

    /// <summary>
    /// Tests the connection to the API by calling the /peek endpoint.
    /// </summary>
    procedure TestConnection(DirectionsSetup: Record "Directions Connection Setup")
    var
        HttpClient: HttpClient;
        HttpRequest: HttpRequestMessage;
        HttpResponse: HttpResponseMessage;
    begin
        if DirectionsSetup."API Base URL" = '' then
            Error('Please specify the API Base URL.');

        if not DirectionsSetup.Registered then
            Error('Please register first to get an API key.');

        // Prepare HTTP request
        HttpRequest.Method := 'GET';
        HttpRequest.SetRequestUri(DirectionsSetup."API Base URL" + 'peek');
        AddAuthHeader(HttpRequest, DirectionsSetup);

        // Send request
        if not HttpClient.Send(HttpRequest, HttpResponse) then
            Error('Failed to connect to the API server.');

        if not HttpResponse.IsSuccessStatusCode() then
            Error('Connection test failed with status: %1', HttpResponse.HttpStatusCode());
    end;

    /// <summary>
    /// Adds the authentication header to an HTTP request.
    /// </summary>
    procedure AddAuthHeader(var HttpRequest: HttpRequestMessage; DirectionsSetup: Record "Directions Connection Setup")
    begin
        HttpRequest.GetHeaders().Add('X-Service-Key', DirectionsSetup.GetAPIKeyText());
    end;

    /// <summary>
    /// Gets the connection setup record, ensuring it exists.
    /// </summary>
    procedure GetConnectionSetup(var DirectionsSetup: Record "Directions Connection Setup")
    begin
        if not DirectionsSetup.Get() then
            Error('Directions Connector is not configured. Please open the Directions Connection Setup page.');

        if DirectionsSetup."API Base URL" = '' then
            Error('API Base URL is not configured.');

        if not DirectionsSetup.Registered then
            Error('Not registered with the API. Please register first.');
    end;
}
