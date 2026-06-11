namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;
using Microsoft.Sales.Customer;

/// <summary>
/// Pattern C — a post-migration validation. Runs after every migrator finishes and reports
/// cross-cutting integrity issues (orphan references, missing values, balance mismatches).
/// </summary>
/// <remarks>
/// Validation vs. action vs. cloud-migration warning — pick the smallest one that fits:
/// <code>
///   Need                                              | Use
///   --------------------------------------------------+----------------------------------------
///   Only check state, log/telemetry on failure        | "BC14 Migration Validation" (this)
///   Mutate data after migration (post journal, etc.)  | "BC14 Post Migration Action"
///   Surface a yellow banner on the platform UI        | "Cloud Migration Warning" interface
/// </code>
/// Read-only by contract: validations must not mutate migrated data. Use telemetry / the
/// error-handler logger for anything that needs to reach the user.
/// </remarks>
codeunit 50102 "Contoso Loyalty Validation" implements "BC14 Migration Validation"
{
    var
        DisplayNameLbl: Label 'Contoso Loyalty Tier Validation', Locked = true;
        MissingTierTelemetryLbl: Label 'Contoso loyalty validation: %1 customers are missing a loyalty tier code after migration.', Locked = true,
            Comment = '%1 = count of customers with empty Loyalty Tier Code';
        OrphanTierTelemetryLbl: Label 'Contoso loyalty validation: customer %1 references unknown tier %2.', Locked = true,
            Comment = '%1 = customer no., %2 = referenced tier code';
        TelemetryCategoryTok: Label 'Contoso Loyalty', Locked = true;

    procedure GetDisplayName(): Text[250]
    begin
        exit(DisplayNameLbl);
    end;

    procedure IsEnabled(): Boolean
    var
        LoyaltyTier: Record "Contoso Loyalty Tier";
    begin
        // Skip the check on tenants that don't use the loyalty feature at all.
        exit(not LoyaltyTier.IsEmpty());
    end;

    procedure Execute()
    var
        Customer: Record Customer;
        LoyaltyTier: Record "Contoso Loyalty Tier";
        MissingCount: Integer;
    begin
        Customer.SetLoadFields("No.", "Contoso Loyalty Tier Code");
        if not Customer.FindSet() then
            exit;

        repeat
            if Customer."Contoso Loyalty Tier Code" = '' then
                MissingCount += 1
            else
                if not LoyaltyTier.Get(Customer."Contoso Loyalty Tier Code") then
                    Session.LogMessage(
                        'CON0001',
                        StrSubstNo(OrphanTierTelemetryLbl, Customer."No.", Customer."Contoso Loyalty Tier Code"),
                        Verbosity::Warning, DataClassification::OrganizationIdentifiableInformation,
                        TelemetryScope::ExtensionPublisher, 'Category', TelemetryCategoryTok);
        until Customer.Next() = 0;

        if MissingCount > 0 then
            Session.LogMessage(
                'CON0002',
                StrSubstNo(MissingTierTelemetryLbl, MissingCount),
                Verbosity::Warning, DataClassification::SystemMetadata,
                TelemetryScope::ExtensionPublisher, 'Category', TelemetryCategoryTok);
    end;
}
