// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.KPI;

table 53701 "Sales Ret. Agent KPI"
{
    Access = Internal;
    Caption = 'Sales Ret. Agent KPI';
    DataClassification = SystemMetadata;
    InherentEntitlements = RIMX;
    InherentPermissions = RIMX;
    ReplicateData = false;
    DataPerCompany = false;

    fields
    {
        // This field is part of the IAgentMetadata.GetSummaryPageId() contract.
        // The platform filters on "User Security ID" when opening the summary page,
        // so it must be the primary key of this table.
        field(1; "User Security ID"; Guid)
        {
            Caption = 'User Security ID';
            ToolTip = 'Specifies the unique identifier for the agent user.';
            Editable = false;
        }
        field(10; "Credit Memos Created"; Integer)
        {
            Caption = 'Credit Memos Created';
            ToolTip = 'Specifies the number of sales credit memos created by the agent.';
            DataClassification = CustomerContent;
        }
    }
    keys
    {
        key(Key1; "User Security ID")
        {
            Clustered = true;
        }
    }
}
