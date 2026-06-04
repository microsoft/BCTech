// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Profile;

using Microsoft.Sales.Document;

pagecustomization "SR Sales Cr. Memo Subform" customizes "Sales Cr. Memo Subform"
{
    ClearLayout = true;
    ClearActions = true;
    InsertAllowed = true;
    ModifyAllowed = true;
    DeleteAllowed = true;

    layout
    {
        modify(Type)
        {
            Visible = true;
        }
        modify("No.")
        {
            Visible = true;
        }
        modify(Description)
        {
            Visible = true;
        }
        modify(Quantity)
        {
            Visible = true;
        }
        modify("Unit of Measure Code")
        {
            Visible = true;
        }
        modify("Unit Price")
        {
            Visible = true;
        }
        modify("Line Amount")
        {
            Visible = true;
        }
    }
}
