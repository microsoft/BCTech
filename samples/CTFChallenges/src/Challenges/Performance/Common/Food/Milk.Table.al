// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 50102 Milk
{
    Access = Internal;
    DataClassification = SystemMetadata;

    fields
    {
        field(1; "Carton No."; Integer)
        {
            AutoIncrement = true;
        }
        field(2; "Cal Per 100 ml"; Integer)
        {
            InitValue = 40;
        }
        field(3; "Plant-Based"; Boolean)
        {
            InitValue = true;
        }
        field(4; "Amount Left"; Decimal)
        {
            InitValue = 1.0;
        }
    }

    keys
    {
        key(PK; "Carton No.")
        {
            Clustered = true;
        }
    }
}