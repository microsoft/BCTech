// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Profile;

using Microsoft.Sales.Document;

pagecustomization "SR Sales Credit Memo" customizes "Sales Credit Memo"
{
    ClearLayout = true;
    ClearActions = true;
    InsertAllowed = true;
    ModifyAllowed = true;
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
        modify("Posting Description")
        {
            Visible = true;
        }
        modify("Posting Date")
        {
            Visible = true;
        }
        modify("Document Date")
        {
            Visible = true;
        }
        modify("External Document No.")
        {
            Visible = true;
        }
        modify("Applies-to Doc. Type")
        {
            Visible = true;
        }
        modify("Applies-to Doc. No.")
        {
            Visible = true;
        }
        modify(WorkDescription)
        {
            Visible = true;
        }
        modify(SalesLines)
        {
            Visible = true;
        }
    }

    actions
    {
        modify(Post)
        {
            Visible = true;
        }
        modify(Release)
        {
            Visible = true;
        }
        modify(Reopen)
        {
            Visible = true;
        }
        modify(DocAttach)
        {
            Visible = true;
        }
        modify(TestReport)
        {
            Visible = true;
            AboutText = 'Generate PDF Test Report';
        }
    }
}
