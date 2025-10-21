// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Integration;

/// <summary>
/// Setup page for DirectionsConnector API connection.
/// Allows users to configure the API URL and register to get an API key.
/// </summary>
page 81100 "Directions Connection Setup"
{
    Caption = 'Directions Connector Setup';
    PageType = Card;
    SourceTable = "Directions Connection Setup";
    InsertAllowed = false;
    DeleteAllowed = false;
    ApplicationArea = All;
    UsageCategory = Administration;

    layout
    {
        area(content)
        {
            group(General)
            {
                Caption = 'Connection Settings';

                field("API Base URL"; Rec."API Base URL")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the base URL for the Directions API server (e.g., https://workshop-server.azurewebsites.net/)';
                    ShowMandatory = true;
                }
                field("User Name"; Rec."User Name")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the user name to register with the API';
                    ShowMandatory = true;
                }
                field(Registered; Rec.Registered)
                {
                    ApplicationArea = All;
                    ToolTip = 'Indicates whether you have successfully registered with the API';
                    Style = Favorable;
                    StyleExpr = Rec.Registered;
                }
                field("API Key"; APIKeyText)
                {
                    ApplicationArea = All;
                    Caption = 'API Key';
                    ToolTip = 'Specifies the API key received after registration';
                    Editable = false;
                    ExtendedDatatype = Masked;

                    trigger OnAssistEdit()
                    var
                        NewAPIKey: Text;
                    begin
                        NewAPIKey := APIKeyText;
                        if NewAPIKey <> '' then begin
                            Rec.SetAPIKey(NewAPIKey);
                            Rec.Modify();
                            CurrPage.Update();
                        end;
                    end;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(Register)
            {
                ApplicationArea = All;
                Caption = 'Register';
                ToolTip = 'Register with the Directions API to get an API key';
                Image = Approve;
                Promoted = true;
                PromotedCategory = Process;
                PromotedOnly = true;

                trigger OnAction()
                var
                    DirectionsAuth: Codeunit "Directions Auth";
                begin
                    DirectionsAuth.RegisterUser(Rec);
                    CurrPage.Update();
                    UpdateAPIKeyText();
                    Message('Registration successful! Your API key has been saved.');
                end;
            }
            action(TestConnection)
            {
                ApplicationArea = All;
                Caption = 'Test Connection';
                ToolTip = 'Test the connection to the Directions API';
                Image = ValidateEmailLoggingSetup;
                Promoted = true;
                PromotedCategory = Process;
                PromotedOnly = true;

                trigger OnAction()
                var
                    DirectionsAuth: Codeunit "Directions Auth";
                begin
                    DirectionsAuth.TestConnection(Rec);
                    Message('Connection test successful!');
                end;
            }
        }
    }

    trigger OnOpenPage()
    begin
        Rec.GetOrCreate();
        UpdateAPIKeyText();
    end;

    trigger OnAfterGetCurrRecord()
    begin
        UpdateAPIKeyText();
    end;

    local procedure UpdateAPIKeyText()
    begin
        if not IsNullGuid(Rec."API Key") then
            APIKeyText := Rec.GetAPIKeyText()
        else
            APIKeyText := '';
    end;

    var
        APIKeyText: Text;
}
