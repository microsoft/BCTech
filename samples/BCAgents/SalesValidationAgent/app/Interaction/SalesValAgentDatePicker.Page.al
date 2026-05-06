// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Interaction;

/// <summary>
/// A simple dialog page that prompts the user to select a shipment date
/// for the Sales Validation Agent to process open sales orders.
/// </summary>
page 53608 "Sales Val. Agent Date Picker"
{
    PageType = StandardDialog;
    Caption = 'Select Shipment Date';
    InherentEntitlements = X;
    InherentPermissions = X;

    layout
    {
        area(Content)
        {
            field(ShipmentDate; ShipmentDate)
            {
                Caption = 'Shipment Date';
                ToolTip = 'Specifies the shipment date to validate sales orders for.';
                ApplicationArea = All;
            }
        }
    }

    var
        ShipmentDate: Date;

    procedure GetShipmentDate(): Date
    begin
        exit(ShipmentDate);
    end;

    procedure SetShipmentDate(NewDate: Date)
    begin
        ShipmentDate := NewDate;
    end;
}
