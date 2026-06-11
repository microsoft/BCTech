<#
.SYNOPSIS
    Lists the available run versions for an AIT suite, or resolves the latest one.

.DESCRIPTION
    "Analyze the latest <suite> run" has no version number — you must discover it.
    This helper queries the AIT Toolkit OData API for a suite's log entries and
    reports the versions present (with row + Success/Error counts), or, with
    -Latest, emits just the highest version number so a caller can pipe it into
    the analysis step.

    It reuses the same DPAPI-encrypted credential cache as Analyze-Run.ps1 (run
    `Analyze-Run.ps1 -SaveCredential ...` once), so under a non-interactive agent
    shell no secret ever appears on the command line. Pass -UseDefaultCredentials
    to authenticate as the current Windows identity instead (no cached secret).

.EXAMPLE
    # Which versions exist for a suite, and how did each do?
    .\Get-AITRunVersions.ps1 -SuiteCode 'SOA-P0'

.EXAMPLE
    # Just the latest version number (for scripting):
    $v = .\Get-AITRunVersions.ps1 -SuiteCode 'SOA-P0' -Latest

.EXAMPLE
    # Machine-readable per-version breakdown:
    .\Get-AITRunVersions.ps1 -SuiteCode 'BAR-AC1' -AsJson | ConvertFrom-Json
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string] $SuiteCode,

    # --- scenario identity / connection ---
    [string] $CompanyName = 'CRONUS International Ltd.',
    [string] $BaseUrl     = 'http://localhost:7047/Navision_NAV',
    [int]    $CodeunitId  = 0,                                   # 0 = don't filter by codeunit

    # --- output mode ---
    [switch] $Latest,                                           # emit only the max version (int)
    [switch] $AsJson,

    [switch] $UseDefaultCredentials,                            # current Windows identity (no cached secret)
    [string] $CredentialPath = (Join-Path $env:TEMP 'ait_cred.xml')
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Strict-mode safe property read: returns $null when the property is absent.
function Get-Prop($obj, [string]$name) {
    if ($null -ne $obj -and ($obj.PSObject.Properties.Name -contains $name)) { return $obj.$name }
    return $null
}

# ------------------------------------------------------------------ resolve credential
# Priority: -UseDefaultCredentials -> cached clixml -> AIT_USER/AIT_PWD -> error.
$irmExtra = @{}
if ($UseDefaultCredentials) {
    $irmExtra.UseDefaultCredentials = $true
}
else {
    $cred = $null
    if (Test-Path $CredentialPath) {
        $cred = Import-Clixml -Path $CredentialPath
    }
    elseif ($env:AIT_USER -and $env:AIT_PWD) {
        $sec  = ConvertTo-SecureString $env:AIT_PWD -AsPlainText -Force
        $cred = [System.Management.Automation.PSCredential]::new($env:AIT_USER, $sec)
    }
    else {
        throw "No cached credential. Run once: Analyze-Run.ps1 -SaveCredential -User <u> -Password <p>  (or set AIT_USER/AIT_PWD, or pass -UseDefaultCredentials)."
    }
    $irmExtra.Credential = $cred
}

# ------------------------------------------------------------------ fetch
$apiRoot = "$BaseUrl/api/microsoft/aiTestToolkit/v2.0"
if ($BaseUrl -like 'http://*' -and (Get-Command Invoke-RestMethod).Parameters.ContainsKey('AllowUnencryptedAuthentication')) {
    $irmExtra.AllowUnencryptedAuthentication = $true
}

$encName   = [uri]::EscapeDataString($CompanyName)
$companies = Invoke-RestMethod -Uri "$apiRoot/companies?`$filter=name eq '$encName'" -Method Get @irmExtra
$companyVals = Get-Prop $companies 'value'
if (-not $companyVals) { throw "Company '$CompanyName' not found." }
$companyId = Get-Prop $companyVals[0] 'id'

$filter = "aitCode eq '$SuiteCode'"
if ($CodeunitId) { $filter += " and codeunitId eq $CodeunitId" }
$select = '$select=version,codeunitId,status'
$url    = "$apiRoot/companies($companyId)/aitTestLogEntries?`$filter=$([uri]::EscapeDataString($filter))&$select"
$resp   = Invoke-RestMethod -Uri $url -Method Get @irmExtra
$respVals = Get-Prop $resp 'value'
if (-not $respVals) { throw "No log entries found for suite '$SuiteCode'." }

# ------------------------------------------------------------------ aggregate per version
$byVersion = $respVals | Group-Object version | ForEach-Object {
    $rows = $_.Group
    [pscustomobject]@{
        Version   = [int]$_.Name
        Rows      = $rows.Count
        Success   = ($rows | Where-Object { $_.status -eq 'Success' }).Count
        Error     = ($rows | Where-Object { $_.status -ne 'Success' }).Count
        Codeunits = ($rows.codeunitId | Sort-Object -Unique) -join ','
    }
} | Sort-Object Version

$maxVersion = ($byVersion | Measure-Object Version -Maximum).Maximum

# ------------------------------------------------------------------ emit
if ($Latest) {
    return [int]$maxVersion
}
if ($AsJson) {
    [pscustomobject]@{ SuiteCode = $SuiteCode; LatestVersion = $maxVersion; Versions = $byVersion } | ConvertTo-Json -Depth 5
    return
}

Write-Host ""
Write-Host ("=== AIT run versions for suite {0} ===" -f $SuiteCode) -ForegroundColor Cyan
$byVersion | Format-Table -AutoSize | Out-Host
Write-Host ("latest version: {0}" -f $maxVersion) -ForegroundColor Green
return $byVersion
