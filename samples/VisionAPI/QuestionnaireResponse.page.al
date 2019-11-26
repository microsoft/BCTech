page 50002 "Questionnaire Response"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "questionnaire response header";

    layout
    {
        area(Content)
        {
            group(General)
            {
                field("Profile Code"; "Profile Code")
                {
                    ApplicationArea = All;
                }
                field("Contact No."; "Contact No.")
                {
                    ApplicationArea = All;
                }
                field("Created On"; "Created On")
                {
                    ApplicationArea = All;
                }
            }
            part(Lines; "Questionnaire Response Subpage")
            {
                ApplicationArea = All;
                SubPageLink = "Header Entry No." = field("Entry No.");
            }
        }
        area(FactBoxes)
        {
            part(PictureCard; "Questionnaire Response Picture")
            {
                ApplicationArea = All;
                SubPageLink = "Entry No." = field("Entry No.");
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ProcessPicture)
            {
                ApplicationArea = All;
                Caption = 'Process Picture';
                RunObject = codeunit "Process Questionnaire Response";
                RunPageOnRec = true;
                Promoted = true;
                Image = FindCreditMemo;
            }
            action(Setup)
            {
                ApplicationArea = All;
                Caption = 'Setup';
                RunObject = page "Questionnaire OCR Setup";
                Promoted = true;
                Image = Setup;
            }
        }
    }
}