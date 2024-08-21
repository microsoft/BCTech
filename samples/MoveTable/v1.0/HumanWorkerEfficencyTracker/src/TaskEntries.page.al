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
                    ToolTip = 'Specifies the date and time when the task entry was started.';
                }
                field("End Datetime"; Rec."End Datetime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the date and time when the task entry was ended.';
                }
                field("Expected Duration"; Rec."Expected Duration")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the expected duration of the task entry.';
                }
                field("Actual Duration"; Rec."Actual Duration")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the actual duration of the task entry.';
                }
                field("Efficiency Score"; Rec."Efficiency Score")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the efficiency score of the task entry.';
                }
            }
        }
    }
}