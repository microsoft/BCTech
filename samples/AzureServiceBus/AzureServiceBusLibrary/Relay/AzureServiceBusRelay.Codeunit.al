// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50139 AzureServiceBusRelay
{
    //
    // ServiceBus Relay REST functionality
    //

    var
        SharedAccessTokenGenerator: codeunit SharedAccessTokenGenerator;
        BaseRelayUri: Text;
        SharedKeyName: Text;
        [NonDebuggable]
        SharedKey: Text;
        LastReasonPhrase: Text;
        RelayNotEnabledErr: label 'AzureServiceBusRelay is not set-up. Please go to Service Connections to set-up';
        RequestErr: Label 'HttpError Code:%1, Reason:%2', Locked = true;
        IsInitialized: Boolean;

    //
    // Initialize the ServiceBus Relay
    // 
    procedure Initialize(resourceUri: Text; keyName: Text; keyValue: Text; ttl: Integer);
    begin
        BaseRelayUri := resourceUri;
        SharedKeyName := KeyName;
        SharedKey := keyValue;

        IsInitialized := true;
    end;

    //
    // Initialize the ServiceBus Relay from the persisted setup.
    // 
    procedure Initialize();
    begin
        Initialize('');
    end;

    //
    // Initialize the ServiceBus Relay from the persisted setup.
    // 
    procedure Initialize(SubBaseUri: Text);
    var
        AzureServiceBusRelaySetup: Record AzureServiceBusRelaySetup;
    begin
        if not AzureServiceBusRelaySetup.Get() then
            Error(RelayNotEnabledErr);

        if not AzureServiceBusRelaySetup.IsEnabled then
            Error(RelayNotEnabledErr);

        BaseRelayUri := StrSubstNo('https://%1.servicebus.windows.net/%2%3',
            AzureServiceBusRelaySetup.AzureRelayNamespace,
            AzureServiceBusRelaySetup.HybridConnectionName,
            SubBaseUri);
        SharedKeyName := AzureServiceBusRelaySetup.KeyName;
        SharedKey := AzureServiceBusRelaySetup.GetSharedAccessKey();

        IsInitialized := true;
    end;

    procedure CheckInitialized();
    begin
        if not IsInitialized then
            Initialize();
    end;

    [TryFunction]
    procedure Get(SubRelayUri: Text; var content: Text);
    var
        response: HttpResponseMessage;
    begin
        CheckInitialized();

        Get(SubRelayUri, response);
        response.Content().ReadAs(content);
    end;

    [TryFunction]
    procedure Get(SubRelayUri: Text; var content: InStream);
    var
        response: HttpResponseMessage;
    begin
        CheckInitialized();

        Get(SubRelayUri, response);
        response.Content().ReadAs(content);
    end;

    local procedure Get(SubRelayUri: Text; var response: HttpResponseMessage)
    var
        client: HttpClient;
        request: HttpRequestMessage;
    begin
        InitializeRequest('GET', BaseRelayUri + SubRelayUri, request);
        client.Send(request, response);
        if not CheckResponse(response) then
            Error(RequestErr, response.HttpStatusCode(), response.ReasonPhrase());
    end;

    procedure Put(SubRelayUri: Text; input: Text; var result: Text);
    var
        request: HttpRequestMessage;
        response: HttpResponseMessage;
    begin
        CheckInitialized();

        request.Content().WriteFrom(input);
        Put(SubRelayUri, request, response);
        response.Content().ReadAs(result);
    end;

    procedure Put(SubRelayUri: Text; Input: InStream; Length: Integer; ContentType: Text; var result: Text);
    var
        request: HttpRequestMessage;
        response: HttpResponseMessage;
        headers: HttpHeaders;
    begin
        CheckInitialized();

        request.Content().WriteFrom(Input);
        request.Content().GetHeaders(headers);
        headers.Add('Content-Length', Format(Length));
        headers.Add('Content-Type', ContentType);

        Put(SubRelayUri, request, response);
        response.Content().ReadAs(result);
    end;

    local procedure Put(SubRelayUri: Text; request: HttpRequestMessage; var response: HttpResponseMessage)
    var
        Client: HttpClient;
    begin
        InitializeRequest('PUT', BaseRelayUri + SubRelayUri, request);
        Client.Send(request, response);
        CheckResponse(response);
    end;

    local procedure InitializeRequest(Verb: Text; Uri: Text; var Request: HttpRequestMessage)
    var
        Headers: HttpHeaders;
    begin
        Request.GetHeaders(Headers);
        Headers.Add('ServiceBusAuthorization', SharedAccessTokenGenerator.GetSasToken(Uri, SharedKeyName, SharedKey));
        Request.Method := Verb;
        Request.SetRequestUri(Uri);
    end;

    local procedure CheckResponse(var response: HttpResponseMessage): Boolean
    begin
        if not response.IsSuccessStatusCode() then begin
            LastReasonPhrase := response.ReasonPhrase();
            exit(false);
        end;

        LastReasonPhrase := '';
        exit(true);
    end;

}