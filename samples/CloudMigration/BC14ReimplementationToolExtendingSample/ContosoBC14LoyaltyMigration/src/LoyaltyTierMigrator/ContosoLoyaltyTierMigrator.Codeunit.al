namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;

/// <summary>
/// Pattern B — step 3 of 5. The migrator codeunit. Implements <c>"BC14 Migrator"</c> and is
/// registered in <see cref="enumextension::"Contoso Setup Migrator Ext""/> on the Setup phase.
/// </summary>
/// <remarks>
/// Picking the phase to register on (this matters; intra-phase order does not):
/// <code>
///   Data shape                                       | Register on
///   -------------------------------------------------+---------------------------
///   Lookup tables referenced by master records       | BC14 Setup Migrator
///   Master data (Customer/Vendor/Item peers)         | BC14 Master Migrator
///   Open documents, journals, ledger entries         | BC14 Transaction Migrator
///   Closed / posted history                          | BC14 Historical Migrator
/// </code>
/// Standard skeleton (every shipping migrator follows this):
/// <code>
///   1. GetDisplayName()             - appears in logs and on the status pages
///   2. RegisterReplicationMappings  - tell the platform "source table X -> buffer table Y"
///   3. IsEnabled()                  - gate the migrator behind a setting or a data check
///   4. Migrate()                    - drive the per-record loop
///   5. GetRemainingPercentage()     - power the progress UI
///   6. OnRun(Rec)                   - invoked once per record by the loop
/// </code>
/// Per-record loop — why we don't call <c>"BC14 Migration Loop".RunRecordLoop</c>:
/// that codeunit is <c>Access = Internal</c>. Partner extensions roll their own loop using
/// <c>[TryFunction]</c> + the public helper <c>"BC14 Migration Error Handler".LogError(...)</c>.
/// Failures then surface on the existing <b>BC14 Migration Error Overview</b> page alongside
/// the box-product ones and obey the platform retry flow.
/// </remarks>
codeunit 50101 "Contoso Loyalty Tier Migrator" implements "BC14 Migrator"
{
    TableNo = "Contoso BC14 Loyalty Tier";

    trigger OnRun()
    begin
        MigrateLoyaltyTier(Rec);
    end;

    var
        MigratorNameLbl: Label 'Contoso Loyalty Tier Migrator';

    procedure GetDisplayName(): Text[250]
    begin
        exit(MigratorNameLbl);
    end;

    procedure RegisterReplicationMappings(CompanyName: Text)
    var
        BC14MigrationSetup: Codeunit "BC14 Migration Setup";
    begin
        // Tell the cloud-migration platform: replicate rows from the on-prem
        // "Contoso Loyalty Tier" table into our cloud-side "Contoso BC14 Loyalty Tier" buffer.
        BC14MigrationSetup.InsertPerCompanyMapping(
            CompanyName,
            Database::"Contoso Loyalty Tier",
            Database::"Contoso BC14 Loyalty Tier");
    end;

    procedure IsEnabled(): Boolean
    var
        BC14LoyaltyTier: Record "Contoso BC14 Loyalty Tier";
    begin
        // Simplest possible gate: only run if the buffer has data. Real extensions usually
        // also check a company setting (see how BC14 Customer Migrator checks the
        // "Receivables Module" toggle on BC14CompanyMigrationInfo).
        exit(not BC14LoyaltyTier.IsEmpty());
    end;

    procedure Migrate(): Boolean
    var
        BC14LoyaltyTier: Record "Contoso BC14 Loyalty Tier";
        BC14MigrationErrorHandler: Codeunit "BC14 Migration Error Handler";
        AggregateSuccess: Boolean;
    begin
        AggregateSuccess := true;
        if not BC14LoyaltyTier.FindSet() then
            exit(true);

        repeat
            if not TryMigrateLoyaltyTier(BC14LoyaltyTier) then begin
                // LogError is the same public helper the box-product migrators use; failures
                // surface on BC14 Migration Error Overview and obey the platform retry flow.
                BC14MigrationErrorHandler.LogError(
                    MigratorNameLbl,
                    Database::"Contoso BC14 Loyalty Tier",
                    BC14LoyaltyTier.TableCaption(),
                    BC14LoyaltyTier."Code",
                    Database::"Contoso Loyalty Tier",
                    GetLastErrorText(),
                    BC14LoyaltyTier.RecordId);
                ClearLastError();
                AggregateSuccess := false;
            end;
        until BC14LoyaltyTier.Next() = 0;

        exit(AggregateSuccess);
    end;

    [TryFunction]
    local procedure TryMigrateLoyaltyTier(BC14LoyaltyTier: Record "Contoso BC14 Loyalty Tier")
    begin
        MigrateLoyaltyTier(BC14LoyaltyTier);
    end;

    procedure GetRemainingPercentage(): Integer
    var
        BC14LoyaltyTier: Record "Contoso BC14 Loyalty Tier";
        BC14RecordTracker: Codeunit "BC14 Migration Record Tracker";
    begin
        exit(BC14RecordTracker.GetRemainingPercentage(
            Database::"Contoso BC14 Loyalty Tier", BC14LoyaltyTier.Count()));
    end;

    internal procedure MigrateLoyaltyTier(BC14LoyaltyTier: Record "Contoso BC14 Loyalty Tier")
    var
        LoyaltyTier: Record "Contoso Loyalty Tier";
        IsNew: Boolean;
    begin
        IsNew := not LoyaltyTier.Get(BC14LoyaltyTier."Code");
        if IsNew then begin
            LoyaltyTier.Init();
            LoyaltyTier."Code" := BC14LoyaltyTier."Code";
        end;

        // Direct assignment — source data already validated upstream in BC14.
        LoyaltyTier."Description" := BC14LoyaltyTier."Description";
        LoyaltyTier."Discount %" := BC14LoyaltyTier."Discount %";

        if IsNew then
            LoyaltyTier.Insert()
        else
            LoyaltyTier.Modify();
    end;
}
