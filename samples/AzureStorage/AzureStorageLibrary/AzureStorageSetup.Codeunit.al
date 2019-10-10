// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50170 AzureStorageSetup
{
    [EventSubscriber(ObjectType::Table, Database::"Service Connection", 'OnRegisterServiceConnection', '', false, false)]
    procedure OnRegisterServiceConnection(var ServiceConnection: Record "Service Connection");
    var
        AzureStorageSetup: Record AzureStorageSetup;
        RecRef: RecordRef;
    begin
        if not AzureStorageSetup.Get() then begin
            if not AzureStorageSetup.WritePermission() then
                exit;
            AzureStorageSetup.Init();
            AzureStorageSetup.Insert();
        end;

        RecRef.GetTable(AzureStorageSetup);
        if AzureStorageSetup.IsEnabled then
            ServiceConnection.Status := ServiceConnection.Status::Enabled
        else
            ServiceConnection.Status := ServiceConnection.Status::Disabled;

        ServiceConnection.InsertServiceConnection(
            ServiceConnection, RecRef.RecordId(), AzureStorageSetup.TableCaption(),
            AzureStorageSetup.AccountName,
            PAGE::AzureStorageSetup);
    end;
}