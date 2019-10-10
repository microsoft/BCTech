// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50140 AzureServiceBusRelaySetup
{
    AccessByPermission = TableData AzureServiceBusRelaySetup = IM;
    Caption = 'Azure Service Bus Relay Setup';
    SourceTable = AzureServiceBusRelaySetup;
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
                field(AzureRelayNamespace; AzureRelayNamespace)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the namespace of the Azure Relay';
                }
                field(HybridConnectionName; HybridConnectionName)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the Hybrid Connection ';
                }
                field(KeyName; KeyName)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the Shared Access Policy to for authentication';
                }
                field(SharedAccessKey; SharedAccessKeyValue)
                {
                    ApplicationArea = All;
                    ExtendedDatatype = Masked;
                    ToolTip = 'Specifies the key used for authentication';

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
                    ToolTip = 'Specifies whether the Azure Service Bus Relay integration is enabled';
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