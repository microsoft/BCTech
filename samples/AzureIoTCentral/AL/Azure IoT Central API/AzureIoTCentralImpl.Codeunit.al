// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

codeunit 51001 "Azure IoT Central Impl."
{
    Access = Internal;

    procedure ShowDigitalTwin(DeviceId: Text[40])
    var
        AzureIoTCentralDigitalTwinPage: Page "Azure IoT Central Digital Twin";
    begin
        AzureIoTCentralDigitalTwinPage.SetDeviceId(DeviceId);
        AzureIoTCentralDigitalTwinPage.Refresh();
        AzureIoTCentralDigitalTwinPage.Run()
    end;

    procedure ShowSettings(DeviceId: Text[40]);
    var
        AzureIoTCentralDeviceSettings: Page "Azure IoT C. Device Settings";
    begin
        AzureIoTCentralDeviceSettings.SetDeviceId(DeviceId);
        AzureIoTCentralDeviceSettings.Refresh();
        AzureIoTCentralDeviceSettings.Run()
    end;

    procedure GetDigitalTwin(DeviceId: Text[40]; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    var
        Json: Codeunit Json;
    begin
        Json.ParseJObjectToKeyValuePair(GetDigitalTwinAsJObject(DeviceId), JsonKeyValuePair);
    end;

    local procedure GetDigitalTwinAsJObject(DeviceId: Text[40]) JObject: JsonObject
    var
        AzureIoTCentralTokenImpl: Codeunit "Azure IoT Central Token Impl.";
        [NonDebuggable]
        IoTHttpHeaders: HttpHeaders;
        [NonDebuggable]
        IoTHttpClient: HttpClient;
        [NonDebuggable]
        IoTHttpContent: HttpContent;
        [NonDebuggable]
        IoTHttpResponseMessage: HttpResponseMessage;
        [NonDebuggable]
        AuthorizationToken: Text;
        IoTHubTenantId: Text;
        [NonDebuggable]
        ContentAsString: Text;
        [NonDebuggable]
        errorMessage: Text;
    begin
        if not AzureIoTCentralTokenImpl.HasIoTCentralAccessToken() then
            exit;

        AuthorizationToken := AzureIoTCentralTokenImpl.GetIoTHubAccessToken();

        IoTHttpHeaders := IoTHttpClient.DefaultRequestHeaders();
        IoTHttpHeaders.Add('Authorization', AuthorizationToken);
        IotHubTenantId := AzureIoTCentralTokenImpl.GetIoTHubTenantId(AuthorizationToken);
        if IotHubTenantId = '' then
            exit;
        IoTHttpClient.Get(StrSubstNo(IoTHubDigitalTwinUrl, IotHubTenantId, lowercase(DelChr(DeviceId, '<>', '{}'))), IotHttpResponseMessage);

        if IotHttpResponseMessage.IsSuccessStatusCode then begin
            IoTHttpContent := IotHttpResponseMessage.Content;
            IoTHttpContent.ReadAs(ContentAsString);
            JObject.ReadFrom(ContentAsString);
        end else begin
            IoTHttpContent := IoTHttpResponseMessage.Content;
            IoTHttpContent.ReadAs(errorMessage);
            error(errorMessage);
        end;
    end;

    procedure GetDeviceSettings(DeviceId: Text[40]; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    var
        Json: Codeunit Json;
        JObject: JsonObject;
        JToken: JsonToken;
        KeyList: List of [Text];
        JKey: Text;
    begin
        JObject := GetDigitalTwinAsJObject(DeviceId);
        JObject.SelectToken('$.properties.desired', JToken);
        JObject := JToken.AsObject();
        KeyList := JObject.Keys;
        foreach JKey in KeyList do begin
            if Jkey = '$metadata' then
                JObject.Remove(Jkey); // remove metadata properties
        end;
        Json.ParseJObjectToKeyValuePair(JObject, JsonKeyValuePair); // start at indent 0
    end;

    procedure UpdateDeviceSettings(DeviceId: Text[40]; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    var
        AzureIoTCentralTokenImpl: Codeunit "Azure IoT Central Token Impl.";
        Json: Codeunit Json;
        [NonDebuggable]
        IoTHttpHeaders: HttpHeaders;
        [NonDebuggable]
        IoTHttpClient: HttpClient;
        [NonDebuggable]
        IoTHttpRequestMessage: HttpRequestMessage;
        [NonDebuggable]
        IoTHttpContent: HttpContent;
        [NonDebuggable]
        IoTHttpResponseMessage: HttpResponseMessage;
        [NonDebuggable]
        AuthorizationToken: Text;
        IoTHubTenantId: Text;
        [NonDebuggable]
        ContentAsString: Text;
        [NonDebuggable]
        errorMessage: Text;
        JObject: JsonObject;
        JToken: JsonToken;
        KeyList: List of [Text];
        JKey: Text;

    begin
        if not AzureIoTCentralTokenImpl.HasIoTCentralAccessToken() then
            exit;

        AuthorizationToken := AzureIoTCentralTokenImpl.GetIoTHubAccessToken();

        IoTHttpHeaders := IoTHttpClient.DefaultRequestHeaders();
        IoTHttpHeaders.Add('Authorization', AuthorizationToken);
        IotHubTenantId := AzureIoTCentralTokenImpl.GetIoTHubTenantId(AuthorizationToken);
        if IotHubTenantId = '' then
            exit;

        ContentAsString := CreateSettingsJsonAsText(JsonKeyValuePair);
        IoTHttpContent.WriteFrom(ContentAsString);

        IoTHttpRequestMessage.Method('PATCH');
        IoTHttpRequestMessage.Content(IoTHttpContent);

        IoTHttpClient.SetBaseAddress(StrSubstNo(IoTHubDigitalTwinUrl, IotHubTenantId, LowerCase(DelChr(DeviceId, '<>', '{}'))));

        IoTHttpClient.Send(IoTHttpRequestMessage, IoTHttpResponseMessage);

        if IotHttpResponseMessage.IsSuccessStatusCode then begin
            IoTHttpContent := IotHttpResponseMessage.Content;
            IoTHttpContent.ReadAs(ContentAsString);
            JObject.ReadFrom(ContentAsString);

            JObject.SelectToken('$.properties.desired', JToken);
            JObject := JToken.AsObject();
            KeyList := JObject.Keys;
            foreach JKey in KeyList do begin
                if Jkey = '$metadata' then
                    JObject.Remove(Jkey); // remove metadata properties
            end;
            Json.ParseJObjectToKeyValuePair(JObject, JsonKeyValuePair); // start at indent 0
        end else begin
            IoTHttpContent := IoTHttpResponseMessage.Content;
            IoTHttpContent.ReadAs(errorMessage);
            error(errorMessage);
        end;
    end;

    local procedure CreateSettingsJsonAsText(var JsonKeyValuePair: Record "Json Key/Value Pair" temporary) JsonString: Text
    var
        Json: Codeunit Json;
        JObject: JsonObject;
        JObject2: JsonObject;
        JKey: Text;
    begin
        JsonKeyValuePair.Setfilter("Key", '<>%1', '$version');
        if not JsonKeyValuePair.findset then
            exit;

        JObject := Json.CreateJsonFromKeyValuePairPartial(JsonKeyValuePair);

        JObject2.Add('desired', JObject.AsToken());
        Clear(JObject);
        JObject.Add('properties', JObject2.AsToken());
        JObject.WriteTo(JsonString);
    end;

    [EventSubscriber(ObjectType::Table, DataBase::"Az. Service Bus Message", 'OnAfterInsertEvent', '', true, true)]
    local procedure InsertDeviceSetupOnAfterInsertAzServiceBusMessage(var Rec: Record "Az. Service Bus Message")
    var
        JsonKeyValuePair: Record "Json Key/Value Pair" temporary;
        AzureIoTCentralDevice: Record "Azure IoT Central Device";
        Json: Codeunit Json;
        JObject: JsonObject;
    begin
        if not (Rec."Queue Name" = 'devices') then
            exit;

        // parse device id from content field
        JObject.ReadFrom(Rec.Content);
        Json.ParseJObjectToKeyValuePair(JObject, JsonKeyValuePair);

        JsonKeyValuePair.SetRange("Key", 'deviceId');
        if JsonKeyValuePair.FindFirst() then
            if not AzureIoTCentralDevice.Get(DelChr(JsonKeyValuePair.Value, '<>', '"')) then begin
                AzureIoTCentralDevice."Device Id" := DelChr(JsonKeyValuePair.Value, '<>', '"');
                JsonKeyValuePair.SetRange("Key", 'name');
                if JsonKeyValuePair.FindFirst() then
                    AzureIoTCentralDevice."Device Name" := DelChr(JsonKeyValuePair.Value, '<>', '"');
                AzureIoTCentralDevice.Insert;
            end;
    end;

    [EventSubscriber(ObjectType::Table, Database::"Az. Service Bus Message", 'OnBeforeInsertEvent', '', true, true)]
    local procedure EnsureTemporaryOnBeforeInsertAzServiceBusMessage(var Rec: Record "Az. Service Bus Message")
    begin
        if not Rec.IsTemporary then
            error('%1 record variable must be temporary.', Rec.TableCaption)
    end;

    var
        IoTHubDigitalTwinUrl: Label 'https://%1/twins/%2?api-version=2018-06-30', Locked = true;
}