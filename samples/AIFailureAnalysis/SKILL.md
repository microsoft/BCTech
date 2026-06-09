---
name: ai-eval-failure-analysis
description: >-
  Analyze Business Central AI Test Toolkit (AIT) evaluation results to explain
  WHY an eval run failed. Use when the user has an AIT run (a version/suite, or a
  saved aitTestLogEntries JSON export) and wants failure analysis, a confusion
  matrix, precision/recall/F1, specificity, MatchRate, sibling-confusion, model
  comparison, or a breakdown of failing rows (misses, spurious matches, wrong
  accounts, crashes). Triggers on phrases like "analyze my eval", "why did the
  AIT fail", "compare these model runs", "precision/recall for this run",
  "AI test failure analysis".
---

# AI Eval Failure Analysis (Business Central AIT)

You help developers understand *why* a Business Central AI Test Toolkit (AIT) run
failed and how strong a model actually is on the task — going well beyond the
toolkit's built-in pass/fail count.

## When to use this skill

Use it whenever the user references an AIT run and wants more than "X/Y passed":
failure root-causing, richer metrics, or a comparison across model/prompt versions.

Inputs the skill understands:
- A **saved export** of the AIT log (`aitTestLogEntries` rows as JSON) — preferred,
  works anywhere.
- A **live run** identified by suite code + codeunit + version, fetched from the AIT
  Toolkit OData API (only on a box with the server + credentials).

## The AIT log shape (what you are reasoning over)

Each row in the log is one dataset line and carries:

| Field | Meaning |
|-------|---------|
| `inputData` | JSON string: `{ "input": <prompt context>, "expected_output": <ground truth> }` |
| `outputData` | JSON string: `{ "answer": <model output> }` |
| `status` | `Success` (row fully correct) or `Error` (assertion failed / code crashed) |
| `datasetLineNumber` | stable id for the row |

`input` lists candidate lines (each with an `LID:` line id). `expected_output` lists
the lines that SHOULD get a result and the correct answer id (`AID:`) for each. A line
present in `input` but absent from `expected_output` is a **hard negative** — the model
is supposed to abstain on it.

## Core workflow

1. **Acquire the run.** If the user gives a file, use it. If they give a
   suite/version on a server box, fetch it. If unsure which, ask.
2. **Run the bundled engine** (`scripts/Get-AITMetrics.ps1`) to compute metrics +
   categorize failures. Prefer `-AsJson` when you need to reason over the data; use
   the console form when the user just wants to read the report.
3. **Interpret**, don't just dump numbers. Identify the *dominant* failure mode and
   the discriminating axis (see below). Tie each metric back to model behavior.
4. **Pick an example** failing row and walk through exactly what the model did vs the
   ground truth, using the per-row `Reasons`.
5. **Locate the prompt under test** (see *Acquiring the prompt* below) — you cannot
   recommend edits to a prompt you have not read.
6. **Recommend** the highest-leverage fix. When it is a prompt change, map the
   *dominant failure category* to a concrete edit using the
   *Category → prompt remedy* table below, and quote the specific lines you would
   add/change. Flag `Crash` as a code fix, not a prompt fix. Offer a comparison if
   multiple versions exist (see *The baseline run registry*).

### Acquiring the prompt under test

Metric analysis is only half the job — the user usually wants prompt edits. Before
recommending any, get the actual prompt being evaluated:

- **Ask the user** for the prompt file if you don't know it, or
- **Find it in the app under test.** Production prompts ship as resource files in the
  app enlistment, typically `…\<App>\app\.resources\*.baseline.md` (e.g. the Bank Acc.
  Rec. baseline is `BankAccRecAITransToGLAccount1.baseline.md`). The file name often
  encodes the scenario.

Read the prompt and reason about *why its wording produces the dominant failure*
(e.g. a prompt that says "match each line to one of the accounts" implicitly forbids
abstention, driving `SpuriousMatch`). Only then propose edits.

### Category → prompt remedy (scenario-agnostic)

Once you know the dominant failure category, the prompt lever is largely the same
across matching scenarios — not just BAR:

| Dominant category | Prompt lever |
|-------------------|--------------|
| `SpuriousMatch` / low specificity | State that "no answer / no match" is a **valid and common** outcome; add a "when in doubt, do not answer" tie-breaker; add an explicit confidence threshold for what counts as a match. |
| `Miss` / low recall | Loosen the matching criteria; confirm the discriminating signal is actually present in the context; remove over-strict "only if absolutely certain" language. |
| `WrongAccount` / sibling confusion | Add disambiguation guidance for near-duplicate candidates; ask the model to pick the **most specific** match and to justify its choice among siblings. |
| `Crash` | **Not a prompt fix** — guard the production parser so malformed/empty output degrades to "no match" instead of erroring. |

See `references/failure-taxonomy.md` for the per-category root causes behind each lever.

## Running the engine

```powershell
# Saved export (repo-independent)
scripts\Get-AITMetrics.ps1 -ResultsPath .\ver29_results.json -ShowFailures

# Live fetch on a server box
scripts\Get-AITMetrics.ps1 -Fetch -SuiteCode 'BAR-AC1' -CodeunitId 133573 `
    -TestRunVersion 29 -CompanyName 'CRONUS International Ltd.' -Credential $cred

# Machine-readable (for your own analysis)
scripts\Get-AITMetrics.ps1 -ResultsPath .\ver29_results.json -AsJson | ConvertFrom-Json
```

The engine returns a summary object plus a `Failures` array (one entry per non-clean
row with `Categories` and line-level `Reasons`). The LID/AID/answer-pair regexes are
parameters (`-LineIdPattern`, `-ExpectedPattern`, `-AnswerPattern`, `-OutOfSetPattern`)
so the same engine serves other scenarios; the defaults target GL-account matching.

**"run N" means version N** — map a user's "run 61" to `-TestRunVersion 61`.

### Credentials & running under an agent (non-interactive shells)

Live fetch hits the BC OData API, which needs auth. Under an automated agent the
PowerShell session is **non-interactive**, so the usual prompts fail. Use this order:

1. Try `-UseDefaultCredentials` first (current Windows identity, no password).
2. On `401 Unauthorized`, fall back to an explicit `-Credential`. **Do not** rely on
   `Get-Credential` or a `Start-Process` GUI popup — the first throws
   *"PowerShell is in NonInteractive mode"* and the second is unreliable (often
   returns no credential). Instead, collect the username/password from the user and
   build the credential inline:

   ```powershell
   $sec  = ConvertTo-SecureString $pwd -AsPlainText -Force
   $cred = [System.Management.Automation.PSCredential]::new($user, $sec)
   scripts\Get-AITMetrics.ps1 -Fetch -SuiteCode 'BAR-AC1' -CodeunitId 133573 `
       -TestRunVersion 61 -CompanyName 'CRONUS International Ltd.' -Credential $cred -AsJson
   ```

3. Treat credentials as sensitive: keep them in-session only, never echo them back,
   and delete any temp artifacts (e.g. an exported `*.xml` credential file) when done.

### Handling large `-AsJson` output

A full run's `-AsJson` payload can be tens of KB and overflow an agent's tool-output
buffer. Don't dump it inline — write it to a file, then query the parts you need:

```powershell
scripts\Get-AITMetrics.ps1 -Fetch ... -AsJson > $env:TEMP\run.json
$j = Get-Content -Raw $env:TEMP\run.json | ConvertFrom-Json
$j.Summary | Format-List
$j.Failures | Where-Object { ($_.Categories -join ',') -eq 'SpuriousMatch' } | Select-Object -First 1
```

## Metrics and what they tell you

Metrics fall into two scopes. Be explicit about which you're citing — only the first
scope is meaningful for *every* AIT run.

### Task-agnostic metrics (any AIT run)

- **MatchRate** — fraction of rows fully correct (= the toolkit's pass/fail rate).
  All-or-nothing per row; good for ranking but blind to *how* a row failed.
- **Crash** — rows that errored with unusable model output (often a production parser
  bug). Counted for any run.

### Matching-scenario metrics (line-match + abstention tasks only)

**These apply only to the Bank Acc. Rec. family** — evals where each input line either
should map to an answer id or should be abstained on. They are produced by the
LID/AID/answer regex parameters and are **meaningless on a non-matching AIT scenario**
(e.g. summarization, free-text classification). The engine emits them by default and
suppresses them under `-GenericOnly`; in the summary object they sit alongside the
generic fields, and `Scenario` labels which task they describe.

- **Decision confusion matrix** (per line): TP / FP / FN / TN over the binary
  "should this line get a match?" decision → **Precision, Recall, Specificity, F1**.
  - **Recall** = did it find the true matches? **Specificity** = did it correctly
    abstain on hard negatives? These two often diverge sharply.
- **Sample-averaged P/R/F1** — per-row metrics averaged over rows; more forgiving than
  MatchRate (a mostly-right row still scores partial credit).
- **Account-level** (when answers carry answer-ids): splits true matches into
  **TP-correct** vs **TP-wrong-account** (a *sibling* confusion) → **sibling-confusion
  rate**. `TP-out-of-set` flags matches to entities outside the dataset.

**Interpretation heuristic:** compute *all* axes, then lead with whichever one
actually separates *this* run's models/versions — do not assume which it is.
**Verify, don't assume:** in the BC matching evals seen so far (the Bank Acc. Rec.
family) recall and sibling precision saturate early and **abstention (specificity)**
is the separating axis, but a different matching scenario could be separated by recall
or sibling-confusion instead. Confirm against the numbers before you assert a story,
and lead with the discriminating axis rather than MatchRate alone.

**Quantify the fix upside (addressable rows):** count failing rows whose `Categories`
contain exactly one category — those rows would flip to pass if that single failure mode
were fixed. This converts "specificity is low" into a concrete payoff (e.g. *"94 of 96
failing rows fail only on SpuriousMatch, so fixing abstention alone lifts MatchRate from
0.21 to ~0.98"*). Compute it from the `Failures` array and put it in the recommendation:

```powershell
($j.Failures | Where-Object { $_.Categories.Count -eq 1 -and $_.Categories[0] -eq 'SpuriousMatch' }).Count
```

## Failure taxonomy (matching-scenario categories the engine emits)

These categories also belong to the **matching-scenario** scope above (except `Crash`,
which is generic). A non-matching scenario won't produce Miss/SpuriousMatch/WrongAccount.

| Category | Meaning | Usual root cause |
|----------|---------|------------------|
| `Miss` | expected a match, model abstained (FN) | model too conservative / context too weak |
| `SpuriousMatch` | matched a hard negative that should abstain (FP) | weak abstention; prompt doesn't reward "no match" |
| `WrongAccount` | right line, wrong sibling answer-id | near-duplicate candidates; insufficient disambiguation |
| `Crash` | row errored with unusable output (generic) | production parser bug on malformed/empty model output |

See `references/failure-taxonomy.md` for the deeper version, including how the same
crash can be counted at row vs line level.

## Applying the engine to a new (non-matching) scenario

The matching metrics assume a line-match + abstention shape. For a different AIT task:

- If it's still line-matching but with a different answer format, override the
  `-LineIdPattern` / `-ExpectedPattern` / `-AnswerPattern` / `-OutOfSetPattern` regexes
  and set a descriptive `-ScenarioName`, then add a file under `scenarios/`.
- If it isn't a matching task at all, run with `-GenericOnly` and report only MatchRate
  and Crash — do **not** present precision/recall/specificity, which would be noise.

## Scenario subskills

Scenario-specific details (suite codes, codeunit ids, the exact answer/expected
formats, known production bugs, and what "good" looks like) live under `scenarios/`.
Load the matching one when the user names a feature:

- **Bank Account Reconciliation** (GL-account matching, suite `BAR-AC1`):
  `scenarios/BankAccountReconciliation.md`

When the user works on a scenario not yet documented, run the engine with the default
patterns first; if the answer/expected format differs, adjust the `-*Pattern`
parameters and consider adding a new file under `scenarios/`. Start from
`scenarios/_TEMPLATE.md`, which lays out the identity table, data formats, the four
regex overrides, "what good looks like," and known production bugs to capture.

## The baseline run registry

`baseline_prompt_runs.csv` is a registry of baseline-prompt runs across models and
versions. Use it to place a run in context — e.g. a user's "run 60" is GPT-5.4-nano
and the weakest of the baseline set — and as the source for cross-version/model
comparisons the workflow's final step offers. Columns: `Model, Version, Dataset,
Prompt, MatchRate, Matches, CostPerRunUSD`. Map "run N" to the `Version` column
(`vN`). When you analyze a run that isn't listed, append a row so the registry stays
current. The registry currently covers the Bank Acc. Rec. baseline; when you onboard a
non-BAR scenario, add a `Scenario` (or `Suite`) column so rows stay unambiguous across
scenarios.

## Output style

Be concise and lead with the finding. A good analysis answer contains: the headline
metric movement, the dominant failure category, one worked example, the discriminating
axis, and a concrete next step. Avoid dumping raw tables unless asked — summarize.
