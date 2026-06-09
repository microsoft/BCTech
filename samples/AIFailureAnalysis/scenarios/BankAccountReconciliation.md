# Scenario: Bank Account Reconciliation — GL-account matching

This subskill specializes the core AI-eval failure-analysis skill for the
**Bank Account Reconciliation With AI** app's GL-account-matching eval.

## Identity

| Property | Value |
|----------|-------|
| AIT suite code | `BAR-AC1` |
| Test codeunit id | `133573` (`Bank Rec. With AI Match Acc.`) |
| Production codeunit under test | `Bank Acc. Rec. Trans. to Acc.` (id 7251) |
| Entry point exercised | `GetMostAppropriateGLAccountNos` |
| Typical company | `CRONUS International Ltd.` |

The test maps each dataset transaction to the most appropriate G/L account, then
the codeunit reverse-maps the chosen G/L No back to the dataset's answer-id (AID)
so the logged output is directly comparable to `expected_output`.

## Data formats (engine defaults already match these)

- **input** lines: `LID: <lineno>, ...` (one candidate transaction per line).
- **expected_output** lines: `LID: 20000, AID: 2330` — the lines that should be
  matched and the correct account AID. Lines absent here are **hard negatives**
  (the model must return no match).
- **answer**: parenthesized pairs `(<LID>,<AID>)`, e.g. `(20000,2330)(30000,2415)`.
  If the model picks an account outside the dataset, the codeunit can't map it to an
  AID and emits a raw G/L No that looks like `GU0000…` — the engine buckets these as
  `TP-out-of-set` rather than scoring them as sibling errors.

Default engine patterns (`-LineIdPattern`, `-ExpectedPattern`, `-AnswerPattern`,
`-OutOfSetPattern`) are tuned for exactly this scenario, so no overrides are needed.

## What "good" looks like

On the **hard** dataset (122 rows), observed model behavior:

- **Recall is ~1.0 for every model** — finding the true match is effectively solved.
- **Sibling-confusion collapses to ~0–1% on capable models** — picking the right
  account among near-duplicates is solved by mid-tier models and up.
- **Specificity (correct abstention) is the discriminating axis** — it ranges widely
  (≈0.09 on the weakest small models to ≈0.45 on the strongest). When you explain a
  result here, **lead with specificity / SpuriousMatch count**, not MatchRate.

So the canonical story for this scenario: *"The model rarely misses a real match and
rarely picks the wrong sibling; almost all damage is failed abstention — it matches
hard-negative lines it should have left alone."* Verify that thesis against the actual
numbers before asserting it; don't assume.

## Known production bug — CopyStr crash (Crash category)

In `BankAccRecTransToAcc.Codeunit.al`, the answer parser uses parenthesis-offset
`CopyStr` calls that can crash on malformed or empty model output, producing rows with
`status = Error`. This is a **production** bug, not a test artifact, and shows up as the
`Crash` failure category. Two ways it surfaces:

- **Row level:** the crash fails the whole row → counts against MatchRate.
- **Line level:** the model's `answer` is captured *before* the production parser
  crashes, so the underlying match decision is still scorable from the log.

If you see `Crash` rows, call out that the fix is to guard the CopyStr offsets so
malformed output degrades to "no match" instead of erroring.

## Account-level availability

Account-level metrics require the answer to carry AIDs (the reverse-map). A single
out-of-dataset match (raw `GU…` G/L No) is bucketed as `TP-out-of-set` and does **not**
disable account-level for the run. Account-level only reports N/A when a pre-AID build
emitted opaque G/L Nos for *every* match.

## Suggested analysis flow for this scenario

1. Run the engine on the run (file or live fetch with the identity values above).
2. Report MatchRate, then immediately the specificity / SpuriousMatch count as the
   real story. Mention recall and sibling-confusion only to confirm they're solved.
3. If comparing versions, rank by MatchRate but explain rank changes via specificity.
4. Pick one `SpuriousMatch` row from the `Failures` array and show the hard-negative
   line the model wrongly matched.
5. Recommend a prompt nudge toward abstention as the highest-leverage fix; flag any
   `Crash` rows as a separate production fix.
