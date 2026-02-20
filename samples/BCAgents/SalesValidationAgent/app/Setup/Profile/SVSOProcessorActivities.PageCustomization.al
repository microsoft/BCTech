namespace SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.RoleCenters;

pagecustomization SVSOProcessorActivities customizes "SO Processor Activities"
{
    ClearLayout = true;
    ClearActions = true;

    layout
    {
        modify("Sales Orders - Open")
        {
            Visible = true;
        }
        modify(SalesOrdersReservedFromStock)
        {
            Visible = true;
        }
    }
}
