// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

codeunit 51002 "Azure IoT Central Token Impl."
{
    Access = Internal;

    procedure GetNewIoTHubAccessToken() sasToken: Text
    var
        [NonDebuggable]
        IoTHttpHeaders: HttpHeaders;
        [NonDebuggable]
        IoTHttpClient: HttpClient;
        [NonDebuggable]
        IoTHttpContent: HttpContent;
        [NonDebuggable]
        IoTHttpResponseMessage: HttpResponseMessage;
        IoTCentralTenantId: Text;
        [NonDebuggable]
        ContentAsString: Text;
        [NonDebuggable]
        JObject: JsonObject;
        [NonDebuggable]
        JToken: JsonToken;
        [NonDebuggable]
        JValue: JsonValue;
        errorMessage: Text;
        expiration: Integer;
    begin
        IoTHttpHeaders := IoTHttpClient.DefaultRequestHeaders();
        IoTHttpHeaders.Add('Authorization', GetIoTCentralAccessToken());
        IotCentralTenantId := GetIoTCentralTenantId();
        if IotCentralTenantId = '' then
            exit;
        IoTHttpClient.Post(StrSubstNo(AuthorizationUrl, IotCentralTenantId), IoTHttpContent, IotHttpResponseMessage);

        if IotHttpResponseMessage.IsSuccessStatusCode then begin
            IoTHttpContent := IotHttpResponseMessage.Content;
            IoTHttpContent.ReadAs(ContentAsString);
            JObject.ReadFrom(ContentAsString);
            if JObject.Get('expiry', JToken) then begin
                JValue := JToken.AsValue();
                expiration := JValue.AsInteger();
            end;
            if JObject.Get('iothubTenantSasToken', JToken) then begin
                JObject := JToken.AsObject();
                if JObject.Get('sasToken', JToken) then begin
                    Jvalue := JToken.AsValue();
                    sasToken := JValue.AsText();
                end;
            end;

            if (expiration <> 0) and (sasToken <> '') then
                SetIoTHubAccessToken(sasToken, expiration)
            else
                error('error parsing sasToken from response');
        end else begin
            IoTHttpContent := IoTHttpResponseMessage.Content;
            IoTHttpContent.ReadAs(errorMessage);
            error(errorMessage);
        end;
    end;

    [NonDebuggable]
    procedure HasIoTCentralAccessToken(): Boolean
    begin
        exit(IsolatedStorage.Contains(IoTCentralAccessTokenKey, DataScope::Module))
    end;

    [NonDebuggable]
    procedure SetIoTCentralAccessToken(AccessToken: Text): Boolean
    begin
        exit(IsolatedStorage.Set(IoTCentralAccessTokenKey, AccessToken, DataScope::Module))
    end;

    [NonDebuggable]
    local procedure GetIoTCentralAccessToken() AccessToken: Text
    begin
        IsolatedStorage.Get(IoTCentralAccessTokenKey, DataScope::Module, AccessToken);
    end;

    [NonDebuggable]
    procedure ClearIoTCentralAccessToken(): Boolean
    begin
        if HasIoTCentralAccessToken() then
            exit(IsolatedStorage.Delete(IoTCentralAccessTokenKey, DataScope::Module))
    end;

    [NonDebuggable]
    procedure HasValidIoTHubAccessToken(): Boolean
    var
        ExpirationText: Text;
        Expiration: BigInteger;
        ExpirationDateTime: DateTime;
        ExpirationDuration: Duration;
    begin
        if IsolatedStorage.Contains(IoTHubAccessTokenExpirationKey) then begin
            IsolatedStorage.Get(IoTHubAccessTokenExpirationKey, DataScope::Module, ExpirationText);
            evaluate(Expiration, ExpirationText);

            // convert Expiration (Unix timestamp) to local time
            ExpirationDuration := Expiration * 1000;
            ExpirationDateTime := createdatetime(dmy2date(1, 1, 1970), 0T) + ExpirationDuration;
            ExpirationDateTime := ExpirationDateTime + (DatetimeOffset() * 3600 * 1000);
            if (CurrentDateTime < ExpirationDateTime) then
                exit(IsolatedStorage.Contains(IoTHubAccessTokenKey))
        end;
    end;

    [NonDebuggable]
    procedure SetIoTHubAccessToken(AccessToken: Text; Expiration: Integer): Boolean
    begin
        IsolatedStorage.Set(IoTHubAccessTokenExpirationKey, format(Expiration), DataScope::Module);
        exit(IsolatedStorage.Set(IoTHubAccessTokenKey, AccessToken, DataScope::Module));
    end;

    [NonDebuggable]
    procedure GetIoTHubAccessToken(): Text
    var
        [NonDebuggable]
        AccessToken: Text;
    begin
        if HasValidIoTHubAccessToken() then begin
            IsolatedStorage.Get(IoTHubAccessTokenKey, DataScope::Module, AccessToken);
            exit(AccessToken);
        end;

        exit(GetNewIoTHubAccessToken())
    end;

    [NonDebuggable]
    procedure ClearIoTHubAccessToken(): Boolean
    begin
        if IsolatedStorage.Contains(IoTHubAccessTokenExpirationKey) then
            exit(IsolatedStorage.Delete(IoTHubAccessTokenExpirationKey, DataScope::Module));
        if IsolatedStorage.Contains(IoTHubAccessTokenKey) then
            exit(IsolatedStorage.Delete(IoTHubAccessTokenKey, DataScope::Module));
    end;

    [NonDebuggable]
    local procedure GetIoTCentralTenantId(): Text
    var
    begin
        exit(GetTenantIdFromsasToken(GetIoTCentralAccessToken()))
    end;

    [NonDebuggable]
    procedure GetIoTHubTenantId(sasToken: Text): Text
    var
    begin
        exit(GetTenantIdFromsasToken(sasToken))
    end;

    [NonDebuggable]
    local procedure GetTenantIdFromsasToken(sasToken: Text) TenantId: Text
    begin
        if StrPos(sasToken, 'sr=') = 0 then
            exit('');

        TenantId := CopyStr(sasToken, StrPos(sasToken, 'sr=') + 3);
        if StrPos(TenantId, '&') = 0 then
            exit(TenantId);

        exit(CopyStr(TenantId, 1, StrPos(TenantId, '&') - 1))
    end;

    [NonDebuggable]
    local procedure DatetimeOffset() Offset: Integer
    var
        DatetimeText: Text;
    begin
        // calc timezone offset
        DateTimeText := format(CreateDatetime(dmy2date(1, 1, 1970), 120000T), 0, 9); // XML format like 1970-01-01T11:00:00Z
        evaluate(Offset, CopyStr(DateTimeText, StrPos(DateTimeText, 'T') + 1, 2)); // copy out the HH part
        Offset := 12 - Offset; // timezone offset
    end;

    var
        [NonDebuggable]
        AuthorizationUrl: Label 'https://api.azureiotcentral.com/v1-beta/applications/%1/diagnostics/sasTokens', Locked = true;
        [NonDebuggable]
        IoTCentralAccessTokenKey: Label 'IoTCentralAccessToken', Locked = true;
        [NonDebuggable]
        IotHubAccessTokenKey: Label 'IoTHubAccessToken', Locked = true;
        [NonDebuggable]
        IoTHubAccessTokenExpirationKey: Label 'IoTHubAccessTokenExpiration', Locked = true;
}