pageextension 50100 EffTrackerRC extends "Order Processor Role Center"
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
            action(CoffeeConsumption)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Coffee Consumption Entries';
                RunObject = Page "Coffee Consumption Entries";
                ToolTip = 'Edit Coffee Consumption Entries';
            }
            action(EnergyLevel)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Energy Level Entries';
                RunObject = Page "Employee Energy Level Entries";
                ToolTip = 'Edit Energy Level Entries';
            }
            action(TaskEntries)
            {
                ApplicationArea = All;
                Image = "8ball";
                Caption = 'Task Entries';
                RunObject = Page "Task Entries";
                ToolTip = 'Edit Task Entries';
            }
        }
        // Add changes to page actions here
        modify(SalesOrders)
        {
            Visible = false;
        }
        modify(Customers)
        {
            Visible = false;
        }
        modify(SalesOrdersShptNotInv)
        {
            Visible = false;
        }
        modify(SalesOrdersComplShtNotInv)
        {
            Visible = false;
        }
        modify(Items)
        {
            Visible = false;
        }
        modify("Item Journals")
        {
            Visible = false;
        }
        modify(SalesJournals)
        {
            Visible = false;
        }
        modify(CashReceiptJournals)
        {
            Visible = false;
        }
        modify("Transfer Orders")
        {
            Visible = false;
        }
    }
}
