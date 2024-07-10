table 50101 "Employee Energy Level Entry"
{
    DataClassification = CustomerContent;

    fields
    {
        field(1; "EntryNo."; BigInteger)
        {
            AutoIncrement = true;
            Caption = 'Entry No.';
        }
        field(2; "Employee No."; Code[20])
        {
            TableRelation = Employee."No.";
        }
        field(3; "Registered Date"; Date)
        {

        }
        field(4; "Time Of Day"; Enum "Time of Day")
        {
            InitValue = Morning;
        }
        field(5; "Energy Level"; Enum "Employee Energy Level")
        {
            InitValue = Balanced;
        }
    }

    keys
    {
        key(Key1; "EntryNo.")
        {
            Clustered = true;
        }
    }
}