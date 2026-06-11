namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;
using Microsoft.Sales.Customer;

/// <summary>
/// Pattern D — a post-migration action. Runs after every migrator finishes and BEFORE
/// validations. Use this to *do* something to the migrated data; use a validation if you
/// only need to *check* it.
/// </summary>
/// <remarks>
/// Where the action runs in the phase order:
/// <code>
///   Setup -> Master -> Transaction -> Historical -> [Post Migration Actions] -> Validations
///                                                    ^^^^^^^^^^^^^^^^^^^^^^^^^
///                                                    this codeunit
/// </code>
/// Contract:
/// <code>
///   Requirement       | Why
///   ------------------+--------------------------------------------------------------------
///   Idempotent        | actions can be rerun on retry; filter to "work still to do"
///   Return value      | true = success; false = surface as migration error (hard failure)
///   Soft data issues  | log + return true; do not fail the action over expected gaps
/// </code>
/// </remarks>
codeunit 50103 "Contoso Assign Default Tier" implements "BC14 Post Migration Action"
{
    var
        DisplayNameLbl: Label 'Contoso Assign Default Loyalty Tier', Locked = true;
        DefaultTierCodeTok: Label 'STANDARD', Locked = true;
        AssignedTelemetryLbl: Label 'Contoso post-action: assigned default tier %1 to %2 customers.', Locked = true,
            Comment = '%1 = tier code, %2 = customer count';
        TelemetryCategoryTok: Label 'Contoso Loyalty', Locked = true;

    procedure GetDisplayName(): Text[250]
    begin
        exit(DisplayNameLbl);
    end;

    procedure IsEnabled(): Boolean
    var
        LoyaltyTier: Record "Contoso Loyalty Tier";
    begin
        // Only run if the default tier actually exists in the migrated catalog.
        // If a partner ships the action behind a setting (recommended for anything
        // user-visible), gate on that setting here instead.
        exit(LoyaltyTier.Get(DefaultTierCodeTok));
    end;

    procedure RunAction(): Boolean
    var
        Customer: Record Customer;
        AssignedCount: Integer;
    begin
        // Actions are expected to be idempotent — they can be rerun. The filter below
        // ensures only customers still missing a tier get touched.
        Customer.SetRange("Contoso Loyalty Tier Code", '');
        if not Customer.FindSet(true) then
            exit(true);

        repeat
            Customer."Contoso Loyalty Tier Code" := DefaultTierCodeTok;
            Customer.Modify();
            AssignedCount += 1;
        until Customer.Next() = 0;

        Session.LogMessage(
            'CON0003',
            StrSubstNo(AssignedTelemetryLbl, DefaultTierCodeTok, AssignedCount),
            Verbosity::Normal, DataClassification::SystemMetadata,
            TelemetryScope::ExtensionPublisher, 'Category', TelemetryCategoryTok);

        // Return false to signal failure (the orchestrator will surface it as a migration
        // error). Soft data-quality issues like "couldn't fix every customer" should not
        // fail the action — log them and return true.
        exit(true);
    end;
}
