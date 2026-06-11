# Contoso BC14 Loyalty Migration — Sample

This sample shows partners how to plug into the **Business Central 14
Reimplementation Tool** (`Microsoft.DataMigration.BC14Reimplementation`).

It is a single, coherent scenario: Contoso has added a small **Customer
Loyalty** feature to their on-premises BC14 database and wants the data
to flow through the BC14 cloud-migration pipeline to BC Online.

The sample demonstrates the three extension patterns described in
[`../Extending.md`](../Extending.md):

| # | Pattern                          | What it shows                                                                                                                       | Files                                                                                                                                            |
|---|----------------------------------|-------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| A | Extend an existing migrator      | Transfer the partner's custom `Loyalty Tier Code` field from the BC14 Customer buffer to the production `Customer` table via events. | `src/CustomerExtension/*`                                                                                                                        |
| B | Register a new migrator          | Add a brand-new master migrator for the partner's `Contoso Loyalty Tier` table, including the buffer table and enum extension.       | `src/LoyaltyTierMigrator/*`                                                                                                                      |
| C | Plug in a post-migration check   | Add a `BC14 Migration Validation` that asserts every customer ended up with a valid loyalty tier reference.                          | `src/Validation/*`                                                                                                                               |
| D | Plug in a post-migration action  | Add a `BC14 Post Migration Action` that assigns a default loyalty tier to any customer that didn't carry one across.                 | `src/PostAction/*`                                                                                                                               |

> This sample is **documentation only** — it is not built or shipped.
> Copy it into your own AL project, rename to your publisher, and adjust
> the object IDs to your assigned range.

## File layout

```
ContosoBC14LoyaltyMigration/
├── app.json
├── README.md
└── src/
    ├── CustomerExtension/                          # Pattern A
    │   ├── ContosoCustomerLoyalty.TableExt.al      # field on production Customer
    │   ├── ContosoBC14Customer.TableExt.al         # matching field on BC14 Customer buffer
    │   └── ContosoCustomerLoyaltySub.Codeunit.al   # subscribes to OnTransferCustomerCustomFields
    ├── LoyaltyTierMigrator/                        # Pattern B
    │   ├── ContosoLoyaltyTier.Table.al             # production table
    │   ├── ContosoLoyaltyTiers.Page.al             # production list page
    │   ├── ContosoBC14LoyaltyTier.Table.al         # cloud-side buffer table
    │   ├── ContosoLoyaltyTierMigrator.Codeunit.al  # implements "BC14 Migrator"
    │   └── ContosoSetupMigratorExt.EnumExt.al      # registers it on the Setup phase
    ├── Validation/                                 # Pattern C
    │   ├── ContosoLoyaltyValidation.Codeunit.al    # implements "BC14 Migration Validation"
    │   └── ContosoValidationExt.EnumExt.al         # registers it
    ├── PostAction/                                 # Pattern D
    │   ├── ContosoAssignDefaultTier.Codeunit.al    # implements "BC14 Post Migration Action"
    │   └── ContosoPostActionExt.EnumExt.al         # registers it
    └── Permissions/
        └── ContosoLoyalty.PermissionSet.al         # covers the partner's tables
```
