#pragma warning disable AS0043
// false positive: The clustered key 'Key1' has been deleted in TableExtension 'TaskDuration'. Clustered key deletions are not allowed.
tableextension 50110 TaskDuration extends Task
{
    fields
    {
#pragma warning disable AS0013, AS0125
        // The field identifier '3' is not valid. It must be within the range '[50100..50149]', which is allocated to the application, and outside the range '[50000..99999]', which is allocated to per-tenant customizations.
        // The XLIFF translation ID of the 'Property' 'ToolTip' defined in the 'Field' named 'Expected Duration' has changed from 'Table 3994797191 - Field 2655233292 - Property 1295455071' to 'TableExtension 2686566589 - Field 2655233292 - Property 1295455071'. This will break the translations provided by dependent extensions for your extension.
        field(3; "Expected Duration"; Duration)
        {
            DataClassification = CustomerContent;
            MovedFrom = '279534fe-6d71-4abe-81ee-ff81556ffd8c';
        }
#pragma warning restore AS0013
    }
}