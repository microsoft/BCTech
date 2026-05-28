namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;

/// <summary>
/// Pattern A — step 2 of 3. Mirrors the partner field onto the BC14 Customer *buffer*
/// table that cloud replication fills from the on-prem source. The field ID and SQL
/// column name MUST match the on-prem source column — replication maps by name.
/// </summary>
/// <remarks>
/// Field-naming contract:
/// <code>
///   on-prem column name == buffer field name (case-insensitive)  -> replicated
///   on-prem column name != buffer field name                      -> silently skipped
/// </code>
/// </remarks>
tableextension 50101 "Contoso BC14 Customer" extends "BC14 Customer"
{
    fields
    {
        field(50100; "Contoso Loyalty Tier Code"; Code[20])
        {
            Caption = 'Loyalty Tier Code';
            DataClassification = CustomerContent;
        }
    }
}
