// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Profile;

using Microsoft.Finance.RoleCenters;

profile "SR Agent"
{
    Caption = 'Sales Return Agent (Copilot)';
    Enabled = false;
    ProfileDescription = 'Functionality for the Sales Return Agent to efficiently create credit memos for customer returns.';
    Promoted = false;
    RoleCenter = "Account Receivables";
    Customizations =
        "SR Account Receivables",
        "SR Posted Sales Invoices",
        "SR Sales Credit Memo",
        "SR Sales Cr. Memo Subform",
        "SR Customer List";
}
