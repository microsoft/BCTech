<#
.SYNOPSIS
    Creates views for old schema tables
.DESCRIPTION
    Creates views for old schema tables for given database based on current new schema.
    This script is useful when you want to use old schema tables in new database for integration.
    It creates views for all tables in database which have name ending with $ext.

.PARAMETER SQLServer
    Name of the SQL Server

.PARAMETER SQLDB
    Name of the SQL Database

.EXAMPLE
    CreateViewsForOldSchema.ps1 -SQLServer localhost -SQLDB BC230

.NOTES
    If Instance parameter is used, BC admin powershell module needs to be already loaded in the session.
    Is using SqlServer powershell module to interact with SQL server. If not installed, install it with command: Install-Module -Name SqlServer
#>
[CmdletBinding()]
param(
    #Name of the SQL Server
    [Parameter(Mandatory = $true)]
    $SQLServer,
    #Name of the SQL Database
    [Parameter(Mandatory = $true)]
    $SQLDB
)

Write-Host "Connecting to $SQLDB on $SQLServer" -ForegroundColor Green

Write-Host "Getting list of tables" -ForegroundColor Green
$Tables = Invoke-SqlCmd -ServerInstance $SQLServer -Database $SQLDB -Query "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME like '%`$ext'" -Encrypt Optional
foreach ($Table in $Tables) {
    $TableName = $Table.TABLE_NAME
    if ($TableName -match '^(.+)\$([a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})\$ext') {
        $SQLTableName = $Matches[1]
        Write-Host "Getting columns for table $TableName" -ForegroundColor Green
        $Columns = Invoke-SqlCmd -ServerInstance $SQLServer -Database $SQLDB -Query "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$TableName'" -Encrypt Optional
        $Apps = @{}
        
        foreach ($Column in $Columns) {
            $ColumnName = $Column.COLUMN_NAME
            if ($ColumnName -match '^(.+)\$([a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12})$') {
                Write-Host "Adding app $($Matches[2]) column $($Matches[1])"
                $AppId = $Matches[2]
                $ColumnNameClean = $Matches[1]
                if (-not $Apps.ContainsKey($AppId)) {
                    $Apps.Add($AppId, @($ColumnNameClean))
                }
                else {
                    $Apps[$AppId] += $ColumnNameClean
                }
                
            }
            else {
                Write-Host "Adding PK column $ColumnName"
                if (-not $Apps.ContainsKey('')) {
                    $Apps.Add('', @($ColumnName))
                }
                else {
                    $Apps[''] += $ColumnName
                }
            }
        }
        $ColumnsSelect = ''
        foreach ($Column in $Apps['']) {
            $ColumnsSelect += ",[$Column]"
        }<#  #>
        foreach ($App in $Apps.Keys) {
            Write-Host "$($App)"
            if ($App -ne '') {

                $NewViewName = $SQLTableName + '$' + $App
                Write-Host "Creating View [$NewViewName]" -ForegroundColor Green
                foreach ($Column in $Apps[$App]) {
                    $ColumnsSelect += ",[$Column`$$App] as [$Column]"
                }
                $ColumnsSelect = $ColumnsSelect.Trim(',')
                $SQLDrop = "if object_id('$NewViewName','v') is not null drop view [$NewViewName]"
                $SQLCreate = "Create view [$NewViewName] as select $ColumnsSelect from [$TableName]"
                Write-Host $SQLDrop -ForegroundColor Yellow
                Invoke-Sqlcmd -ServerInstance $SQLServer -Database $SQLDB -Query $SQLDrop -Encrypt Optional
                Write-Host $SQLCreate -ForegroundColor Yellow
                Invoke-Sqlcmd -ServerInstance $SQLServer -Database $SQLDB -Query $SQLCreate -Encrypt Optional
            }
        }
    }
}
