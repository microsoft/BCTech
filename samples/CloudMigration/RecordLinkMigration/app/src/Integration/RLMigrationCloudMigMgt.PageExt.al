// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using Microsoft.DataMigration;

pageextension 57500 "RL Migration CloudMigMgt" extends "Cloud Migration Management"
{
    actions
    {
        addlast(Processing)
        {
            action(RecordLinkMigration)
            {
                ApplicationArea = All;
                Caption = 'Record Link Migration';
                ToolTip = 'Open the Record Link Migration dashboard to transfer record links and notes from the buffer to the system.';
                Image = Links;
                RunObject = page "RL Migration Dashboard";
            }
        }
        addlast(Promoted)
        {
            actionref(RecordLinkMigration_Promoted; RecordLinkMigration) { }
        }
    }
}
