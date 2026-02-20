namespace SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.Document;

pagecustomization SVSalesOrderSubform customizes "Sales Order Subform"
{
    ClearLayout = true;
    ClearActions = true;

    layout
    {
        modify(Type)
        {
            Visible = true;
        }
        modify("No.")
        {
            Visible = true;
        }
        modify(Description)
        {
            Visible = true;
        }
        modify("Location Code")
        {
            Visible = true;
        }
        modify(Quantity)
        {
            Visible = true;
        }
        modify("Reserved Quantity")
        {
            Visible = true;
        }
        modify("Shipment Date")
        {
            Visible = true;
        }
    }
}
