pageextension 50102 NrgTrackerRC extends "Order Processor Role Center"
{
    actions
    {
        // Add changes to page actions here
        addfirst(embedding)
        {
            action(ExpCoffeeEnergyBoostList)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Exp. Energy Boost';
                RunObject = Page "Exp. Coffee Energy Boost List";
                ToolTip = 'View SideWays List.';
            }
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
