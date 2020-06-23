// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51003 "Azure IoT Central Devices"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Azure IoT Central Device";

    layout
    {
        area(Content)
        {
            repeater(GroupName)
            {
                field(DeviceId; "Device Id")
                {
                    ApplicationArea = All;
                }
                field(DeviceName; "Device Name")
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(DigitalTwin)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Scope = "Repeater";

                trigger OnAction();
                var
                    AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
                begin
                    AzureIoTCentralImpl.ShowDigitalTwin("Device Id");
                end;
            }
            action(DeviceSettings)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Scope = "Repeater";

                trigger OnAction();
                var
                    AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
                begin
                    AzureIoTCentralImpl.ShowSettings("Device Id");
                end;
            }
        }
    }
}