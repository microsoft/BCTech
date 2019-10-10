// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50140 AzureServiceBusRelaySetup
{
    Access = Internal;
    Caption = 'Azure Service Bus Relay Setup';

    fields
    {
        field(1; PrimaryKey; Code[20])
        {
            Caption = 'Primary Key';
        }
        field(2; AzureRelayNamespace; Text[250])
        {
            Caption = 'Azure Relay Namespace';
        }
        field(3; HybridConnectionName; Text[250])
        {
            Caption = 'Hybrid Connection Name';
        }
        field(4; KeyName; Text[250])
        {
            Caption = 'Shared Access Policy Name';
        }
        field(5; KeyStorageId; Guid)
        {
        }
        field(6; IsEnabled; Boolean)
        {
            Caption = 'Is Enabled';
        }
    }

    procedure GetServiceUri(): Text[250];
    begin
        exit(CopyStr(StrSubstNo('https://%1.servicebus.windows.net/%2', AzureRelayNamespace, HybridConnectionName), 1, 250));
    end;

    procedure SetSharedAccessKey(SharedAccessKey: Text)
    begin
        if IsNullGuid(KeyStorageId) then
            KeyStorageId := CreateGuid();

        if not EncryptionEnabled() then
            IsolatedStorage.Set(KeyStorageId, SharedAccessKey, Datascope::Module)
        else
            IsolatedStorage.SetEncrypted(KeyStorageId, SharedAccessKey, Datascope::Module);
    end;

    [NonDebuggable]
    internal procedure GetSharedAccessKey(): Text
    var
        Value: Text;
    begin
        if not IsNullGuid(KeyStorageId) then
            IsolatedStorage.Get(KeyStorageId, Datascope::Module, Value);
        exit(Value);
    end;

    [NonDebuggable]
    procedure HasSharedAccessKey(): Boolean
    begin
        exit(GetSharedAccessKey() <> '');
    end;

}