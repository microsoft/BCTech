// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51010 "Azure IoT Central Digital Twin"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Json Key/Value Pair";
    SourceTableTemporary = true;
    Extensible = false;

    layout
    {
        area(Content)
        {
            Group(Device)
            {
                field(DeviceId; DeviceId)
                {
                    ApplicationArea = All;
                    Visible = DeviceIdVisible;

                    trigger OnValidate()
                    begin
                        Refresh();
                        CurrPage.Update(false);
                    end;
                }

            }
            repeater(GroupName)
            {
                IndentationColumn = Indent;
                IndentationControls = "Key";
                ShowAsTree = true;
                Editable = false;

                field("Key"; "Key")
                {
                    ApplicationArea = All;
                }
                field("Value"; "Value")
                {
                    ApplicationArea = All;
                }

            }
        }
    }

    actions
    {
        area(Creation)
        {
            action(SaveDevice)
            {
                Caption = 'Save Device';
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = New;

                trigger OnAction()
                var
                    AzureIoTCentralDevice: Record "Azure IoT Central Device";
                begin
                    if not AzureIoTCentralDevice.Get(DeviceId) then begin
                        AzureIoTCentralDevice."Device Id" := DeviceId;
                        AzureIoTCentralDevice.Insert();
                    end;
                end;
            }
        }
        area(Processing)
        {
            action(RefreshAction)
            {
                Caption = 'Refresh';
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;

                trigger OnAction();
                begin
                    Refresh();
                    CurrPage.Update(false);
                end;
            }
        }
    }
    var
        DeviceId: Text[40];
        DeviceIdVisible: Boolean;

    trigger OnOpenPage()
    begin
        DeviceIdVisible := DeviceId <> '';
    end;

    procedure SetDeviceId(DeviceId2: Text[40])
    begin
        DeviceId := DeviceId2;
    end;

    procedure Refresh()
    var
        AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
    begin
        AzureIoTCentralImpl.GetDigitalTwin(DeviceId, Rec);
    end;
}