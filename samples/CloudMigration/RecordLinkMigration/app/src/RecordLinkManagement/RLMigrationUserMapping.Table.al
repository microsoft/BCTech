// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using System.Security.AccessControl;

table 57502 "RL Migration User Mapping"
{
    DataPerCompany = false;
    ReplicateData = false;
    Extensible = false;
    Caption = 'RL Migration User Mapping';
    DataClassification = SystemMetadata;
    InherentEntitlements = RIMD;
    InherentPermissions = RIMD;

    fields
    {
        field(1; "Source User Name"; Code[50])
        {
            Caption = 'Source User Name';
            Description = 'The on-premises username (e.g., DOMAIN\user)';
        }
        field(2; "Dest User Name"; Code[50])
        {
            Caption = 'Destination User Name';
            Description = 'The SaaS username (e.g., user@contoso.com)';
        }
        field(3; "Is Mapped"; Boolean)
        {
            Caption = 'Is Mapped';
            Description = 'Indicates whether this user has been successfully mapped';
        }
    }

    keys
    {
        key(Key1; "Source User Name")
        {
            Clustered = true;
        }
    }
}
