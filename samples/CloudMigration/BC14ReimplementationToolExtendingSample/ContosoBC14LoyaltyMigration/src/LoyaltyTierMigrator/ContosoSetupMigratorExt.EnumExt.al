namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;

/// <summary>
/// Pattern B — step 4 of 5. Registers the migrator on the <b>Setup</b> phase. The platform
/// auto-discovers every value on this enum, so there is no central list to edit.
/// </summary>
/// <remarks>
/// We chose the Setup phase because customer records reference
/// <c>"Contoso Loyalty Tier Code"</c> — the lookup must exist before the Master phase runs.
/// Within a phase, execution order is the order migrators are added in
/// <c>BC14 Migration Runner.PopulateSetupMigrators</c>, NOT the numeric enum value.
/// </remarks>
enumextension 50100 "Contoso Setup Migrator Ext" extends "BC14 Setup Migrator"
{
    value(50100; "Contoso Loyalty Tier")
    {
        Caption = 'Contoso Loyalty Tier';
        Implementation = "BC14 Migrator" = "Contoso Loyalty Tier Migrator";
    }
}
