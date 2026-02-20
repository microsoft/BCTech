namespace ThirdPartyPublisher.SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.RoleCenters;


pagecustomization SVOrderProcessorRC customizes "Order Processor Role Center"
{
    ClearLayout = true;
    ClearActions = true;

    layout
    {
        modify(Control1901851508)
        {
            Visible = true;
        }
    }
    actions
    {
        modify("Sales Orders")
        {
            Visible = true;
        }
    }
}
