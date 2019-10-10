// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50170 AzureStorageSetup
{
    AccessByPermission = TableData AzureStorageSetup = IM;
    Caption = 'Azure Storage Account Setup';
    SourceTable = AzureStorageSetup;
    UsageCategory = Administration;
    ApplicationArea = All;
    DeleteAllowed = false;
    InsertAllowed = false;
    LinksAllowed = false;
    ShowFilter = false;

    layout
    {
        area(Content)
        {
            group(General)
            {
                field(AccountName; AccountName)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the Storage Account name';
                }
                field(SharedAccessKey; SharedAccessKeyValue)
                {
                    ApplicationArea = All;
                    ExtendedDatatype = Masked;
                    ToolTip = 'Specifies the access key used for authentication';

                    trigger OnValidate()
                    begin
                        if (SharedAccessKeyValue <> '') and (not EncryptionEnabled()) then
                            if Confirm(EncryptionIsNotActivatedQst) then
                                PAGE.RunModal(PAGE::"Data Encryption Management");
                        SetSharedAccessKey(SharedAccessKeyValue);
                    end;
                }

                field(IsEnabled; IsEnabled)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies whether the Azure Storage Account is enabled';
                }
            }

        }
    }

    var
        SharedAccessKeyValue: Text;
        EncryptionIsNotActivatedQst: Label 'Data encryption is currently not enabled. We recommend that you encrypt data. \Do you want to open the Data Encryption Management window?';

    trigger OnAfterGetRecord()
    begin
        if HasSharedAccessKey() then
            SharedAccessKeyValue := '*';
    end;
}