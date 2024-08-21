page 50102 "Task Entries"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Task Entry";

    layout
    {
        area(Content)
        {
            repeater(repeater)
            {
                field("EntryNo."; Rec."EntryNo.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the task entry.';
                }
                field(TaskCode; Rec.TaskCode)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the task.';
                }
                field("Employee No."; Rec."Employee No.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the employee.';
                }
                field("Start Datetime"; Rec."Start Datetime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the start datetime of the task.';
                }
                field("End Datetime"; Rec."End Datetime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the end datetime of the task.';
                }
            }
        }
    }
}