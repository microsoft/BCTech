# Framework Architecture

## Overview

The BCTalent.EscapeRoom framework provides a complete infrastructure for creating gamified learning experiences in Business Central. The architecture is designed around **interface-based extensibility**, enabling other apps to add new venues, rooms, and tasks without modifying the framework code.

---

## Core Components

### Three-Level Hierarchy

```
┌─────────────────────────────┐
│  Escape Room Venue (73926)  │  ← Collection of themed rooms
│  - Id, Name, Venue Enum     │
│  - Full Name, Partner Name  │
│  - Start/Stop DateTime      │
└──────────────┬──────────────┘
               │
               ├─ implements: iEscapeRoomVenue interface
               │
               ▼
┌─────────────────────────────┐
│   Escape Room (73920)       │  ← Individual learning challenge
│   - Venue Id, Name          │
│   - Status, Sequence        │
│   - Description (BLOB)      │
│   - Start/Stop DateTime     │
│   - Solution + Delay        │
└──────────────┬──────────────┘
               │
               ├─ implements: iEscapeRoom interface
               │
               ▼
┌─────────────────────────────┐
│  Escape Room Task (73922)   │  ← Specific validation step
│  - Venue Id, Room Name      │
│  - Status, Sequence         │
│  - TestCodeunitId           │
│  - Hint, Stop DateTime      │
└─────────────────────────────┘
               │
               └─ validates via: IEscape Room Task Validation interface
```

---

## Key Design Patterns

### 1. Interface-Based Extension

**Extension apps do not modify framework tables directly.** Instead, they:

1. **Extend enums** - Add values for Venue, Room, and Task
2. **Implement interfaces** - Define behavior for venues, rooms, and tasks
3. **Call framework codeunit** - Register via `UpdateVenue()` procedure
4. **Framework auto-creates records** - Tables populated from interface data

**Example Flow:**
```al
// Extension App (e.g., Development.1)
codeunit 50100 "My Venue Implementation" implements iEscapeRoomVenue
{
    procedure GetVenueRec() VenueRec: Record "Escape Room Venue"
    begin
        VenueRec.Id := 'DEV1';
        VenueRec.Name := 'Development Track';
        VenueRec.Venue := "Escape Room Venue"::Development1;
    end;
    
    procedure GetRooms() Rooms: List of [Interface iEscapeRoom]
    begin
        // Return list of room implementations
    end;
}

// Framework automatically:
// 1. Creates/updates Venue record
// 2. Calls GetRooms() to get room list
// 3. Creates/updates Room records
// 4. Calls GetTasks() on each room
// 5. Creates/updates Task records
```

---

### 2. Automatic Record Management

**Codeunit 73922 "Escape Room"** handles all record creation:

```
UpdateVenue(Interface iEscapeRoomVenue)
    ↓
GetVenueRec() → Create/Update Venue Record
    ↓
RefreshRooms()
    ↓
GetRooms() → For each room interface:
    ↓
    GetRoomRec() → Create/Update Room Record
        ↓
        RefreshTasks()
            ↓
            GetTasks() → For each task interface:
                ↓
                GetTaskRec() → Create/Update Task Record
                    ↓
                    (Optional) Register TestCodeunitId for validation
```

**Key Logic:**
- Uses `Find('=')` to check existing records
- Creates if not found, updates if found
- Commits after each level to ensure data consistency
- Maintains sequence numbers automatically

---

### 3. Status Management

**Venues** (tracked via Start/Stop DateTime):
- Start: When Full Name or Partner Name validated
- Stop: When all rooms completed

**Rooms** (Enum "Escape Room Status"):
- `NotStarted` - Default state
- `InProgress` - When room opened
- `Completed` - When all tasks completed

**Tasks** (Enum "Escape Room Task Status"):
- `Open` - Not yet completed
- `Completed` - Validation passed

**Automatic Status Updates:**
- Tasks validated → Update task status
- All tasks complete → Room status becomes Completed
- All rooms complete → Venue Stop DateTime set
- Framework procedures: `CloseRoomIfCompleted()`, `CloseVenueIfCompleted()`

---

### 4. FlowField Calculations

**Room table** includes calculated field:
```al
field(30; "No. of Uncompleted Tasks"; Integer)
{
    FieldClass = FlowField;
    CalcFormula = count("Escape Room Task" 
        where("Venue Id" = field("Venue Id"), 
              "Room Name" = field(Name), 
              Status = const(Open)));
}
```

Used to automatically determine room completion status.

---

## Extension Points

### For Creating New Venues

**1. Extend Venue Enum:**
```al
enumextension 50100 "My Venue Extension" extends "Escape Room Venue"
{
    value(50100; MyVenue)
    {
        Caption = 'My Venue';
    }
}
```

**2. Implement iEscapeRoomVenue:**
```al
codeunit 50100 "My Venue Impl" implements iEscapeRoomVenue
{
    procedure GetVenueRec() VenueRec: Record "Escape Room Venue";
    procedure GetVenue(): enum "Escape Room Venue";
    procedure GetRooms() Rooms: list of [Interface iEscapeRoom];
    procedure GetRoomCompletedImage() InStr: InStream;
    procedure GetVenueCompletedImage() InStr: InStream;
}
```

**3. Register with Framework:**
```al
local procedure RegisterVenue()
var
    EscapeRoom: Codeunit "Escape Room";
    MyVenue: Codeunit "My Venue Impl";
begin
    EscapeRoom.UpdateVenue(MyVenue);
end;
```

---

### For Creating New Rooms

**1. Extend Room Enum:**
```al
enumextension 50101 "My Room Extension" extends "Escape Room"
{
    value(50101; MyRoom)
    {
        Caption = 'My Learning Room';
    }
}
```

**2. Implement iEscapeRoom:**
```al
codeunit 50101 "My Room Impl" implements iEscapeRoom
{
    procedure GetRoomRec() RoomRec: Record "Escape Room";
    procedure GetRoom(): enum "Escape Room";
    procedure GetTasks() Tasks: list of [Interface iEscapeRoomTask];
    procedure GetRoomDescription() Description: Text;
    procedure GetRoomSolution() Solution: Text;
    procedure GetHintImage() InStr: InStream;
}
```

---

### For Creating New Tasks

**1. Extend Task Enum:**
```al
enumextension 50102 "My Task Extension" extends "Escape Room Task"
{
    value(50102; MyTask)
    {
        Caption = 'Complete Challenge';
    }
}
```

**2. Implement iEscapeRoomTask:**
```al
codeunit 50102 "My Task Impl" implements iEscapeRoomTask
{
    procedure GetTaskRec() TaskRec: Record "Escape Room Task";
    procedure GetTask(): enum "Escape Room Task";
    procedure GetTestCodeunitId(): Integer;
    procedure GetHint(): Text;
}
```

**3. (Optional) Implement Validation:**
```al
codeunit 50103 "My Task Validation" implements "IEscape Room Task Validation"
{
    procedure TaskCode(): Code[20]
    begin
        exit('MYTASK');
    end;
    
    procedure GetValidationDescription(): Text
    begin
        exit('Validates that challenge is complete');
    end;
    
    procedure ValidateTask(TaskCode: Code[20]): Boolean
    var
        // Custom validation logic
    begin
        // Return true if task is complete
    end;
}
```

---

## Data Model

### Escape Room Venue Table (73926)

| Field | Type | Description |
|-------|------|-------------|
| Id | Text[50] | Unique venue identifier (PK) |
| Name | Text[100] | Display name |
| Description | Text[250] | Brief description |
| App ID | Guid | Extension app identifier |
| Publisher | Text[100] | App publisher |
| Full Name | Text[100] | Participant full name (triggers Start) |
| Partner Name | Text[100] | Participant partner name |
| Venue | Enum | Venue enum value |
| Start DateTime | DateTime | When venue started |
| Stop DateTime | DateTime | When venue completed |

---

### Escape Room Table (73920)

| Field | Type | Description |
|-------|------|-------------|
| Venue Id | Text[50] | Parent venue (PK1) |
| Name | Text[100] | Room name (PK2) |
| Description | Text[250] | Brief description |
| Sequence | Integer | Order within venue |
| Status | Enum | NotStarted / InProgress / Completed |
| Room | Enum | Room enum value |
| Big Description | Blob | Full HTML description |
| Start DateTime | DateTime | When room started |
| Stop DateTime | DateTime | When room completed |
| SolutionDelayInMinutes | Integer | Minutes before solution available (default 10) |
| Solution | Text[2048] | Solution text |
| Solution DateTime | DateTime | When solution was viewed |
| No. of Uncompleted Tasks | Integer | FlowField counting open tasks |

---

### Escape Room Task Table (73922)

| Field | Type | Description |
|-------|------|-------------|
| Venue Id | Text[50] | Parent venue (PK1) |
| Room Name | Text[100] | Parent room (PK2) |
| Name | Text[100] | Task name (PK3) |
| Description | Text[250] | Brief description |
| Sequence | Integer | Order within room |
| Status | Enum | Open / Completed |
| Task | Enum | Task enum value |
| TestCodeunitId | Integer | Validation codeunit ID |
| Stop DateTime | DateTime | When task completed |
| Hint | Text[2048] | Hint text |
| Hint DateTime | DateTime | When hint was viewed |

---

## UI Components

### Pages

- **Escape Room Venue List (73927)** - List of all venues
- **Escape Room Venue Card (73926)** - Venue details with room list
- **Escape Room Page (73920)** - Room card with task list
- **Escape Room Task List (73923)** - Task details
- **Picture Page (73928)** - Display images (badges)
- **Rich Text Box Page (73929)** - Display HTML descriptions/solutions

### Page Extensions

- **Business Manager RC Ext (73921)** - Adds "Escape Room Venues" action to Role Center

---

## Timing and Workflow

**Venue Lifecycle:**
1. Extension app calls `UpdateVenue()` (typically on install/upgrade)
2. Framework creates Venue, Room, and Task records
3. User opens Venue Card, enters Full Name/Partner Name
4. OnValidate trigger calls `Start()` → Sets Start DateTime
5. User opens first room → Room status becomes InProgress
6. User completes tasks → Tasks become Completed
7. All tasks complete → `CloseRoomIfCompleted()` sets room Completed
8. All rooms complete → `CloseVenueIfCompleted()` sets Stop DateTime
9. Telemetry tracks all events for leaderboard

**Room Navigation:**
- `OpenNextRoom()` procedure finds next room by sequence
- Sets current key and ascending order for proper ordering
- Only InProgress or NotStarted rooms can be opened

---

## Key Procedures

### Codeunit 73922 "Escape Room"

```al
procedure UpdateVenue(Venue: Interface iEscapeRoomVenue)
// Main entry point - creates/updates venue and all children

procedure RefreshRooms(Venue: Interface iEscapeRoomVenue)
// Creates/updates all rooms for a venue

procedure RefreshTasks(Room: Interface iEscapeRoom)
// Creates/updates all tasks for a room
```

### Escape Room Venue Table Triggers

```al
trigger OnValidate() on "Full Name" and "Partner Name"
// Automatically calls Start() when participant info entered

procedure Start()
// Sets Start DateTime and logs telemetry
```

### Escape Room Table Procedures

```al
procedure UpdateStatus()
// Checks task completion and updates room status

procedure CloseRoomIfCompleted()
// Sets room to Completed when all tasks done
// Opens next room automatically
// Logs telemetry for room completion
```

### Escape Room Task Table Procedures

```al
procedure SetStatusCompleted()
// Marks task complete and logs telemetry
// Triggers room completion check
```

---

## Best Practices

### For Extension Developers

1. **Always implement all interface methods** - Framework expects complete implementations
2. **Use enum extensions** - Never hardcode values, always extend enums
3. **Sequence matters** - Tasks/rooms execute in sequence order
4. **Commit after registration** - Call `UpdateVenue()` in install/upgrade codeunits
5. **Test validation logic** - Ensure task validation is reliable and repeatable
6. **Provide helpful hints** - Users rely on hints when stuck
7. **Solution delays** - Set appropriate delay (10+ minutes) before solution available

### For Framework Maintainers

1. **Preserve interface contracts** - Breaking changes affect all extensions
2. **Maintain telemetry consistency** - Leaderboard queries depend on event structure
3. **Test with multiple venues** - Ensure proper isolation between venues
4. **Validate sequence ordering** - Room/task order must be deterministic
5. **Handle concurrent users** - Multiple participants may complete simultaneously

---

## Related Documentation

- [Creating New Rooms](Creating-Rooms.md) - Step-by-step guide for building rooms
- [Task Validation System](Task-Validation.md) - How validation interfaces work
- [Telemetry Integration](Telemetry-Integration.md) - Scoring and tracking details
- [API Reference](../Dev/API-Reference.md) - Complete interface documentation

---

**Last Updated:** January 7, 2026
