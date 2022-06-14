// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Settings for CTF challeneges
/// </summary>
table 50104 "CTF Challenges Setup"
{
    Access = Internal;

    fields
    {
        field(1; "No."; Integer)
        {
        }
        // use CTF mode 

    }

    keys
    {
        key(PK; "No.")
        {
            Clustered = true;
        }
    }
}
