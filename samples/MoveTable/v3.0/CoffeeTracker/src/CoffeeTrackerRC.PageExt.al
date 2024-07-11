#pragma warning disable AS0031
// The action with name 'ExpCoffeeEnergyBoostList' defined in PageExtension 'CoffeeTrackerRC' was found in the previous version, but is missing in the current extension. This will break dependent extensions.
pageextension 50103 CoffeeTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addfirst(embedding)
        {
            action(CoffeeConsumption)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Coffee Consumption Entries';
                RunObject = Page "Coffee Consumption Entries";
                ToolTip = 'Edit Coffee Consumption Entries';
            }
        }
    }
}
