// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Profile;

using Microsoft.Finance.RoleCenters;

pagecustomization "SR Account Receivables" customizes "Account Receivables"
{
    ClearLayout = true;
    ClearActions = true;

    actions
    {
        modify(Customers)
        {
            Visible = true;
        }
        modify("Posted Sales Invoices")
        {
            Visible = true;
        }
    }
}
