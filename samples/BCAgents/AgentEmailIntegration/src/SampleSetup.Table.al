// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;

using System.Email;

table 50100 "Sample Setup"
{
    Access = Internal;
    Extensible = false;
    InherentEntitlements = RIMDX;
    InherentPermissions = RIMDX;
    DataClassification = SystemMetadata;

    fields
    {
        field(1; Id; Integer)
        {
            AutoIncrement = true;
        }
        field(2; "Agent User Security ID"; Guid)
        {
        }
        field(3; "Email Account ID"; Guid)
        {
        }
        field(4; "Email Connector"; Enum "Email Connector")
        {
        }
        field(5; "Email Address"; Text[2048])
        {
        }

        field(6; "Last Sync At"; DateTime)
        {
        }
        field(7; "Earliest Sync At"; DateTime)
        {
        }
        field(8; "Scheduled Task ID"; Guid)
        {
        }
    }

    keys
    {
        key(Key1; Id)
        {
            Clustered = true;
        }
    }
}