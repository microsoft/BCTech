// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 50101 Cereal
{
    Access = Internal;
    DataClassification = SystemMetadata;

    fields
    {
        field(1; "Box No."; Integer)
        {
            AutoIncrement = true;
        }
        field(4; "Amount Left"; Decimal)
        {
            InitValue = 2.0;
        }
    }

    keys
    {
        key(PK; "Box No.")
        {
            Clustered = true;
        }
    }
}