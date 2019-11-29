// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50110 "AutoRefresh"
{
    DataClassification = SystemMetadata;

    fields
    {
        field(1; Id; Integer)
        {
            DataClassification = SystemMetadata;
            Caption = 'Identifier';
            AutoIncrement = true;
        }

        field(2; Date; DateTime)
        {
            DataClassification = SystemMetadata;
            Caption = 'Created';
        }

        field(3; CreatedBySessionId; Integer)
        {
            DataClassification = SystemMetadata;
            Caption = 'Created by session';
        }
    }

    keys
    {
        key(PK; Id)
        {
            Clustered = true;
        }
    }
}