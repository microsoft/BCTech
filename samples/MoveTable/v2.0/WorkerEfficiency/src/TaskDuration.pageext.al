pageextension 50110 TaskDuration extends "Tasks List"
{
    layout
    {
        // Add changes to page layout here
        addafter(Description)
        {
#pragma warning disable AS0125
            //The XLIFF translation ID changed. This will break the translations provided by dependent extensions for your extension.
            field("Expected Duration"; Rec."Expected Duration")
            {
                ApplicationArea = All;
                ToolTip = 'Specifies the expected duration of the task.';
            }
#pragma warning restore AS0125
        }
    }
}