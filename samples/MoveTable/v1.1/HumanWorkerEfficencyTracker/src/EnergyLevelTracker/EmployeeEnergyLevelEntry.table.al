table 50101 "Employee Energy Level Entry"
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moving to separate Energy Level Tracker app.';
    ObsoleteState = PendingMove;
    ObsoleteTag = '1.1.0.0';
    MovedTo = '1f9ab25f-1aaf-42dc-81cc-facfde1f942f';

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
        key(Key1; "EntryNo."
)
        {
            Clustered = true;
        }
    }
}