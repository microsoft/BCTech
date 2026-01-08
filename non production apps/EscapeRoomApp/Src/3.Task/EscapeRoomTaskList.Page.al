page 73923 "Escape Room Task List"
{
    ApplicationArea = All;
    Caption = 'Tasks';
    PageType = ListPart;
    SourceTable = "Escape Room Task";
    Editable = false;
    SourceTableView = sorting(Sequence) order(ascending);

    layout
    {
        area(content)
        {
            repeater(General)
            {
                field(Name; Rec.Name)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the name of the task.';
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the description of the task.';
                }
                field(Status; Rec.Status)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the status of the task.';
                }
                field("Stop DateTime"; Rec."Stop DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the stop date and time of the escape room.';
                }
            }
        }
    }
}
