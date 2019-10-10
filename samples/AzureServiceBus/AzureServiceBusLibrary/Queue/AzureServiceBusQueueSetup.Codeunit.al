// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50142 AzureServiceBusQueueSetup
{
    [EventSubscriber(ObjectType::Table, Database::"Service Connection", 'OnRegisterServiceConnection', '', false, false)]
    procedure OnRegisterServiceConnection(var ServiceConnection: Record "Service Connection");
    var
        AzureServiceBusQueueSetup: Record AzureServiceBusQueueSetup;
        RecRef: RecordRef;
    begin
        if not AzureServiceBusQueueSetup.Get() then begin
            if not AzureServiceBusQueueSetup.WritePermission() then
                exit;
            AzureServiceBusQueueSetup.Init();
            AzureServiceBusQueueSetup.Insert();
        end;

        RecRef.GetTable(AzureServiceBusQueueSetup);
        if AzureServiceBusQueueSetup.IsEnabled then
            ServiceConnection.Status := ServiceConnection.Status::Enabled
        else
            ServiceConnection.Status := ServiceConnection.Status::Disabled;

        ServiceConnection.InsertServiceConnection(
            ServiceConnection, RecRef.RecordId(), AzureServiceBusQueueSetup.TableCaption(),
            AzureServiceBusQueueSetup.GetServiceUri(),
            PAGE::AzureServiceBusQueueSetup);

    end;

}