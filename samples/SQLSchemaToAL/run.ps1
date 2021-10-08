param (
    [switch] $GPExample
)

if($GPExample){
    if(-not( Test-Path -Path 'examples\GP-TWO\')){
        New-Item -Path 'examples\GP-TWO' -Type Directory
    }

    .\SQLSchema-To-ALExtension.ps1 .\examples\GP-TWO-schema.sql -OutputFolder .\examples\GP-TWO\ -ExtensionName ExtendGPMigration -TablesSubfolder GPTables -GenSQLStatsQuery
}