namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;

/// <summary>
/// Pattern C registration. Adding a value here causes the BC14 migration orchestrator to
/// pick up the validation automatically during the post-migration validation phase.
/// </summary>
enumextension 50101 "Contoso Validation Ext" extends "BC14 Migration Validation"
{
    value(50100; "Contoso Loyalty Tier")
    {
        Caption = 'Contoso Loyalty Tier';
        Implementation = "BC14 Migration Validation" = "Contoso Loyalty Validation";
    }
}
