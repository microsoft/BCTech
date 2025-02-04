// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace DefaultPublisher.EDocDemo;


page 50100 "Demo Setup"
{
    PageType = Card;
    SourceTable = "Demo Setup";
    ApplicationArea = Basic, Suite;
    UsageCategory = None;
    Caption = 'E-Document Setup';
    InherentEntitlements = X;
    InherentPermissions = X;

    layout
    {
        area(Content)
        {
            group(General)
            {
                field("API Url"; Rec."API Url")
                {

                }
                field("API Key"; Rec."API Key")
                {

                }
                field("Service Name"; Rec."Service Name")
                {

                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        if not Rec.FindFirst() then begin
            Rec.Init();
            Rec."API Url" := 'https://bc-edoc-workshop.azurewebsites.net/demo-api/';
            Rec.Insert();
        end;
    end;


}
