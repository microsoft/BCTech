// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace BCTech.EServices.EDocumentConnector;

using System.Environment;
using System.Utilities;

page 50105 "ClearModelExtConnSetupCard"
{
    PageType = Card;
    SourceTable = "ClearModelExtConnectionSetup";
    ApplicationArea = Basic, Suite;
    UsageCategory = None;
    Caption = 'Clearance Service Connection Setup';

    layout
    {
        area(Content)
        {
            group(General)
            {
                field(ClientID; ClientID)
                {
                    Caption = 'Client ID';
                    ToolTip = 'Specifies the client ID token.';
                    ApplicationArea = Basic, Suite;
                    ExtendedDatatype = Masked;
                    Visible = not IsSaaSInfrastructure;
                    ShowMandatory = true;

                    trigger OnValidate()
                    begin
                        ClearanceAuth.SetClientId(Rec."Client ID", ClientID);
                    end;
                }
                field(ClientSecret; ClientSecret)
                {
                    Caption = 'Client Secret';
                    ToolTip = 'Specifies the client secret token.';
                    ApplicationArea = Basic, Suite;
                    ExtendedDatatype = Masked;
                    Visible = not IsSaaSInfrastructure;
                    ShowMandatory = true;

                    trigger OnValidate()
                    begin
                        ClearanceAuth.SetClientSecret(Rec."Client Secret", ClientSecret);
                    end;
                }
                field("Authentication URL"; Rec."Authentication URL")
                {
                    ApplicationArea = Basic, Suite;
                    ToolTip = 'Specifies the URL to connect to Service Online.';
                    Visible = not IsSaaSInfrastructure;
                }
                field("FileAPI URL"; Rec."FileAPI URL")
                {
                    ApplicationArea = Basic, Suite;
                    ToolTip = 'Specifies the file API URL.';
                }
                field("Company Id"; Rec."Company Id")
                {
                    ApplicationArea = Basic, Suite;
                    ToolTip = 'Specifies the company ID.';
                    ShowMandatory = true;
                }
            }
        }
    }

    actions
    {
        area(processing)
        {
            action(OpenOAuthSetup)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Open OAuth 2.0 setup';
                Image = Setup;
                Promoted = true;
                PromotedCategory = Process;
                PromotedOnly = true;
                ToolTip = 'Opens the OAuth 2.0 setup for the current user.';

                trigger OnAction()
                var
                    ClearanceAuth: Codeunit "Clearance Auth.";
                begin
                    ClearanceAuth.OpenOAuthSetupPage();
                end;
            }
            action(ResetSetup)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Reset Setup';
                Image = Restore;
                ToolTip = 'Resets the Clearance setup.';

                trigger OnAction()
                var
                    ClearanceAuth: Codeunit "Clearance Auth.";
                    ConfirmMgt: Codeunit "Confirm Management";
                begin
                    if ConfirmMgt.GetResponse(ResetQst) then
                        ClearanceAuth.ResetConnectionSetup();
                end;
            }
        }
    }

    trigger OnOpenPage()
    var
        EnvironmentInfo: Codeunit "Environment Information";

    begin
        IsSaaSInfrastructure := EnvironmentInfo.IsSaaSInfrastructure();

        ClearanceAuth.InitConnectionSetup();
        ClearanceAuth.IsClientCredsSet(ClientID, ClientSecret);
    end;

    trigger OnClosePage()
    var
    begin
        Rec.TestField("Company Id");
    end;

    var
        ClearanceAuth: Codeunit "Clearance Auth.";
        [NonDebuggable]
        ClientID, ClientSecret : Text;
        IsSaaSInfrastructure: Boolean;
        ResetQst: Label 'Are you sure you want to reset the setup?';
}
