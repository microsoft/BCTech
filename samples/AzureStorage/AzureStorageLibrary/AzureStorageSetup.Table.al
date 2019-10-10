// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50170 AzureStorageSetup
{
    Access = Internal;
    Caption = 'Azure Storage Account Setup';

    fields
    {
        field(1; PrimaryKey; Code[20])
        {
            Caption = 'Primary Key';
        }
        field(2; AccountName; Text[250])
        {
            Caption = 'Account Name';
        }
        field(5; KeyStorageId; Guid)
        {
        }
        field(6; IsEnabled; Boolean)
        {
            Caption = 'Is Enabled';
        }
    }

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