namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;

/// <summary>
/// Pattern D registration. Adding a value here causes the BC14 migration orchestrator to
/// run the action automatically during the post-migration actions phase, before validations.
/// </summary>
enumextension 50102 "Contoso Post Action Ext" extends "BC14 Post Migration Action"
{
    value(50100; "Contoso Assign Default Tier")
    {
        Caption = 'Contoso Assign Default Loyalty Tier';
        Implementation = "BC14 Post Migration Action" = "Contoso Assign Default Tier";
    }
}
