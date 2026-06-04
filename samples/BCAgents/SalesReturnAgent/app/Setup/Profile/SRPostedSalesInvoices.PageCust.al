// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Profile;

using Microsoft.Sales.History;

pagecustomization "SR Posted Sales Invoices" customizes "Posted Sales Invoices"
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
        modify("Sell-to Customer No.")
        {
            Visible = true;
        }
        modify("Sell-to Customer Name")
        {
            Visible = true;
        }
        modify("Posting Date")
        {
            Visible = true;
        }
        modify("Due Date")
        {
            Visible = true;
        }
        modify("Amount")
        {
            Visible = true;
        }
        modify("Amount Including VAT")
        {
            Visible = true;
        }
    }

    actions
    {
        modify(CreateCreditMemo_Promoted)
        {
            Visible = true;
        }
    }
}
