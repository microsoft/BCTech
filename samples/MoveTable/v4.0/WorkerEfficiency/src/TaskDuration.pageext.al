pageextension 50110 TaskDuration extends "Tasks List"
{
    layout
    {
        // Add changes to page layout here
        addafter(Description)
        {
            field("Expected Duration"; Rec."Expected Duration")
            {
                ApplicationArea = All;
                ToolTip = 'Specifies the expected duration of the task.';
            }
        }
    }
}