table 50100 "Coffee Consumption Entry"
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
        field(3; "Consumed Date"; Date)
        {

        }
        field(4; "Time Of Day"; Enum "Time of Day")
        {
            InitValue = Morning;
        }
        field(5; "Number of Cups Consumed"; Integer)
        {
            MinValue = 0;
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