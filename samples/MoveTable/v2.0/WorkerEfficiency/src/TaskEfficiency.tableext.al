#pragma warning disable AS0043
// false positive The clustered key 'Key1' has been deleted in TableExtension 'TaskEfficiency'. Clustered key deletions are not allowed.
tableextension 50111 TaskEfficiency extends "Task Entry"
{
    fields
    {
#pragma warning disable AS0013, AS0125
        // The field identifier '6' is not valid. It must be within the range '[50100..50149]', which is allocated to the application, and outside the range '[50000..99999]', which is allocated to per-tenant customizations.
        // The XLIFF translation ID of the 'Property' 'ToolTip' defined in the 'Field' named 'Expected Duration' has changed from 'Table 2603496267 - Field 2655233292 - Property 1295455071' to 'TableExtension 2618399574 - Field 2655233292 - Property 1295455071'. This will break the translations provided by dependent extensions for your extension.
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