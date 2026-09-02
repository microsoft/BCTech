<#
.Description
Takes an SQL schema definition (as scripted by SSMS "Script Table as > CREATE To") and
generates the files needed for a BC extension whose data can be imported by Cloud Migration.

Works with NAV/BC on-prem schemas where object names contain spaces and '$'
(e.g. [dbo].[CRONUS Danmark A_S$Vendor]) as well as with Dynamics GP schemas.

.Parameter InputSchema
File path of the SQL schema definition
.Parameter Prefix
Prefix to add to the AL table definitions
.Parameter StartId
Starting ID for the AL table definitions
.Parameter OutputFolder
Folder where files will be generated
.Parameter ExtensionName
Name for the extension.
.Parameter TablesSubfolder
Name of the folder where AL table files will be stored on the extension.
.Parameter StartCodeunitsId
ID given to the codeunit object
.Parameter CompanyName
Company name to strip from '<Company>$<Table>' style NAV/BC table names. When omitted the
company name is auto-detected as the text before the first '$'.
.Parameter NoStripCompanyName
Keep the full SQL table name, do not strip a '<Company>$' prefix.
.Parameter GenSQLStatsQuery
Switch to generate the SQL stats query
#>
param (
    [Parameter(Mandatory = $true)][string]$InputSchema,
    [string]$Prefix = 'MSFT',
    [int]$StartId = 50000,
    [string]$OutputFolder = '',
    [string]$ExtensionName = '',
    [string]$TablesSubfolder = '',
    [int]$StartCodeunitsId = 57000,
    [string]$CompanyName = '',
    [switch]$NoStripCompanyName,
    [switch]$GenSQLStatsQuery
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path -Path $InputSchema)) {
    Write-Host "Input schema doesn't exist."
    exit 1
}
if (($OutputFolder -ne '') -and (-not (Test-Path -Path $OutputFolder))) {
    Write-Host "Output folder doesn't exist."
    exit 1
}

$extensionFolder = $OutputFolder
if ($extensionFolder -eq '') { $extensionFolder = '.' }
if (-not $extensionFolder.EndsWith('\')) {
    $extensionFolder += '\'
}
if ($ExtensionName -ne '') {
    $extensionFolder += "$ExtensionName\"
}

$tablesFolder = "${extensionFolder}$TablesSubfolder"
if (-not $tablesFolder.EndsWith('\')) {
    $tablesFolder += '\'
}

if (Test-Path $extensionFolder) {
    if ($ExtensionName -eq '') {
        # Never wipe a folder that this script did not create; -ExtensionName gives it a
        # dedicated subfolder that is safe to regenerate.
        Write-Host "Generating into the existing folder '$extensionFolder'. Pass -ExtensionName to generate into a dedicated subfolder that is recreated on every run."
    }
    else {
        Remove-Item $extensionFolder -Recurse -Force
    }
}
if (-not (Test-Path $extensionFolder)) {
    New-Item -Path $extensionFolder -Type Directory | Out-Null
}
if (-not (Test-Path $tablesFolder)) {
    New-Item -Path $tablesFolder -Type Directory | Out-Null
}

# -Raw keeps the line structure; the original cast of a string[] to [String] joined the
# lines with single spaces, which silently corrupted anything that relied on line breaks.
[String]$schema = Get-Content -Path $InputSchema -Raw

$mappingCodeunit = @"
codeunit CODEUNITID "CODEUNITNAME"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Hybrid Cloud Management", 'OnInsertDefaultTableMappings', '', false, false)]
    local procedure OnInsertDefaultTableMappings(DeleteExisting: Boolean; ProductID: Text[250])
    begin
UPDATEORINSERTMAPPINGS
    end;

    local procedure UpdateOrInsertRecord(TableID: Integer; SourceTableName: Text)
    var
        MigrationTableMapping: Record "Migration Table Mapping";
        CurrentModuleInfo:  ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(CurrentModuleInfo);
        if MigrationTableMapping.Get(CurrentModuleInfo.Id(), TableID) then
            MigrationTableMapping.Delete();

        MigrationTableMapping."App ID" := CurrentModuleInfo.Id();
        MigrationTableMapping.Validate("Table ID", TableID);
        MigrationTableMapping."Source Table Name" := SourceTableName;
        MigrationTableMapping.Insert();
    end;
}
"@

$permissionsXML = @"
<?xml version="1.0" encoding="utf-8"?>
<PermissionSets>
  <PermissionSet RoleID="EXTENSIONNAMEHERE" RoleName="EXTENSIONNAMEHERE">
PERMISSIONSHERE
  </PermissionSet>
</PermissionSets>
"@

$script:permissionsXMLList = @()
$permissionXML = @"
    <Permission>
      <ObjectType>OBJECTTYPEHERE</ObjectType>
      <ObjectID>OBJECTIDHERE</ObjectID>
      <ReadPermission>Yes</ReadPermission>
      <InsertPermission>Yes</InsertPermission>
      <ModifyPermission>Yes</ModifyPermission>
      <DeletePermission>Yes</DeletePermission>
      <ExecutePermission>Yes</ExecutePermission>
      <SecurityFilter />
    </Permission>
"@

$script:CodeunitMappings = ''
$script:SQLStatsQ = @()

# A bracketed SQL identifier may contain spaces, '$', '.', '(' ... anything except ']'.
$identifier = "(?:\[[^\]]+\]|[A-Za-z0-9_@#\$]+)"

function Remove-Brackets($v) {
    $t = "$v".Trim()
    if ($t.StartsWith('[') -and $t.EndsWith(']')) {
        return $t.Substring(1, $t.Length - 2)
    }
    return $t
}

function Get-ALTypeFromSQLType($sqlType, $len, $isKeyCandidate) {
    $t = (Remove-Brackets $sqlType).ToLower()

    # length / precision, e.g. nvarchar(50) -> '50', decimal(38, 20) -> '38'
    $first = ''
    if ($len -ne $null -and "$len".Trim() -ne '') {
        $first = "$len".Split(',')[0].Trim()
    }

    switch ($t) {
        # --- integers ---
        'int' { return 'Integer' }
        'smallint' { return 'Integer' }
        'bigint' { return 'BigInteger' }
        # NAV/BC stores Boolean as tinyint; GP uses tinyint as a small integer.
        'tinyint' { return 'Boolean' }
        'bit' { return 'Boolean' }

        # --- decimals ---
        'decimal' { return 'Decimal' }
        'numeric' { return 'Decimal' }
        'money' { return 'Decimal' }
        'smallmoney' { return 'Decimal' }
        'float' { return 'Decimal' }
        'real' { return 'Decimal' }

        # --- date / time ---
        'datetime' { return 'DateTime' }
        'datetime2' { return 'DateTime' }
        'smalldatetime' { return 'DateTime' }
        'datetimeoffset' { return 'DateTime' }
        'date' { return 'Date' }
        'time' { return 'Time' }

        # --- guid ---
        'uniqueidentifier' { return 'Guid' }

        # --- binary / blob ---
        'image' { return 'Blob' }
        'varbinary' { return 'Blob' }
        # kept as-is for backwards compatibility with already generated GP extensions
        'binary' { return 'Text[50]' }
        'xml' { return 'Blob' }
        'sql_variant' { return 'Blob' }

        # --- text ---
        { $_ -in @('nvarchar', 'varchar', 'nchar', 'char') } {
            if ($first -eq 'max') { return 'Blob' }
            if ($first -eq '') { return 'Text[2048]' }
            $n = 0
            if (-not [int]::TryParse($first, [ref]$n)) { return 'Text[250]' }
            if ($n -gt 2048) { $n = 2048 }
            if ($n -lt 1) { $n = 1 }
            return "Text[$n]"
        }
        # kept as-is for backwards compatibility with already generated GP extensions
        'text' { return 'Text[2048]' }
        'ntext' { return 'Text[2048]' }
    }

    return 'UNKNOWN'
}

function Get-CleanTableName($rawName) {
    # [dbo].[CRONUS Danmark A_S$Vendor] -> CRONUS Danmark A_S$Vendor
    $parts = [Regex]::Matches($rawName, "(?:\[[^\]]+\]|[^\.\[\]]+)")
    $name = Remove-Brackets $parts[$parts.Count - 1].Value

    if (-not $NoStripCompanyName) {
        if ($CompanyName -ne '') {
            $p = "$CompanyName`$"
            if ($name.StartsWith($p, [System.StringComparison]::OrdinalIgnoreCase)) {
                $name = $name.Substring($p.Length)
            }
        }
        elseif ($name.Contains('$')) {
            # NAV/BC company tables are named '<Company>$<Table>'
            $name = $name.Substring($name.IndexOf('$') + 1)
        }
    }

    # extension tables carry a trailing '$<app guid>'
    $name = [Regex]::Replace($name, '\$[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$', '')
    return $name.Trim()
}

function Get-ALObjectName($name) {
    # AL object names allow letters, digits, spaces, '_' and a few more; '$' and '.' are not safe.
    $clean = [Regex]::Replace($name, '[^A-Za-z0-9 _]', '_')
    return $clean.Trim()
}

$script:usedObjectNames = @{}

function Get-UniqueALObjectName($candidate) {
    # AL application object names are limited to 30 characters. Long SQL table names (very common
    # in NAV/BC, e.g. 'Item Charge Assignment (Purch)') would otherwise fail to compile with AL0305.
    # The AL object name does not have to match the SQL table name: the generated table mapping
    # codeunit maps Database::"<AL object>" to the source SQL table name explicitly.
    $maxLen = 30
    $name = $candidate
    if ($name.Length -gt $maxLen) { $name = $name.Substring(0, $maxLen).TrimEnd() }
    if (-not $script:usedObjectNames.ContainsKey($name.ToLowerInvariant())) {
        $script:usedObjectNames[$name.ToLowerInvariant()] = $true
        return $name
    }
    for ($n = 2; $n -lt 100000; $n++) {
        $suffix = "$n"
        $stem = $candidate
        if ($stem.Length -gt ($maxLen - $suffix.Length)) {
            $stem = $stem.Substring(0, $maxLen - $suffix.Length)
        }
        $cand = $stem.TrimEnd() + $suffix
        if (-not $script:usedObjectNames.ContainsKey($cand.ToLowerInvariant())) {
            $script:usedObjectNames[$cand.ToLowerInvariant()] = $true
            return $cand
        }
    }
    throw "Unable to generate a unique AL object name for '$candidate'."
}

function Split-CommaParams($tablecontent) {
    $pCount = 0
    $bCount = 0
    $current = ''
    $params = @()
    for ($i = 0; $i -lt $tablecontent.Length; $i++) {
        $c = $tablecontent[$i]
        if (($c -eq ',') -and ($pCount -eq 0) -and ($bCount -eq 0)) {
            $params += $current
            $current = ''
            continue
        }
        if ($c -eq '(') { $pCount++ }
        elseif ($c -eq '[') { $bCount++ }
        elseif ($c -eq ')') { $pCount-- }
        elseif ($c -eq ']') { $bCount-- }
        $current += $c
    }
    if ($current.Trim() -ne '') { $params += $current }
    return , $params
}

$columnRegex = [Regex]::new("^\s*(?<colid>$identifier)\s+(?<colty>$identifier)\s*(\(\s*(?<len>[^\)]*)\))?", 'IgnoreCase')
$primKeyRegex = [Regex]::new("primary\s+key[^\(]*\(\s*(?<colkeys>[^\)]+)\)", 'IgnoreCase, Singleline')
$keyColRegex = [Regex]::new("(?<c>\[[^\]]+\]|[A-Za-z0-9_@#\$]+)", 'IgnoreCase')

function ConvertTo-ALTable($tableid, $tablecontent, $tableCount) {
    $sqlTableName = Get-CleanTableName $tableid
    $objectName = Get-ALObjectName $sqlTableName
    $wantedName = "$Prefix$objectName"
    $baseName = Get-UniqueALObjectName $wantedName
    if ($baseName -ne $wantedName) {
        Write-Host "AL object name '$wantedName' is not usable (30 character limit); using '$baseName' instead. The source table mapping is unaffected."
    }
    $filename = "$baseName.Table.al"
    $id = $StartId + $tableCount

    $sb = [System.Text.StringBuilder]::new()
    [void]$sb.AppendLine("table $id `"$baseName`"")
    [void]$sb.AppendLine('{')
    [void]$sb.AppendLine('    DataClassification = CustomerContent;')
    [void]$sb.AppendLine('    fields')
    [void]$sb.AppendLine('    {')

    $keyscontent = @()
    $fieldNo = 0
    $blobFields = @()
    $emittedFields = @()

    foreach ($p in (Split-CommaParams $tablecontent)) {
        $trimmed = $p.Trim()
        if ($trimmed -eq '') { continue }

        $firstWord = ($trimmed -split '\s+')[0]
        if ($firstWord -match '^(?i)(constraint|primary|unique|check|index)$') {
            $result = $primKeyRegex.Matches($trimmed)
            if ($result.Count -eq 0) {
                # UNIQUE / CHECK / INDEX constraints are not translated
                continue
            }
            # Key columns are '<identifier> ASC|DESC' pairs; a bracketed identifier can contain
            # spaces, so it cannot be extracted by splitting on whitespace.
            foreach ($km in $keyColRegex.Matches($result[0].Groups['colkeys'].Value)) {
                $keycolname = Remove-Brackets $km.Groups['c'].Value
                if ($keycolname -match '^(?i)(asc|desc)$') { continue }
                $keyscontent += $keycolname
            }
            continue
        }

        $t = $columnRegex.Matches($trimmed)
        if ($t.Count -eq 0) {
            Write-Host "Unrecognized column definition '$trimmed'. For table $sqlTableName"
            continue
        }

        $colname = Remove-Brackets $t[0].Groups['colid'].Value
        $rawType = $t[0].Groups['colty'].Value
        $rawLen = $t[0].Groups['len'].Value

        # rowversion column exists on every NAV/BC table and must not become an AL field
        if ((Remove-Brackets $rawType).ToLower() -in @('timestamp', 'rowversion')) {
            continue
        }

        # BC system columns ($systemId, $systemCreatedAt, ...) are platform managed. They are
        # not valid AL field names and the platform provides them on the target table anyway.
        if ($colname.StartsWith('$')) {
            Write-Host "Skipping BC system column '$colname' on table $sqlTableName."
            continue
        }

        $colType = Get-ALTypeFromSQLType $rawType $rawLen $false
        if ($colType -eq 'UNKNOWN') {
            Write-Host "Unknown column type '$rawType' for column '$colname' on table $sqlTableName. Skipping field."
            continue
        }
        if ($colType -eq 'Blob') { $blobFields += $colname }
        $emittedFields += $colname

        # AL field names are limited to 30 characters. The name is not truncated automatically
        # because the migration relies on it matching the source column.
        if ($colname.Length -gt 30) {
            Write-Host "Column '$colname' on table $sqlTableName is longer than the 30 character AL field name limit. Shorten it manually and map it in the migration code."
        }

        $fieldNo++
        [void]$sb.AppendLine("        field($fieldNo; `"$colname`"; $colType)")
        [void]$sb.AppendLine('        {')
        [void]$sb.AppendLine('            DataClassification = CustomerContent;')
        [void]$sb.AppendLine('        }')
    }
    [void]$sb.AppendLine('    }')

    # A key can only reference fields that were actually emitted, and BLOB fields cannot be
    # part of a key.
    $droppedKeyCols = @($keyscontent | Where-Object { ($emittedFields -notcontains $_) -or ($blobFields -contains $_) })
    if ($droppedKeyCols.Count -gt 0) {
        Write-Host "Primary key of table $sqlTableName references unusable column(s): $($droppedKeyCols -join ', ')."
    }
    $keyscontent = @($keyscontent | Where-Object { ($emittedFields -contains $_) -and ($blobFields -notcontains $_) })

    if ($keyscontent.Count -eq 0) {
        Write-Host "Table $sqlTableName without usable primary key definition. Ignoring."
        return
    }

    $ks = ($keyscontent | ForEach-Object { "`"$_`"" }) -join ', '
    [void]$sb.AppendLine('    keys')
    [void]$sb.AppendLine('    {')
    [void]$sb.AppendLine("        key(Key1; $ks)")
    [void]$sb.AppendLine('        {')
    [void]$sb.AppendLine('            Clustered = true;')
    [void]$sb.AppendLine('        }')
    [void]$sb.AppendLine('    }')
    [void]$sb.AppendLine('}')

    $sb.ToString() | Out-File -FilePath "$tablesFolder$filename" -Encoding UTF8

    $pxml = $permissionXML -replace 'OBJECTTYPEHERE', 'TableData'
    $pxml = $pxml -replace 'OBJECTIDHERE', $id
    $script:permissionsXMLList += $pxml

    $script:CodeunitMappings = "$script:CodeunitMappings        UpdateOrInsertRecord(Database::`"$baseName`", '$sqlTableName');`n"
    $script:SQLStatsQ += "SELECT '$($sqlTableName.Replace("'", "''"))', COUNT(*) from $tableid"
}

$useDBregex = "(?i)\buse\s+(?<dbname>\[[^\]]+\]|[A-Za-z0-9_\-]+)"
$dbname = ''
if ($schema -match $useDBregex) {
    $dbname = Remove-Brackets $matches['dbname']
}

$createTableRegex = [Regex]::new("(?i)\bcreate\s+table\s+(?<tableid>$identifier(?:\s*\.\s*$identifier)*)\s*\(", 'IgnoreCase')
$result = $createTableRegex.Matches($schema)

if ($result.Count -eq 0) {
    Write-Host 'Unable to parse schema definitions'
    exit 1
}

for ($i = 0; $i -lt $result.Count; $i++) {
    $tableidValue = $result[$i].Groups['tableid'].Value
    $afterMatch = ($result[$i].Index) + ($result[$i].Length)
    $contentIndex = $afterMatch
    $parencount = 1
    $innerLen = 0
    while (($contentIndex -lt $schema.Length) -and ($parencount -gt 0)) {
        if ($schema[$contentIndex] -eq '(') { $parencount++ }
        elseif ($schema[$contentIndex] -eq ')') { $parencount-- }
        $contentIndex++
        $innerLen++
    }
    if ($parencount -gt 0) {
        Write-Host 'Unmatched parentheses after CREATE TABLE expression'
        exit 1
    }
    $tablecontent = $schema.Substring($afterMatch, $innerLen - 1)

    ConvertTo-ALTable $tableidValue $tablecontent $i
}

$cId = $StartCodeunitsId
$codeunit = $mappingCodeunit -replace 'CODEUNITID', $cId
$mappingName = Get-UniqueALObjectName "$Prefix - Default table mapping"
$codeunit = $codeunit -replace 'CODEUNITNAME', $mappingName
$codeunit = $codeunit -replace 'UPDATEORINSERTMAPPINGS', $script:CodeunitMappings
$codeunit | Out-File -FilePath "$extensionFolder${Prefix}DefaultTableMapping.Codeunit.al" -Encoding UTF8

$pxml = $permissionXML -replace 'OBJECTTYPEHERE', 'Codeunit'
$pxml = $pxml -replace 'OBJECTIDHERE', $cId
$script:permissionsXMLList += $pxml

$permissionsContent = $permissionsXML -replace 'PERMISSIONSHERE', ($script:permissionsXMLList -join "`n")
$permissionsContent -replace 'EXTENSIONNAMEHERE', $ExtensionName | Out-File -FilePath "${extensionFolder}Permissions.xml" -Encoding UTF8

if ($GenSQLStatsQuery) {
    $sqlscript = ''
    if ($dbname -ne '') { $sqlscript = "use [$dbname]`n" }
    $sqlscript = "${sqlscript}declare @stats table (tbl nvarchar(255), nrecords int);`n"
    $sqlscript = "${sqlscript}insert into @stats`n"
    $sqlscript = "$sqlscript$($script:SQLStatsQ -join "`nunion all`n");"
    $sqlscript = "${sqlscript}`nselect * from @stats;`n"
    $sqlscript = "${sqlscript}select * from @stats where nrecords=0;`n"
    $sqlscript | Out-File -FilePath "${extensionFolder}stats.sql" -Encoding UTF8
}

Write-Host "Generated $($result.Count) table definition(s) in $tablesFolder"
