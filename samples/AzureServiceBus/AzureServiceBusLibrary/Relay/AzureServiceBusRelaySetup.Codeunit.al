// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50140 AzureServiceBusRelaySetup
{
    [EventSubscriber(ObjectType::Table, Database::"Service Connection", 'OnRegisterServiceConnection', '', false, false)]
    procedure OnRegisterServiceConnection(var ServiceConnection: Record "Service Connection");
    var
        AzureServiceBusRelaySetup: Record AzureServiceBusRelaySetup;
        RecRef: RecordRef;
    begin
        if not AzureServiceBusRelaySetup.Get() then begin
            if not AzureServiceBusRelaySetup.WritePermission() then
                exit;
            AzureServiceBusRelaySetup.Init();
            AzureServiceBusRelaySetup.Insert();
        end;

        RecRef.GetTable(AzureServiceBusRelaySetup);
        if AzureServiceBusRelaySetup.IsEnabled then
            ServiceConnection.Status := ServiceConnection.Status::Enabled
        else
            ServiceConnection.Status := ServiceConnection.Status::Disabled;

        ServiceConnection.InsertServiceConnection(
            ServiceConnection, RecRef.RecordId(), AzureServiceBusRelaySetup.TableCaption(),
            AzureServiceBusRelaySetup.GetServiceUri(),
            PAGE::AzureServiceBusRelaySetup);

    end;

}