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

    trigger OnClosePage()
    var
    begin
        Rec.TestField("Company Id");
    end;

    var
        [NonDebuggable]
        ClientID, ClientSecret : Text;
        IsSaaSInfrastructure: Boolean;
        ResetQst: Label 'Are you sure you want to reset the setup?';
}
