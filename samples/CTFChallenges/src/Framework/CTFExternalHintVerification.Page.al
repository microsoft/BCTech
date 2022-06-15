// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The page to verify the access to hints.
/// </summary>
page 50104 "CTF External Hint Verification"
{
    PageType = StandardDialog;

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                ShowCaption = false;

                field(Input; InputText)
                {
                    Caption = 'Verification code';
                    ApplicationArea = All;
                }
            }
        }
    }

    trigger OnQueryClosePage(CloseAction: Action): Boolean
    begin
        if CloseAction = CloseAction::LookupOK then begin
            if InputText <> VerificationToken then
                Error(WrongCodeErr);
        end;
    end;

    internal procedure SetHint(HintText: Text)
    var
        CryptographyManagement: Codeunit "Cryptography Management";
        HashAlgorithmType: Option MD5,SHA1,SHA256,SHA384,SHA512;
        HashText: Text;
    begin
        HashText := CryptographyManagement.GenerateHash(HintText, HashAlgorithmType::SHA256);
        VerificationToken := 'CTF_' + HashText.Substring(1, 8);
    end;

    var
        VerificationToken: Text;
        InputText: Text;
        WrongCodeErr: Label 'Wrong code';
}