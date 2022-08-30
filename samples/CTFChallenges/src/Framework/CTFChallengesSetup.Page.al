// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 50105 "CTF Challenges Setup"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "CTF Challenges Setup";
    AccessByPermission = tabledata "CTF Challenges Setup" = M;

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                ShowCaption = false;

                field(Mode; Rec."External Mode")
                {
                    ApplicationArea = All;
                    Caption = 'Enable Microsoft CTF portal mode';
                }

                field("Hint keys"; HintKeys)
                {
                    ApplicationArea = All;
                    Editable = false;
                    ShowCaption = false;
                    MultiLine = true;
                    Caption = ' ';

                }
            }
        }
    }

    trigger OnOpenPage()
    var
        CTFChallengesSetup: Record "CTF Challenges Setup";
        CTFChallenges: Codeunit "CTF Challenges";
    begin



        if not CTFChallengesSetup.WritePermission() then
            Error(NoPermissionErr);
        HintKeys := CTFChallenges.GetHintKeys();
    end;

    var
        HintKeys: Text;
        NoPermissionErr: Label 'You do not have the permission to access this page.';
}