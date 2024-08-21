page 50102 "Task Entries"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
#pragma warning disable AL0801
    // Table 'Task Entry' is marked to be moved. Reason: moved to separate Task Tracker app.. Tag: 1.1.0.0.
    SourceTable = "Task Entry";
#pragma warning restore AL0801

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
            }
        }
    }
}