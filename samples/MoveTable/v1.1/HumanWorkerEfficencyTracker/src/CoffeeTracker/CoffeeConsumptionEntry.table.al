table 50100 "Coffee Consumption Entry"
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moving to separate Coffee Tracker app.';
    ObsoleteState = PendingMove;
    ObsoleteTag = '1.1.0.0';
    MovedTo = 'e22bf1de-1ac3-4ef8-983e-945f50436d34';

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
        key(Key1; "EntryNo."
)
        {
            Clustered = true;
        }
    }
}