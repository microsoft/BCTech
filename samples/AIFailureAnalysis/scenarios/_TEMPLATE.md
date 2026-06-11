# Scenario: <Feature name> — <one-line task description>

Template for documenting a new AIT scenario for the AI-eval failure-analysis skill.
Copy this file to `scenarios/<FeatureName>.md`, fill every section, then link it from
the *Scenario subskills* list in `SKILL.md`. Delete guidance lines (the italic notes)
as you fill them in.

## Identity

| Property | Value |
|----------|-------|
| AIT suite code | `<SUITE-CODE>` |
| Test codeunit id | `<id>` (`<codeunit name>`) |
| Production codeunit under test | `<name>` (id `<id>`) |
| Entry point exercised | `<method name>` |
| Typical company | `CRONUS International Ltd.` |
| Baseline prompt file | `…\<App>\app\.resources\<name>.baseline.md` |

*The prompt file is what you edit when recommending fixes — record its exact path.*

## Data formats

*Describe how `input`, `expected_output`, and `answer` are shaped for this scenario,
and which lines are hard negatives (present in `input`, absent from `expected_output`).*

- **input** lines: `<describe LID format>`.
- **expected_output** lines: `<describe LID/AID format>`. Lines absent here are
  **hard negatives** (the model must return no match).
- **answer**: `<describe the model output format>`.

## Engine pattern overrides

*If the engine defaults (tuned for BAR GL-matching) don't match this scenario's
formats, record the overrides here so anyone can reproduce the analysis.*

```powershell
scripts\Get-AITMetrics.ps1 -Fetch -SuiteCode '<SUITE-CODE>' -CodeunitId <id> `
    -TestRunVersion <N> -CompanyName 'CRONUS International Ltd.' -Credential $cred -AsJson `
    -ScenarioName '<descriptive name>' `
    -LineIdPattern   '<regex>' `
    -ExpectedPattern '<regex>' `
    -AnswerPattern   '<regex>' `
    -OutOfSetPattern '<regex>'
```

*If this is not a line-match + abstention task at all, use `-GenericOnly` and report
only MatchRate and Crash — precision/recall/specificity would be noise.*

## What "good" looks like

*Record observed model behavior so the next analyst knows which axis to lead with.
Do NOT copy BAR's "specificity is the discriminating axis" claim blindly — measure it
for this scenario. Note here which axis (recall, specificity, sibling-confusion)
actually separates models, with rough ranges.*

## Known production bugs

*List any production-code defects that surface as `Crash` rows (e.g. an unguarded
parser), with the source file and the fix. These are code fixes, not prompt fixes.*

## Suggested analysis flow for this scenario

1. Run the engine on the run (file or live fetch with the identity values above).
2. Report MatchRate, then lead with the axis that actually separates the runs here.
3. If comparing versions, rank by MatchRate but explain rank changes via that axis.
4. Pick one failing row from the `Failures` array and walk through the worked example.
5. Read the baseline prompt file, then recommend the highest-leverage prompt edit using
   the *Category → prompt remedy* table in `SKILL.md`; flag any `Crash` rows as a
   separate production fix.
