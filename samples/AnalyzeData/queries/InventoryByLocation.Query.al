#pragma warning disable AA0247
query 37015 "Inventory by Location"
{
    QueryType = Normal;
    DataAccessIntent = ReadOnly;
    UsageCategory = ReportsAndAnalysis;
    Caption = 'Inventory by Location Analysis';
    AboutTitle = 'About Inventory by Location Analysis';
    AboutText = 'The Inventory by Location Analysis is a query that joins data from item ledger entries with location master data.';

    elements
    {
        dataitem(ItemLedgerEntry; "Item Ledger Entry")
        {
            DataItemTableFilter = Open = const(true);

            column(ItemNo; "Item No.")
            {
                Caption = 'Item No.';
            }
            column(VariantCode; "Variant Code")
            {
                Caption = 'Variant Code';
            }
            column(LotNo; "Lot No.")
            {
                Caption = 'Lot No.';
            }
            column(SerialNo; "Serial No.")
            {
                Caption = 'Serial No.';
            }
            column(PackageNo; "Package No.")
            {
                Caption = 'Package No.';
            }
            column(ExpirationDate; "Expiration Date")
            {
                Caption = 'Expiration Date';
            }
            column(UnitOfMeasureCode; "Unit of Measure Code")
            {
                Caption = 'Unit of Measure Code';
            }
            column(QtyPerUnitOfMeasure; "Qty. per Unit of Measure")
            {
                Caption = 'Qty. per Unit of Measure';
            }
            column(Quantity; Quantity)
            {
                Caption = 'Quantity';
                Method = Sum;
            }
            column(RemainingQuantity; "Remaining Quantity")
            {
                Caption = 'Remaining Quantity';
                Method = Sum;
            }
            column(ReservedQuantity; "Reserved Quantity")
            {
                Caption = 'Reserved Quantity';
                Method = Sum;
            }
            column(CountryRegionCode; "Country/Region Code")
            {
                Caption = 'Country/Region Code';
            }
            dataitem(Location; Location)
            {
                DataItemLink = Code = ItemLedgerEntry."Location Code";

                column(LocationCode; Code)
                {
                    Caption = 'Location Code';
                }
                column(LocationName; Name)
                {
                    Caption = 'Location Name';
                }
                column(LocationCity; City)
                {
                    Caption = 'Location City';
                }
                column(LocationCounty; County)
                {
                    Caption = 'Location County';
                }
                column(LocationCountryRegionCode; "Country/Region Code")
                {
                    Caption = 'Location Country/Region Code';
                }
            }
        }
    }
}
