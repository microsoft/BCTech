table 52802 "Sales Order Additional Info"
{
    Permissions =;
    Caption = 'Sales order additional info';
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
        field(3; ParentId; Code[10])
        {
            Caption = 'Parent ID';
        }
        field(4; Miscellaneous; Text[256])
        {
            Caption = 'Miscellaneous';
        }
    }

    keys
    {
        key(Key1; ParentId, ID)
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
    }
}

