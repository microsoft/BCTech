// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace SalesValidationAgent.Setup.Profile;

using Microsoft.Sales.RoleCenters;

profile "SV Agent"
{
    Caption = 'Sales Validation Agent (Copilot)';
    Enabled = false;
    ProfileDescription = 'Functionality for the Sales Validation Agent to efficiently validate and process sales orders.';
    Promoted = false;
    RoleCenter = "Order Processor Role Center";
    Customizations = "SV Sales Order", "SV Sales Order Subform", "SV Sales Order Statistics", "SV Order Processor RC", "SV SO Processor Activities", "SV Sales Order List";
}