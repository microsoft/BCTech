// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

table 50106 Treasure
{
    Access = Internal;
    DataClassification = SystemMetadata;

    fields
    {
        field(1; ID; Guid)
        {
        }
        field(4; Value; Text[250])
        {
        }
    }

    keys
    {
        key(PK; ID)
        {
            Clustered = true;
        }
    }
}