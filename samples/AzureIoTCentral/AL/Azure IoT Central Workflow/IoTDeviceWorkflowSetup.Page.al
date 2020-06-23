// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51100 "IoT Device Workflow Setup"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "IoT Device Workflow Setup";
    Caption = 'IoT Device Workflow Setup';

    layout
    {
        area(Content)
        {
            repeater(GroupName)
            {
                field(DeviceID; "Device ID")
                {
                    ApplicationArea = All;
                    Tooltip = 'The Device ID from Azure IoT Central';

                }
                field(DeviceConnectionID; "Device Connection ID")
                {
                    ApplicationArea = All;
                }
                field(DeviceName; "Device Name")
                {
                    ApplicationArea = All;
                }
                field(RuleID; "Rule ID")
                {
                    ApplicationArea = All;
                    Tooltip = 'The Rule ID from Azure IoT Central';
                }
                field(RuleName; "Rule Name")
                {
                    ApplicationArea = All;
                }
                field(ItemNo; "Item No.")
                {
                    ApplicationArea = All;
                    Tooltip = 'The Item No. will be used by Workflows when a rule is triggered in Azure IoT Central for this Device ID';
                }
                field(ItemDescription; "Item Description")
                {
                    ApplicationArea = All;
                }
                field(VendorNo; "Vendor No.")
                {
                    ApplicationArea = All;
                    Tooltip = 'The Vendor No. will be used by Workflows when a rule is triggered in Azure IoT Central for this Device ID';
                }
                field(VendorName; "Vendor Name")
                {
                    ApplicationArea = All;
                }
            }
        }
    }
}