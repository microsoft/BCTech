# Copilot Instructions: Creating Escape Room Venues on BCTalent.EscapeRoom

## What Is This?

The **BCTalent.EscapeRoom** framework (ID range 73920-73999) enables developers to create gamified, task-validated learning experiences inside Business Central. You build a **venue app** — a separate AL extension — that depends on the framework and adds venues, rooms, and tasks using AL interfaces.

This file is the complete guide for an agent creating a new escape room venue from scratch. See [Docs/Framework/](Docs/Framework/) for detailed reference.

---

## Part 1: Project Setup

### app.json

```json
{
  "id": "your-guid-here",
  "name": "My Escape Room Venue",
  "publisher": "Your Name",
  "version": "1.0.0.0",
  "dependencies": [
    {
      "id": "f03c0f0c-d887-4279-b226-dea59737ecf8",
      "name": "BCTalent.EscapeRoom",
      "publisher": "waldo & AJ",
      "version": "1.3.0.0"
    }
  ],
  "idRanges": [{ "from": 50000, "to": 50099 }]
}
```

Always use the **vjeko-al-objid** tool (`getNextObjectId`) before creating any AL object to reserve a unique object ID. Never hardcode or guess IDs.

### Recommended Folder Structure

```
MyVenueApp/
├── Venue/
│   ├── MyVenue.Codeunit.al          (implements iEscapeRoomVenue)
│   └── EscapeRoomVenueExt.EnumExt.al
├── Rooms/
│   ├── Room1MyRoom.Codeunit.al      (implements iEscapeRoom)
│   └── EscapeRoomExt.EnumExt.al
├── Tasks/
│   ├── R1T1MyTask.Codeunit.al       (implements iEscapeRoomTask)
│   └── EscapeRoomTaskExt.EnumExt.al
├── Resources/
│   ├── Room1MyRoomDescription.html
│   ├── Room1MyRoomSolution.html
│   └── RoomCompletedImage.png
└── Install/
    └── InstallVenue.Codeunit.al     (registers venue via UpdateVenue)
```

---

## Part 2: Extending the Enums

Three enums must be extended — one for each level of the hierarchy.

```al
enumextension 50000 "My Venue Enum Ext" extends "Escape Room Venue"
{
    value(50000; MyVenue) { Caption = 'My Learning Venue'; }
}

enumextension 50001 "My Rooms Enum Ext" extends "Escape Room"
{
    value(50001; Room1Introduction) { Caption = 'Room 1: Introduction'; }
    value(50002; Room2Challenge) { Caption = 'Room 2: Challenge'; }
}

enumextension 50002 "My Tasks Enum Ext" extends "Escape Room Task"
{
    value(50010; R1Task1CreateField) { Caption = 'Create Custom Field'; }
    value(50011; R1Task2AddLogic) { Caption = 'Add Business Logic'; }
    value(50020; R2Task1OptimizeQuery) { Caption = 'Optimize Query'; }
}
```

---

## Part 3: Implementing the Interfaces

### 3.1 iEscapeRoomVenue

```al
codeunit 50000 "My Venue" implements iEscapeRoomVenue
{
    procedure GetVenueRec() VenueRec: Record "Escape Room Venue"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);
        VenueRec.Id := Me.Name;
        VenueRec.Name := Me.Name;
        VenueRec.Description := 'One-line description of what participants will do';
        VenueRec.Venue := Enum::"Escape Room Venue"::MyVenue;
        VenueRec."App ID" := Me.Id;
        VenueRec.Publisher := Me.Publisher;
    end;

    procedure GetVenue(): Enum "Escape Room Venue"
    begin
        exit(Enum::"Escape Room Venue"::MyVenue);
    end;

    procedure GetRooms() Rooms: List of [Interface iEscapeRoom]
    var
        Room1: Codeunit "Room1 Introduction";
        Room2: Codeunit "Room2 Challenge";
    begin
        Rooms.Add(Room1);
        Rooms.Add(Room2);
    end;

    procedure GetRoomCompletedImage() InStr: InStream
    begin
        // Return empty stream or load an image from resources
    end;

    procedure GetVenueCompletedImage() InStr: InStream
    begin
        // Return empty stream or load a completion badge
    end;
}
```

### 3.2 iEscapeRoom

```al
codeunit 50010 "Room1 Introduction" implements iEscapeRoom
{
    procedure GetRoomRec() RoomRec: Record "Escape Room"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);
        RoomRec."Venue Id" := Me.Name;
        RoomRec.Name := Format(this.GetRoom());
        RoomRec.Description := 'Short plain-text description for quick preview';
        RoomRec.Sequence := 1;
    end;

    procedure GetRoom(): Enum "Escape Room"
    begin
        exit(Enum::"Escape Room"::Room1Introduction);
    end;

    procedure GetTasks() Tasks: List of [Interface iEscapeRoomTask]
    var
        Task1: Codeunit "R1T1 Create Custom Field";
        Task2: Codeunit "R1T2 Add Business Logic";
    begin
        Tasks.Add(Task1);
        Tasks.Add(Task2);
    end;

    procedure GetRoomDescription() Description: Text
    begin
        // Return HTML content string, or leave empty and load via BLOB
        // See Resources/Room1IntroductionDescription.html
    end;

    procedure GetRoomSolution() Solution: Text
    begin
        // Return HTML content string
        // See Resources/Room1IntroductionSolution.html
    end;

    procedure GetHintImage() InStr: InStream
    begin
        // Optional — return empty stream if no image
    end;
}
```

### 3.3 iEscapeRoomTask

```al
codeunit 50020 "R1T1 Create Custom Field" implements iEscapeRoomTask
{
    SingleInstance = true;  // Required when using event subscribers

    var
        Room: Codeunit "Room1 Introduction";

    procedure GetTaskRec() TaskRec: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);
        TaskRec."Venue Id" := Me.Name;
        TaskRec."Room Name" := Format(Room.GetRoom());
        TaskRec.Name := Format(this.GetTask());
        TaskRec.Description := 'Add an "Information" field to the Customer table.';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(Enum::"Escape Room Task"::R1Task1CreateField);
    end;

    procedure IsValid(): Boolean
    var
        Field: Record Field;
    begin
        Field.SetRange(TableNo, Database::Customer);
        Field.SetRange(FieldName, 'Information');
        exit(not Field.IsEmpty());
    end;

    procedure GetHint(): Text
    begin
        exit('Add a text field named "Information" to the Customer table using a table extension.');
    end;
}
```

---

## Part 4: Registering the Venue

The venue must be registered on install (and upgrade). The framework creates all records automatically from the interface implementations.

```al
codeunit 50001 "Install My Venue"
{
    Subtype = Install;

    trigger OnInstallAppPerCompany()
    begin
        RegisterVenue();
    end;

    local procedure RegisterVenue()
    var
        EscapeRoom: Codeunit "Escape Room";
        MyVenue: Codeunit "My Venue";
    begin
        EscapeRoom.UpdateVenue(MyVenue);
    end;
}
```

The `UpdateVenue()` call walks the interface hierarchy (`GetRooms()` → `GetTasks()`) and creates or updates all records in the framework tables.

---

## Part 5: Task Validation Patterns

See [Docs/Framework/Task-Validation.md](Docs/Framework/Task-Validation.md) for full examples. Summary of the three patterns:

### Pattern 1: Polling

`IsValid()` returns `true` when the condition is met. Framework calls it periodically via `UpdateStatus()`.

**Use for:** Field existence, object existence, configuration checks, app installation.

```al
procedure IsValid(): Boolean
var
    Field: Record Field;
begin
    Field.SetRange(TableNo, Database::Customer);
    Field.SetRange(FieldName, 'Information');
    exit(not Field.IsEmpty());
end;
```

### Pattern 2: Event Subscriber

`IsValid()` returns `false`. A separate event subscriber detects when the participant performs the right action and sets a flag.

**Use for:** Monitoring table inserts/modifies, detecting specific user interactions, tracking data flow.

**Critical requirements:**
- Codeunit must have `SingleInstance = true`
- Use `OnAfterInsertEvent` for new records (NOT `OnAfterModifyEvent` — inserts and modifies are distinct events)
- Store state in a codeunit-level variable

```al
codeunit 50021 "R1T2 Add Business Logic" implements iEscapeRoomTask
{
    SingleInstance = true;

    var
        TaskCompleted: Boolean;

    procedure IsValid(): Boolean
    begin
        exit(TaskCompleted);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Sales Header", OnAfterInsertEvent, '', false, false)]
    local procedure OnSalesHeaderInserted(var Rec: Record "Sales Header"; RunTrigger: Boolean)
    begin
        if Rec."Document Type" <> Rec."Document Type"::Order then
            exit;
        TaskCompleted := true;
    end;
    // ... (GetTaskRec, GetTask, GetHint as usual)
}
```

### Pattern 3: Test Codeunit

Framework runs a test codeunit to validate complex UI or workflow scenarios.

```al
procedure GetTaskRec() TaskRec: Record "Escape Room Task"
begin
    // ...
    TaskRec.TestCodeunitId := Codeunit::"My Test Validation";  // Framework runs this
end;
```

---

## Part 6: Room and Task Design

### Task Selection: Only Observable Problems

Only include a task if the performance problem (or challenge) is **viscerally observable** — participants must be able to feel or clearly see the difference before and after.

- Exclude tasks that only matter at extreme scale not present in the demo environment
- Exclude tasks that are "interesting in theory" but produce no noticeable effect in practice
- A locking/blocking scenario requires **actual concurrent blocking** between two live sessions — a slow sequential query is NOT a locking demonstration

### For Performance Rooms: Key Rules

See [Docs/Framework/Performance-Room-Design.md](Docs/Framework/Performance-Room-Design.md) for the full guide. The essential rules:

1. **Minimum data:** At least 25,000 records are needed for performance differences to be perceptible in a group setting.

2. **NST caching:** Add `SelectLatestVersion()` to every page action that triggers a demo measurement, with this exact comment: `// DO NOT REMOVE — needed for consistent demo results`. Without it, the BC NST cache makes the second run appear artificially fast.

3. **Measurement code naming:** Use `R[room]-[SHORT-DESCRIPTION]` (e.g., `R4-ACTIVE`, `R7-N+1`). This code appears in **two places** — the page action that creates the measurement record, and the task's `IsValid()` that reads it. **They must match exactly.** A mismatch means the task can never be completed.

4. **Use `OnAfterInsertEvent`** when subscribing to Performance Measurement records — they are inserted, never modified.

5. **Compare vs. previous measurement**, not an absolute threshold. Require improvement in both duration AND SQL statement count.

6. **Two-extension architecture (code-based venues only):** When participants need to modify AL code, provide a companion code app (e.g., a PTE). Ideally that app depends only on the framework. If it also needs infrastructure from the venue app (e.g., measurement tables or manager codeunits), that dependency is acceptable provided `"dependencyPublishingOption": "Ignore"` is set in the companion app's `launch.json`. Data/configuration venues (e.g., a consultant track where participants work only in BC UI) have no companion app at all.

7. **Add `"dependencyPublishingOption": "Ignore"` to the companion app's `launch.json`** (code-based venues only) to prevent confusing errors when participants publish from VS Code.

### Selecting the Best Anti-Pattern Example

When choosing an example, rank by: (1) clarity of fix — small, targeted code change; (2) educational commonness — genuinely seen in production code; (3) measurability — dramatic, unambiguous improvement in SQL count and duration.

The canonical N+1 example: **`CalcFields` inside a `repeat...until` loop → `SetAutoCalcFields` before `FindSet`**. It meets all three criteria.

---

## Part 7: HTML Room Content

Every room has a Description HTML file (what participants receive) and a Solution HTML file (what the facilitator and the post-delay view show). See [Docs/Framework/Room-Content-HTML.md](Docs/Framework/Room-Content-HTML.md) for the complete guide. The essential rules follow.

### The One Rule That Overrides All Others

**Descriptions present the MYSTERY. Solutions reveal the ANSWER.**

The self-test: "Could they copy-paste something from this description to solve the task?" If yes, it is too revealing.

### BC HTML Viewer Constraints

No JavaScript, no external CSS, no emoji or emoticons (the BC HTML viewer cannot render them). Use inline styles only.

### Description File Structure (7 sections, in this order)

1. **HTML shell + H1** — `Room X: Short Title`
2. **TL;DR** — 2-3 sentences max. What's broken, what they'll do, how they prove success.
3. **The Challenge** — ONE paragraph (2-4 sentences). Sets scene + one-sentence "why this matters" woven in. No learning objectives.
4. **Your Mission** — H3 headings (descriptive names, never "Task 1:"). Each mission item has inline: goal, object/procedure reference, how-to-trigger steps (numbered `<ol>` for BC UI only), `<strong>Hint:</strong>`, and `<em>` validation note.
5. **Warning box** — Optional. Only for critical setup gotchas.
6. **Update Status reminder** — Required, just before What's Next.
7. **What's Next** — One sentence about the next room only. Omit for the final room.

**Hints must be VAGUE about technique and PRECISE about location.** Use the exact BC action label as it appears in the UI. For non-toolbar actions, include the full navigation path: `Actions → Analytics & Reporting → Customer Order Analytics`.

### Description Tone

| Do | Do Not |
|---|---|
| "Operations are slow" | "Record-by-record loops cause slowness" |
| "We test your ability to..." | "You will learn about..." |
| "Find a better approach" | "Use SetLoadFields to fix this" |
| "Research what AL offers" | "Use DataTransfer for this scenario" |

### Sections Forbidden in Descriptions

`Skills Tested / Learning Objectives` — `Key Concepts / Research Topics` — `Business Impact` — `Performance Results / Baseline tables` — `Profiler Comparisons` — `How Task Validation Works` — `"You will learn"` language — Task numbers in headings — Emoticons / emojis — Standalone `Where to Look` / `Validation` / `What's Next listing all rooms` sections.

### Solution File Structure

1. **H1:** `Solution: Room X - Title`
2. **Introduction** (optional brief paragraph)
3. **Task sections** with descriptive H2 names (not "Task 1:"), step-by-step `<ol>` instructions, complete `<pre>` code blocks, and `<h4>Why This Matters:</h4>` explanations
4. **Forbidden in solutions:** Performance Results comparison tables, Profiler Comparisons sections, task number references in headings
5. **Verification** checklist with Update Status reminder
6. **What You've Accomplished** summary

### File Naming

- Description: `Room[N][Topic]Description.html` — e.g., `Room2DataTransferDescription.html`
- Solution: `Room[N][Topic]Solution.html` — e.g., `Room2DataTransferSolution.html`

---

## Part 8: Quality Checklists

### AL Code Checklist

- [ ] All object IDs reserved via vjeko-al-objid before creating objects
- [ ] Three enum extensions created (Venue, Room, Task)
- [ ] `GetVenueRec()` populates Id, Name, Description, Venue, App ID, Publisher
- [ ] `GetRoomRec()` populates Venue Id, Name, Description, Sequence
- [ ] `GetTaskRec()` populates Venue Id, Room Name, Name, Description
- [ ] Install codeunit calls `EscapeRoom.UpdateVenue()`
- [ ] Task codeunits with event subscribers have `SingleInstance = true`
- [ ] Event subscribers use `OnAfterInsertEvent` (not Modify) where inserting records
- [ ] Measurement codes match exactly between page action writer and `IsValid()` reader
- [ ] `IsValid()` compares vs. previous measurement (not absolute threshold)
- [ ] Both duration AND SQL count required for performance task completion
- [ ] `SelectLatestVersion()` added to all performance demo page actions
- [ ] `// DO NOT REMOVE` comment on `SelectLatestVersion()` calls
- [ ] Companion code app (if any) does NOT depend on venue app (one-way dependency only)
- [ ] `"dependencyPublishingOption": "Ignore"` in companion app's `launch.json` (if applicable)

### HTML Description Checklist

- [ ] Complete HTML structure with DOCTYPE
- [ ] TL;DR (2-3 sentences max)
- [ ] The Challenge (ONE paragraph, "why this matters" woven in as one sentence)
- [ ] Your Mission with descriptive H3 headings (no task numbers)
- [ ] Each mission item is self-contained: goal + location + hint + validation inline
- [ ] Hints use the exact BC UI action label; non-toolbar actions include navigation path
- [ ] No standalone Skills Tested, Key Concepts, Validation, or Where to Look sections
- [ ] No Performance Results or Baseline tables
- [ ] Update Status reminder present
- [ ] What's Next mentions only the next room (one sentence)
- [ ] Problem described ONCE (no repetition across sections)
- [ ] MYSTERIOUS — no method names, no solution code, no technical terms that reveal the answer
- [ ] No "you will learn" language
- [ ] No emoticons or emojis

### HTML Solution Checklist

- [ ] H1 with "Solution: Room X - Title"
- [ ] Descriptive H2/H3 headings (no task numbers)
- [ ] Complete, copy-pasteable code in `<pre>` blocks
- [ ] "Why This Matters" after each major code block
- [ ] No Performance Results comparison tables
- [ ] No Profiler Comparisons sections
- [ ] Verification checklist with Update Status reminder
- [ ] "What You've Accomplished" summary
- [ ] No emoticons or emojis

---

## Reference

| Doc | Contents |
|---|---|
| [Architecture.md](Docs/Framework/Architecture.md) | Core components, design patterns, status management |
| [Creating-Rooms.md](Docs/Framework/Creating-Rooms.md) | Full step-by-step AL walkthrough |
| [Task-Validation.md](Docs/Framework/Task-Validation.md) | Complete examples for all three validation patterns |
| [Room-Content-HTML.md](Docs/Framework/Room-Content-HTML.md) | Full HTML content guide with all rules and examples |
| [Performance-Room-Design.md](Docs/Framework/Performance-Room-Design.md) | Performance-themed room patterns: NST caching, measurement codes, task selection |
| [Telemetry-Integration.md](Docs/Framework/Telemetry-Integration.md) | Application Insights events and scoring |
| [API-Reference.md](Docs/Dev/API-Reference.md) | Complete interface method specifications |
