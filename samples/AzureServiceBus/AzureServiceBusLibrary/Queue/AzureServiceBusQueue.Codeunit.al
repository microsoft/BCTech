// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50141 AzureServiceBusQueue
{
    //
    // ServiceBus Queue REST functionality
    //

    var
        SharedAccessTokenGenerator: codeunit SharedAccessTokenGenerator;
        LastReasonPhrase: Text;
        Resource: Text;
        SharedKeyName: Text;
        [NonDebuggable]
        SharedKey: Text;
        IsInitialized: Boolean;
        QueueNotEnabledErr: label 'AzureServiceBusQueue is not configured. Please go to Service Connections to configure';

    //
    // Initialize the ServiceBus Queue
    // 
    procedure Initialize(ServiceBusNamespace: Text; QueueName: Text; KeyName: Text; KeyValue: Text);
    begin
        Resource := StrSubstNo('https://%1.servicebus.windows.net/%2',
            ServiceBusNamespace,
            QueueName);

        SharedKeyName := KeyName;
        SharedKey := KeyValue;
        IsInitialized := true;
    end;

    procedure Initialize()
    var
        AzureServiceBusQueueSetup: Record AzureServiceBusQueueSetup;
    begin
        if not AzureServiceBusQueueSetup.Get() then
            Error(QueueNotEnabledErr);

        if not AzureServiceBusQueueSetup.IsEnabled then
            Error(QueueNotEnabledErr);

        Resource := StrSubstNo('https://%1.servicebus.windows.net/%2',
            AzureServiceBusQueueSetup.ServiceBusNamespace,
            AzureServiceBusQueueSetup.QueueName);

        SharedKeyName := AzureServiceBusQueueSetup.KeyName;
        SharedKey := AzureServiceBusQueueSetup.GetSharedAccessKey();

        IsInitialized := true;
    end;

    // 
    // Gets a value indicating whether the Azure ServiceBus Queue is configured and enabled in Service Connections.
    // 
    procedure IsEnabled(): Boolean;
    var
        AzureServiceBusQueueSetup: record AzureServiceBusQueueSetup;
    begin
        if AzureServiceBusQueueSetup.Get() then
            if AzureServiceBusQueueSetup.IsEnabled then
                exit(true);

        exit(false);
    end;

    // 
    // Gets the last status message returned from the REST calls
    // 
    procedure GetLastReasonPhrase(): Text;
    begin
        exit(LastReasonPhrase);
    end;

    //
    // Receive and Delete Message (Destructive Read)
    // This operation receives a message from a queue or subscription, 
    // and removes the message from that queue or subscription in one atomic operation.
    //
    // Documentation: https://learn.microsoft.com/en-us/rest/api/servicebus/receive-and-delete-message-destructive-read
    // 
    procedure ReceiveAndDeleteMessage(var Message: Text): Boolean;
    var
        Client: HttpClient;
        Request: HttpRequestMessage;
        Response: HttpResponseMessage;
    begin
        CheckInitialized();

        InitializeRequest('DELETE', resource + '/messages/head', Request);
        Client.Send(Request, Response);

        if Response.HttpStatusCode() <> 200 then begin
            LastReasonPhrase := Response.ReasonPhrase();
            exit(false);
        end;

        Response.Content().ReadAs(Message);
        LastReasonPhrase := '';
        exit(true);
    end;

    //
    // Peek-Lock Message (Non-Destructive Read)
    // This operation atomically retrieves and locks a message from a queue or subscription for 
    // processing. The message is guaranteed not to be delivered to other receivers (on the same 
    // queue or subscription only) during the lock duration specified in the queue/subscription 
    // description. When the lock expires, the message becomes available to other receivers. 
    // In order to complete processing of the message, the receiver should issue a delete command 
    // with the lock ID received from this operation. To abandon processing of the message and 
    // unlock it for other receivers, an Unlock Message command should be issued, otherwise the 
    // lock duration period can expire.
    //
    // Documentation: https://learn.microsoft.com/en-us/rest/api/servicebus/peek-lock-message-non-destructive-read
    // 
    procedure PeekLockMessage(var Message: Text; var LockToken: Text): Boolean;
    var
        Client: HttpClient;
        Request: HttpRequestMessage;
        Response: HttpResponseMessage;
    begin
        CheckInitialized();

        InitializeRequest('POST', resource + '/messages/head', Request);
        Client.Send(Request, Response);

        if Response.HttpStatusCode() <> 201 then begin
            LastReasonPhrase := Response.ReasonPhrase();
            exit(false);
        end;

        Response.Content().ReadAs(Message);
        LockToken := GetHeader(Response.Headers(), 'Location');
        LastReasonPhrase := '';
        exit(true);
    end;

    //
    // Send Message 
    // Send a message to a Service Bus queue or topic.
    // 
    // Documentation: https://learn.microsoft.com/en-us/rest/api/servicebus/send-message-batch
    // 
    procedure SendMessage(message: Text): Boolean;
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
        Content: HttpContent;
    begin
        CheckInitialized();

        InitializeRequest('POST', resource + '/messages', request);

        Content.WriteFrom(message);
        request.Content := Content;
        client.Send(request, response);

        if response.HttpStatusCode() <> 201 then begin
            LastReasonPhrase := response.ReasonPhrase();
            exit(false);
        end;

        LastReasonPhrase := '';
        exit(true);
    end;

    // 
    // Delete Message
    // This operation completes the processing of a locked message and deletes it from the 
    // queue or subscription. This operation should only be called after successfully processing 
    // a previously locked message, in order to maintain At-Least-Once delivery assurances.
    //
    // Documentation: https://learn.microsoft.com/en-us/rest/api/servicebus/delete-message
    // 
    procedure DeleteMessage(LockToken: Text): Boolean;
    begin
        exit(UnlockOrDeleteMessage('DELETE', LockToken));
    end;

    // 
    // Unlock Message
    // Unlocks a message for processing by other receivers on a specified subscription. This 
    // operation deletes the lock object, causing the message to be unlocked. Before the operation 
    // is called, a receiver must first lock the message.
    // 
    // Documentation: https://learn.microsoft.com/en-us/rest/api/servicebus/unlock-message
    // 
    procedure UnlockMessage(LockToken: Text): Boolean;
    begin
        exit(UnlockOrDeleteMessage('PUT', LockToken));
    end;

    //
    // Shared implementation for Unlock and Delete message.
    // 
    local procedure UnlockOrDeleteMessage(Verb: Text; LockToken: Text): Boolean;
    var
        Client: HttpClient;
        Request: HttpRequestMessage;
        Response: HttpResponseMessage;
    begin
        CheckInitialized();

        InitializeRequest(Verb, LockToken, Request);
        Client.Send(Request, Response);

        exit(CheckResponse(Response));
    end;

    local procedure CheckInitialized();
    begin
        if not IsInitialized then
            Initialize();
    end;

    local procedure GetHeader(Headers: HttpHeaders; "key": Text): Text;
    var
        values: array[1] of Text;
    begin
        Headers.GetValues("key", values);
        exit(values[1]);
    end;

    local procedure InitializeRequest(Verb: Text; Uri: Text; var Request: HttpRequestMessage)
    var
        Headers: HttpHeaders;
    begin
        Request.GetHeaders(Headers);
        Headers.Add('Authorization', SharedAccessTokenGenerator.GetSasToken(Resource, SharedKeyName, SharedKey));
        Request.Method := Verb;
        Request.SetRequestUri(Uri);
    end;

    local procedure CheckResponse(var Response: HttpResponseMessage): Boolean
    begin
        if not Response.IsSuccessStatusCode() then begin
            LastReasonPhrase := Response.ReasonPhrase();
            exit(false);
        end;

        LastReasonPhrase := '';
        exit(true);
    end;
}