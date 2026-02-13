# Extending the BC14 Reimplementation Tool

This example demonstrates how partners can extend the **BC14 Reimplementation Tool** to migrate additional tables from BC14 on-premises to Business Central online. We use "Item Category" as a concrete example.

---

## Architecture Overview

The BC14 Reimplementation Tool uses an **enum-implements-interface** pattern. The migration framework defines four data categories, each with its own enum and interface:

| Data Type | Enum | Interface | Example Tables |
|-----------|------|-----------|----------------|
| **Setup** | `BC14 Setup Migrator` | `ISetupMigrator` | Dimension, Payment Terms, Currency |
| **Master** | `BC14 Master Migrator` | `IMasterMigrator` | Customer, Vendor, Item, G/L Account |
| **Transaction** | `BC14 Transaction Migrator` | `ITransactionMigrator` | G/L Entries |
| **Historical** | `BC14 Historical Migrator` | `IHistoricalMigrator` | Posted Sales Invoices |

The migration runs in order: **Setup → Master → Transaction → Historical**.

To add your own table migration, you need **4 files** (described below).

---

## Step-by-Step Guide

### Step 1: Create a Buffer Table

**File:** `BC14ItemCategoryBuffer.Table.al`

The buffer table is a staging area that receives data replicated from the BC14 on-premises SQL database. During cloud migration, the pipeline copies rows from the BC14 source table directly into this buffer table.

**Key rule:** Mirror the source table structure — use the same field numbers, data types, and lengths as the BC14 source table so that the replication pipeline can copy data without conversion errors.

> **Tip:** You don't need to create buffer tables by hand. Use the [GenerateALTablesFromSQLSchema](https://github.com/microsoft/BCTech/tree/master/samples/CloudMigration/GenerateALTablesFromSQLSchema) PowerShell script to automatically generate AL buffer table definitions from your BC14 SQL database schema.

```al
table 50200 "BC14 Item Category"
{
    DataClassification = CustomerContent;
    ReplicateData = false;
    Extensible = false;

    fields
    {
        field(1; "Code"; Code[20]) { }
        field(2; "Parent Category"; Code[20]) { }
        field(3; Description; Text[100]) { }
    }

    keys
    {
        key(PK; "Code") { Clustered = true; }
    }
}
```

---

### Step 2: Register Replication Table Mappings

**File:** `BC14ItemCategoryMapping.Codeunit.al`

This codeunit tells the cloud migration pipeline **how** to replicate data — mapping the BC14 source SQL table name to the buffer table's SQL table name in BC online.

You subscribe to an integration event published by `Codeunit "BC14 Migration Setup"`. The event you choose depends on your data type:

| Event Name | Use For |
|------------|----------|
| `OnAfterSetupReplicationMappings` | Master / Transaction / Historical data (per-company) |
| `OnAfterSetupReplicationTableMapping` | Per-database tables (no company prefix) |
| `OnAfterSetupMigrationSetupMappings` | Setup / Configuration data (per-company) |
| `OnAfterSetupMigrationSetupTableMapping` | Setup per-database tables |

**SQL table name convention:**
- **Source:** `{CompanyName}${SourceTableName}` (e.g. `CRONUS$Item Category`)
- **Destination:** `{CompanyName}${BufferTableName}${AppId}` (e.g. `CRONUS$BC14 Item Category$d1a8b9ba-...`)
- Use `ConvertStr(..., '.\/', '___')` to replace invalid SQL characters.
- The `AppId` is obtained from `BC14MigrationProvider.GetAppId()`.

```al
[EventSubscriber(ObjectType::Codeunit, Codeunit::"BC14 Migration Setup",
                 'OnAfterSetupReplicationMappings', '', false, false)]
local procedure AddItemCategoryMapping(CompanyName: Text[30])
var
    ReplicationMapping: Record "Replication Table Mapping";
    BC14MigrationProvider: Codeunit "BC14 Migration Provider";
begin
    // ... build source & destination SQL names, then insert mapping record
end;
```

---

### Step 3: Implement the Migrator Codeunit

**File:** `BC14ItemCategoryMigrator.Codeunit.al`

This codeunit contains the actual **data transformation logic**. It reads from the buffer table (Step 1) and writes into the standard BC online table.

Choose the interface that matches your data type:

| Interface | Required Methods | Data Type |
|-----------|-----------------|-----------|
| `IMasterMigrator` | `GetName`, `IsEnabled`, `Migrate`, `RetryFailedRecords`, `GetRecordCount` | Master data |
| `ISetupMigrator` | `GetName`, `IsEnabled`, `Migrate` | Setup data |
| `ITransactionMigrator` | `GetName`, `IsEnabled`, `Migrate`, `RetryFailedRecords`, `GetRecordCount` | Transaction data |
| `IHistoricalMigrator` | `GetName`, `IsEnabled`, `Migrate`, `RetryFailedRecords`, `GetRecordCount` | Historical data |

**Recommended patterns:**

1. **TryFunction pattern** — Wrap each record's migration in a `[TryFunction]` so that one record's failure does not abort the entire batch.

2. **Error logging** — Use `Codeunit "BC14 Migration Error Handler"` to log failures for review on the Error Overview page.

```al
codeunit 50201 "BC14 Item Category Migrator" implements "IMasterMigrator"
{
    procedure Migrate(StopOnFirstError: Boolean): Boolean
    var
        BC14ItemCategory: Record "BC14 Item Category";
    begin
        if not BC14ItemCategory.FindSet() then
            exit(true);
        repeat
            if not TryMigrateItemCategory(BC14ItemCategory) then
                // Log error, optionally stop
        until BC14ItemCategory.Next() = 0;
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
            // Update existing record
            ItemCategory.Modify(true);
        end else begin
            // Insert new record
            ItemCategory.Insert(true);
        end;
    end;
}
```

---

### Step 4: Register via Enum Extension

**File:** `BC14ItemCategoryMasterExt.EnumExt.al`

Extend the appropriate enum to register your migrator with the framework. The migration runner iterates over all enum values at runtime, so once registered, your migrator will be discovered and executed automatically.

| Your Data Type | Extend This Enum | Map To Interface |
|----------------|------------------|------------------|
| Master data | `"BC14 Master Migrator"` | `"IMasterMigrator"` |
| Setup data | `"BC14 Setup Migrator"` | `"ISetupMigrator"` |
| Transaction data | `"BC14 Transaction Migrator"` | `"ITransactionMigrator"` |
| Historical data | `"BC14 Historical Migrator"` | `"IHistoricalMigrator"` |

```al
enumextension 50200 "BC14 Item Category Master Ext" extends "BC14 Master Migrator"
{
    value(100; "Item Category")
    {
        Implementation = "IMasterMigrator" = "BC14 Item Category Migrator";
    }
}
```

---

## File Summary

```
ItemCategoryExample/
├── app.json                              # Extension manifest with dependencies
├── BC14ItemCategoryBuffer.Table.al       # Step 1: Buffer table
├── BC14ItemCategoryMapping.Codeunit.al   # Step 2: Replication mapping
├── BC14ItemCategoryMigrator.Codeunit.al  # Step 3: Migration logic
├── BC14ItemCategoryMasterExt.EnumExt.al  # Step 4: Enum registration
└── README.md                             # This file
```

## Dependencies

Your extension must declare dependencies on:

1. **BC14 Reimplementation Tool** — provides the migration framework (enums, interfaces, error handling, migration runner)
2. **Intelligent Cloud Base** — provides `Record "Replication Table Mapping"` and the cloud migration infrastructure

```json
"dependencies": [
    {
        "id": "2363a2b7-1018-4976-a32a-c77338dc9f16",
        "publisher": "Microsoft",
        "name": "BC14ReimplementationTool",
        "version": "28.0.0.0"
    },
    {
        "id": "58623bfa-0559-4bc2-ae1c-0979c29fd9e0",
        "publisher": "Microsoft",
        "name": "Intelligent Cloud Base",
        "version": "28.0.0.0"
    }
]
```

## Quick Checklist

- [ ] **Buffer table** mirrors the BC14 source table fields exactly
- [ ] **Mapping codeunit** subscribes to the correct `OnAfter...` event
- [ ] **Migrator codeunit** implements the correct interface for your data type
- [ ] **Enum extension** extends the correct enum and maps to your migrator
- [ ] **app.json** includes both required dependencies
