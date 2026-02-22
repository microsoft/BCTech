# Performance-Themed Room Design

## Overview

This guide covers patterns and rules specific to escape rooms where participants must diagnose and fix **performance problems** — slow queries, N+1 patterns, locking, or suboptimal AL code.

Performance rooms can be structured in two ways:

### Code-Based Venues (Two-Extension Architecture)

Used when participants must **write or modify AL code** to fix the challenge:

- **Venue app** (e.g., `OptimAL.EscapeRoom1`) — the framework implementation, rooms, tasks, HTML content, and measurement logic
- **Companion code app** (e.g., `OptimAL.PTE`) — the "broken" extension participants download, fix, and republish

This is the right pattern when the fix requires editing AL source code (codeunit procedures, table extensions, etc.).

### Data/Configuration Venues (Single-Extension Architecture)

Used when participants are **Business Central users or consultants** working entirely in the BC UI — configuring settings, creating records, running reports, or analysing data. There is **no companion code app**. Task validation uses the Polling pattern (checking for records, field values, or configuration) or the Event Subscriber pattern (monitoring table inserts/modifies).

Example: A consultant-track venue that tests knowledge of BC functional areas. Participants set up data, run processes, or configure modules — all inside BC. The venue app is the only extension.

The rest of this document covers patterns that apply to **both** architectures, with the two-extension-specific rules called out explicitly.

---

## Two-Extension Architecture (Code-Based Venues Only)

> This section applies only when participants need to modify and republish AL code. Skip this for data/configuration venues.

### Dependency Direction

The ideal dependency graph is strictly one-way:

```
OptimAL.EscapeRoom1  ──depends on──►  BCTalent.EscapeRoom (framework)
OptimAL.PTE          ──depends on──►  BCTalent.EscapeRoom (framework)
```

The companion app ideally depends only on the framework. However, if it also needs infrastructure from the venue app (for example, a performance measurement table or manager codeunit that lives in the venue app), a dependency on the venue app is acceptable — provided `"dependencyPublishingOption": "Ignore"` is set in the companion app's `launch.json` (see below).

**Why this matters:** Without `"dependencyPublishingOption": "Ignore"`, publishing from VS Code temporarily uninstalls all dependency apps including the venue app, causing "could not be loaded" errors. The `Ignore` setting prevents this.

### Publishing from VS Code

Participants publish the companion app from VS Code using F5. Add this to its `launch.json`:

```json
{
    "dependencyPublishingOption": "Ignore"
}
```

Without this, the dependent venue app gets briefly uninstalled during the companion app's publishing cycle, causing confusing errors.

---

## Task Selection: Only Observable Problems

A performance task earns its place only if attendees can **physically feel the slowness** with the current dataset.

**Include a task if:**
- The operation takes several seconds with the test dataset
- Participants can trigger it easily from the UI and compare before/after themselves
- The improvement after fixing is dramatic and unambiguous (e.g., 30 seconds → 1 second)

**Do NOT include a task if:**
- The problem only shows at extreme scale (>1M records) not present in the demo environment
- The performance difference requires instrumentation to notice
- The scenario is "interesting in theory" but produces no observable difference in practice
- The locking scenario requires timing that is impossible to reproduce consistently

### Locking / Blocking Scenarios

A "blocking" or "locking" demo requires **actual concurrent blocking** between two live sessions:

- Two browser tabs logged in as different users
- One tab runs a long-running write (without committing)
- The other tab is blocked attempting to read or write the same records

A **slow sequential query** does not demonstrate locking. Do not conflate the two — participants will be confused. If you cannot demonstrate actual blocking reliably in a workshop, do not include the task.

---

## Minimum Data Requirements

At least **25,000 records** are needed for performance problems to register as meaningfully slow in a group workshop setting. Smaller datasets may produce imperceptible differences.

Include a **"Generate More Test Data"** action in the PTE app or venue app. If the environment has fewer than 25,000 records, the description Warning box must instruct participants to run this first.

---

## Measuring Performance: The Measurement Pattern

Performance rooms use a custom measurement table in the venue app to record operation metrics. The key types are duration and SQL statement count.

### Measurement Code Naming Convention

Format: `R[room]-[SHORT-DESCRIPTION]`

Examples:
- `R4-ACTIVE` — Room 4, Active Customer Report
- `R7-N+1` — Room 7, N+1 query operation
- `R2-TRANSFER` — Room 2, table transfer operation

The measurement code is the **primary key** that connects:
1. The page action that **creates** the measurement record
2. The task codeunit's `IsValid()` that **reads** the measurement record

**These two strings must match exactly.** A mismatch means the task can never be completed — participants will see a task stuck permanently on "Open" with no error message.

**Verification step after implementing any task:** Search the codebase for both occurrences of the measurement code string and confirm they are identical.

---

## NST Caching: The SelectLatestVersion Rule

The BC NST (NAV Service Tier) caches data between requests. On the **second run** of a performance measurement, the NST cache can make the operation appear dramatically faster than it actually is. This destroys the demo — participants will think they have "fixed" the problem when they have not.

**Fix:** Add `SelectLatestVersion()` to every page action that triggers a performance measurement demo:

```al
action(RunActiveCustomerReport)
{
    ApplicationArea = All;
    Caption = 'Run Active Customer Report';

    trigger OnAction()
    begin
        // DO NOT REMOVE — SelectLatestVersion needed for consistent demo results
        // Without it, NST caching makes the second run appear artificially fast.
        CurrPage.SetSelectionFilter(Rec);
        Rec.SelectLatestVersion();

        PerformanceMeasurementMgr.StartMeasurement('R4-ACTIVE');
        RunActiveCustomerReport();
        PerformanceMeasurementMgr.StopMeasurement('R4-ACTIVE');
    end;
}
```

Add the comment `// DO NOT REMOVE — ... needed for demo purposes` so that future developers do not clean it up.

---

## Task Validation for Performance Rooms

Performance rooms use `IsValid()` on a task codeunit to decide whether a task is complete. The two common approaches are polling the measurement table directly or subscribing to table events.

### Which Event to Subscribe To

The Performance Measurement pattern has two phases:

- `StartMeasurement()` **inserts** a new record (the "start" entry).
- `StopMeasurement()` **modifies** that same record to write Duration, SQL count, and other metrics.

Choose the event based on what you want to detect:

| Goal | Event |
|---|---|
| Detect that the participant ran the operation at all | `OnAfterInsertEvent` — fires when `StartMeasurement()` inserts the record |
| Validate the actual performance metrics (duration, SQL count) | `OnAfterModifyEvent` — fires when `StopMeasurement()` writes the results |

Example — subscribe to metrics written by `StopMeasurement()`:

```al
[EventSubscriber(ObjectType::Table, Database::"Performance Measurement", OnAfterModifyEvent, '', false, false)]
local procedure OnMeasurementCompleted(var Rec: Record "Performance Measurement")
begin
    if Rec.Code <> 'R4-ACTIVE' then
        exit;
    if Rec.SqlStatementsCount < 10 then
        TaskCompleted := true;
end;
```

Example — subscribe to record existence inserted by `StartMeasurement()`:

```al
[EventSubscriber(ObjectType::Table, Database::"Performance Measurement", OnAfterInsertEvent, '', false, false)]
local procedure OnMeasurementStarted(var Rec: Record "Performance Measurement")
begin
    if Rec.Code <> 'R1-BASELINE-UPGRADE' then
        exit;
    TaskCompleted := true;  // Participant triggered the operation
end;
```

### Validation Approach

Two approaches work well for validating performance improvements:

**Absolute threshold** — require the metric to fall below a fixed value (e.g., SQL count < 10). Simpler, deterministic, and works without a prior baseline. Better for workshop settings where participants may not have a baseline measurement recorded.

**Relative comparison** — compare the latest measurement against the previous measurement for the same code. Requires the participant to run the operation twice. More flexible but fails if no baseline exists.

Example of absolute threshold (simpler for workshops):

```al
procedure IsValid(): Boolean
var
    Measurement: Record "Performance Measurement";
begin
    Measurement.SetRange(Code, 'R4-ACTIVE');
    Measurement.SetCurrentKey(EntryNo);
    if not Measurement.FindLast() then
        exit(false);
    exit(Measurement.SqlStatementsCount < 10);
end;
```

Example of relative comparison (if a baseline is always present):

```al
local procedure ValidateCurrentMeasurement(NewMeasurement: Record "Performance Measurement"): Boolean
var
    PreviousMeasurement: Record "Performance Measurement";
begin
    PreviousMeasurement.SetRange(Code, NewMeasurement.Code);
    PreviousMeasurement.SetFilter(EntryNo, '<%1', NewMeasurement.EntryNo);
    if not PreviousMeasurement.FindLast() then
        exit(false);  // No baseline yet
    exit(
        (NewMeasurement.SqlStatementsCount < PreviousMeasurement.SqlStatementsCount * 0.1) and
        (NewMeasurement.DurationMs < PreviousMeasurement.DurationMs * 0.5)
    );
end;
```

### SingleInstance and Event Subscribers

If the task codeunit uses event subscribers and stores state in codeunit-level variables, add `SingleInstance = true`:

```al
codeunit 74250 "R4 Active Customer Task" implements iEscapeRoomTask
{
    SingleInstance = true;
    // SingleInstance required: event subscriber sets TaskCompleted flag on this instance

---

## Selecting the Best Anti-Pattern Example

When choosing which performance anti-pattern to demonstrate in a room, rank candidates by:

1. **Clarity of fix** — The correct solution is a small, targeted code change. Not a restructuring.
2. **Educational commonness** — This pattern genuinely appears in real production AL code. Developers recognise it.
3. **Measurability** — Fixing it produces a dramatic, unambiguous drop in SQL statement count and duration.

### Canonical Examples by Room Type

| Room Type | Anti-Pattern | Fix | Why It Works |
|---|---|---|---|
| N+1 Queries | `CalcFields` in a loop | `SetAutoCalcFields` before `FindSet` | Clear one-liner fix; N queries → 1; extremely common in production |
| Bulk Transfer | Record-by-record Copy loop | `DataTransfer` object | Small change; thousands of inserts → 1 operation |
| Bulk Update | Field update in a loop | `ModifyAll` | One call replaces the entire loop |
| Profiler | Any slow procedure | Identify via AL Profiler | Teaches the discovery process, not just the fix |

**The best N+1 example:** `CalcFields` inside a `repeat...until` loop. It has a clear, elegant fix (`SetAutoCalcFields`), produces dramatic measurable improvement (N queries → 1), and is the most common N+1 pattern real developers write.

---

## Companion App Starting State (Code-Based Venues Only)

> This section applies only when a companion code app exists.

The companion app ships with **deliberately un-optimized code** — that is the escape room starting state. The Solution HTML files describe exactly what code changes solve each problem. After designing all rooms:

1. Write the Solution HTML files with the exact "good" code
2. Implement the "bad" code in the companion app (the opposite of the solution)
3. Verify that running the operations in a fresh environment produces the expected slowness

**Do not add "good" code to the companion app during room design.** The version shipped to attendees must have the unoptimized code. Participants replace it when they fix each room.
