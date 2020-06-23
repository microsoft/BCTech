// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 51002 "Az. Service Bus Message"
{
    DataClassification = SystemMetadata;
    Access = Public;
    Extensible = False;

    fields
    {
        field(1; "Message Id"; Text[100])
        {
            DataClassification = SystemMetadata;
        }
        field(2; "Queue Name"; Text[30])
        {
            DataClassification = SystemMetadata;
        }
        field(3; "Content Type"; Text[50])
        {
            DataClassification = SystemMetadata;
        }
        field(4; Content; Text[2048])
        {
            DataClassification = SystemMetadata;
        }
        field(5; "Correlation Id"; Text[100])
        {
            DataClassification = SystemMetadata;
        }
        field(6; "Label"; Text[50])
        {
            DataClassification = SystemMetadata;
        }
        field(7; "Lock Token"; Text[40])
        {
            DataClassification = SystemMetadata;
        }
        field(8; "Reply To"; Text[50])
        {
            DataClassification = SystemMetadata;
        }
        field(9; "Reply To Session Id"; Text[50])
        {
            DataClassification = SystemMetadata;
        }
        field(10; "Scheduled Enqueue Time Utc"; Text[30])
        {
            DataClassification = SystemMetadata;
        }
        field(11; "Session Id"; Text[50])
        {
            DataClassification = SystemMetadata;
        }
        field(12; "Time To Live"; Text[20])
        {
            DataClassification = SystemMetadata;
        }
        field(13; "To"; Text[50])
        {
            DataClassification = SystemMetadata;
        }
    }

    keys
    {
        key(PK; "Message Id")
        {
            Clustered = true;
        }
    }
}