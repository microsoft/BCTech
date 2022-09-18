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
                    Caption = 'Enable competition mode';
                    ToolTip = 'In this mode, the challenges, flags, and hints are managed on e.g. the ctfd.io website. See the CTFD-portal-setup readme file for more information.';
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
        CTFChallenges: Codeunit "CTF Challenges";
        PasswordDialogManagement: Codeunit "Password Dialog Management";
        Password: Text;
    begin
        Password := PasswordDialogManagement.OpenPasswordDialog(true, true);
        if Password <> 'clue' then
            Error('You don''t have access to the setup page.');

        HintKeys := CTFChallenges.GetHintKeys();
    end;

    var
        HintKeys: Text;
        NoPermissionErr: Label 'You do not have the permission to access this page.';
}