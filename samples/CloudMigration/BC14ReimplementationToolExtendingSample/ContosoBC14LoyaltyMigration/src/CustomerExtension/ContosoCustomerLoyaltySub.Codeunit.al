namespace Contoso.Loyalty;

using Microsoft.DataMigration.BC14Reimplementation;
using Microsoft.Sales.Customer;

/// <summary>
/// Pattern A — step 3 of 3. Subscribes to the BC14 Customer Migrator's per-record event
/// and copies the partner field from the buffer record to the production record.
/// </summary>
/// <remarks>
/// Which event should I subscribe to?
/// <code>
///   Need                                              | Event to subscribe to
///   --------------------------------------------------+--------------------------------------
///   Copy additional field values only                 | OnTransferCustomerCustomFields
///   Touch related records (history, side-tables, ...) | OnAfterMigrateCustomer
///   Replace the entire transfer for this entity       | OnMigrateCustomer (sets Handled)
/// </code>
/// Always use direct field assignment (no Validate) — the source data was already
/// validated upstream in BC14 and re-validation would mutate it.
/// </remarks>
codeunit 50100 "Contoso Customer Loyalty Sub"
{
    Access = Internal;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"BC14 Customer Migrator", 'OnTransferCustomerCustomFields', '', false, false)]
    local procedure OnTransferCustomerCustomFields(BC14Customer: Record "BC14 Customer"; var Customer: Record Customer)
    begin
        // Direct field assignment (no Validate) is the migration convention — the source
        // data has already been validated in BC14 and re-validating would mutate it.
        Customer."Contoso Loyalty Tier Code" := BC14Customer."Contoso Loyalty Tier Code";
    end;
}
