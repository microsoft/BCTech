table 50103 Task
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moved to separate Task Tracker app.';
    ObsoleteState = Moved;
    ObsoleteTag = '2.0.0.0';
    MovedTo = '279534fe-6d71-4abe-81ee-ff81556ffd8c';

    fields
    {
        field(1; "TaskCode"; Code[20])
        {
            Caption = 'Task Code';
        }
        field(2; Description; Text[250])
        {
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