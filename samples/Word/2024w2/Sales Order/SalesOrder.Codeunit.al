codeunit 52800 "Sales Order"
{
    Subtype = Normal;

    trigger OnRun()
    begin
    end;

    procedure GenerateSalesOrderDescription(Index: Integer): Text
    var
        Descriptions: List of [Text];
    begin
        Descriptions.Add('10 units of “Eco-Friendly Bamboo Notebooks” from Green World Inc. Expected delivery by 15-Nov-2024. Payment via bank transfer. Free shipping included.');
        Descriptions.Add('Purchase of 25 “Ergonomic Office Chairs” from Comfort Seating Co. to be delivered by 22-Nov-2024. Payment via credit card.');
        Descriptions.Add('5 sets of “Wireless Noise-Canceling Headphones” from Adatum Electronics. Expected delivery date: 10-Nov-2024. Payment on delivery.');
        Descriptions.Add('8 pieces of “Handmade Ceramic Planters” from Contoso Ltd. Scheduled delivery: 12-Nov-2024. Paid with card.');
        Descriptions.Add('Purchase of 15 “Smart LED Desk Lamps” from Lightwave Solutions, expected by 20-Nov-2024. Payment via digital wallet.');

        exit(Descriptions.Get(Index));
    end;

    procedure GenerateSalesOrderLineDescription(Index: Integer): Text
    var
        Descriptions: List of [Text];
    begin
        Descriptions.Add('Sustainable Wooden Sunglasses crafted from 100% recycled materials. Available in multiple colors. UV protection lenses. Eco-friendly packaging included.');
        Descriptions.Add('Bluetooth Smart Speaker with high-fidelity sound, 10-hour battery life, voice assistant integration. Color options: black, white, red.');
        Descriptions.Add('Handmade Leather Journal with 150 pages of acid-free paper, refillable inserts. Crafted from premium, ethically sourced leather.');
        Descriptions.Add('Portable Solar Charger, compatible with smartphones and tablets. Lightweight design, waterproof casing. Ideal for travel.');
        Descriptions.Add('Organic Essential Oils Set including lavender, peppermint, and eucalyptus. 10ml bottles with droppers. Pure, therapeutic-grade oils.');

        exit(Descriptions.Get(Index));
    end;

    procedure GenerateSalesOrderAdditionalInfoDescription(Index: Integer): Text
    var
        Descriptions: List of [Text];
    begin
        Descriptions.Add('Portable Espresso Maker that brews quality coffee on the go, compact and lightweight, with a manual pump operation, perfect for travelers and coffee enthusiasts.');
        Descriptions.Add('Adjustable Standing Desk Converter with a smooth, hydraulic lift system, fits most desks, designed for improving posture and productivity during work hours.');
        Descriptions.Add('Customizable LED Strip Lights, 5 meters, with multiple color options and remote control, suitable for home decoration or gaming setups.');
        Descriptions.Add('Smart Wi-Fi Thermostat with touch screen display, energy-saving features, remote control via smartphone app, compatible with Alexa and Google Assistant.');
        Descriptions.Add('Wireless Ergonomic Mouse with a high-precision sensor, adjustable DPI settings, and a comfortable, ambidextrous design for extended use.');

        exit(Descriptions.Get(Index));
    end;

    procedure GenerateSalesOrderAdditionalInfoMiscellaneous(Index: Integer): Text
    var
        Descriptions: List of [Text];
    begin
        Descriptions.Add('On 12-Dec-2024, a shipment to Germany amounted to €2,150.');
        Descriptions.Add('');
        Descriptions.Add('A consignment to Japan was priced at ¥320,000, dated 05-Jan-2025.');
        Descriptions.Add('');
        Descriptions.Add('By 20-Nov-2024, the cost for an order to Canada was CAD 1,800.');

        exit(Descriptions.Get(Index));
    end;

    [Normal]
    procedure InitData(DataItemCount: Integer; LineItemCount: Integer; AdditionalItemCount: Integer)
    var
        DataItem: Record "Sales Orders";
        LineItem: Record "Sales Order Lines";
        AdditionalItem: Record "Sales Order Additional Info";
        i, j, k : Integer;
        PK1, PK2, PK3 : Code[20];
    begin
        // Initialize test data for the reports.
        DataItem.DeleteAll;
        LineItem.DeleteAll;
        AdditionalItem.DeleteAll;
        for i := 1 to DataItemCount do begin
            PK1 := StrSubstNo('00%1', i);
            DataItem.Init;
            DataItem.Id := PK1;
            DataItem.Description := GenerateSalesOrderDescription(i);
            DataItem.Insert;

            for j := 1 to LineItemCount do begin
                LineItem.Init;
                PK2 := StrSubstNo('100%1%2', i, j);
                LineItem.Id := PK2;
                LineItem.Description := GenerateSalesOrderLineDescription(1 + ((i + j) MOD 5));
                LineItem.Quantity := j;
                LineItem.Amount := j * 100;
                LineItem.ParentId := PK1;
                LineItem.FieldToHide := (i + j) * i * j;
                LineItem.RowToHide := j MOD 2 = 0 ? 1 : 0;
                LineItem.ColumnToHide := StrSubstNo('Column to hide %1', j);
                LineItem.Insert;
            end;

            for k := 1 to AdditionalItemCount do begin
                AdditionalItem.Init;
                PK3 := StrSubstNo('200%1%2', i, k);
                AdditionalItem.Id := PK3;
                AdditionalItem.Description := GenerateSalesOrderAdditionalInfoDescription(1 + ((i + k) MOD 5));
                AdditionalItem.ParentId := PK1;
                AdditionalItem.Miscellaneous := GenerateSalesOrderAdditionalInfoMiscellaneous(1 + ((i + k) MOD 5));
                AdditionalItem.Insert;
            end;
        end;
        Commit;
    end;

    [Normal]
    procedure InitConditionalData(DataItemCount: Integer; LineItemCount: Integer; AdditionalItemCount: Integer)
    var
        DataItem: Record "Sales Orders";
        LineItem: Record "Sales Order Lines";
        AdditionalItem: Record "Sales Order Additional Info";
        i, j, k : Integer;
        PK1, PK2, PK3 : Code[20];
    begin
        // Initialize test data for report with conditional visibility controls.
        DataItem.DeleteAll;
        LineItem.DeleteAll;
        AdditionalItem.DeleteAll;
        for i := 1 to DataItemCount do begin
            PK1 := StrSubstNo('00%1', i);
            DataItem.Init;
            DataItem.Id := PK1;
            DataItem.Description := i MOD 2 = 0 ? '' : GenerateSalesOrderDescription(i);
            DataItem.Insert;

            for j := 1 to LineItemCount do begin
                LineItem.Init;
                PK2 := StrSubstNo('100%1%2', i, j);
                LineItem.Id := PK2;
                LineItem.Description := GenerateSalesOrderLineDescription(1 + ((i + j) MOD 5));
                LineItem.Quantity := j;
                LineItem.Amount := j * 100;
                LineItem.ParentId := PK1;
                LineItem.FieldToHide := j MOD 2 = 0 ? 0 : (i + j) * i * j;
                LineItem.RowToHide := j MOD 2 = 0 ? 1 : 0;
                LineItem.ColumnToHide := '';
                LineItem.Insert;
            end;

            for k := 1 to AdditionalItemCount do begin
                if i MOD 2 <> 0 then begin
                    AdditionalItem.Init;
                    PK3 := StrSubstNo('200%1%2', i, k);
                    AdditionalItem.Id := PK3;
                    AdditionalItem.Description := GenerateSalesOrderAdditionalInfoDescription(1 + ((i + k) MOD 5));
                    AdditionalItem.ParentId := PK1;
                    AdditionalItem.Miscellaneous := GenerateSalesOrderAdditionalInfoMiscellaneous(1 + ((i + k) MOD 5));
                    AdditionalItem.Insert;
                end;
            end;
        end;
        Commit;
    end;
}