namespace SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.RoleCenters;

profile "Sales Validation Agent"
{
    Caption = 'Sales Validation Agent (Copilot)';
    Enabled = false;
    ProfileDescription = 'Functionality for the Sales Validation Agent to efficiently validate and process sales orders.';
    Promoted = false;
    RoleCenter = "Order Processor Role Center";
    Customizations = SVOrderProcessorRC, SVSalesOrder, SVSalesOrderStatistics, SVSOProcessorActivities, SVSalesOrderList, SVSalesOrderSubform;
}
