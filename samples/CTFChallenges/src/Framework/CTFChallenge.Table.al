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
        field(1; "No."; Integer)
        {
        }
        field(2; "CTF Challenge"; Enum "CTF Challenge")
        {
        }
        field(3; "Entry Type"; Option)
        {
            DataClassification = SystemMetadata;
            OptionMembers = Category,Name,RunCode,Hint;
        }
        field(4; "Display Text"; Text[2048])
        {
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
