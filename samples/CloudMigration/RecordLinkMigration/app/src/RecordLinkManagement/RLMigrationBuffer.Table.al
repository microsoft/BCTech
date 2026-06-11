// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

table 57500 "RL Migration Buffer"
{
    DataPerCompany = false;
    ReplicateData = true;
    Extensible = false;
    Caption = 'RL Migration Buffer';
    DataClassification = SystemMetadata;
    InherentEntitlements = RIMD;
    InherentPermissions = RIMD;

    fields
    {
        field(1; "Link ID"; Integer)
        {
            Caption = 'Link ID';
        }
        field(2; "Record ID"; RecordID)
        {
            Caption = 'Record ID';
        }
        field(3; URL1; Text[2048])
        {
            Caption = 'URL1';
        }
        field(7; Description; Text[250])
        {
            Caption = 'Description';
        }
        field(8; Type; Option)
        {
            Caption = 'Type';
            OptionCaption = 'Link,Note';
            OptionMembers = Link,Note;
        }
        field(9; Note; BLOB)
        {
            Caption = 'Note';
            SubType = Memo;
        }
        field(10; Created; DateTime)
        {
            Caption = 'Created';
        }
        field(11; "User ID"; Text[132])
        {
            Caption = 'User ID';
        }
        field(12; Company; Text[30])
        {
            Caption = 'Company';
        }
        field(13; Notify; Boolean)
        {
            Caption = 'Notify';
        }
        field(14; "To User ID"; Text[132])
        {
            Caption = 'To User ID';
        }
    }

    keys
    {
        key(Key1; "Link ID", Company)
        {
            Clustered = true;
        }
        key(Key2; Company)
        {
        }
    }
}
