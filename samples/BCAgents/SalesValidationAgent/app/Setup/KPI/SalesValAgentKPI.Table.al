// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup.KPI;

table 50101 "Sales Val. Agent KPI"
{
    Access = Internal;
    Caption = 'Sales Val. Agent KPI';
    DataClassification = CustomerContent;
    InherentEntitlements = RIMDX;
    InherentPermissions = RIMDX;
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
            DataClassification = EndUserPseudonymousIdentifiers;
            Editable = false;
        }
        field(10; "Orders Released"; Integer)
        {
            Caption = 'Orders Released';
            ToolTip = 'Specifies the number of sales orders released by the agent.';
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
