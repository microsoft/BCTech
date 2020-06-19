// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 51000 "Az. IoT Central Measurement"
{
    DataClassification = SystemMetadata;
    Access = Public;
    Extensible = False;

    fields
    {
        field(1; "Entry No."; Integer)
        {
            DataClassification = SystemMetadata;
            AutoIncrement = true;
        }
        field(2; "Action ID"; Guid)
        {
            DataClassification = SystemMetadata;
        }
        field(3; "Timestamp (UTC)"; DateTime)
        {
            DataClassification = SystemMetadata;
        }
        field(4; "Application ID"; Guid)
        {
            DataClassification = SystemMetadata;
        }
        field(5; "Application Name"; Text[250])
        {
            DataClassification = SystemMetadata;
        }
        field(6; "Application Subdomain"; Text[250])
        {
            DataClassification = SystemMetadata;
        }
        field(7; "Device Connection ID"; Text[36])
        {
            DataClassification = SystemMetadata;
        }
        field(8; "Device ID"; Text[30])
        {
            DataClassification = SystemMetadata;
        }
        field(9; "Device Name"; Text[250])
        {
            DataClassification = SystemMetadata;
        }
        field(10; "Device Simulated"; Boolean)
        {
            DataClassification = SystemMetadata;
        }
        field(11; "Device Template ID"; Text[30])
        {
            DataClassification = SystemMetadata;
        }
        field(12; "Device Template Version"; Text[30])
        {
            DataClassification = SystemMetadata;
        }
        field(13; "measurements"; Text[2048])
        {
            DataClassification = SystemMetadata;
        }
        field(14; "Rule ID"; Guid)
        {
            DataClassification = SystemMetadata;
        }
        field(15; "Rule Name"; Text[250])
        {
            DataClassification = SystemMetadata;
        }
        field(16; "Rule Enabled"; Boolean)
        {
            DataClassification = SystemMetadata;
        }
    }

    keys
    {
        key(PK; "Entry No.")
        {
            Clustered = true;
        }
    }
}