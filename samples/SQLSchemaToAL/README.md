# SQL Schema Definition to AL

Takes an SQL schema definition and generates the appropriate files to have this as a BC extension that can have its data imported by Cloud Migration.

## Usage 

```
.\SQLSchema-To-ALExtension.ps1 [-InputSchema] <String> [[-Prefix] <String>] [[-StartId] <Int32>] [[-OutputFolder] <String>] [[-ExtensionName] <String>] [[-TablesSubfolder] <String>] [[-StartCodeunitsId] <Int32>] [-GenSQLStatsQuery] [<CommonParameters>]
```

### Parameters
- `InputSchema` : File path of the SQL schema definition. Required.
- `Prefix` : Prefix to add to the AL table definitions. Default: MSFT.
- `StartId` : Starting ID for the AL table definitions. Default: 50000.
- `OutputFolder` : Folder where files will be generated.
- `ExtensionName` : Name for the extension.
- `TablesSubfolder` : Name of the folder where AL table files will be stored on the extension.
- `StartCodeunitsId` : ID given to the codeunit object. Default: 57000.
- `GenSQLStatsQuery`: Switch to generate the SQL stats query

## Example

See an example usage on `run.ps1`:
```
.\SQLSchema-To-ALExtension.ps1 .\examples\GP-TWO-schema.sql -OutputFolder .\examples\GP-TWO\ -ExtensionName ExtendGPMigration -TablesSubfolder GPTables -GenSQLStatsQuery
```

The sample output can be seen under `examples\GP-TWO`:
- The extension is under `examples\GP-TWO\ExtendGPMigration`
- An SQL query to gather information of the imported tables is on `examples\GP-TWO\stats.sql`, which can be useful as a sanity test for the cloud migration.