// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup;

table 50100 "Sales Val. Agent Setup"
{
    Access = Internal;
    Caption = 'Sales Val. Agent Setup';
    DataClassification = CustomerContent;
    InherentEntitlements = RIMDX;
    InherentPermissions = RIMDX;
    ReplicateData = false;
    DataPerCompany = false;

    fields
    {
        // The platform uses a field named "User Security ID" to open the setup and summary pages
        // defined in IAgentMetadata. This field must exist with this exact name on the source table.
        field(1; "User Security ID"; Guid)
        {
            Caption = 'User Security ID';
            ToolTip = 'Specifies the unique identifier for the user.';
            DataClassification = EndUserPseudonymousIdentifiers;
            Editable = false;
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