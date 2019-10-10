// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50132 DirectoryItems
{
    fields
    {
        field(1; Name; Text[512])
        {
        }
        field(2; IsDirectory; Boolean)
        {
        }
        field(3; DisplayName; Text[512])
        {
        }
        field(4; Created; DateTime)
        {
        }
    }

    keys
    {
        key(DisplayName; DisplayName) { }
    }

}