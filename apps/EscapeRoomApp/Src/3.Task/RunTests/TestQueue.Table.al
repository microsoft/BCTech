table 73921 "Test Queue"
{
    DataClassification = CustomerContent;
    Caption = 'Test Queue';

    fields
    {
        field(1; "Codeunit Id"; Integer)
        {
            Caption = 'Codeunit Id';
            DataClassification = CustomerContent;
        }
        field(2; Success; Boolean)
        {
            Caption = 'Success';
            DataClassification = CustomerContent;
        }
        field(3; Message; Text[2048])
        {
            Caption = 'Message';
            DataClassification = CustomerContent;
        }
    }
    keys
    {
        key(PK; "Codeunit Id")
        {
            Clustered = true;
        }
    }
}