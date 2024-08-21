#pragma warning disable AL0801
// Table 'Task' is marked to be moved. Reason: moved to separate Task Tracker app.. Tag: 1.1.0.0.
tableextension 50110 TaskDuration extends Task
#pragma warning restore AL0801
{
    fields
    {
        // Add changes to table fields here
        field(3; "Expected Duration"; Duration)
        {
            DataClassification = CustomerContent;
        }
    }
}