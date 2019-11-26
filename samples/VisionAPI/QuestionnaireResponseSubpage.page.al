page 50003 "Questionnaire Response Subpage"
{
    PageType = ListPart;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "questionnaire response line";

    layout
    {
        area(Content)
        {
            repeater(General)
            {
                field("Line No"; "Line No.")
                {
                    ApplicationArea = All;
                    Visible = false;
                }
                field("Profile Code"; "Profile Code")
                {
                    ApplicationArea = All;
                    Visible = false;
                }
                field("Contact No."; "Contact No.")
                {
                    ApplicationArea = All;
                    Visible = false;
                }
                field(Description; Description)
                {
                    ApplicationArea = All;
                }
                field("Response Text"; "Response Text")
                {
                    ApplicationArea = All;
                }
                field("Response Selection"; "Response Selection")
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

    var
        myInt: Integer;
}