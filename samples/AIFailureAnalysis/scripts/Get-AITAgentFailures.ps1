<#
.SYNOPSIS
    Root-causes the failing rows of an *agent* AIT run (e.g. the SOA-* Sales Order
    Agent suites) by walking every turn and surfacing the assertion that failed.

.DESCRIPTION
    Agent suites are not line-matching, so Get-AITMetrics.ps1's matching block is
    meaningless and its `Failures` array comes back empty under -GenericOnly. The
    real reason a row failed lives in `outputData`, per turn:

      * outputData.turns[i].answer.errorReason
            the assertion that failed ("Could not match expected lines …",
            "Expected number of lines (1) does not match actual (0)"). The single
            most useful field — quote it.
      * outputData.turns[i].answer.evaluation_results[].groundedness_evaluation_score
            (1-5) + ..._reason — an LLM-judge score on the generated text. High
            groundedness does NOT mean the row passed; trust errorReason over it.

    This script does the by-hand loop this skill used to require: fetch (or load)
    the run, keep the Error rows, iterate *all* turns (a row often passes turn 0
    and fails the follow-up/update turn), and print/emit each turn's errorReason,
    groundedness, and the expected-vs-got context so you can tell an
    intervention-flag error from a missing line from a wrong field (UoM/variant).

    Credentials resolve exactly like Get-AITRunVersions.ps1: -Credential ->
    -UseDefaultCredentials -> cached ait_cred.xml -> AIT_USER/AIT_PWD. No version
    given => the latest is resolved and echoed back.

.EXAMPLE
    # Latest run of an agent suite, console report:
    .\Get-AITAgentFailures.ps1 -SuiteCode 'SOA-P0'

.EXAMPLE
    # A specific version, machine-readable:
    .\Get-AITAgentFailures.ps1 -SuiteCode 'SOA-P0' -TestRunVersion 1 -AsJson > run.json

.EXAMPLE
    # From a saved aitTestLogEntries export (repo-independent):
    .\Get-AITAgentFailures.ps1 -ResultsPath .\soa_run.json
#>
[CmdletBinding(DefaultParameterSetName = 'Fetch')]
param(
    # --- Input: live fetch ---
    [Parameter(ParameterSetName = 'Fetch', Mandatory = $true, Position = 0)]
    [string] $SuiteCode,
    [Parameter(ParameterSetName = 'Fetch')] [int]    $TestRunVersion = 0,   # 0 = resolve latest
    [Parameter(ParameterSetName = 'Fetch')] [int]    $CodeunitId     = 0,   # 0 = don't filter
    [Parameter(ParameterSetName = 'Fetch')] [string] $CompanyName    = 'CRONUS International Ltd.',
    [Parameter(ParameterSetName = 'Fetch')] [string] $BaseUrl        = 'http://localhost:7047/Navision_NAV',
    [Parameter(ParameterSetName = 'Fetch')] [pscredential] $Credential,
    [Parameter(ParameterSetName = 'Fetch')] [switch] $UseDefaultCredentials,
    [Parameter(ParameterSetName = 'Fetch')] [string] $CredentialPath = (Join-Path $env:TEMP 'ait_cred.xml'),

    # --- Input: saved export ---
    [Parameter(ParameterSetName = 'File', Mandatory = $true)]
    [string] $ResultsPath,

    # --- Output ---
    [switch] $AsJson
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Strict-mode safe property read: returns $null when the property is absent
# (under Set-StrictMode, a bare $obj.Missing reference would throw). AIT agent rows
# carry many optional, turn-shaped fields, so every JSON-sourced read goes through this.
function Get-Prop($obj, [string]$name) {
    if ($null -ne $obj -and ($obj.PSObject.Properties.Name -contains $name)) { return $obj.$name }
    return $null
}

# ------------------------------------------------------------------ acquire rows
function Resolve-Credential {
    if ($Credential)            { return @{ Credential = $Credential } }
    if ($UseDefaultCredentials) { return @{ UseDefaultCredentials = $true } }
    if (Test-Path $CredentialPath) { return @{ Credential = (Import-Clixml -Path $CredentialPath) } }
    if ($env:AIT_USER -and $env:AIT_PWD) {
        $sec = ConvertTo-SecureString $env:AIT_PWD -AsPlainText -Force
        return @{ Credential = [System.Management.Automation.PSCredential]::new($env:AIT_USER, $sec) }
    }
    throw "No credential. Pass -Credential/-UseDefaultCredentials, run Analyze-Run.ps1 -SaveCredential once, or set AIT_USER/AIT_PWD."
}

function Get-RowsFromApi {
    $apiRoot = "$BaseUrl/api/microsoft/aiTestToolkit/v2.0"
    $irm = Resolve-Credential
    if ($BaseUrl -like 'http://*' -and (Get-Command Invoke-RestMethod).Parameters.ContainsKey('AllowUnencryptedAuthentication')) {
        $irm.AllowUnencryptedAuthentication = $true
    }

    $encName   = [uri]::EscapeDataString($CompanyName)
    $companies = Invoke-RestMethod -Uri "$apiRoot/companies?`$filter=name eq '$encName'" -Method Get @irm
    $companyVals = Get-Prop $companies 'value'
    if (-not $companyVals) { throw "Company '$CompanyName' not found." }
    $companyId = Get-Prop $companyVals[0] 'id'

    if (-not $TestRunVersion) {
        $vUrl  = "$apiRoot/companies($companyId)/aitTestLogEntries?`$filter=$([uri]::EscapeDataString("aitCode eq '$SuiteCode'"))&`$select=version"
        $vResp = Invoke-RestMethod -Uri $vUrl -Method Get @irm
        $vVals = Get-Prop $vResp 'value'
        if (-not $vVals) { throw "No log entries found for suite '$SuiteCode'." }
        $script:TestRunVersion = ($vVals.version | Measure-Object -Maximum).Maximum
        Write-Host ("resolved latest version of {0}: v{1}" -f $SuiteCode, $TestRunVersion) -ForegroundColor Green
    }

    $filter = "aitCode eq '$SuiteCode' and version eq $TestRunVersion"
    if ($CodeunitId) { $filter += " and codeunitId eq $CodeunitId" }
    $url  = "$apiRoot/companies($companyId)/aitTestLogEntries?`$filter=$([uri]::EscapeDataString($filter))"
    $resp = Invoke-RestMethod -Uri $url -Method Get @irm
    return (Get-Prop $resp 'value')
}

function Get-RowsFromFile {
    if (-not (Test-Path $ResultsPath)) { throw "Results file not found: $ResultsPath" }
    $raw = (Get-Content -Raw -Encoding utf8 $ResultsPath).TrimStart([char]0xFEFF)
    $obj = $raw | ConvertFrom-Json
    if ($obj.PSObject.Properties.Name -contains 'value') { return $obj.value }
    return $obj
}

$rows = if ($PSCmdlet.ParameterSetName -eq 'File') { Get-RowsFromFile } else { Get-RowsFromApi }
if (-not $rows) { throw "No rows returned." }

# ------------------------------------------------------------------ helpers
function ConvertFrom-JsonSafe([string]$s) {
    if (-not $s) { return $null }
    try { return $s | ConvertFrom-Json } catch { return $null }
}

# A turn list lives at .turns; a single-turn row may carry the answer at the root.
function Get-Turns($obj) {
    if ($null -eq $obj) { return @() }
    if ($obj.PSObject.Properties.Name -contains 'turns' -and $obj.turns) { return @($obj.turns) }
    return @($obj)
}

# ------------------------------------------------------------------ analyse Error rows
$errorRows = @($rows | Where-Object { (Get-Prop $_ 'status') -ne 'Success' })
$analysis  = foreach ($r in $errorRows) {
    $rowStatus = Get-Prop $r 'status'
    $rowId     = Get-Prop $r 'datasetLineNumber'
    $out      = ConvertFrom-JsonSafe ([string](Get-Prop $r 'outputData'))
    $inp      = ConvertFrom-JsonSafe ([string](Get-Prop $r 'inputData'))
    $outTurns = Get-Turns $out
    $inTurns  = Get-Turns $inp

    $turnReports = for ($i = 0; $i -lt $outTurns.Count; $i++) {
        $ans = Get-Prop $outTurns[$i] 'answer'
        $evalResults = Get-Prop $ans 'evaluation_results'
        $grounded = @()
        if ($evalResults) {
            $grounded = @($evalResults | ForEach-Object {
                [pscustomobject]@{
                    Score  = Get-Prop $_ 'groundedness_evaluation_score'
                    Reason = ([string](Get-Prop $_ 'groundedness_evaluation_reason') -replace '\s+', ' ')
                }
            })
        }
        $errReason = Get-Prop $ans 'errorReason'
        [pscustomobject]@{
            TurnIndex    = $i
            ErrorReason  = $errReason
            Failed       = [bool]$errReason
            Groundedness = $grounded
            ExpectedData = if ($i -lt $inTurns.Count)  { Get-Prop $inTurns[$i] 'expected_data' } else { $null }
            Context      = Get-Prop $outTurns[$i] 'context'
        }
    }

    [pscustomobject]@{
        Row            = $rowId
        Status         = $rowStatus
        TurnCount      = $outTurns.Count
        FailingTurns   = @($turnReports | Where-Object Failed | Select-Object -ExpandProperty TurnIndex)
        Turns          = @($turnReports)
    }
}

$result = [pscustomobject]@{
    SuiteCode   = $SuiteCode
    Version     = $TestRunVersion
    TotalRows   = $rows.Count
    ErrorRows   = $errorRows.Count
    SuccessRows = ($rows | Where-Object { (Get-Prop $_ 'status') -eq 'Success' }).Count
    Failures    = @($analysis)
}

# ------------------------------------------------------------------ emit
if ($AsJson) {
    $result | ConvertTo-Json -Depth 12
    return
}

Write-Host ""
Write-Host ("=== Agent failure analysis: {0} v{1} ===" -f $SuiteCode, $TestRunVersion) -ForegroundColor Cyan
Write-Host ("rows: {0}   success: {1}   error: {2}" -f $result.TotalRows, $result.SuccessRows, $result.ErrorRows)
foreach ($f in $analysis) {
    Write-Host ""
    Write-Host ("---- {0}  ({1} turn(s); failing turn(s): {2}) ----" -f `
        $f.Row, $f.TurnCount, (($f.FailingTurns -join ', ') -replace '^$', 'none')) -ForegroundColor Yellow
    foreach ($t in $f.Turns) {
        $tag = if ($t.Failed) { 'FAIL' } else { 'ok  ' }
        $color = if ($t.Failed) { 'Red' } else { 'DarkGray' }
        if ($t.Failed) {
            Write-Host ("  [{0}] turn {1}: {2}" -f $tag, $t.TurnIndex, $t.ErrorReason) -ForegroundColor $color
        } else {
            Write-Host ("  [{0}] turn {1}: (passed)" -f $tag, $t.TurnIndex) -ForegroundColor $color
        }
        foreach ($g in $t.Groundedness) {
            Write-Host ("        groundedness={0} :: {1}" -f $g.Score, $g.Reason) -ForegroundColor DarkGray
        }
    }
}
Write-Host ""
Write-Host "Reminder: errorReason is the source of truth; high groundedness on a passing turn does not mean the row passed." -ForegroundColor DarkCyan
return $result
