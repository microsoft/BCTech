namespace SalesValidationAgent.Interaction;

/// <summary>
/// A simple dialog page that prompts the user to select a shipment date
/// for the Sales Validation Agent to process open sales orders.
/// </summary>
page 50102 "Sales Val. Agent Date Picker"
{
    PageType = StandardDialog;
    Caption = 'Select Shipment Date';

    layout
    {
        area(Content)
        {
            field(ShipmentDate; ShipmentDate)
            {
                Caption = 'Shipment Date';
                ToolTip = 'Specify the shipment date to validate sales orders for.';
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
