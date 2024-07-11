pageextension 50100 TaskTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addfirst(embedding)
        {
#pragma warning disable AS0125
            //The XLIFF translation ID changed. This will break the translations provided by dependent extensions for your extension.
            action(TaskEntries)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Task Entries';
                RunObject = Page "Task Entries";
                ToolTip = 'Edit Task Entries';
            }
#pragma warning restore AS0125
        }
    }
}
