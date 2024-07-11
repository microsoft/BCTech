table 50102 "Task Entry"
{
    DataClassification = CustomerContent;
    MovedFrom = '6b05135b-a955-449b-94cc-d1d5914a168b';

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
        field(3; TaskCode; Code[20])
        {
            TableRelation = Task.TaskCode;
        }
        field(4; "Start Datetime"; DateTime)
        {
        }
        field(5; "End Datetime"; DateTime)
        {
        }
        field(6; "Expected Duration"; Duration)
        {
            Editable = false;
            ObsoleteReason = 'move back to Human Worker Efficiency Tracker app.';
            ObsoleteState = Moved;
            ObsoleteTag = '2.0.0.0';
            MovedTo = '6b05135b-a955-449b-94cc-d1d5914a168b';
        }
        field(7; "Actual Duration"; Duration)
        {
            Editable = false;
            DataClassification = CustomerContent;
            ObsoleteReason = 'move back to Human Worker Efficiency Tracker app.';
            ObsoleteState = Moved;
            ObsoleteTag = '2.0.0.0';
            MovedTo = '6b05135b-a955-449b-94cc-d1d5914a168b';
        }
        field(8; "Efficiency Score"; Integer)
        {
            Editable = false;
            ObsoleteReason = 'move back to Human Worker Efficiency Tracker app.';
            ObsoleteState = Moved;
            ObsoleteTag = '2.0.0.0';
            MovedTo = '6b05135b-a955-449b-94cc-d1d5914a168b';
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