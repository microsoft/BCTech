tableextension 50111 TaskEfficiency extends "Task Entry"
{
    fields
    {
#pragma warning disable AS0013
        // The field identifier '6' is not valid. It must be within the range '[50100..50149]', which is allocated to the application, and outside the range '[50000..99999]', which is allocated to per-tenant customizations.
        field(6; "Expected Duration"; Duration)
        {
            Editable = false;
            DataClassification = CustomerContent;
            MovedFrom = '279534fe-6d71-4abe-81ee-ff81556ffd8c';
        }
        field(7; "Actual Duration"; Duration)
        {
            Editable = false;
            DataClassification = CustomerContent;
            MovedFrom = '279534fe-6d71-4abe-81ee-ff81556ffd8c';
        }
        field(8; "Efficiency Score"; Integer)
        {
            Editable = false;
            DataClassification = CustomerContent;
            MovedFrom = '279534fe-6d71-4abe-81ee-ff81556ffd8c';
        }
#pragma warning restore AS0013
    }
}