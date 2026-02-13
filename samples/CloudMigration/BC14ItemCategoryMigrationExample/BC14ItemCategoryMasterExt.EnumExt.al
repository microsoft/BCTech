// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// STEP 4: Register the Migrator via Enum Extension
///
/// The BC14 Reimplementation Tool uses the enum-implements-interface pattern.
/// Each data type has a corresponding enum. By extending the enum and mapping
/// your value to your Migrator codeunit (Step 3), the migration framework
/// will automatically discover and execute your migrator.
///
/// Available enums to extend (choose based on data type):
///
/// ┌──────────────────────────────┬──────────────────────────────┬──────────────────────────┐
/// │  Enum Name                   │  Interface                   │  Use For                 │
/// ├──────────────────────────────┼──────────────────────────────┼──────────────────────────┤
/// │  "BC14 Master Migrator"      │  "IMasterMigrator"           │  Master data:            │
/// │                              │    GetName()                 │  Customer, Vendor, Item, │
/// │                              │    IsEnabled()               │  G/L Account, etc.       │
/// │                              │    Migrate(StopOnFirstError) │                          │
/// │                              │    RetryFailedRecords(...)   │                          │
/// │                              │    GetRecordCount()          │                          │
/// ├──────────────────────────────┼──────────────────────────────┼──────────────────────────┤
/// │  "BC14 Setup Migrator"       │  "ISetupMigrator"            │  Setup / Config data:    │
/// │                              │    GetName()                 │  Dimensions, Payment     │
/// │                              │    IsEnabled()               │  Terms, Currency, etc.   │
/// │                              │    Migrate(StopOnFirstError) │                          │
/// ├──────────────────────────────┼──────────────────────────────┼──────────────────────────┤
/// │  "BC14 Transaction Migrator" │  "ITransactionMigrator"      │  Transaction data:       │
/// │                              │    GetName()                 │  G/L Entries, Ledger     │
/// │                              │    IsEnabled()               │  Entries, etc.           │
/// │                              │    Migrate(StopOnFirstError) │                          │
/// │                              │    RetryFailedRecords(...)   │                          │
/// │                              │    GetRecordCount()          │                          │
/// ├──────────────────────────────┼──────────────────────────────┼──────────────────────────┤
/// │  "BC14 Historical Migrator"  │  "IHistoricalMigrator"       │  Historical / Posted:    │
/// │                              │    GetName()                 │  Posted Sales Invoices,  │
/// │                              │    IsEnabled()               │  Posted Purchase Orders, │
/// │                              │    Migrate(StopOnFirstError) │  etc.                    │
/// │                              │    RetryFailedRecords(...)   │                          │
/// │                              │    GetRecordCount()          │                          │
/// └──────────────────────────────┴──────────────────────────────┴──────────────────────────┘
///
/// In this example, Item Category is master data, so we extend "BC14 Master Migrator"
/// and map to our "BC14 Item Category Migrator" codeunit.
///
/// Example for other types (commented out):
///
///   // Setup data:
///   // enumextension 50201 "My Setup Ext" extends "BC14 Setup Migrator"
///   // { value(100; "My Config") { Implementation = "ISetupMigrator" = "My Config Migrator"; } }
///
///   // Transaction data:
///   // enumextension 50202 "My Trans Ext" extends "BC14 Transaction Migrator"
///   // { value(100; "My Ledger") { Implementation = "ITransactionMigrator" = "My Ledger Migrator"; } }
///
///   // Historical data:
///   // enumextension 50203 "My Hist Ext" extends "BC14 Historical Migrator"
///   // { value(100; "My Posted Doc") { Implementation = "IHistoricalMigrator" = "My Posted Doc Migrator"; } }
/// </summary>
namespace MS.DataMigration.BC14.Examples;

using MS.DataMigration.BC14;

enumextension 50200 "BC14 Item Category Master Ext" extends "BC14 Master Migrator"
{
    // The value ID (100) must be unique and within your extension's ID range.
    // The Implementation maps the interface to your Migrator codeunit (Step 3).
    value(100; "Item Category")
    {
        Implementation = "IMasterMigrator" = "BC14 Item Category Migrator";
    }
}
