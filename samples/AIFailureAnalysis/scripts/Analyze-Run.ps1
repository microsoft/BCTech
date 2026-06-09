<#
.SYNOPSIS
    Thin, stable wrapper over Get-AITMetrics.ps1 for live AIT runs so an agent issues
    one identical, allow-listable command per run instead of a different inline script
    each time (which forces a fresh confirmation every call).

.DESCRIPTION
    Two responsibilities:

      1. Credential caching. Live fetch needs a BC API credential, but agent shells are
         non-interactive (Get-Credential / GUI popups fail). Run once with -SaveCredential
         to cache a DPAPI-encrypted credential (user+machine scoped) to disk; afterwards
         every analysis call is just `Analyze-Run.ps1 -Version N` with no secret on the
         command line — so the CLI sees a stable command it can be allow-listed once.

      2. Stable invocation. Scenario identity (suite, codeunit, company, base url) is
         defaulted to Bank Acc. Rec. (BAR-AC1) and can be overridden, so the per-run
         command surface never changes shape between versions.

    The credential cache is DPAPI-encrypted: only the same Windows user on the same
    machine can read it. Delete it with -ClearCredential when done.

.EXAMPLE
    # One-time setup (the only call that carries the secret; approve once):
    .\Analyze-Run.ps1 -SaveCredential -User klausmh -Password 'P@ss'

.EXAMPLE
    # Every run thereafter — identical, allow-listable command:
    .\Analyze-Run.ps1 -Version 66

.EXAMPLE
    # Machine-readable, or with per-row failures:
    .\Analyze-Run.ps1 -Version 66 -AsJson
    .\Analyze-Run.ps1 -Version 66 -ShowFailures
#>
[CmdletBinding(DefaultParameterSetName = 'Analyze')]
param(
    [Parameter(ParameterSetName = 'Analyze', Mandatory = $true, Position = 0)]
    [int] $Version,

    # --- scenario identity (defaults target the Bank Acc. Rec. GL-matching suite) ---
    [string] $SuiteCode   = 'BAR-AC1',
    [int]    $CodeunitId  = 133573,
    [string] $CompanyName = 'CRONUS International Ltd.',
    [string] $BaseUrl     = 'http://localhost:7047/Navision_NAV',

    # --- output ---
    [Parameter(ParameterSetName = 'Analyze')] [switch] $ShowFailures,
    [Parameter(ParameterSetName = 'Analyze')] [switch] $AsJson,
    [Parameter(ParameterSetName = 'Analyze')] [switch] $GenericOnly,   # only task-agnostic metrics (MatchRate, Crash)

    # --- credential management ---
    [Parameter(ParameterSetName = 'SaveCred', Mandatory = $true)] [switch] $SaveCredential,
    [Parameter(ParameterSetName = 'SaveCred', Mandatory = $true)] [string] $User,
    [Parameter(ParameterSetName = 'SaveCred', Mandatory = $true)] [string] $Password,
    [Parameter(ParameterSetName = 'ClearCred', Mandatory = $true)] [switch] $ClearCredential,

    [string] $CredentialPath = (Join-Path $env:TEMP 'ait_cred.xml')
)

$ErrorActionPreference = 'Stop'
$engine = Join-Path $PSScriptRoot 'Get-AITMetrics.ps1'

# ------------------------------------------------------------------ credential cache
if ($PSCmdlet.ParameterSetName -eq 'SaveCred') {
    $sec  = ConvertTo-SecureString $Password -AsPlainText -Force
    $cred = [System.Management.Automation.PSCredential]::new($User, $sec)
    $cred | Export-Clixml -Path $CredentialPath          # DPAPI: user+machine scoped
    Write-Host "Credential cached for '$User' at $CredentialPath (DPAPI-encrypted)." -ForegroundColor Green
    Write-Host "Subsequent runs: Analyze-Run.ps1 -Version N" -ForegroundColor Green
    return
}
if ($PSCmdlet.ParameterSetName -eq 'ClearCred') {
    if (Test-Path $CredentialPath) { Remove-Item $CredentialPath -Force; Write-Host "Cleared $CredentialPath." }
    else { Write-Host "No cached credential at $CredentialPath." }
    return
}

# ------------------------------------------------------------------ resolve credential
# priority: cached clixml -> AIT_USER/AIT_PWD env vars -> actionable error
$cred = $null
if (Test-Path $CredentialPath) {
    $cred = Import-Clixml -Path $CredentialPath
}
elseif ($env:AIT_USER -and $env:AIT_PWD) {
    $sec  = ConvertTo-SecureString $env:AIT_PWD -AsPlainText -Force
    $cred = [System.Management.Automation.PSCredential]::new($env:AIT_USER, $sec)
}
else {
    throw "No cached credential. Run once: Analyze-Run.ps1 -SaveCredential -User <u> -Password <p>  (or set AIT_USER/AIT_PWD)."
}

# ------------------------------------------------------------------ fetch + analyze
$engineArgs = @{
    Fetch          = $true
    SuiteCode      = $SuiteCode
    CodeunitId     = $CodeunitId
    TestRunVersion = $Version
    CompanyName    = $CompanyName
    BaseUrl        = $BaseUrl
    Credential     = $cred
}

if ($GenericOnly) { $engineArgs.GenericOnly = $true }

if ($AsJson) {
    & $engine @engineArgs -AsJson
    return
}

# Single fetch; render a compact summary plus the failing-row category-combo
# breakdown (the bit we keep recomputing by hand).
$parsed = (& $engine @engineArgs -AsJson) | ConvertFrom-Json
$s = $parsed.Summary

Write-Host ""
Write-Host ("=== {0} v{1}  ({2} rows) ===" -f $SuiteCode, $Version, $s.Rows) -ForegroundColor Cyan
Write-Host ("scenario: {0}" -f $s.Scenario) -ForegroundColor DarkCyan
# generic (task-agnostic)
Write-Host ("MatchRate  {0:N3}   ({1}/{2} rows)    Crash {3}" -f $s.MatchRate, $s.PassCount, $s.Rows, $s.CrashRows) -ForegroundColor Green

# matching-scenario block (absent under -GenericOnly)
if (-not $GenericOnly) {
    Write-Host ("Precision {0:N3}   Recall {1:N3}   F1 {2:N3}   Specificity {3:N3}   BalancedAcc {4:N3}" -f `
        $s.Precision, $s.Recall, $s.F1, $s.Specificity, $s.BalancedAccuracy)
    Write-Host ("TP {0}  FP {1}  FN {2}  TN {3}    SiblingConfusion {4:N3} ({5} wrong-account)" -f `
        $s.TP, $s.FP, $s.FN, $s.TN, $s.SiblingConfusion, $s.TPwrongAccount)
    Write-Host ("Failure rows: Crash {0}  Miss {1}  SpuriousMatch {2}  WrongAccount {3}" -f `
        $s.FailCrash, $s.FailMiss, $s.FailSpuriousMatch, $s.FailWrongAccount)

    if ($parsed.Failures) {
        Write-Host ""
        Write-Host "Failing-row category combos:" -ForegroundColor Yellow
        $parsed.Failures |
            ForEach-Object { ($_.Categories -join ',') } |
            Group-Object | Sort-Object Count -Descending |
            ForEach-Object { Write-Host ("  {0,-32} {1}" -f $_.Name, $_.Count) }
    }
}

if ($ShowFailures) { & $engine @engineArgs -ShowFailures | Out-Host }

$s
