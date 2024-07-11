page 50103 "Tasks List"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = Task;

    layout
    {
        area(Content)
        {
            repeater(repeater)
            {
                field(TaskCode; Rec.TaskCode)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the task.';
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the description of the task.';
                }
                field("Expected Duration"; Rec."Expected Duration")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the expected duration of the task.';
                }
            }
        }
    }
}