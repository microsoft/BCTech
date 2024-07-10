pageextension 50102 NrgTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addfirst(embedding)
        {
            action(EnergyLevel)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Energy Level Entries';
                RunObject = Page "Employee Energy Level Entries";
                ToolTip = 'Edit Energy Level Entries';
            }
        }
    }
}
