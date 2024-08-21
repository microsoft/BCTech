pageextension 50111 TaskEfficiency extends "Task Entries"
{
    layout
    {
        // Add changes to page layout here
        addafter("End Datetime")
        {
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