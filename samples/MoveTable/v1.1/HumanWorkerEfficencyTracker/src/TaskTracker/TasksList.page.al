page 50103 "Tasks List"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
#pragma warning disable AL0801
    // Table 'Task' is marked to be moved. Reason: moved to separate Task Tracker app.. Tag: 1.1.0.0
    SourceTable = Task;
#pragma warning restore AL0801

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
            }
        }
    }
}