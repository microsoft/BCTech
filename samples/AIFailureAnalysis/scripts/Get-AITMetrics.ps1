<#
.SYNOPSIS
    Computes decision-level and account-level failure metrics for a Business Central
    AI Test Toolkit (AIT) run, and categorizes the failing rows for downstream
    failure analysis.

.DESCRIPTION
    Works on the raw AIT log shape (aitTestLogEntries): an array of rows where each row
    has `inputData` (JSON string with `input` + `expected_output`), `outputData`
    (JSON string with `answer`) and `status` ('Success' / 'Error').

    Input can come from either:
      * a saved JSON export (-ResultsPath) -- repo-independent, preferred for sharing, or
      * a live fetch from the AIT Toolkit OData API (-Fetch) when run on a server box.

    METRIC SCOPE
      Only MatchRate (row pass/fail) and Crash detection are task-agnostic; they apply to
      ANY AIT run. Everything else here -- the decision confusion matrix, precision/recall/
      F1/specificity, sample-averaged P/R/F1, the account-level splits and the
      Miss/SpuriousMatch/WrongAccount taxonomy -- is SCENARIO-SPECIFIC: it assumes a
      line-matching-with-abstention task (the Bank Acc. Rec. GL-matching family), where
      every input line either should map to an answer id or should be abstained on. Those
      metrics are produced by the LID/AID/answer regex parameters and are meaningless on a
      non-matching scenario. Use -GenericOnly to emit just the task-agnostic metrics.

    GENERIC METRICS (any AIT run)
      MatchRate          fraction of rows with status 'Success' (full-match / pass-fail).
      Crash              rows that errored with unusable model output.

    MATCHING-SCENARIO METRICS (line-match + abstention; e.g. Bank Acc. Rec.)
      Decision matrix    per input line, a binary "should match / should not match":
                           TP should match  AND model matched
                           FP should NOT    AND model matched   (spurious / failed abstention)
                           FN should match  AND model did NOT    (miss)
                           TN should NOT     AND model did NOT    (correct abstention)
                         -> Precision, Recall, F1, Specificity, Balanced/Plain accuracy.
      Sample-averaged    per-row P/R/F1 averaged over rows (empty-class convention:
                         no predicted matches -> P=1; no true positives -> R=1).
      Account-level      when matched values carry dataset AIDs, splits TP into
                           TP-correct       matched the expected AID
                           TP-wrong-account matched a different (sibling) AID
                           TP-out-of-set    matched an account outside the dataset
                                            (raw G/L No, not AID-comparable)

    FAILURE CATEGORIZATION (the analysis payload)
      Every non-clean row is emitted with line-level reasons:
        Crash          row status <> 'Success' with no usable answer
        Miss           expected a match, model abstained                  (FN)
        SpuriousMatch  model matched a line that should have abstained     (FP)
        WrongAccount   matched the right line but the wrong sibling account
      Use -ShowFailures for console detail, -AsJson for machine-readable output.

    The LID / AID / answer-pair patterns are parameterized so the same engine can serve
    other AIT scenarios; the defaults match the Bank Acc. Rec. GL-matching suite. See the
    scenario subskills for scenario-specific values.

.EXAMPLE
    # Analyze a saved export
    .\Get-AITMetrics.ps1 -ResultsPath .\ver29_results.json -ShowFailures

.EXAMPLE
    # Live fetch on a server box (Bank Acc. Rec. suite)
    .\Get-AITMetrics.ps1 -Fetch -SuiteCode 'BAR-AC1' -CodeunitId 133573 -TestRunVersion 29 `
        -CompanyName 'CRONUS International Ltd.' -Credential $cred

.EXAMPLE
    # Machine-readable for an agent to consume
    .\Get-AITMetrics.ps1 -ResultsPath .\ver29_results.json -AsJson | ConvertFrom-Json
#>
[CmdletBinding(DefaultParameterSetName = 'File')]
param(
    # --- Input: saved export ---
    [Parameter(ParameterSetName = 'File', Mandatory = $true)]
    [string] $ResultsPath,

    # --- Input: live fetch ---
    [Parameter(ParameterSetName = 'Fetch', Mandatory = $true)]
    [switch] $Fetch,
    [Parameter(ParameterSetName = 'Fetch')] [string] $BaseUrl = 'http://localhost:7047/Navision_NAV',
    [Parameter(ParameterSetName = 'Fetch')] [string] $CompanyId,
    [Parameter(ParameterSetName = 'Fetch')] [string] $CompanyName,
    [Parameter(ParameterSetName = 'Fetch', Mandatory = $true)] [string] $SuiteCode,
    [Parameter(ParameterSetName = 'Fetch')] [int] $CodeunitId = 0,
    [Parameter(ParameterSetName = 'Fetch')] [int] $TestRunVersion = 0,
    [Parameter(ParameterSetName = 'Fetch')] [pscredential] $Credential,
    [Parameter(ParameterSetName = 'Fetch')] [switch] $UseDefaultCredentials,
    [Parameter(ParameterSetName = 'Fetch')] [string] $CredentialPath = (Join-Path $env:TEMP 'ait_cred.xml'),

    # --- Parsing (scenario-configurable; these define the matching-scenario metrics) ---
    [string] $LineIdPattern   = 'LID:\s*([0-9]+)',
    [string] $ExpectedPattern = 'LID:\s*([0-9]+).*?AID:\s*([^,)\r\n]+)',
    [string] $AnswerPattern   = '\(([0-9]+),([^)]+)\)',
    [string] $OutOfSetPattern = 'GU[0-9]',

    # --- Scenario scope ---
    # Generic metrics (MatchRate, Crash) always apply. The matching-decision metrics
    # assume a line-match + abstention task; -GenericOnly suppresses that scenario block.
    [string] $ScenarioName = 'Bank Acc. Rec. (line-match + abstention)',
    [switch] $GenericOnly,

    # --- Output ---
    [switch] $ShowFailures,
    [switch] $AsJson
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# Strict-mode safe property read: returns $null when the property is absent
# (under Set-StrictMode, a bare $obj.Missing reference would throw). Used for the
# AIT log rows and OData payloads, whose optional fields are not guaranteed present.
function Get-Prop($obj, [string]$name) {
    if ($null -ne $obj -and ($obj.PSObject.Properties.Name -contains $name)) { return $obj.$name }
    return $null
}

# Under -GenericOnly the matching-scenario block is suppressed, so the default
# "Bank Acc. Rec." label would mislabel a non-matching run (e.g. an SOA-* agent
# suite). Fall back to a neutral label unless the caller set one explicitly.
if ($GenericOnly -and -not $PSBoundParameters.ContainsKey('ScenarioName')) {
    $ScenarioName = 'Generic (MatchRate + Crash only)'
}

# ------------------------------------------------------------------ acquire results
function Get-ResultsFromFile([string]$path) {
    if (-not (Test-Path $path)) { throw "Results file not found: $path" }
    $raw = Get-Content -Raw -Encoding utf8 $path
    # tolerate a BOM-prefixed file
    $raw = $raw.TrimStart([char]0xFEFF)
    $obj = $raw | ConvertFrom-Json
    if ($obj.PSObject.Properties.Name -contains 'value') { return $obj.value }
    return $obj
}

function Get-ResultsFromApi {
    # Auth resolution (enlistment-independent), in priority order:
    #   1. -UseDefaultCredentials  -> current Windows identity (Negotiate/NTLM), no password
    #   2. -Credential             -> explicit PSCredential supplied by the caller
    #   3. cached ait_cred.xml     -> the DPAPI cache shared with Get-AITRunVersions.ps1
    #   4. AIT_USER / AIT_PWD      -> env-var credential
    #   5. interactive prompt      -> Get-Credential fallback (interactive shells only)
    $apiRoot = "$BaseUrl/api/microsoft/aiTestToolkit/v2.0"
    $irmExtra = @{}
    if ($UseDefaultCredentials) {
        $irmExtra.UseDefaultCredentials = $true
    } else {
        if (-not $Credential -and (Test-Path $CredentialPath)) {
            $Credential = Import-Clixml -Path $CredentialPath
        }
        if (-not $Credential -and $env:AIT_USER -and $env:AIT_PWD) {
            $sec = ConvertTo-SecureString $env:AIT_PWD -AsPlainText -Force
            $Credential = [System.Management.Automation.PSCredential]::new($env:AIT_USER, $sec)
        }
        if (-not $Credential) {
            # Under an automated agent the session is non-interactive, so Get-Credential
            # throws "PowerShell is in NonInteractive mode". Detect that and fail with an
            # actionable message instead of hanging/throwing an opaque error.
            $interactive = [Environment]::UserInteractive `
                -and -not ([Environment]::GetCommandLineArgs() -match '(?i)-NonInteractive')
            if (-not $interactive) {
                throw "Live fetch needs credentials but the session is non-interactive. Pass -UseDefaultCredentials, -Credential, run Analyze-Run.ps1 -SaveCredential once, or set AIT_USER/AIT_PWD (see the skill's credential guidance)."
            }
            $Credential = Get-Credential -Message "Business Central API credential for $BaseUrl"
        }
        if (-not $Credential) { throw "Live fetch needs -UseDefaultCredentials, -Credential, a cached credential, or an interactive credential." }
        $irmExtra.Credential = $Credential
    }
    if ($BaseUrl -like 'http://*' -and (Get-Command Invoke-RestMethod).Parameters.ContainsKey('AllowUnencryptedAuthentication')) {
        $irmExtra.AllowUnencryptedAuthentication = $true
    }

    if (-not $CompanyId) {
        if (-not $CompanyName) { throw "Provide -CompanyId or -CompanyName for live fetch." }
        $encName = [uri]::EscapeDataString($CompanyName)
        $cu = "$apiRoot/companies?`$filter=name eq '$encName'"
        $companies = Invoke-RestMethod -Uri $cu -Method Get @irmExtra
        $companyVals = Get-Prop $companies 'value'
        if (-not $companyVals) { throw "Company '$CompanyName' not found." }
        $script:CompanyId = Get-Prop $companyVals[0] 'id'
    }

    $filter = "aitCode eq '$SuiteCode'"
    if ($TestRunVersion) { $filter += " and version eq $TestRunVersion" }
    if ($CodeunitId)     { $filter += " and codeunitId eq $CodeunitId" }
    $url = "$apiRoot/companies($CompanyId)/aitTestLogEntries?`$filter=$([uri]::EscapeDataString($filter))"
    Write-Verbose "Fetching: $url"
    $resp = Invoke-RestMethod -Uri $url -Method Get @irmExtra
    return (Get-Prop $resp 'value')
}

$results = if ($PSCmdlet.ParameterSetName -eq 'Fetch') { Get-ResultsFromApi } else { Get-ResultsFromFile $ResultsPath }
if (-not $results) { throw "No results returned." }

# ------------------------------------------------------------------ parsing helpers
function Get-LineIds([string]$inputBlock) {
    $ids = @()
    foreach ($l in ($inputBlock -split "`n")) {
        if ($l -match $LineIdPattern) { $ids += $Matches[1] }
    }
    return $ids
}
function Get-ExpectedMap([string]$expBlock) {
    $map = @{}
    foreach ($l in ($expBlock -split "`n")) {
        if ($l.Trim() -and ($l -match $ExpectedPattern)) { $map[$Matches[1]] = $Matches[2].Trim() }
    }
    return $map
}
function Get-AnswerMap([string]$answer) {
    $map = @{}
    foreach ($m in [regex]::Matches($answer, $AnswerPattern)) {
        $map[$m.Groups[1].Value] = $m.Groups[2].Value.Trim()
    }
    return $map
}

# ------------------------------------------------------------------ scoring
$TP = 0; $FP = 0; $FN = 0; $TN = 0
$TPcorrect = 0; $TPwrong = 0; $TPoutOfSet = 0
$aidTokens = 0
$passCount = 0
$rowPsum = 0.0; $rowRsum = 0.0; $rowFsum = 0.0
$failures = @()

foreach ($r in $results) {
    $inputDataRaw  = Get-Prop $r 'inputData'
    $outputDataRaw = Get-Prop $r 'outputData'
    $rowStatus     = Get-Prop $r 'status'
    $rowId         = Get-Prop $r 'datasetLineNumber'

    $inObj = if ($inputDataRaw) { $inputDataRaw | ConvertFrom-Json } else { $null }
    $lids  = Get-LineIds     ([string](Get-Prop $inObj 'input'))
    $exp   = Get-ExpectedMap ([string](Get-Prop $inObj 'expected_output'))

    $answer = ''
    $parseOk = $true
    if ($outputDataRaw) {
        try {
            $parsed = $outputDataRaw | ConvertFrom-Json
            $answer = [string](Get-Prop $parsed 'answer')
        } catch { $answer = ''; $parseOk = $false }
    }
    $ans = Get-AnswerMap $answer

    if ($rowStatus -eq 'Success') { $passCount++ }

    $rowBad = $false
    $reasons = @()
    $rTP = 0; $rFP = 0; $rFN = 0
    foreach ($lid in $lids) {
        $should = $exp.ContainsKey($lid)
        $got    = $ans.ContainsKey($lid)
        if ($should -and $got) {
            $TP++; $rTP++
            $val = $ans[$lid]
            if ($val -match $OutOfSetPattern -and $val -notin $exp.Values) {
                $TPoutOfSet++
            } else {
                $aidTokens++
                if ($val -eq $exp[$lid]) {
                    $TPcorrect++
                } else {
                    $TPwrong++; $rowBad = $true
                    $reasons += "WrongAccount(LID ${lid}: got $val, expected $($exp[$lid]))"
                }
            }
        }
        elseif ($should -and -not $got) {
            $FN++; $rFN++; $rowBad = $true
            $reasons += "Miss(LID ${lid}: expected $($exp[$lid]), none returned)"
        }
        elseif ((-not $should) -and $got) {
            $FP++; $rFP++; $rowBad = $true
            $reasons += "SpuriousMatch(LID ${lid}: returned $($ans[$lid]), should abstain)"
        }
        else { $TN++ }
    }

    $isCrash = ($rowStatus -ne 'Success') -and (-not $parseOk -or $answer -eq '')
    if ($isCrash) { $rowBad = $true; $reasons = @('Crash(row status ' + $rowStatus + ', unusable answer)') + $reasons }

    if ($rowBad) {
        $cats = @()
        if ($isCrash) { $cats += 'Crash' }
        if ($reasons -match 'WrongAccount')   { $cats += 'WrongAccount' }
        if ($reasons -match 'SpuriousMatch')  { $cats += 'SpuriousMatch' }
        if ($reasons -match 'Miss')           { $cats += 'Miss' }
        $failures += [pscustomobject]@{
            Row        = $rowId
            Status     = $rowStatus
            Categories = ($cats | Select-Object -Unique)
            Reasons    = $reasons
        }
    }

    $rP = if (($rTP + $rFP) -eq 0) { 1.0 } else { $rTP / ($rTP + $rFP) }
    $rR = if (($rTP + $rFN) -eq 0) { 1.0 } else { $rTP / ($rTP + $rFN) }
    $rF = if (($rP + $rR) -eq 0) { 0.0 } else { 2 * $rP * $rR / ($rP + $rR) }
    $rowPsum += $rP; $rowRsum += $rR; $rowFsum += $rF
}

$prec = if ($TP + $FP) { $TP / ($TP + $FP) } else { 0 }
$rec  = if ($TP + $FN) { $TP / ($TP + $FN) } else { 0 }
$f1   = if ($prec + $rec) { 2 * $prec * $rec / ($prec + $rec) } else { 0 }
$spec = if ($TN + $FP) { $TN / ($TN + $FP) } else { 0 }
$acc  = ($TP + $TN) / [math]::Max(1, ($TP + $TN + $FP + $FN))
$bal  = ($rec + $spec) / 2

$nRows     = [math]::Max(1, $results.Count)
$matchRate = $passCount / $nRows
$sP = $rowPsum / $nRows
$sR = $rowRsum / $nRows
$sF = $rowFsum / $nRows
$accountLevelAvailable = ($aidTokens -gt 0)
$aidMatches = $TPcorrect + $TPwrong
$sibRate = if ($aidMatches) { $TPwrong / $aidMatches } else { 0 }

# ------------------------------------------------------------------ failure category tally
$catCounts = @{ Crash = 0; Miss = 0; SpuriousMatch = 0; WrongAccount = 0 }
foreach ($f in $failures) { foreach ($c in $f.Categories) { $catCounts[$c]++ } }

# ------------------------------------------------------------------ summary (scoped)
# Generic block always present; matching-scenario block added only when applicable.
$summary = [ordered]@{
    Scenario  = $ScenarioName
    Version   = $TestRunVersion
    Rows      = $results.Count
    # --- generic (task-agnostic) ---
    MatchRate = [math]::Round($matchRate, 3)
    PassCount = $passCount
    CrashRows = $catCounts.Crash
}
if (-not $GenericOnly) {
    # --- matching-scenario (line-match + abstention) ---
    $summary['TP'] = $TP; $summary['FP'] = $FP; $summary['FN'] = $FN; $summary['TN'] = $TN
    $summary['Precision']   = [math]::Round($prec, 3); $summary['Recall'] = [math]::Round($rec, 3); $summary['F1'] = [math]::Round($f1, 3)
    $summary['Specificity'] = [math]::Round($spec, 3); $summary['BalancedAccuracy'] = [math]::Round($bal, 3); $summary['Accuracy'] = [math]::Round($acc, 3)
    $summary['SamplePrecision'] = [math]::Round($sP, 3); $summary['SampleRecall'] = [math]::Round($sR, 3); $summary['SampleF1'] = [math]::Round($sF, 3)
    $summary['TPcorrect'] = $TPcorrect; $summary['TPwrongAccount'] = $TPwrong; $summary['TPoutOfSet'] = $TPoutOfSet
    $summary['SiblingConfusion'] = [math]::Round($sibRate, 3)
    $summary['AccountLevelAvailable'] = $accountLevelAvailable
    $summary['FailCrash'] = $catCounts.Crash; $summary['FailMiss'] = $catCounts.Miss
    $summary['FailSpuriousMatch'] = $catCounts.SpuriousMatch; $summary['FailWrongAccount'] = $catCounts.WrongAccount
}
$summary = [pscustomobject]$summary

# ------------------------------------------------------------------ emit
if ($AsJson) {
    [pscustomobject]@{ Summary = $summary; Failures = $failures } | ConvertTo-Json -Depth 6
    return
}

Write-Host ""
$suiteLabel   = if ($SuiteCode)      { $SuiteCode }      else { '(file)' }
$versionLabel = if ($TestRunVersion) { $TestRunVersion } else { '-' }
Write-Host ("=== AIT metrics  (suite {0}  version {1}) ===" -f $suiteLabel, $versionLabel) -ForegroundColor Cyan
Write-Host ("scenario: {0}" -f $ScenarioName) -ForegroundColor DarkCyan
Write-Host ""
Write-Host "GENERIC (task-agnostic, any AIT run):" -ForegroundColor Cyan
Write-Host ("  MatchRate (full match / pass-fail)  {0:N3}   ({1}/{2} rows)" -f $matchRate, $passCount, $results.Count) -ForegroundColor Green
Write-Host ("  Crash rows                          {0}" -f $catCounts.Crash)

if (-not $GenericOnly) {
    Write-Host ""
    Write-Host "MATCHING-SCENARIO (line-match + abstention; e.g. Bank Acc. Rec.):" -ForegroundColor Cyan
    Write-Host ("  total lines: {0}" -f ($TP + $FP + $FN + $TN))
    Write-Host ""
    Write-Host "  Match-decision confusion matrix (micro / per-line):"
    Write-Host ("    TP {0}   FP {1}   FN {2}   TN {3}" -f $TP, $FP, $FN, $TN)
    Write-Host ""
    Write-Host ("    Precision                  {0:N3}" -f $prec)
    Write-Host ("    Recall (sensitivity)       {0:N3}" -f $rec)
    Write-Host ("    F1                         {0:N3}" -f $f1)
    Write-Host ("    Specificity (abstain rec.) {0:N3}" -f $spec)
    Write-Host ("    Balanced accuracy          {0:N3}" -f $bal)
    Write-Host ("    Plain accuracy             {0:N3}" -f $acc)
    Write-Host ""
    Write-Host "  Sample-averaged (per-row, positive = should match):"
    Write-Host ("    Precision   {0:N3}" -f $sP)
    Write-Host ("    Recall      {0:N3}" -f $sR)
    Write-Host ("    F1          {0:N3}" -f $sF)
    Write-Host ""
    if ($accountLevelAvailable) {
        Write-Host "  Account-level (positives):"
        Write-Host ("    TP-correct        {0}" -f $TPcorrect)
        Write-Host ("    TP-wrong-account  {0}   (sibling-confusion rate {1:N3} of AID-resolvable matches)" -f $TPwrong, $sibRate)
        if ($TPoutOfSet) {
            Write-Host ("    TP-out-of-set     {0}   (matched an account outside the dataset; raw G/L No, not AID-comparable)" -f $TPoutOfSet) -ForegroundColor DarkYellow
        }
    } else {
        Write-Host "  Account-level: N/A (no AID-carrying matches; pre-AID codeunit run)" -ForegroundColor DarkYellow
    }
    Write-Host ""
    Write-Host "  Failure categories (rows affected):"
    Write-Host ("    Crash {0}   Miss {1}   SpuriousMatch {2}   WrongAccount {3}" -f `
        $catCounts.Crash, $catCounts.Miss, $catCounts.SpuriousMatch, $catCounts.WrongAccount)
}

if ($ShowFailures -and $failures.Count) {
    Write-Host ""
    Write-Host ("Failing rows ({0}):" -f $failures.Count) -ForegroundColor Yellow
    foreach ($f in $failures) {
        Write-Host ("  row {0} [{1}] {2}" -f $f.Row, ($f.Categories -join ','), $f.Status)
        foreach ($rsn in $f.Reasons) { Write-Host ("      - {0}" -f $rsn) }
    }
}

$summary
