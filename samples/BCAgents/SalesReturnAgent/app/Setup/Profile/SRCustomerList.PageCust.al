// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Profile;

using Microsoft.Sales.Customer;

pagecustomization "SR Customer List" customizes "Customer List"
{
    ClearLayout = true;
    ClearActions = true;
    ClearViews = true;
    InsertAllowed = false;
    ModifyAllowed = false;
    DeleteAllowed = false;

    layout
    {
        modify("No.")
        {
            Visible = true;
        }
        modify(Name)
        {
            Visible = true;
        }
        modify("Phone No.")
        {
            Visible = true;
        }
        modify("Balance (LCY)")
        {
            Visible = true;
        }
        addafter("Phone No.")
        {
            field("SR E-Mail"; Rec."E-Mail")
            {
                ApplicationArea = All;
                Visible = true;
            }
        }
    }
}
