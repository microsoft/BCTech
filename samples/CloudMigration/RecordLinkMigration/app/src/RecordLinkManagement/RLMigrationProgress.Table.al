// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

table 57503 "RL Migration Progress"
{
    DataPerCompany = false;
    ReplicateData = false;
    Extensible = false;
    Caption = 'RL Migration Progress';
    DataClassification = SystemMetadata;
    InherentEntitlements = RIMD;
    InherentPermissions = RIMD;

    fields
    {
        field(1; "Company Name"; Text[30])
        {
            Caption = 'Company Name';
        }
        field(2; "Total Records"; Integer)
        {
            Caption = 'Total Records';
        }
        field(3; "Migrated Records"; Integer)
        {
            Caption = 'Migrated Records';
        }
        field(4; "Last Processed Link ID"; Integer)
        {
            Caption = 'Last Processed Link ID';
        }
        field(5; Status; Option)
        {
            Caption = 'Status';
            OptionCaption = 'Not Started,In Progress,Completed';
            OptionMembers = "Not Started","In Progress",Completed;
        }
        field(6; "Last Migration DateTime"; DateTime)
        {
            Caption = 'Last Migration DateTime';
        }
        field(7; "Error Count"; Integer)
        {
            Caption = 'Error Count';
        }
    }

    keys
    {
        key(Key1; "Company Name")
        {
            Clustered = true;
        }
    }
}
