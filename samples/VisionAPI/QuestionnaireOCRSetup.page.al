page 50001 "Questionnaire OCR Setup"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Questionnaire OCR Setup";

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
                field("Ocr Url"; "OCR Url")
                {
                    ApplicationArea = All;
                }
                field("OCR Subscription Key"; "OCR Subscription Key")
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ActionName)
            {
                ApplicationArea = All;

                trigger OnAction()
                begin

                end;
            }
        }
    }
    trigger OnOpenPage()
    begin
        if not get then
            insert(true);
    end;
}