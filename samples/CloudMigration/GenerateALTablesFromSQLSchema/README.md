# SQL Schema Definition to AL

Takes an SQL schema definition (as scripted by SSMS **Script Table as > CREATE To**) and generates the appropriate files to have this as a BC extension that can have its data imported by Cloud Migration.

Works with **NAV/Business Central on-premises** schemas, where object names contain spaces and `$` (for example `[dbo].[CRONUS Danmark A_S$Vendor]`), as well as with **Dynamics GP** schemas.

## Usage

```
.\SQLSchema-To-ALExtension.ps1 [-InputSchema] <String> [[-Prefix] <String>] [[-StartId] <Int32>] [[-OutputFolder] <String>] [[-ExtensionName] <String>] [[-TablesSubfolder] <String>] [[-StartCodeunitsId] <Int32>] [[-CompanyName] <String>] [-NoStripCompanyName] [-GenSQLStatsQuery] [<CommonParameters>]
```

### Parameters
- `InputSchema` : File path of the SQL schema definition. Required.
- `Prefix` : Prefix to add to the AL table definitions. Default: MSFT.
- `StartId` : Starting ID for the AL table definitions. Default: 50000.
- `OutputFolder` : Folder where files will be generated.
- `ExtensionName` : Name for the extension. When supplied, the subfolder is recreated on every run.
- `TablesSubfolder` : Name of the folder where AL table files will be stored on the extension.
- `StartCodeunitsId` : ID given to the codeunit object. Default: 57000.
- `CompanyName` : Company name to strip from `<Company>$<Table>` style NAV/BC table names. When omitted, the company name is auto-detected as the text before the first `$`.
- `NoStripCompanyName` : Keep the full SQL table name, do not strip a `<Company>$` prefix.
- `GenSQLStatsQuery`: Switch to generate the SQL stats query

## Example

```
.\SQLSchema-To-ALExtension.ps1 .\path-to-sql-schema.sql -OutputFolder .\path-output-folder\ -ExtensionName YourExtensionName -TablesSubfolder GPTables -GenSQLStatsQuery
```

See an extension created with this script in `PTEExample`.

## NAV / Business Central notes

### Company name in table names

NAV/BC company-specific tables are named `<Company>$<Table>`, for example `CRONUS Danmark A_S$Vendor`. By default the script strips the company part, so the generated object is `MSFTVendor` and the mapping codeunit maps it to the source table name `Vendor`. Use `-CompanyName` to strip an explicit company name, or `-NoStripCompanyName` to keep the full name.

A trailing `$<app guid>` (used by extension tables) is removed as well.

### Columns that are skipped

- The `timestamp` / `rowversion` column that exists on every NAV/BC table.
- The platform-managed system columns `$systemId`, `$systemCreatedAt`, `$systemCreatedBy`, `$systemModifiedAt` and `$systemModifiedBy`. These are not valid AL field names, and the platform maintains them on the target table.

Tables without a usable primary key are skipped, as are key columns that map to a field the script cannot generate (for example a BLOB).

### AL object names

AL application object names are limited to 30 characters. When `<Prefix><TableName>` is longer, the script shortens it and, if needed, appends a number to keep it unique. The **source table name in the mapping codeunit is not changed**, so the Cloud Migration mapping still points at the correct SQL table. The script prints the name it used.

### Type mapping caveats

The generated tables are *buffer* tables meant to receive the raw data, so the mapping is intentionally conservative. Review them before adding business logic:

| SQL type | Generated AL type | Note |
|---|---|---|
| `nvarchar(n)` | `Text[n]` | In BC, `Code` and `Text` fields are both `nvarchar`. The distinction cannot be recovered from the SQL schema, so `Text` is used. Change it to `Code[n]` where the source field is a `Code` field. |
| `varchar(n)` | `Text[n]` | In a NAV/BC schema a `varchar` column is almost always a `DateFormula` field — see the note below. In a GP schema it is an ordinary string column. |
| `nvarchar(max)` | `Blob` | |
| `int` | `Integer` | BC stores `Option`/`Enum` fields as `int`. Convert the ordinal in your migration code. |
| `tinyint` | `Boolean` | BC stores `Boolean` as `tinyint`. |
| `decimal` / `numeric` / `money` / `float` | `Decimal` | |
| `uniqueidentifier` | `Guid` | |
| `datetime` / `datetime2` | `DateTime` | |
| `date` | `Date` | |
| `time` | `Time` | |
| `image` / `varbinary` | `Blob` | |

`Code` vs `Text` and `Option`/`Enum` vs `Integer` are both stored identically in SQL (`nvarchar(n)` and `int` respectively), so replication into the buffer table is unaffected — the conversion only matters when you copy from the buffer into the production record.

`DateFormula` is the exception. BC stores it as `varchar`, not `nvarchar`, so the generated `Text[n]` field is the one case where the buffer column's SQL type does not match the source. Review `varchar` columns individually: in a NAV/BC schema they are almost always `DateFormula` (for example `Lead Time Calculation` on `Vendor`). Change the field to `DateFormula`, or evaluate the text in your migration code.
