// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The temporary table to hold information about slow running examples
/// </summary>
table 50100 "Slow Code Example"
{
    TableType = Temporary;
    Access = Internal;

    fields
    {
        field(1; "Slow Code Example"; Enum "Slow Code Examples")
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
        key(PK; "Slow Code Example", "Entry Type")
        {
            Clustered = true;
        }
    }
}
