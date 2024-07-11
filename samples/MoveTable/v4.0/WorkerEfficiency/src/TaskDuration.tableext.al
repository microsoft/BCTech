tableextension 50110 TaskDuration extends Task
{
    fields
    {
#pragma warning disable AS0013
        // The field identifier '3' is not valid. It must be within the range '[50100..50149]', which is allocated to the application, and outside the range '[50000..99999]', which is allocated to per-tenant customizations.
        field(3; "Expected Duration"; Duration)
        {
            DataClassification = CustomerContent;
            MovedFrom = '279534fe-6d71-4abe-81ee-ff81556ffd8c';
        }
#pragma warning restore AS0013
    }
}