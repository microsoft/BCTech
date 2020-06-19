// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 51300 "Json Key/Value Pair"
{
    DataClassification = SystemMetadata;
    Extensible = false;

    fields
    {
        field(1; "Entry No."; Integer)
        {
            DataClassification = SystemMetadata;
        }
        field(2; "Key"; Text[250])
        {
            DataClassification = SystemMetadata;
        }
        field(3; "Value"; Text[2048])
        {
            DataClassification = SystemMetadata;
        }
        field(10; ValueType; Enum JsonDatatype)
        {
            DataClassification = SystemMetadata;
            InitValue = Null;
        }
        field(20; Indent; Integer)
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