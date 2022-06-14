// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 50103 "Quick Item Flag_6e5b1753"
{
    Access = Internal;
    DataClassification = SystemMetadata;

    fields
    {
        field(1; "No."; Integer)
        {
            AutoIncrement = true;
        }
    }

    keys
    {
        key(PK; "No.")
        {
            Clustered = true;
        }
    }
}