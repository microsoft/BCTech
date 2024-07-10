pageextension 50103 CoffeeTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addfirst(embedding)
        {
#pragma warning disable AS0125
            //The XLIFF translation ID changed. This will break the translations provided by dependent extensions for your extension.
            action(ExpCoffeeEnergyBoostList)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Exp. Energy Boost';
                RunObject = Page "Exp. Coffee Energy Boost List";
                ToolTip = 'View SideWays List.';
            }
            action(CoffeeConsumption)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Coffee Consumption Entries';
                RunObject = Page "Coffee Consumption Entries";
                ToolTip = 'Edit Coffee Consumption Entries';
            }
#pragma warning restore AS0125
        }
    }
}
