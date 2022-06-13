// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The temporary table to hold information about CTF challenges.
/// </summary>
table 50100 "CTF Challenge"
{
    TableType = Temporary;
    Access = Internal;

    fields
    {
        field(1; "CTF Challenge"; Enum "CTF Challenge")
        {
        }
        field(2; "Entry Type"; Option)
        {
            DataClassification = SystemMetadata;
            OptionMembers = Name,RunCode,Hint;
        }
        field(3; "Display Text"; Text[2048])
        {
        }
    }

    keys
    {
        key(PK; "CTF Challenge", "Entry Type")
        {
            Clustered = true;
        }
    }
}
