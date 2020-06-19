// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51011 "Azure IoT C. Device Settings"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Json Key/Value Pair";
    SourceTableTemporary = true;
    Extensible = false;
    InsertAllowed = false;
    DeleteAllowed = false;

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

                field("Key"; "Key")
                {
                    ApplicationArea = All;
                    Editable = false;
                }
                field("Value"; "Value")
                {
                    ApplicationArea = All;
                    Editable = ValueEditable;

                    trigger OnValidate()
                    var
                        AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
                    begin
                        Rec.Modify;
                        AzureIoTCentralImpl.UpdateDeviceSettings(DeviceId, Rec);
                        CurrPage.Update(false);
                    end;
                }
                field(ValueType; ValueType)
                {
                    ApplicationArea = All;
                    Editable = false;
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
        ValueEditable: Boolean;

    trigger OnOpenPage()
    begin
        DeviceIdVisible := DeviceId <> '';
    end;

    trigger OnAfterGetCurrRecord()
    var
        JsonDatatype: Enum JsonDatatype;
    begin
        ValueEditable := ValueType <> JsonDatatype::Null;
    end;

    procedure SetDeviceId(DeviceId2: Text[40])
    begin
        DeviceId := DeviceId2;
    end;

    procedure Refresh()
    var
        AzureIoTCentralImpl: Codeunit "Azure IoT Central Impl.";
    begin
        AzureIoTCentralImpl.GetDeviceSettings(DeviceId, Rec);
    end;
}