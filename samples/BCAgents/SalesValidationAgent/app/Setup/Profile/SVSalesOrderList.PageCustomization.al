namespace SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.Document;

pagecustomization SVSalesOrderList customizes "Sales Order List"
{
    ClearLayout = true;
    ClearActions = true;
    ClearViews = true;

    layout
    {
        modify("No.")
        {
            Visible = true;
        }
        modify("Sell-to Customer No.")
        {
            Visible = true;
        }
        modify("Sell-to Customer Name")
        {
            Visible = true;
        }
        modify("Location Code")
        {
            Visible = true;
        }
        modify(Status)
        {
            Visible = true;
        }
        modify("Shipment Date")
        {
            Visible = true;
        }
        modify("Completely Shipped")
        {
            Visible = true;
        }
    }
    actions
    {
        modify(Release)
        {
            Visible = true;
        }
        modify(Reopen)
        {
            Visible = true;
        }
        modify(SalesOrderStatistics)
        {
            Visible = true;
        }
    }
}
