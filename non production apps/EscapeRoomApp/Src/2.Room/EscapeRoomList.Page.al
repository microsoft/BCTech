page 73921 "Escape Room List"
{
    ApplicationArea = All;
    Caption = 'Escape Rooms';
    PageType = List;
    SourceTable = "Escape Room";
    UsageCategory = Lists;
    CardPageId = "Escape Room";
    Editable = false;
    SourceTableView = sorting(Sequence) order(ascending);

    layout
    {
        area(content)
        {
            repeater(General)
            {
                Editable = false;

                field(Name; Rec.Name)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the escape room.';
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the description of the escape room.';
                }
                field(Sequence; Rec.Sequence)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the sequence number of the escape room.';
                }
                field(Status; Rec.Status)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the status of the escape room.';
                }
                field("Start DateTime"; Rec."Start DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the start date and time of the escape room.';
                }
                field("Stop DateTime"; Rec."Stop DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the stop date and time of the escape room.';
                }
                field("Solution Delay In Minutes"; Rec.SolutionDelayInMinutes)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the delay in minutes for the solution to become available.';
                }
            }
        }
    }
}
