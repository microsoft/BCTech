// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51000 "Az. IoT Central Measurements"
{
    PageType = API;
    Caption = 'Azure IoT Central Measurements';
    APIPublisher = 'IoT';
    APIGroup = 'AzureIoTCentral';
    APIVersion = 'v2.0';
    EntityName = 'measurement';
    EntitySetName = 'measurements';
    SourceTable = "Az. IoT Central Measurement";
    DelayedInsert = true;
    Extensible = false;

    layout
    {
        area(Content)
        {
            repeater(GroupName)
            {
                field("ActionID"; "Action ID")
                {
                    ApplicationArea = All;
                }
                field("Timestamp"; "Timestamp (UTC)")
                {
                    ApplicationArea = All;
                }
                field("ApplicationID"; "Application ID")
                {
                    ApplicationArea = All;
                }
                field("ApplicationName"; "Application Name")
                {
                    ApplicationArea = All;
                }
                field("ApplicationSubdomain"; "Application Subdomain")
                {
                    ApplicationArea = All;
                }
                field("DeviceConnectionID"; "Device Connection ID")
                {
                    ApplicationArea = All;
                }
                field("DeviceID"; "Device ID")
                {
                    ApplicationArea = All;
                }
                field("DeviceName"; "Device Name")
                {
                    ApplicationArea = All;
                }
                field("DeviceSimulated"; "Device Simulated")
                {
                    ApplicationArea = All;
                }
                field("DeviceTemplateID"; "Device Template ID")
                {
                    ApplicationArea = All;
                }
                field("DeviceTemplateVersion"; "Device Template Version")
                {
                    ApplicationArea = All;
                }
                field("measurements"; "measurements")
                {
                    ApplicationArea = All;
                }
                field("RuleID"; "Rule ID")
                {
                    ApplicationArea = All;
                }
                field("RuleName"; "Rule Name")
                {
                    ApplicationArea = All;
                }
                field("RuleEnabled"; "Rule Enabled")
                {
                    ApplicationArea = All;
                }

            }
        }
    }
}