pageextension 50100 TaskTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addlast(embedding)
        {
            action(TaskEntries)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Task Entries';
                RunObject = Page "Task Entries";
                ToolTip = 'Edit Task Entries';
            }
        }
    }
}
