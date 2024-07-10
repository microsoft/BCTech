pageextension 50101 HideEmbeddingRC extends "Order Processor Role Center"
{
    actions
    {
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
