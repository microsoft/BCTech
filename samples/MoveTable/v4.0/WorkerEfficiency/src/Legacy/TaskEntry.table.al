table 50102 "Task Entry"
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moved to separate Task Tracker app.';
    ObsoleteState = Moved;
    ObsoleteTag = '2.0.0.0';
    MovedTo = '279534fe-6d71-4abe-81ee-ff81556ffd8c';

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
    }

    keys
    {
        key(Key1; "EntryNo.")
        {
            Clustered = true;
        }
    }
}