pageextension 50102 NrgTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addfirst(embedding)
        {
#pragma warning disable AS0125
            //The XLIFF translation ID changed. This will break the translations provided by dependent extensions for your extension.
            action(EnergyLevel)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Energy Level Entries';
                RunObject = Page "Employee Energy Level Entries";
                ToolTip = 'Edit Energy Level Entries';
            }
#pragma warning restore AS0125
        }
    }
}
