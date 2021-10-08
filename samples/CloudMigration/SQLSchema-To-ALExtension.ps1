<#
.Description
Takes an SQL schema definition and generates the appropriate files to have this as a BC extension that can have its data imported by Cloud Migration.
Requires the folder `stubs` to be on the same path.

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
.Parameter GenSQLStatsQuery
Switch to generate the SQL stats query

#>
param (
    [Parameter(Mandatory=$true)][string]$InputSchema,
    [string]$Prefix = 'MSFT',
    [int]$StartId = 50000,
    [string]$OutputFolder = '',
    [string]$ExtensionName = '',
    [string]$TablesSubfolder = '',
    [int]$StartCodeunitsId = 57000,
    [switch]$GenSQLStatsQuery
)


if (-not( Test-Path -Path $InputSchema)) {
    Write-Host "Input schema doesn't exist."
    exit
}
if (-not( Test-Path -Path $OutputFolder)){
    Write-Host "Output folder doesn't exist."
    exit
}

$extensionFolder = $OutputFolder
if($extensionFolder -ne ''){
    $extensionFolder += '\'
}
if($ExtensionName -ne ''){
    $extensionFolder += "$ExtensionName\"
}

$tablesFolder = "${extensionFolder}$TablesSubfolder\"

if(Test-Path $extensionFolder){
    Remove-Item $extensionFolder
}
New-Item -Path $extensionFolder -Type Directory
New-Item -Path $tablesFolder -Type Directory

[String] $schema =  (Get-Content $InputSchema)

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

# if it's worth it, a proper SQL parser could be fitting and less error prone...

$sf = "(\s|\n)+" 
$s = "(\s|\n)*" 

function mLit($w) {
    $l = $w.ToLower()
    $u = $w.ToUpper()
    return "($l|$u)"`
}

function SQLColTypeToAL($c) {
    $sqlType = (fromBrackets $c).ToLower()
    $alType = "UNKNOWN"
    $params = ""
    switch ($sqlType) {
        "int" { $alType = "Integer" }
        "smallint" { $alType = "Integer" }
        "tinyint" { $alType = "Boolean" }
        "char" { $alType = "Text" }
        "binary" { 
            $alType = "Text"
            $params = "[50]"
        }
        "numeric" { $alType = "Decimal" }
        "datetime" { $alType = "DateTime" }
        "text" {
            $alType = "Text"
            $params = "[2048]"
        }
        "ntext" {
            $alType = "Text"
            $params = "[2048]"
        }
        "image" {
            $alType = "Blob"
        }
        "varbinary"{ $alType = "Blob" }
        "varchar" { $alType = "Text" }
    }
    $pr = [Regex]::new("\((?<paren>[^\)]*)\)")
    $x = $pr.Matches($c)

    if ($x.Count -ne 0){
        $ps = $x[0].Groups['paren'].value.Split(",")
        if ($params -eq ""){
            switch ($alType) {
                "Text" {
                    $params = "[$($ps[0].Trim())]"
                }
                "Blob" {$params = ""}
            }
        }
    }

    return "$alType$params"
}

$create = "$($(mLit create))${sf}$($(mLit table))"
$tableid = "(?<tableid>[^\s\n\(]+)"
$colid = "(?<colid>[a-zA-Z\d_\[\]]+)"
$colty = "(?<colty>[a-zA-Z_\[\]]+($sf|(\($s[a-zA-Z\d]+($s,$s[a-zA-Z\d]+$s)*\))))"
$primkeys = "$($(mLit constraint))$sf.+$($(mLit primary))$sf$($(mLit key))$sf[^\(]+\((?<colkeys>[^\)]+)\)"

$createTableRegex = [Regex]::new("$create$sf$tableid$s\(")

$script:CodeunitMappings = ""
$script:SQLStatsQ = @()

function fromBrackets($v){
    $r = [Regex]::new("\[?(?<name>[^\[\]]+)\]?")
    $t = $r.Matches($v)
    return $t[0].Groups['name'].value
}
function splitCommaParams($tablecontent){
    $i = 0
    $pCount = 0
    $bCount = 0
    $current = "" 
    $params = @()
    for ($i = 0; $i -lt $tablecontent.Length; $i++){
        $c = $tablecontent[$i]
        if(($c -eq ',') -and ($pCount -eq 0) -and ($bCount -eq 0)){
            $params += $current
            $current = ""
            Continue
        }
        if($c -eq '('){
            $pCount++
        }
        elseif($c -eq '[') {
            $bCount++
        }
        elseif($c -eq ')'){
            $pCount--
        }
        elseif($c -eq ']'){
            $bCount--
        }
        $current += $c
    }
    if($current -ne ''){
        $params += $current
    }
    return $params
}
function SQLTableToAL($tableid, $tablecontent, $tableCount){
    $ss = $tableid.Split(".")
    if($ss.Count -eq 1){
        $tableName = $ss[0]
    }
    else {
        $tableName = $ss[1]
    }
    $r = [Regex]::new("\[?(?<name>[^\[\]]+)\]?")
    $result = $r.Matches($tableName)
    $tableName = $result[0].Groups['name'].value
    $baseName = "$Prefix$tableName"
    $filename = "$basename.Table.al"


    $id = $StartId+$tableCount

    $pxml = $permissionXML -replace "OBJECTTYPEHERE","TableData"
    $pxml = $pxml -replace "OBJECTIDHERE",$id
    $script:permissionsXMLList += $pxml

    $content = "table $id $basename `n"
    $content = "$content{`n"
    $content = "$content    DataClassification = CustomerContent;`n"
    $content = "$content    fields`n"
    $content = "$content    {`n"
    
    $keyscontent = @()

    $tableParams = splitCommaParams $tablecontent
    if($tableParams.Count -eq 1){
        $tableParams = @($tableParams)
    }
    for($i = 0; $i -lt $tableParams.Count; $i++){
        [String] $p = $tableParams[$i]
        $fstword = $p.Trim().Split(' ')[0]
        if($fstword -match (mLit constraint)){
            # process constraint
            $keysr = [Regex]::new($primkeys)
            $result = $keysr.Matches($p)
            if($result.Count -eq 0){
                Write-Host "Unrecognized constraint $p"
                Continue
            }

            $keydefs = $result[0].Groups['colkeys'].value.Split(",")
            for($j = 0; $j -lt $keydefs.Count; $j++){
                $keydef = $keydefs[$j]
                $keycolname = fromBrackets ($keydef.Trim() -replace "$sf"," ").Split()[0]
                $keyscontent += $keycolname
            }
            Continue
        }
        # process column definition
        $r = [Regex]::new("$s$colid$sf$colty")
        $t = $r.Matches($p)
        if($t.Count -eq 0){
            Write-Host "Unrecognized column definition '$p'. For table $tableName"
            Continue
        }
        $colname = fromBrackets ($t[0].Groups['colid'].value)
        $alcolty = $t[0].Groups['colty'].value
        $colType = SQLColTypeToAL $alcolty
        if($colType -eq "UNKNOWN"){
            Write-Host "Unkown column type $alcolty on table $tableName."
        }
        $content = "$content        field($($i+1); $colname; $colType)`n"
        $content = "$content        {`n"
        $content = "$content            DataClassification = CustomerContent;`n"
        $content = "$content        }`n"
    }
    $content = "$content    }`n"
    if($keyscontent.Count -gt 0){
        $ks = $keyscontent -join ","
        $content = "$content    keys`n"
        $content = "$content    {`n"
        $content = "$content        key(Key1; $ks)`n"
        $content = "$content        {`n"
        $content = "$content            Clustered = true;`n"
        $content = "$content        }`n"
        $content = "$content    }`n"
    }
    else {
        Write-Host "Table $tableName without keys definition. Ignoring."
        return
    }
    $content = "$content}`n"
    $content | Out-File -FilePath "$tablesFolder$filename"
    $script:CodeunitMappings = "$CodeunitMappings        UpdateOrInsertRecord(Database::$baseName, '$tableName');`n"
    $script:SQLStatsQ += "SELECT '$tableName', COUNT(*) from $tableName"
    return
}

$useDBregex = "$($(mLit use))$sf(?<dbname>[a-zA-Z0-9_\-\[\]]+)"
$hasDBName = $schema -match $useDBregex
if($hasDBName){
    $dbname = fromBrackets $matches['dbname']
}
$result = $createTableRegex.Matches($schema)


if($result.Count -gt 0){
    for ($i = 0 ; $i -lt $result.Count; $i++){
        $tableid = $result[$i].Groups['tableid'].value
        $afterMatch = ($result[$i].Index)+($result[$i].Length)
        $contentIndex = $afterMatch
        $parencount = 1
        $innerLen = 0
        while (($contentIndex -lt $schema.Length) -and ($parencount -gt 0)){
            if ($schema[$contentIndex] -eq '('){
                $parencount++
            }
            elseif ($schema[$contentIndex] -eq ')') {
                $parencount--
            }
            $contentIndex++
            $innerLen++
        }
        if($contentIndex -eq $schema.Length){
            Write-Host "Unmatched parentheses after CREATE TABLE expression"
            Exit
        }
        $tablecontent = $schema.Substring($afterMatch, $innerLen-1)
        SQLTableToAL $tableid $tablecontent $i
    }

    $cId = $StartCodeunitsId;
    $codeunit = $mappingCodeunit -replace "CODEUNITID",$cId
    $mappingName = "$Prefix - Default table mapping"
    $codeunit = $codeunit -replace "CODEUNITNAME",$mappingName
    $codeunit = $codeunit -replace "UPDATEORINSERTMAPPINGS",$script:CodeunitMappings
    $codeunit | Out-File -FilePath "$extensionFolder${Prefix}DefaultTableMapping.Codeunit.al"
    $pxml = $permissionXML -replace "OBJECTTYPEHERE","Codeunit"
    $pxml = $pxml -replace "OBJECTIDHERE",$cId
    $script:permissionsXMLList += $pxml
    $cId += 1

    $permissionsContent = $permissionsXML -replace "PERMISSIONSHERE",($script:permissionsXMLList -join "`n") 
    $permissionsContent -replace "EXTENSIONNAMEHERE",$ExtensionName | Out-File -FilePath "$extensionFolder\Permissions.xml"

    if ($GenSQLStatsQuery) {
        $sqlscript = "use $dbname`n"
        $sqlscript = "${sqlscript}declare @stats table (tbl varchar(255), nrecords int);`n"
        $sqlscript = "${sqlscript}insert into @stats`n"

        $x = $script:SQLStatsQ -join "`nunion`n"
        $sqlscript = "$sqlscript$x;"
        $sqlscript = "${sqlscript}`nselect * from @stats;`n"
        $sqlscript = "${sqlscript}select * from @stats where nrecords=0;`n"
        $sqlscript | Out-File -FilePath "${OutputFolder}stats.sql"
    }
}
else {
    Write-Host "Unable to parse schema definitions"
}
