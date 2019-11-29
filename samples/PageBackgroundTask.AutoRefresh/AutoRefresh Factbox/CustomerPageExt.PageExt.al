// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

pageextension 50102 "Customer List PageExt" extends "Customer List"
{
    layout
    {
        addfirst(factboxes)
        {
            part("AutoRefresh Factbox"; "AutoRefresh Factbox")
            {
                ApplicationArea = Basic, Suite;
            }
        }
    }
}

