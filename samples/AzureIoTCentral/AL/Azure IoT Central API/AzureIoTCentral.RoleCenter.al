// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51001 "Azure IoT Central RC"
{
    PageType = RoleCenter;

    layout
    {
        area(RoleCenter)
        {
            part(Cues; "Azure IoT Central Cues")
            {
                ApplicationArea = All;
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ConnectionSetup)
            {
                ApplicationArea = All;
                Caption = 'Connection Setup';
                RunObject = Page "Azure IoT Central Connection";
            }
        }
    }
}