namespace SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.Document;

pagecustomization SVSalesOrderStatistics customizes "Sales Order Statistics"
{
    ClearLayout = true;
    ClearActions = true;

    layout
    {
        modify(General)
        {
            Visible = true;
        }
        modify("Reserved From Stock")
        {
            Visible = true;
        }
    }
}
