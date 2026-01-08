# API Reference

Complete interface and API specifications for the BCTalent.EscapeRoom framework.

---

## Interfaces

### iEscapeRoomVenue

**Purpose:** Defines venue behavior and structure

**Methods:**

```al
procedure GetVenueRec() VenueRec: Record "Escape Room Venue"
```
- Returns venue record with all fields populated
- Framework uses this to create/update venue in database
- Called during venue registration

```al
procedure GetVenue(): enum "Escape Room Venue"
```
- Returns enum value identifying this venue
- Used for filtering and identification
- Must match enum extension value

```al
procedure GetRooms() Rooms: list of [Interface iEscapeRoom]
```
- Returns ordered list of room implementations
- Order determines room sequence
- Framework creates room records from this list

```al
procedure GetRoomCompletedImage() InStr: InStream
```
- Returns image displayed when room completes
- Optional - can return empty stream
- Used for visual feedback/badges

```al
procedure GetVenueCompletedImage() InStr: InStream
```
- Returns image displayed when entire venue completes
- Optional - can return empty stream
- Used for completion badges/certificates

---

### iEscapeRoom

**Purpose:** Defines room behavior and tasks

**Methods:**

```al
procedure GetRoomRec() RoomRec: Record "Escape Room"
```
- Returns room record with all fields populated
- Includes Venue Id, Name, Description, Sequence
- Framework uses to create/update room in database

```al
procedure GetRoom(): enum "Escape Room"
```
- Returns enum value identifying this room
- Used for filtering and identification
- Must match enum extension value

```al
procedure GetTasks() Tasks: list of [Interface iEscapeRoomTask]
```
- Returns ordered list of task implementations
- Order determines task sequence
- Framework creates task records from this list

```al
procedure GetRoomDescription() Description: Text
```
- Returns plain text room description
- Used for quick preview
- More detailed description can be in BLOB field

```al
procedure GetRoomSolution() Solution: Text
```
- Returns solution text for room
- Displayed after solution delay expires
- Should provide complete solution guidance

```al
procedure GetHintImage() InStr: InStream
```
- Returns optional hint image
- Can be diagram, screenshot, etc.
- Return empty stream if no image

---

### iEscapeRoomTask

**Purpose:** Defines task behavior and validation

**Methods:**

```al
procedure GetTaskRec() TaskRec: Record "Escape Room Task"
```
- Returns task record with all fields populated
- Includes Venue Id, Room Name, Name, Description
- Framework uses to create/update task in database

```al
procedure GetTask(): enum "Escape Room Task"
```
- Returns enum value identifying this task
- Used for filtering and identification
- Must match enum extension value

```al
procedure IsValid(): Boolean
```
- Performs actual validation logic
- Returns true if task complete, false otherwise
- Called by framework to check task completion via `UpdateStatus()`
- **Common pattern**: Return `false` when using event subscribers for automatic monitoring
- **Polling pattern**: Return `true` when task conditions are met

```al
procedure GetHint(): Text
```
- Returns hint text for task
- Displayed when user requests hint (-1 point)
- Should guide without giving complete solution

---

## Validation Patterns

### Event Subscriber Pattern (Most Common in Dev1/Dev2)

Tasks return `false` from `IsValid()` and use event subscribers to monitor participant actions:

```al
SingleInstance = true;

var
    Room: Codeunit "My Room";

procedure IsValid(): Boolean
begin
    exit(false);  // Event handles validation
end;

[EventSubscriber(ObjectType::Table, Database::Customer, OnAfterInsertEvent, '', false, false)]
local procedure OnCustomerInsert(var Rec: Record Customer)
begin
    if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
        exit;
    
    if Rec.Name = 'BC Talent' then
        this.GetTaskRec().SetStatusCompleted();
end;
```

### Polling Pattern (Simple Checks)

Tasks return `true` from `IsValid()` when complete. Framework calls periodically:

```al
procedure IsValid(): Boolean
var
    Field: Record Field;
begin
    Field.SetRange(TableNo, Database::Item);
    Field.SetRange(FieldName, 'Information');
    exit(not Field.IsEmpty());  // True when field exists
end;
```

---

## Enums (Extensible)

### Escape Room Venue

**Purpose:** Identifies available venues

**Base Values:** None (all from extensions)

**Extension Pattern:**
```al
enumextension 50000 "My Venue" extends "Escape Room Venue"
{
    value(50000; MyVenue)
    {
        Caption = 'My Venue Name';
    }
}
```

**ID Range Recommendations:**
- 74000-74099: Development.1
- 74100-74199: Development.2  
- 74200-74299: Consultant.1
- 50000+: Custom extensions

---

### Escape Room

**Purpose:** Identifies available rooms

**Base Values:** None (all from extensions)

**Extension Pattern:**
```al
enumextension 50001 "My Rooms" extends "Escape Room"
{
    value(50001; IntroRoom)
    {
        Caption = 'Introduction Room';
    }
    value(50002; AdvancedRoom)
    {
        Caption = 'Advanced Room';
    }
}
```

**Naming Convention:** Use descriptive names, not codes

---

### Escape Room Task

**Purpose:** Identifies available tasks

**Base Values:** None (all from extensions)

**Extension Pattern:**
```al
enumextension 50002 "My Tasks" extends "Escape Room Task"
{
    value(50010; CreatePage)
    {
        Caption = 'Create Setup Page';
    }
    value(50011; AddField)
    {
        Caption = 'Add Custom Field';
    }
}
```

**Naming Convention:** Action-oriented names

---

### Escape Room Status

**Purpose:** Tracks room progress

**Values:**
- `NotStarted` - Default state
- `InProgress` - Room opened by participant
- `Completed` - All tasks finished

**NOT Extensible** - Fixed framework states

---

### Escape Room Task Status

**Purpose:** Tracks task completion

**Values:**
- `Open` - Task not yet completed
- `Completed` - Task validated successfully

**NOT Extensible** - Fixed framework states

---

## Framework Codeunits

### Codeunit 73922 "Escape Room"

**Purpose:** Core framework logic for venue/room/task management

**Public Procedures:**

```al
procedure UpdateVenue(Venue: Interface iEscapeRoomVenue)
```
- Main entry point for venue registration
- Creates/updates venue record
- Calls RefreshRooms to sync rooms
- Should be called from Install/Upgrade codeunits

**Parameters:**
- `Venue`: Implementation of iEscapeRoomVenue interface

**Usage:**
```al
local procedure RegisterMyVenue()
var
    EscapeRoom: Codeunit "Escape Room";
    MyVenue: Codeunit "My Venue Implementation";
begin
    EscapeRoom.UpdateVenue(MyVenue);
end;
```

---

**Internal Procedures** (framework use only):

```al
internal procedure RefreshRooms(Venue: Interface iEscapeRoomVenue)
```
- Syncs rooms for a venue
- Creates/updates room records
- Calls RefreshTasks for each room
- Called automatically by UpdateVenue

```al
internal procedure RefreshTasks(Room: Interface iEscapeRoom)
```
- Syncs tasks for a room
- Creates/updates task records
- Registers validation codeunits
- Called automatically by RefreshRooms

---

### Codeunit 73925 "Escape Room Telemetry"

**Purpose:** Telemetry logging and scoring

**Public Procedures:**

```al
procedure LogFinishedTask(var Task: Record "Escape Room Task")
```
- Logs task completion event
- Awards +3 points
- Call when task is validated

```al
procedure LogHintRequested(var Task: Record "Escape Room Task")
```
- Logs hint request event
- Deducts -1 point
- Call when user views hint

```al
procedure LogSolutionRequested(var Room: record "Escape Room")
```
- Logs solution request event
- Deducts -3 points
- Call when user views solution

```al
procedure LogRoomStarted(var Room: Record "Escape Room")
```
- Logs room start event
- No point change (0 points)
- Call when room status → InProgress

```al
procedure LogRoomCompleted(var Room: Record "Escape Room")
```
- Logs room completion event
- Awards +5 bonus points
- Call when all tasks complete

```al
procedure LogVenueCompleted(var Venue: Record "Escape Room Venue")
```
- Logs venue completion event
- Awards +10 bonus points
- Call when all rooms complete

```al
procedure LogNotification(NotificationText: Text)
```
- Logs notification sent to user
- No point change
- For tracking framework notifications

---

## Table APIs

### Escape Room Venue Table (73926)

**Fields:**
- `Id`: Text[50] - Primary key
- `Name`: Text[100] - Display name
- `Description`: Text[250] - Brief description
- `App ID`: Guid - Extension app identifier
- `Publisher`: Text[100] - Publisher name
- `Full Name`: Text[100] - Participant name (triggers Start)
- `Partner Name`: Text[100] - Partner/company name
- `Venue`: Enum - Venue identifier
- `Start DateTime`: DateTime - When started
- `Stop DateTime`: DateTime - When completed

**Key Procedures:**

```al
procedure Start()
```
- Sets Start DateTime
- Logs telemetry
- Called by OnValidate on Full Name/Partner Name

```al
procedure CloseVenueIfCompleted()
```
- Checks if all rooms complete
- Sets Stop DateTime
- Logs venue completion telemetry
- Call after room completion

---

### Escape Room Table (73920)

**Fields:**
- `Venue Id`: Text[50] - Parent venue (PK1)
- `Name`: Text[100] - Room name (PK2)
- `Description`: Text[250] - Brief description
- `Sequence`: Integer - Display order
- `Status`: Enum - NotStarted/InProgress/Completed
- `Room`: Enum - Room identifier
- `Big Description`: Blob - HTML description
- `Start DateTime`: DateTime - When started
- `Stop DateTime`: DateTime - When completed
- `SolutionDelayInMinutes`: Integer - Minutes before solution available
- `Solution`: Text[2048] - Solution text
- `Solution DateTime`: DateTime - When solution viewed
- `No. of Uncompleted Tasks`: Integer (FlowField) - Count of open tasks

**Key Procedures:**

```al
procedure UpdateStatus()
```
- Checks task completion
- Updates room status
- Call periodically or after task changes

```al
procedure CloseRoomIfCompleted()
```
- Checks if all tasks complete
- Sets room to Completed
- Opens next room
- Logs telemetry
- Calls CloseVenueIfCompleted
- Call after task completion

```al
procedure OpenNextRoom()
```
- Finds next room by sequence
- Sets status to InProgress
- Returns next room record

---

### Escape Room Task Table (73922)

**Fields:**
- `Venue Id`: Text[50] - Parent venue (PK1)
- `Room Name`: Text[100] - Parent room (PK2)
- `Name`: Text[100] - Task name (PK3)
- `Description`: Text[250] - Brief description
- `Sequence`: Integer - Display order
- `Status`: Enum - Open/Completed
- `Task`: Enum - Task identifier
- `TestCodeunitId`: Integer - Validation codeunit ID (0 = manual)
- `Stop DateTime`: DateTime - When completed
- `Hint`: Text[2048] - Hint text
- `Hint DateTime`: DateTime - When hint viewed

**Key Procedures:**

```al
procedure SetStatusCompleted()
```
- Sets status to Completed
- Sets Stop DateTime
- Logs telemetry (+3 points)
- Triggers room completion check
- Call after successful validation

```al
procedure UpdateStatus()
```
- Runs validation if TestCodeunitId set
- Calls SetStatusCompleted if validation passes
- Framework can call periodically

---

## Extension Points Summary

### Required for New Venue

1. ✅ Extend "Escape Room Venue" enum
2. ✅ Implement iEscapeRoomVenue interface
3. ✅ Call UpdateVenue() in Install codeunit

### Required for New Room

1. ✅ Extend "Escape Room" enum
2. ✅ Implement iEscapeRoom interface
3. ✅ Return from venue's GetRooms()

### Required for New Task

1. ✅ Extend "Escape Room Task" enum
2. ✅ Implement iEscapeRoomTask interface
3. ✅ Return from room's GetTasks()

### Optional for Task Validation

1. ✅ Implement "IEscape Room Task Validation" interface
2. ✅ Set TestCodeunitId in GetTaskRec()

---

## Backward Compatibility Guarantee

**Framework Version 1.x Promise:**
- All interfaces remain compatible
- New optional parameters only at end
- No removal of existing methods
- No breaking enum changes
- Telemetry event names stable

**Breaking Changes Require:**
- Major version bump (2.0+)
- Migration guide
- Deprecation period
- Clear communication

---

## Related Documentation

- [Framework Architecture](../Framework/Architecture.md) - Design patterns
- [Creating New Rooms](../Framework/Creating-Rooms.md) - Implementation guide
- [Task Validation System](../Framework/Task-Validation.md) - Validation details
- [Developer Guide](README.md) - Framework development

---

**Last Updated:** January 7, 2026
