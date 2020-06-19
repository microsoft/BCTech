// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

///
///<Summary></Summary>
///

codeunit 51000 "Azure IoT Central"
{
    Access = Public;

    /// <summary>
    ///
    /// </summary>
    procedure GetDigitalTwin(DeviceId: Text[40]; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    var
        AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
    begin
        AzureIoTCentralImpl.GetDigitalTwin(DeviceId, JsonKeyValuePair);
    end;

    /// <summary>
    ///
    /// </summary>
    procedure GetDeviceSettings(DeviceId: Text[40]; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    var
        AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
    begin
        AzureIoTCentralImpl.GetDeviceSettings(DeviceId, JsonKeyValuePair);
    end;
}