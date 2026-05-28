# Extending the BC14 Reimplementation Tool

This guide explains how partners and customers can extend the
**Business Central 14 Reimplementation Tool** to migrate their own
fields, tables, and post-migration logic alongside the data that ships
out of the box.

A working sample that uses every pattern below lives next to this
file: [`ContosoBC14LoyaltyMigration`](./ContosoBC14LoyaltyMigration/README.md).

---

## 1. How the migration is organised

A migration run executes in fixed phases, in this order:

```
Setup → Master → Transaction → Historical → Post-Migration Actions → Validations
```

| Phase                    | Driver                                           | What lives here                                                 |
|--------------------------|--------------------------------------------------|-----------------------------------------------------------------|
| Setup                    | enum `BC14 Setup Migrator`                       | Lookups & posting groups (Dimension, Currency, Posting Setup …) |
| Master                   | enum `BC14 Master Migrator`                      | Customer, Vendor, Item, G/L Account …                           |
| Transaction              | enum `BC14 Transaction Migrator`                 | Open documents, journals, item entries …                        |
| Historical               | enum `BC14 Historical Migrator`                  | Closed / posted ledgers                                         |
| Post-Migration Actions   | enum `BC14 Post Migration Action`                | One-shot work that *consumes* migrated data (e.g. post journal) |
| Validations              | enum `BC14 Migration Validation`                 | Integrity checks across the migrated data                       |

All six enums are `Extensible = true`. Adding a value with
`Implementation = …` plugs your codeunit in automatically — there is no
central registration list to edit.

Within a phase, **execution order is the order migrators are added to
the phase list** in `BC14 Migration Runner` (`PopulateSetupMigrators`,
`PopulateMasterMigrators`, …). The numeric enum value does **not**
control order. If your migrator must run before/after a particular
built-in one, ensure cross-dependency only on data the previous phases
have produced; do not rely on intra-phase ordering.

### Data flow per migrator

```
on-prem source table  ──replication──►  BC14 <Entity> buffer table  ──Migrate()──►  production table
```

1. The on-prem source row is copied by the cloud-migration platform
   into a **buffer table** named `BC14 <Entity>` (`ReplicateData = false`,
   same field IDs and SQL column names as the source).
2. The migrator codeunit reads each buffer row and writes a
   production-side record (e.g. `Customer`, `G/L Account`).
3. The shared `BC14 Migration Loop` codeunit drives the per-record
   iteration, error logging, and retry behaviour — your codeunit only
   provides the *per-record* transformation.

---

## 2. Step-by-step walkthrough — add your first migrator

This is the linear path for someone doing this for the first time. It walks through
adding a brand-new entity (Pattern B) end-to-end. After you've done it once, the
[pattern reference](#3-extension-patterns) below is the day-to-day lookup.

If you only need to carry an extra field on an entity that already migrates, skip
straight to [Pattern A](#pattern-a--add-fields-to-an-entity-that-already-migrates).

### Step 1 — Set up your AL project

- Pick an object ID range you own (the sample uses `50100..50149`).
- In `app.json`, declare a dependency on **Business Central 14 Reimplementation Tool**
  (id `2363a2b7-1018-4976-a32a-c77338dc9f16`) and on **Intelligent Cloud Base**.
- Decide on a namespace for your code (e.g. `Contoso.Loyalty`).

See: [`./ContosoBC14LoyaltyMigration/app.json`](./ContosoBC14LoyaltyMigration/app.json).

### Step 2 — Define the destination and the buffer

You need two tables:

1. The **production** table that will live in the cloud tenant after migration
   (`Extensible = true`, normal business semantics).
2. The **buffer** table that the migration platform fills from the on-prem source
   (`ReplicateData = false`, `Extensible = false`, field IDs and SQL column names
   match the on-prem source exactly).

Don't hand-write wide buffer tables. The BCTech
[GenerateALTablesFromSQLSchema](https://github.com/microsoft/BCTech/tree/master/samples/CloudMigration/GenerateALTablesFromSQLSchema)
PowerShell script generates AL buffer definitions directly from your BC14 SQL schema.

See: [`ContosoLoyaltyTier.Table.al`](./ContosoBC14LoyaltyMigration/src/LoyaltyTierMigrator/ContosoLoyaltyTier.Table.al)
and [`ContosoBC14LoyaltyTier.Table.al`](./ContosoBC14LoyaltyMigration/src/LoyaltyTierMigrator/ContosoBC14LoyaltyTier.Table.al).

### Step 3 — Write the migrator codeunit

Implement `"BC14 Migrator"` and declare `TableNo = <your buffer table>`. The interface
has five methods plus the `OnRun(Rec)` trigger — the file-level summary on the sample
migrator codeunit lists the canonical skeleton.

Key implementation choices:

- `RegisterReplicationMappings(CompanyName)` → call
  `BC14 Migration Setup.InsertPerCompanyMapping(CompanyName, Database::<source>, Database::<buffer>)`.
- `Migrate()` → iterate the buffer, wrap each record in a `[TryFunction]`, and on
  failure call `BC14 Migration Error Handler.LogError(...)` so your errors surface on
  the existing **BC14 Migration Error Overview** page alongside box-product ones.
- The per-record transformation → **direct field assignment** (no `Validate`),
  **idempotent** (`Get` first; branch on `IsNew`), populate the production record
  fully *before* `Insert` / `Modify`.

See: [`ContosoLoyaltyTierMigrator.Codeunit.al`](./ContosoBC14LoyaltyMigration/src/LoyaltyTierMigrator/ContosoLoyaltyTierMigrator.Codeunit.al).

### Step 4 — Register the migrator on the right phase

Add one enum value pointing `Implementation = "BC14 Migrator" = <your codeunit>` to
the phase enum that matches your data dependency:

| Data shape                                       | Phase enum                       |
|--------------------------------------------------|----------------------------------|
| Lookup tables referenced by master records       | `"BC14 Setup Migrator"`          |
| Master data (Customer/Vendor/Item peers)         | `"BC14 Master Migrator"`         |
| Open documents, journals, ledger entries         | `"BC14 Transaction Migrator"`    |
| Closed / posted history                          | `"BC14 Historical Migrator"`     |

The orchestrator picks up your value automatically — there is no central
registration list to edit. Intra-phase ordering is set by
`BC14 Migration Runner.Populate<Phase>Migrators`, not by the numeric enum value.

See: [`ContosoSetupMigratorExt.EnumExt.al`](./ContosoBC14LoyaltyMigration/src/LoyaltyTierMigrator/ContosoSetupMigratorExt.EnumExt.al).

### Step 5 — Ship a permission set

Grant `RIMD` on both your buffer table and your production table. Without this the
cloud-migration UI and any end user reading the production table hit
“you do not have permission to read …” errors.

See: [`ContosoLoyalty.PermissionSet.al`](./ContosoBC14LoyaltyMigration/src/Permissions/ContosoLoyalty.PermissionSet.al).

### (Optional) Step 6 — Add a post-migration validation or action

If you need to *check* the migrated data, implement `"BC14 Migration Validation"`
and extend the matching enum. If you need to *do* something with the migrated
data, implement `"BC14 Post Migration Action"` and extend that enum instead.
These are covered in [Pattern C](#pattern-c--add-a-post-migration-validation-or-action)
below.

When you've completed steps 1–5, jump to the
[Quick checklist](#5-quick-checklist-pattern-b) and verify each item.

---

## 3. Extension patterns

There are three patterns you will need. Pick the smallest one that
solves your problem.

### Pattern A — Add fields to an entity that already migrates

Use this when the entity (Customer, Item, Ship-to Address, …) is
already on the migrator list and you only need to carry one or more
*extra fields* along with it.

1. Add the field on the **production** table (e.g. `Customer`) with a
   `tableextension`.
2. Add the same field — same ID, same SQL name — on the matching
   **BC14 buffer** table with a second `tableextension`. The platform
   replicates the column automatically as long as the names match.
3. Subscribe to the migrator's `OnTransfer<Entity>CustomFields` event
   (or `OnAfterMigrate<Entity>` if you also need to touch related
   records) and copy the value across with **direct assignment** —
   `Validate` is intentionally avoided in this codepath because the
   source data has already been validated in BC14 and re-validating
   would mutate it.

See: `src/CustomerExtension/*` in the sample.

> The existing migrators each raise a small set of integration events
> with stable shapes (`OnMigrate<Entity>` for full overrides,
> `OnTransfer<Entity>CustomFields` for additive fields,
> `OnAfterMigrate<Entity>` for follow-up work). Browse the migrator
> codeunits in the **Business Central 14 Reimplementation Tool** app
> (object designer / symbol search) for the complete list.

### Pattern B — Add a new entity (your own table)

Use this when you have your own master/setup/transactional table that
isn't part of the box product. You will add a buffer table, a migrator
codeunit, an enum extension, and a permission set.

1. **Buffer table** — `ReplicateData = false`, `Extensible = false`,
   mirroring your source table's column names exactly. Don't write
   these by hand for wide tables — the [GenerateALTablesFromSQLSchema](https://github.com/microsoft/BCTech/tree/master/samples/CloudMigration/GenerateALTablesFromSQLSchema)
   PowerShell script in BCTech generates AL buffer-table definitions
   directly from your BC14 SQL schema.
2. **Migrator codeunit** implementing `"BC14 Migrator"`:
   - `TableNo = <your buffer table>`
   - `OnRun(Rec)` invokes your single-record transformation.
   - `RegisterReplicationMappings(CompanyName)` calls
     `BC14 Migration Setup.InsertPerCompanyMapping(...)` so the
     platform knows source → buffer for this company.
   - `IsEnabled()`, `GetDisplayName()`, `Migrate()`,
     `GetRemainingPercentage()` round out the interface.
   - `Migrate()` drives the per-record loop. The box product has a
     shared helper (`BC14 Migration Loop.RunRecordLoop`) but it is
     `Access = Internal`, so partner extensions must implement the
     loop themselves. The recommended shape is: iterate the buffer,
     wrap each record in a `[TryFunction]`, and on failure call the
     public helper `BC14 Migration Error Handler.LogError(...)` — the
     same logger the box-product migrators use, so your failures
     surface on the existing **BC14 Migration Error Overview** page
     alongside theirs and obey the platform retry flow. See
     `src/LoyaltyTierMigrator/` in the sample for a worked example.
3. **Enum extension** on the matching phase enum (Setup / Master /
   Transaction / Historical) pointing
   `Implementation = "BC14 Migrator" = <your codeunit>`.

Choose the phase by data dependency:

- Anything that other entities reference by code (your loyalty tier,
  your custom dimension) → **Setup**.
- Records that depend on Customer/Vendor/Item but are themselves
  master data → **Master**.
- Open documents, journals, ledger entries → **Transaction**.
- Closed / historical entries → **Historical**.

See: `src/LoyaltyTierMigrator/*` in the sample.

### Pattern C — Add a post-migration validation or action

Use **validation** when you only need to *check* the migrated data and
report problems. Use **action** when you need to *do* something after
migration completes (post journals, run a sync, fire a workflow).

| Need                       | Interface                          | Enum to extend                  |
|----------------------------|------------------------------------|---------------------------------|
| Check state, report issues | `"BC14 Migration Validation"`      | `"BC14 Migration Validation"`   |
| Perform follow-up work     | `"BC14 Post Migration Action"`     | `"BC14 Post Migration Action"`  |

Both interfaces have the same tiny shape: `GetDisplayName`,
`IsEnabled`, and `Execute` / `RunAction`. They are picked up
automatically when registered in the enum.

For validations that should also surface in the platform's
**Cloud Migration Warning** UI (the standard yellow-banner experience),
also implement the platform interface `"Cloud Migration Warning"` and
extend `"Cloud Migration Warning Type"` — the **BC14 Balance Warning**
codeunit in the box product is a worked example.

See: `src/Validation/*` in the sample for the validation variant, and
`src/PostAction/*` for the action variant.

---

## 4. Conventions to follow

These conventions match the built-in migrators and are not optional —
deviating from them will cause real bugs at scale.

- **Use direct field assignment, not `Validate`,** when copying from
  the BC14 buffer to the production record. The source was validated
  upstream; re-validating mutates the data (clears fields,
  auto-creates side records, etc.).
- **Populate the production record completely before `Insert` or
  `Modify`.** A successful `Insert` followed by a failed `Modify`
  leaves a "zombie" record that breaks idempotency on rerun.
- **Be idempotent.** Migrators are rerun on retry. Always `Get`
  first and branch on `IsNew`; never assume an empty destination.
- **Don't write to the buffer.** Treat `BC14 <Entity>` as read-only
  after replication. The migration error / record tracker uses the
  buffer's `RecordId` for retry; mutating it corrupts that tracking.
- **Stay inside your assigned ID range.** Object IDs `46850..46999`
  belong to the box product; partner extensions must use their own
  reserved range (the sample uses `50100..50149`).
- **Match SQL column names on the buffer table.** Replication maps
  source → buffer by name; a typo silently drops the column.
- **Ship a permission set** covering your buffer table and your
  production table. Without it, the cloud-migration UI and any user
  reading the production table will hit "you do not have permission
  to read …" errors.

---

## 5. Quick checklist (Pattern B)

When adding a brand-new entity to the migration, work through this list:

- [ ] Buffer table mirrors the BC14 source table's column names and types exactly
- [ ] Buffer table sets `ReplicateData = false` and `Extensible = false`
- [ ] Migrator codeunit `implements "BC14 Migrator"` and declares `TableNo = <your buffer>`
- [ ] `RegisterReplicationMappings` calls `BC14 Migration Setup.InsertPerCompanyMapping`
- [ ] `Migrate()` wraps the per-record call in a `[TryFunction]` and routes failures through `BC14 Migration Error Handler.LogError`
- [ ] Per-record transformation uses **direct field assignment** (no `Validate`) and is **idempotent** (`Get` + branch on `IsNew`)
- [ ] Enum extension on the correct phase enum (Setup / Master / Transaction / Historical) maps `Implementation = "BC14 Migrator" = <your codeunit>`
- [ ] Permission set covers both the buffer table and the production destination table
- [ ] All object IDs sit inside your assigned ID range
- [ ] `app.json` depends on **Intelligent Cloud Base** and **Business Central 14 Reimplementation Tool**

---

## 6. Reference

The interfaces and enums you extend ship inside the **Business Central 14
Reimplementation Tool** app. Once you declare a dependency on that app in
your `app.json`, the AL compiler resolves them by name — use object
designer or symbol search to inspect the source.

| Concept                     | Object name                       |
|-----------------------------|-----------------------------------|
| Migrator interface          | `"BC14 Migrator"`                 |
| Setup phase enum            | `"BC14 Setup Migrator"`           |
| Master phase enum           | `"BC14 Master Migrator"`          |
| Transaction phase enum      | `"BC14 Transaction Migrator"`     |
| Historical phase enum       | `"BC14 Historical Migrator"`      |
| Post-migration action interface | `"BC14 Post Migration Action"` |
| Post-migration action enum  | `"BC14 Post Migration Action"`    |
| Validation interface        | `"BC14 Migration Validation"`     |
| Validation enum             | `"BC14 Migration Validation"`     |
| Mapping registration helper | codeunit `"BC14 Migration Setup"` — call `InsertPerCompanyMapping` |
| Error logger                | codeunit `"BC14 Migration Error Handler"` — call `LogError` from your `Migrate()` |
| Progress helper             | codeunit `"BC14 Migration Record Tracker"` — call `GetRemainingPercentage` |
| Shared per-record loop      | codeunit `"BC14 Migration Loop"` (`Access = Internal`; partners roll their own loop) |
