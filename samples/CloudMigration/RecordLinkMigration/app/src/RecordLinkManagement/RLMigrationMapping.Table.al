// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

table 57501 "RL Migration Mapping"
{
    DataPerCompany = false;
    ReplicateData = false;
    Extensible = false;
    Caption = 'RL Migration Mapping';
    DataClassification = SystemMetadata;
    InherentEntitlements = RIMD;
    InherentPermissions = RIMD;

    fields
    {
        field(1; "Source Link ID"; Integer)
        {
            Caption = 'Source Link ID';
        }
        field(2; "Target Link ID"; Integer)
        {
            Caption = 'Target Link ID';
        }
        field(3; Company; Text[30])
        {
            Caption = 'Company';
        }
        field(4; "Record ID"; RecordID)
        {
            Caption = 'Record ID';
        }
        field(5; Type; Option)
        {
            Caption = 'Type';
            OptionCaption = 'Link,Note';
            OptionMembers = Link,Note;
        }
        field(6; Description; Text[250])
        {
            Caption = 'Description';
        }
        field(7; "Note Prefix"; Text[2048])
        {
            Caption = 'Note Prefix';
        }
        field(8; "Is Duplicate"; Boolean)
        {
            Caption = 'Is Duplicate';
        }
        field(9; "Duplicate Action"; Option)
        {
            Caption = 'Duplicate Action';
            OptionCaption = 'Pending,Skip,Overwrite';
            OptionMembers = Pending,Skip,Overwrite;
        }
    }

    keys
    {
        key(Key1; "Source Link ID", Company)
        {
            Clustered = true;
        }
        key(Key2; "Target Link ID")
        {
        }
        key(Key3; "Record ID", Type, Description)
        {
        }
    }
}
