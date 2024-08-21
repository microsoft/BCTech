table 50103 Task
{
    DataClassification = CustomerContent;
    LookupPageId = "Tasks List";

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