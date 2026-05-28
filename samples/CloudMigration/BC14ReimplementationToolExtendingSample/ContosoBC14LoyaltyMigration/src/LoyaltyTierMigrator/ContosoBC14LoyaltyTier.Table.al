namespace Contoso.Loyalty;

/// <summary>
/// Pattern B — step 2 of 5. Cloud-side *buffer* table that the migration platform fills
/// from the on-prem source table of the same SQL name. The migrator codeunit later reads
/// each row and writes a record into the production table.
/// </summary>
/// <remarks>
/// Buffer-table contract:
/// <code>
///   Property                         | Required value         | Why
///   ---------------------------------+------------------------+----------------------------
///   field IDs + SQL column names     | match source exactly   | replication maps by name
///   ReplicateData                    | false                  | not replicated to satellites
///   Extensible                       | false                  | buffer is migration-internal
///   InherentEntitlements             | X                      | accessible to migration UI
///   InherentPermissions              | RIMDX                  | migrator writes during fill
///   DataClassification               | mirror production      | telemetry compliance
/// </code>
/// Tip: do not hand-write buffer tables for wide entities. The BCTech
/// GenerateALTablesFromSQLSchema PowerShell script generates them from your BC14 SQL schema:
/// https://github.com/microsoft/BCTech/tree/master/samples/CloudMigration/GenerateALTablesFromSQLSchema
/// </remarks>
table 50101 "Contoso BC14 Loyalty Tier"
{
    Caption = 'Loyalty Tier Migration Data';
    DataClassification = CustomerContent;
    ReplicateData = false;
    Extensible = false;
    InherentEntitlements = X;
    InherentPermissions = RIMDX;

    fields
    {
        field(1; "Code"; Code[20]) { Caption = 'Code'; }
        field(2; "Description"; Text[100]) { Caption = 'Description'; }
        field(3; "Discount %"; Decimal) { Caption = 'Discount %'; DecimalPlaces = 0 : 2; }
    }

    keys
    {
        key(PK; "Code") { Clustered = true; }
    }
}
