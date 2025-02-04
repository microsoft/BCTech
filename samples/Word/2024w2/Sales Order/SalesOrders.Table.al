table 52800 "Sales Orders"
{
    Permissions =;
    Caption = 'Sales orders';

    fields
    {
        field(1; Id; Code[20])
        {
            Caption = 'ID';
        }
        field(2; Description; Text[256])
        {
            Caption = 'Description';
        }
    }

    keys
    {
        key(Key1; Id)
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
    }
}

