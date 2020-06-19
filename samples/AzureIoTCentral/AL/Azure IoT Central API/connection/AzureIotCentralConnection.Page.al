// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51005 "Azure IoT Central Connection"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    Extensible = false;

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                field(AccessTokenIsSet; AccessTokenIsSet)
                {
                    ApplicationArea = All;
                    Editable = false;
                }
                field(NewAuthorizationString; NewAuthorizationString)
                {
                    ApplicationArea = All;
                    HideValue = true;

                    trigger OnValidate()
                    var
                        AzureIoTCentralTokenImpl: Codeunit "Azure IoT Central Token Impl.";
                    begin
                        AccessTokenIsSet := AzureIoTCentralTokenImpl.SetIoTCentralAccessToken(NewAuthorizationString);
                        CurrPage.Update(false);
                    end;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ClearAccessToken)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;

                trigger OnAction()
                var
                    AzureIoTCentralTokenImpl: Codeunit "Azure IoT Central Token Impl.";
                begin
                    AzureIotCentralTokenImpl.ClearIoTCentralAccessToken();
                    AccessTokenIsSet := AzureIoTCentralTokenImpl.HasIoTCentralAccessToken();
                    CurrPage.Update(false);
                end;
            }
        }
    }

    trigger OnOpenPage()
    var
        AzureIoTCentralTokenImpl: Codeunit "Azure IoT Central Token Impl.";
    begin
        AccessTokenIsSet := AzureIoTCentralTokenImpl.HasIoTCentralAccessToken();
    end;

    var
        [NonDebuggable]
        NewAuthorizationString: Text;
        AccessTokenIsSet: Boolean;
}