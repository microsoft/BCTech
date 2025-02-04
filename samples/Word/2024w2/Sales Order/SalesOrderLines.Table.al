table 52801 "Sales Order Lines"
{
    Permissions =;
    Caption = 'Sales order lines';
    fields
    {
        field(1; Id; Code[10])
        {
            Caption = 'ID';
        }
        field(2; Description; Text[256])
        {
            Caption = 'Description';
        }
        field(3; Quantity; Decimal)
        {
            Caption = 'Quantity';
        }
        field(4; Amount; Decimal)
        {
            Caption = 'Amount';
        }
        field(5; LineNo; Integer)
        {
            Caption = 'Line No.';
        }
        field(6; ParentId; Code[20])
        {
            Caption = 'Parent ID';
        }
        field(7; FieldToHide; Decimal)
        {
            Caption = 'Field To Hide';
        }
        field(8; RowToHide; Decimal)
        {
            Caption = 'Row To Hide';
        }
        field(9; ColumnToHide; Text[30])
        {
            Caption = 'Column To Hide';
        }
    }

    keys
    {
        key(Key1; ParentId, Id)
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
    }
}

