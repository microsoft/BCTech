// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51004 "Azure IoT Central Cues"
{
    Caption = 'Activities';
    PageType = CardPart;
    RefreshOnActivate = true;
    SourceTable = "Azure IoT Central Device Cue";

    layout
    {
        area(content)
        {
            cuegroup(Devices)
            {
                Caption = 'Devices';
                CueGroupLayout = Wide;
                field("Device Count"; "Device Count")
                {
                    ApplicationArea = All;
                    Caption = 'Devices';

                    trigger OnDrillDown()
                    begin
                        OpenDevicesPage;
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        if not Get() then
            Insert();
    end;

    local procedure OpenDevicesPage()
    begin
        Page.Run(Page::"Azure IoT Central Devices");
    end;
}
