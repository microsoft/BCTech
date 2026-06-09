# Failure taxonomy & metric reference

Reference for the AI-eval failure-analysis skill. The engine
(`scripts/Get-AITMetrics.ps1`) emits the categories and metrics below.

## Decision model

Every line in a row is one binary decision: *should this line receive a match?*

|                         | model matched | model abstained |
|-------------------------|---------------|-----------------|
| **should match**        | TP            | FN (Miss)       |
| **should NOT match**    | FP (Spurious) | TN              |

- **should match** = the line appears in `expected_output`.
- **should NOT match** = the line is in `input` but not `expected_output` (a hard
  negative).

## Failure categories

### Miss (FN)
The line had a correct answer but the model returned nothing.
- Signals: model is too conservative, or the prompt/context lacks the signal needed.
- Rare in practice — recall is usually near 1.0.
- **Prompt remedy:** loosen the matching criteria; verify the discriminating signal is
  actually present in the context; remove over-strict "only if absolutely certain"
  language that suppresses valid matches.

### SpuriousMatch (FP)
The model returned a match on a hard-negative line that should have been left alone.
- Signals: **weak abstention** — the dominant failure mode in most BC matching evals.
- Driven by prompts that implicitly reward "always answer". The highest-leverage fix is
  usually a prompt change that explicitly rewards "no match when uncertain".
- Specificity = TN / (TN + FP) is the headline metric for this category.
- **Prompt remedy:** state that "no answer / no match" is a valid and common outcome;
  add a "when in doubt, do not answer" tie-breaker; add an explicit confidence
  threshold for what qualifies as a match.

### WrongAccount (sibling confusion)
The model matched the correct *line* but chose the wrong answer-id — typically a
**sibling**: a near-duplicate candidate (e.g., two similarly named accounts).
- Only measurable when answers carry answer-ids (account-level available).
- sibling-confusion rate = TP-wrong / (TP-correct + TP-wrong).
- Tends to be solved by mid-tier models; a high rate points to candidates that are too
  close to disambiguate from the given context.
- **Prompt remedy:** add disambiguation guidance for near-duplicate candidates; ask the
  model to choose the most specific match and to justify its pick among siblings.

### Crash
The row errored (`status = Error`) with unusable output.
- Often a **production** parser bug on malformed/empty model output, not a test defect.
- Counted at the **row level** (fails the row → hurts MatchRate). The model's raw
  `answer`, if present, may still be captured *before* the crash, so the underlying
  decision can sometimes be scored at the **line level** even though the row errored.
- Fix the production parser (degrade malformed output to "no match") rather than the
  test. **This is a code fix, not a prompt fix** — do not propose prompt edits for it.

A single row can carry multiple categories (e.g., one Miss and one SpuriousMatch),
so category counts can sum to more than the number of failing rows.

## Metrics cheat-sheet

| Metric | Formula | Reads as |
|--------|---------|----------|
| MatchRate | rows fully correct / rows | toolkit pass/fail rate; all-or-nothing |
| Precision | TP / (TP+FP) | of matches made, how many were valid |
| Recall (sensitivity) | TP / (TP+FN) | of true matches, how many were found |
| Specificity | TN / (TN+FP) | of hard negatives, how many were correctly abstained |
| F1 | 2PR / (P+R) | harmonic mean of precision & recall |
| Balanced accuracy | (Recall + Specificity) / 2 | accuracy that ignores class imbalance |
| Sample P/R/F1 | per-row P/R/F1, averaged | partial credit per row; gentler than MatchRate |
| Sibling-confusion | TP-wrong / (TP-correct + TP-wrong) | wrong-account rate among matches |

### Sample-averaged empty-class convention
Per row, positive = "should match":
- if the row has no predicted matches → precision = 1 (can't be wrong about matches it
  didn't make);
- if the row has no true positives to find → recall = 1 (can't miss what isn't there);
- a forced match on a pure-negative row → precision 0 → row F1 = 0 (correctly punished).

## Choosing which metric to lead with

- **Ranking models / prompts:** MatchRate (and Sample F1 as a gentler companion).
- **Explaining *why* a run scored as it did:** lead with the axis that actually
  separates the runs. In BC matching evals that is almost always **specificity /
  SpuriousMatch**, because recall and sibling precision saturate early.
- **Spotting infrastructure problems:** a non-trivial `Crash` count means fix code,
  not the model or prompt.
