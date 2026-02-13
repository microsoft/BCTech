// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// STEP 3: Implement the Migrator Codeunit
///
/// This codeunit contains the actual data transformation logic. It reads from the
/// buffer table (Step 1) and writes into the standard BC online table.
///
/// You must implement the interface that matches your data type:
///   - "IMasterMigrator"      → for master data (requires: GetName, IsEnabled, Migrate,
///                               RetryFailedRecords, GetRecordCount)
///   - "ISetupMigrator"       → for setup data (requires: GetName, IsEnabled, Migrate)
///   - "ITransactionMigrator" → for transaction data (requires: GetName, IsEnabled, Migrate,
///                               RetryFailedRecords, GetRecordCount)
///   - "IHistoricalMigrator"  → for historical data (requires: GetName, IsEnabled, Migrate,
///                               RetryFailedRecords, GetRecordCount)
///
/// Key patterns used in this example:
///   - TryFunction pattern: wraps each record migration in a TryFunction so that one
///     record's failure does not abort the entire batch.
///   - Error logging: uses "BC14 Migration Error Handler" to log failures with source
///     table ID, record key, and error text for later review and retry.
///   - StopOnFirstError: when true, exits immediately on first failure; when false,
///     continues to process all records and collects all errors.
///   - RetryFailedRecords: re-processes records previously logged as errors, supporting
///     the retry workflow from the Migration Error Overview page.
///
/// In this example, Item Category is master data → implements "IMasterMigrator".
/// </summary>
namespace MS.DataMigration.BC14.Examples;

using MS.DataMigration.BC14;
using Microsoft.Inventory.Item;

codeunit 50201 "BC14 Item Category Migrator" implements "IMasterMigrator"
{
    var
        MigratorNameLbl: Label 'Item Category Migrator';

    procedure GetName(): Text[250]
    begin
        exit(MigratorNameLbl);
    end;

    procedure IsEnabled(): Boolean
    begin
        exit(GetRecordCount() > 0);
    end;

    procedure Migrate(StopOnFirstError: Boolean): Boolean
    var
        BC14ItemCategory: Record "BC14 Item Category";
        BC14MigrationErrorHandler: Codeunit "BC14 Migration Error Handler";
        Success: Boolean;
    begin
        Success := true;

        if not BC14ItemCategory.FindSet() then
            exit(true);

        repeat
            if not TryMigrateItemCategory(BC14ItemCategory) then begin
                BC14MigrationErrorHandler.LogError(
                    GetName(),
                    Database::"BC14 Item Category",
                    'BC14 Item Category',
                    BC14ItemCategory.Code,
                    Database::"Item Category",
                    GetLastErrorText(),
                    BC14ItemCategory.RecordId);
                ClearLastError();
                Success := false;

                if StopOnFirstError then
                    exit(false);
            end;
        until BC14ItemCategory.Next() = 0;

        exit(Success);
    end;

    [TryFunction]
    local procedure TryMigrateItemCategory(BC14ItemCategory: Record "BC14 Item Category")
    begin
        MigrateItemCategory(BC14ItemCategory);
    end;

    local procedure MigrateItemCategory(BC14ItemCategory: Record "BC14 Item Category")
    var
        ItemCategory: Record "Item Category";
    begin
        if ItemCategory.Get(BC14ItemCategory.Code) then begin
            ItemCategory.Description := BC14ItemCategory.Description;
            ItemCategory."Parent Category" := BC14ItemCategory."Parent Category";
            ItemCategory.Modify(true);
        end else begin
            ItemCategory.Init();
            ItemCategory.Code := BC14ItemCategory.Code;
            ItemCategory.Description := BC14ItemCategory.Description;
            ItemCategory."Parent Category" := BC14ItemCategory."Parent Category";
            ItemCategory.Insert(true);
        end;
    end;

    procedure RetryFailedRecords(StopOnFirstError: Boolean): Boolean
    var
        BC14MigrationErrors: Record "BC14 Migration Errors";
        BC14ItemCategory: Record "BC14 Item Category";
        Success: Boolean;
    begin
        Success := true;
        BC14MigrationErrors.SetRange("Source Table ID", Database::"BC14 Item Category");
        BC14MigrationErrors.SetRange("Company Name", CompanyName());
        BC14MigrationErrors.SetRange("Scheduled For Retry", true);
        BC14MigrationErrors.SetRange("Resolved", false);

        if BC14MigrationErrors.FindSet() then
            repeat
                if BC14ItemCategory.Get(BC14MigrationErrors."Source Record Key") then
                    if TryMigrateItemCategory(BC14ItemCategory) then
                        BC14MigrationErrors.MarkAsResolved('Migrated on retry')
                    else begin
                        BC14MigrationErrors."Retry Count" += 1;
                        BC14MigrationErrors."Last Retry On" := CurrentDateTime();
                        BC14MigrationErrors."Error Message" := CopyStr(GetLastErrorText(), 1, 250);
                        BC14MigrationErrors.Modify();
                        Success := false;
                        if StopOnFirstError then
                            exit(false);
                        ClearLastError();
                    end;
            until BC14MigrationErrors.Next() = 0;

        exit(Success);
    end;

    procedure GetRecordCount(): Integer
    var
        BC14ItemCategory: Record "BC14 Item Category";
    begin
        exit(BC14ItemCategory.Count());
    end;
}
