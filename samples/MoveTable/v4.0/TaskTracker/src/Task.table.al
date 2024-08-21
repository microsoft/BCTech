table 50103 Task
{
    DataClassification = CustomerContent;
    LookupPageId = "Tasks List";
    MovedFrom = '6b05135b-a955-449b-94cc-d1d5914a168b';

    fields
    {
        field(1; TaskCode; Code[20])
        {
        }
        field(2; Description; Text[250])
        {
        }
        field(3; "Expected Duration"; Duration)
        {
            DataClassification = CustomerContent;
            ObsoleteReason = 'move back to Human Worker Efficiency Tracker app.';
            ObsoleteState = Moved;
            ObsoleteTag = '2.0.0.0';
            MovedTo = '6b05135b-a955-449b-94cc-d1d5914a168b';
        }
    }

    keys
    {
        key(Key1; TaskCode)
        {
            Clustered = true;
        }
    }
}