table 50103 Task
{
    DataClassification = CustomerContent;
    LookupPageId = "Tasks List";
    ObsoleteReason = 'moving to separate Task Tracker app.';
    ObsoleteState = PendingMove;
    ObsoleteTag = '1.1.0.0';
    MovedTo = '279534fe-6d71-4abe-81ee-ff81556ffd8c';

    fields
    {
        field(1; TaskCode; Code[20])
        {
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