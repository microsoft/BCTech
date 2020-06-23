// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 51100 "IoT Device Workflow Setup"
{
    DataClassification = SystemMetadata;

    fields
    {
        field(1; "Device ID"; Text[30])
        {
            DataClassification = SystemMetadata;
            Caption = 'Device ID';
        }
        field(2; "Device Connection ID"; Text[40])
        {
            DataClassification = SystemMetadata;
            Caption = 'Device Connection ID';
            TableRelation = "Azure IoT Central Device"."Device Id";
        }
        field(3; "Device Name"; Text[100])
        {
            Caption = 'Device Name';
            FieldClass = FlowField;
            CalcFormula = lookup ("Azure IoT Central Device"."Device Name" Where("Device Id" = FIELD("Device Connection Id")));
            Editable = false;
        }
        field(10; "Rule ID"; Guid)
        {
            DataClassification = SystemMetadata;
            Caption = 'Rule ID';
        }
        field(11; "Rule Name"; Text[100])
        {
            DataClassification = SystemMetadata;
            Caption = 'Rule Name';
            Editable = false;
        }
        field(20; "Item No."; Code[20])
        {
            DataClassification = SystemMetadata;
            TableRelation = Item."No.";
        }
        field(21; "Item Description"; Text[50])
        {
            Caption = 'Item Description';
            FieldClass = FlowField;
            CalcFormula = Lookup (Item.Description WHERE("No." = FIELD("Item No.")));
            Editable = false;
        }
        field(30; "Vendor No."; Code[20])
        {
            DataClassification = SystemMetadata;
            TableRelation = Vendor."No.";
        }
        field(31; "Vendor Name"; Text[50])
        {
            Caption = 'Vendor Name';
            FieldClass = FlowField;
            CalcFormula = Lookup (Vendor.Name WHERE("No." = FIELD("Vendor No.")));
            Editable = false;
        }

    }

    keys
    {
        key(PK; "Device ID", "Rule ID")
        {
            Clustered = true;
        }
    }
}