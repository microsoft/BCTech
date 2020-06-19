// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 51001 "Azure IoT Central Device"
{
    DataClassification = SystemMetadata;
    Extensible = false;

    fields
    {
        field(1; "Device Id"; Text[40])
        {
            DataClassification = SystemMetadata;
        }
        field(2; "Device Name"; Text[100])
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(PK; "Device Id")
        {
            Clustered = true;
        }
    }
}