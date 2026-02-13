// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// STEP 2: Register Replication Table Mappings
///
/// This codeunit tells the cloud migration pipeline HOW to replicate data from the
/// BC14 on-premises SQL table into the buffer table (Step 1) in BC online.
///
/// The BC14 Migration Setup codeunit publishes four integration events you can
/// subscribe to, depending on the type of data you are migrating:
///
/// ┌─────────────────────────────────────────────────────────────────────────────────┐
/// │  Event Name                              │  Use For              │ Per-Company │
/// ├──────────────────────────────────────────┼───────────────────────┼─────────────┤
/// │  OnAfterSetupReplicationMappings         │ Master / Transaction  │     Yes     │
/// │                                          │ / Historical data     │             │
/// │  OnAfterSetupReplicationTableMapping     │ Per-database tables   │     No      │
/// │  OnAfterSetupMigrationSetupMappings      │ Setup / Config data   │     Yes     │
/// │  OnAfterSetupMigrationSetupTableMapping  │ Setup per-database    │     No      │
/// └──────────────────────────────────────────┴───────────────────────┴─────────────┘
///
/// Choose the event that matches your data type:
///   - Master data (Customer, Vendor, Item, etc.)      → OnAfterSetupReplicationMappings
///   - Transaction data (G/L Entries, etc.)             → OnAfterSetupReplicationMappings
///   - Historical data (Posted Sales Invoices, etc.)    → OnAfterSetupReplicationMappings
///   - Setup data (Dimensions, Payment Terms, etc.)     → OnAfterSetupMigrationSetupMappings
///
/// SQL Table Name Convention:
///   - Source:      {CompanyName}${SourceTableName}
///   - Destination: {CompanyName}${BufferTableName}${AppId}
///   - Replace '.', '\', '/' with '_' using ConvertStr
///   - AppId is obtained from BC14MigrationProvider.GetAppId(), formatted as lowercase GUID
///
/// In this example, "Item Category" is master data, so we subscribe to
/// OnAfterSetupReplicationMappings.
/// </summary>
namespace MS.DataMigration.BC14.Examples;

using MS.DataMigration.BC14;
using Microsoft.DataMigration;

codeunit 50200 "BC14 Item Category Mapping"
{
    // Subscribe to the appropriate event based on your data type.
    // Item Category is master data → OnAfterSetupReplicationMappings (per-company).
    //
    // If you were migrating a setup table (e.g. a custom configuration table), use:
    //   [EventSubscriber(ObjectType::Codeunit, Codeunit::"BC14 Migration Setup",'OnAfterSetupMigrationSetupMappings', '', false, false)]
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"BC14 Migration Setup", 'OnAfterSetupReplicationMappings', '', false, false)]
    local procedure AddItemCategoryMapping(CompanyName: Text[30])
    var
        ReplicationMapping: Record "Replication Table Mapping";
        BC14MigrationProvider: Codeunit "BC14 Migration Provider";
        SourceSqlName: Text[128];
        DestinationSqlName: Text[250];
        AppIdSuffix: Text[50];
    begin
        // Build the source SQL table name: "{CompanyName}$Item Category"
        // ConvertStr replaces invalid SQL characters ('.', '\', '/') with '_'
        SourceSqlName := ConvertStr(CompanyName + '$Item Category', '.\/', '___');

        // Build the destination SQL table name: "{CompanyName}$BC14 Item Category${AppId}"
        // The AppId suffix is required because buffer tables live in an extension namespace
        AppIdSuffix := Format(BC14MigrationProvider.GetAppId()).TrimEnd('}').TrimStart('{').ToLower();
        DestinationSqlName := ConvertStr(CompanyName + '$BC14 Item Category$' + AppIdSuffix, '.\/', '___');

        // Insert the mapping record that the replication pipeline will use
        ReplicationMapping.Init();
        ReplicationMapping."Source Sql Table Name" := CopyStr(SourceSqlName, 1, 128);
        ReplicationMapping."Destination Sql Table Name" := CopyStr(DestinationSqlName, 1, 128);
        ReplicationMapping."Company Name" := CompanyName;
        ReplicationMapping."Table Name" := 'BC14 Item Category';
        ReplicationMapping."Preserve Cloud Data" := false; // Set to true if you want to keep existing cloud data
        if ReplicationMapping.Insert(true) then;
    end;
}
