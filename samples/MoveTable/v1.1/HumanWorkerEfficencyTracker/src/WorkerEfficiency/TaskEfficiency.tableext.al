#pragma warning disable AL0801
// Table 'Task Entry' is marked to be moved. Reason: moved to separate Task Tracker app.. Tag: 1.1.0.0.
tableextension 50111 TaskEfficiency extends "Task Entry"
{
#pragma warning restore AL0801
    fields
    {
        field(6; "Expected Duration"; Duration)
        {
            Editable = false;
            DataClassification = CustomerContent;
        }
        field(7; "Actual Duration"; Duration)
        {
            Editable = false;
            DataClassification = CustomerContent;
        }
        field(8; "Efficiency Score"; Integer)
        {
            Editable = false;
            DataClassification = CustomerContent;
        }
    }
}