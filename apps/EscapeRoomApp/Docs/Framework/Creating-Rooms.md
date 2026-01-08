# Creating New Rooms - Step-by-Step Guide

## Overview

This guide walks you through creating new escape rooms for the BCTalent.EscapeRoom framework. You'll learn how to define venues, rooms, and tasks using the interface-based extension pattern.

---

## Prerequisites

- Business Central AL development environment
- BCTalent.EscapeRoom framework app as dependency
- Understanding of AL interfaces and enums
- Familiarity with [Framework Architecture](Architecture.md)

---

## Step 1: Create Your Extension App

### 1.1 Create New AL Project

```json
// app.json
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
      "version": "1.0.0.0"
    }
  ],
  "idRanges": [
    {
      "from": 50000,
      "to": 50099
    }
  ]
}
```

### 1.2 Project Structure

```
MyEscapeRoomVenue/
  ├── Venue/
  │   ├── MyVenue.Codeunit.al (implements iEscapeRoomVenue)
  │   └── MyVenueEnum.EnumExt.al
  ├── Rooms/
  │   ├── Room1.Codeunit.al (implements iEscapeRoom)
  │   ├── Room2.Codeunit.al
  │   └── RoomsEnum.EnumExt.al
  ├── Tasks/
  │   ├── Task1.Codeunit.al (implements iEscapeRoomTask)
  │   ├── Task1Validator.Codeunit.al (implements IEscapeRoomTaskValidation)
  │   └── TasksEnum.EnumExt.al
  └── Install/
      └── InstallVenue.Codeunit.al (registers venue)
```

---

## Step 2: Define Enums

### 2.1 Extend Venue Enum

```al
enumextension 50000 "My Venue Extension" extends "Escape Room Venue"
{
    value(50000; MyVenue)
    {
        Caption = 'My Learning Venue';
    }
}
```

### 2.2 Extend Room Enum

```al
enumextension 50001 "My Rooms Extension" extends "Escape Room"
{
    value(50001; IntroRoom)
    {
        Caption = 'Introduction Room';
    }
    value(50002; ChallengeRoom)
    {
        Caption = 'Challenge Room';
    }
    value(50003; AdvancedRoom)
    {
        Caption = 'Advanced Room';
    }
}
```

### 2.3 Extend Task Enum

```al
enumextension 50002 "My Tasks Extension" extends "Escape Room Task"
{
    value(50010; CreateSetupPage)
    {
        Caption = 'Create Setup Page';
    }
    value(50011; AddField)
    {
        Caption = 'Add Custom Field';
    }
    value(50012; ImplementLogic)
    {
        Caption = 'Implement Business Logic';
    }
}
```

---

## Step 3: Implement Venue

### 3.1 Create Venue Codeunit

```al
codeunit 50000 "My Venue Implementation" implements iEscapeRoomVenue
{
    procedure GetVenueRec() VenueRec: Record "Escape Room Venue"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        VenueRec."Id" := Me.Name;
        VenueRec.Name := Me.Name;
        VenueRec.Description := 'Learn BC development through hands-on challenges';
        VenueRec.Venue := enum::"Escape Room Venue"::MyVenue;
        VenueRec."App ID" := Me.Id;
        VenueRec.Publisher := Me.Publisher;
    end;

    procedure GetVenue(): enum "Escape Room Venue"
    begin
        exit(enum::"Escape Room Venue"::MyVenue);
    end;

    procedure GetRooms() Rooms: List of [Interface iEscapeRoom]
    begin
        // Add rooms by enum - framework automatically instantiates
        Rooms.Add(enum::"Escape Room"::IntroRoom);
        Rooms.Add(enum::"Escape Room"::ChallengeRoom);
        Rooms.Add(enum::"Escape Room"::AdvancedRoom);
    end;

    procedure GetRoomCompletedImage() InStr: InStream
    begin
        // Load PNG from Resources folder
        NavApp.GetResource('RoomCompleted.png', InStr);
    end;

    procedure GetVenueCompletedImage() InStr: InStream
    begin
        // Load PNG from Resources folder
        NavApp.GetResource('VenueCompleted.png', InStr);
    end;
}
```

---

## Step 4: Implement Rooms

### 4.1 Create Room Codeunit

```al
codeunit 50001 "My Intro Room" implements iEscapeRoom
{
    procedure GetRoomRec() RoomRec: Record "Escape Room"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        RoomRec."Venue Id" := Me.Name;
        RoomRec.Name := Format(this.GetRoom());
        RoomRec.Description := 'Get started with BC development basics';
        RoomRec.SolutionDelayInMinutes := 15;
        
        // Set rich description
        SetBigDescription(RoomRec);
    end;

    procedure GetRoom(): enum "Escape Room"
    begin
        exit("Escape Room"::IntroRoom);
    end;

    procedure GetRoom(): Enum "Escape Room"
    begin
        exit(enum::"Escape Room"::IntroRoom);
    end;

    procedure GetTasks() Tasks: List of [Interface iEscapeRoomTask]
    begin
        // Add tasks by enum - framework automatically instantiates
        Tasks.Add(enum::"Escape Room Task"::CreateSetupPage);
        Tasks.Add(enum::"Escape Room Task"::AddField);
    end;

    procedure GetRoomDescription() Description: Text
    begin
        // Load HTML from Resources folder
        Description := NavApp.GetResourceAsText('IntroductionRoomDescription.html');
    end;

    procedure GetRoomSolution() Solution: Text
    begin
        exit('To complete this room, create a table extension...');
    end;

    procedure GetHintImage() InStr: InStream
    begin
        // Optional: Return hint image from Resources
        // NavApp.GetResource('HintImage.png', InStr);
    end;
}
```

---

## Step 5: Implement Tasks

### 5.1 Create Task Codeunit

```al
codeunit 50010 "My Task 1" implements iEscapeRoomTask
{
    SingleInstance = true;  // Common pattern for event subscribers

    var
        Room: Codeunit "My Intro Room";

    procedure GetTaskRec() TaskRec: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        TaskRec."Venue Id" := Me.Name;
        TaskRec."Room Name" := Format(Room.GetRoom());
        TaskRec.Name := Format(this.GetTask());
        TaskRec.Description := 'Create a setup page for configuration.';
    end;

    procedure GetTask(): enum "Escape Room Task"
    begin
        exit(enum::"Escape Room Task"::CreateSetupPage);
    end;

    procedure IsValid(): Boolean
    begin
        // Return false when using event subscribers for automatic monitoring
        exit(false);
    end;

    procedure GetHint(): Text
    begin
        exit('Create a page of type Card with a table as source.');
    end;

    // Event subscriber automatically completes task when condition met
    [EventSubscriber(ObjectType::Table, Database::Customer, OnAfterInsertEvent, '', false, false)]
    local procedure OnCustomerInsert(var Rec: Record Customer)
    begin
        // Check if room is active
        if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
            exit;

        // Validate condition
        if Rec.Name = 'BC Talent' then
            this.GetTaskRec().SetStatusCompleted();
    end;
}
```

**Common Task Patterns:**

### Pattern 1: Field Existence Check (Polling)

```al
procedure IsValid(): Boolean
var
    Field: Record Field;
begin
    Field.SetRange(TableNo, Database::Item);
    Field.SetRange(FieldName, 'Information');
    exit(not Field.IsEmpty());
end;
```

### Pattern 2: Object Existence Check (Polling)

```al
procedure IsValid(): Boolean
var
    AllObjWithCaption: Record AllObjWithCaption;
begin
    AllObjWithCaption.SetRange("Object Type", AllObjWithCaption."Object Type"::Table);
    AllObjWithCaption.SetFilter("Object Name", '*My Custom Table*');
    exit(not AllObjWithCaption.IsEmpty());
end;
```

### Pattern 3: Event Subscriber with Field Value Validation

```al
SingleInstance = true;

var
    Room: Codeunit "My Room";

procedure IsValid(): Boolean
begin
    // Event subscriber handles validation
    exit(false);
end;

[EventSubscriber(ObjectType::Table, Database::"Sales Line", OnAfterValidateEvent, "No.", false, false)]
local procedure ValidateFieldCopy(var Rec: Record "Sales Line")
var
    RecRef: RecordRef;
    FldRef: FieldRef;
    Field: Record Field;
    Item: Record Item;
    ItemInfo: Text;
    SalesInfo: Text;
begin
    // Check if room is active
    if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
        exit;

    if Rec.Type <> Rec.Type::Item then
        exit;

    // Get source value from Item using RecordRef/FieldRef pattern
    Item.Get(Rec."No.");
    Field.SetRange(TableNo, Database::Item);
    Field.SetRange(FieldName, 'Information');
    if not Field.FindFirst() then
        exit;

    RecRef.GetTable(Item);
    FldRef := RecRef.Field(Field."No.");
    ItemInfo := Format(FldRef.Value);

    if ItemInfo = '' then
        exit;

    // Check if copied to Sales Line
    Field.SetRange(TableNo, Database::"Sales Line");
    Field.SetRange(FieldName, 'Information');
    if not Field.FindFirst() then
        exit;

    RecRef.GetTable(Rec);
    FldRef := RecRef.Field(Field."No.");
    SalesInfo := Format(FldRef.Value);

    // Validate values match
    if ItemInfo = SalesInfo then
        this.GetTaskRec().SetStatusCompleted();
end;
```

---

## Step 6: Validation Patterns

### Event Subscriber Pattern (Most Common)

**When to use:**
- Monitor table insert/modify/delete events
- Validate field value changes
- React to business logic execution
- Track participant interactions

**Best practices:**
- Add `SingleInstance = true` to codeunit
- Always check room status: `if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then exit;`
- Return `false` from `IsValid()` when using events
- Use `this.GetTaskRec().SetStatusCompleted()` when condition met
- Use List to track created records (see CreateCustomer example below)

### Polling Validation Pattern

**When to use:**
- Check object existence (tables, pages, fields)
- Verify configuration settings
- Validate app installations
- Simple existence checks

**Best practices:**
- Return `true` from `IsValid()` when complete
- Framework calls periodically via `UpdateStatus()`
- Keep validation fast and efficient
- Use system tables: Field, AllObjWithCaption, NAV App Installed App

---

## Step 7: Register Venue

Create an install codeunit to register your venue:

```al
codeunit 50009 "Install My Venue"
{
    Subtype = Install;

    trigger OnInstallAppPerCompany()
    var
        EscapeRoom: Codeunit "Escape Room";
    begin
        EscapeRoom.UpdateVenue(enum::"Escape Room Venue"::MyVenue);
    end;
}
```

**Important:**
- Call `UpdateVenue()` with your venue enum value
- Framework automatically creates/updates venue, rooms, and tasks
- Use `OnInstallAppPerCompany()` trigger
- For updates, add Upgrade codeunit with same logic

---

## Real-World Examples

Based on Development.1 and Development.2 implementations:

### Example: Development.1 Venue Structure

```
Development.1/
├── Src/
│   ├── EscapeRoomVenueDev1.Codeunit.al
│   ├── EscapeRoomVenueDevelopment1.EnumExt.al
│   ├── InstallAppDEV1.Codeunit.al
│   └── Rooms/
│       ├── EscapeRoomsDev1.EnumExt.al
│       ├── 1. Introduction/
│       │   ├── IntroductionRoomDev1.Codeunit.al
│       │   └── Tasks/
│       │       ├── IntroductionRoomTasksDev1.EnumExt.al
│       │       └── Implementations/
│       │           ├── CreateCustomerDev1.Codeunit.al
│       │           └── CreateExtensionDev1.Codeunit.al
│       ├── 2. Read Full Assignment/
│       ├── 3. Add Fields on Tables/
│       ├── 4. Add Fields on Pages/
│       ├── 5. Test and see if it fails/
│       ├── 6. Implement Business Logic/
│       └── 7. Also post to Item Ledger Entry/
└── Resources/
    ├── IntroductionRoomDescription.html
    ├── IntroductionRoomSolution.html
    ├── AddFieldsOnAllTablesDescription.html
    ├── AddFieldsOnAllTablesSolution.html
    ├── RoomCompleted.png
    └── VenueCompleted.png
```

### Example 1: Create Customer Task (Event Subscriber with List tracking)

```al
// From Development.1 - IntroductionRoom
codeunit 74012 "CreateCustomer Dev1" implements iEscapeRoomTask
{
    SingleInstance = true;

    var
        Room: Codeunit "IntroductionRoom Dev1";
        InsertedCustomers: List of [Text];  // Track inserted customers

    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        EscapeRoomTask."Venue Id" := Me.Name;
        EscapeRoomTask."Room Name" := Format(Room.GetRoom());
        EscapeRoomTask.Name := Format(this.GetTask());
        EscapeRoomTask.Description := 'Create a customer record.';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(Enum::"Escape Room Task"::CreateCustomerDev1);
    end;

    procedure IsValid(): Boolean
    begin
        exit(false); // Event subscriber handles validation
    end;

    procedure GetHint(): Text
    begin
        exit('Navigate to the Customer Card page.');
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, OnAfterInsertEvent, '', false, false)]
    local procedure CustomerOnAfterInsert(var Rec: Record Customer)
    begin
        if Room.GetRoomRec().GetStatus() <> Enum::"Escape Room Status"::InProgress then
            exit;

        InsertedCustomers.Add(Rec."No.");

        if Rec.Name <> 'BC Talent' then
            exit;

        this.GetTaskRec().SetStatusCompleted();
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, OnAfterModifyEvent, '', false, false)]
    local procedure CustomerOnAfterModify(var Rec: Record Customer)
    begin
        if Room.GetRoomRec().GetStatus() <> Enum::"Escape Room Status"::InProgress then
            exit;

        // Only check customers that were inserted during this room
        if not InsertedCustomers.Contains(Rec."No.") then
            exit;

        if Rec.Name <> 'BC Talent' then
            exit;

        this.GetTaskRec().SetStatusCompleted();
    end;
}
```

### Example 2: Field Existence Task (Polling Validation)

```al
// From Development.1 - Add Fields on Tables room
codeunit 74019 "Exists On Item Dev1" implements iEscapeRoomTask
{
    var
        Room: Codeunit "AddFieldsOnAllTables Dev1";

    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        EscapeRoomTask."Venue Id" := Me.Name;
        EscapeRoomTask."Room Name" := Format(Room.GetRoom());
        EscapeRoomTask.Name := Format(this.GetTask());
        EscapeRoomTask.Description := '"Information"-field on the Item table.';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(enum::"Escape Room Task"::ExistsOnItemDev1);
    end;

    procedure IsValid(): Boolean
    var
        Field: Record Field;
    begin
        Field.SetRange(TableNo, Database::Item);
        Field.SetRange(FieldName, 'Information');
        exit(not Field.IsEmpty());
    end;

    procedure GetHint(): Text
    begin
        exit('Create an "Information" field on the Item table.');
    end;
}
```

### Example 3: App Installation Check

```al
// From Development.1 - Introduction room
codeunit 74014 "CreateExtension Dev1" implements iEscapeRoomTask
{
    var
        Room: Codeunit "IntroductionRoom Dev1";

    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        EscapeRoomTask."Venue Id" := Me.Name;
        EscapeRoomTask."Room Name" := Format(Room.GetRoom());
        EscapeRoomTask.Name := Format(this.GetTask());
        EscapeRoomTask.Description := 'Create an extension and install in this environment.';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(Enum::"Escape Room Task"::CreateExtensionDev1);
    end;

    procedure IsValid(): Boolean
    var
        NavAppInstalled: Record "NAV App Installed App";
    begin
        NavAppInstalled.SetRange(Name, 'Beach House Heist');
        exit(not NavAppInstalled.IsEmpty);
    end;

    procedure GetHint(): Text
    begin
        exit('Create an extension with VSCode, download symbols, publish to this environment.');
    end;
}
```

### Example 4: RecordRef/FieldRef Pattern for Dynamic Field Validation

```al
// From Development.1 - Implement Business Logic room
codeunit 74038 "Sales Line Success Dev1" implements iEscapeRoomTask
{
    var
        Room: Codeunit "ImplementBusinessLogic Dev1";

    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        EscapeRoomTask."Venue Id" := Me.Name;
        EscapeRoomTask."Room Name" := Format(Room.GetRoom());
        EscapeRoomTask.Name := Format(this.GetTask());
        EscapeRoomTask.Description := 'Implement Information field copy from Item to Sales Line';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(enum::"Escape Room Task"::SalesLineSuccessDev1);
    end;

    procedure IsValid(): Boolean
    begin
        exit(false); // Event subscriber handles validation
    end;

    procedure GetHint(): Text
    begin
        exit('Use an Item WITH a value in the "Information" field - add this item to a Sales Order.');
    end;

    [EventSubscriber(ObjectType::Table, Database::"Sales Line", OnAfterValidateEvent, "No.", false, false)]
    local procedure CheckIfInformationFieldIsFilled(var Rec: Record "Sales Line")
    var
        RecRef: RecordRef;
        FldRef: FieldRef;
        Field: Record Field;
        Item: Record Item;
        ItemInformation: Text;
        SalesInformation: Text;
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
    begin
        // Check if room is active
        if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
            exit;

        // Check document type
        if (Rec."Document Type" <> Rec."Document Type"::Order) and
           (Rec."Document Type" <> Rec."Document Type"::Invoice) then
            exit;

        if Rec.Type <> Rec.Type::Item then
            exit;

        // Get Item
        Item.Get(Rec."No.");

        // Get Information field value from Item using RecordRef/FieldRef
        Field.SetRange(TableNo, Database::Item);
        Field.SetRange(FieldName, 'Information');
        if not Field.FindFirst() then
            exit; // Field doesn't exist yet

        RecRef.GetTable(Item);
        FldRef := RecRef.Field(Field."No.");
        ItemInformation := Format(FldRef.Value);

        if ItemInformation = '' then begin
            EscapeRoomNotifications.Warning('Use an Item with a value in the "Information" field');
            exit;
        end;

        // Check if Information field exists on Sales Line
        Field.SetRange(TableNo, Database::"Sales Line");
        Field.SetRange(FieldName, 'Information');
        if not Field.FindFirst() then
            exit; // Field doesn't exist yet

        // Get Information field value from Sales Line
        RecRef.GetTable(Rec);
        FldRef := RecRef.Field(Field."No.");
        SalesInformation := Format(FldRef.Value);

        // Validate values match
        if ItemInformation <> SalesInformation then begin
            EscapeRoomNotifications.Warning('The "Information" field value should match the Item value.');
            exit;
        end;

        // Task completed!
        this.GetTaskRec().SetStatusCompleted();
    end;
}
```

### Example 5: Page Event for UI Interaction

```al
// From Development.1 - Read Full Assignment room
codeunit 74017 "Read the full assignment Dev1" implements iEscapeRoomTask
{
    var
        Room: Codeunit "ReadFullAssignment Dev1";

    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        EscapeRoomTask."Venue Id" := Me.Name;
        EscapeRoomTask."Room Name" := Format(Room.GetRoom());
        EscapeRoomTask.Name := Format(this.GetTask());
        EscapeRoomTask.Description := 'Read Full Assignment.';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(enum::"Escape Room Task"::ReadTheFullAssignmentDev1);
    end;

    procedure IsValid(): Boolean
    begin
        exit(false); // Event subscriber monitors page interaction
    end;

    procedure GetHint(): Text
    begin
        exit('Open the room details page and read the assignment.');
    end;

    [EventSubscriber(ObjectType::Page, Page::"Escape Room", OnAfterGetCurrRecordEvent, '', false, false)]
    local procedure CheckIfTaskIsCompleted(var Rec: Record "Escape Room")
    var
        RoomName: Text;
    begin
        // Check if room is active
        if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
            exit;

        // Check if participant opened the correct room page
        RoomName := Room.GetRoomRec().Name;
        if Rec.Name = RoomName then
            this.GetTaskRec().SetStatusCompleted();
    end;
}
```

---

## Common Patterns Summary

### Venue Registration
✅ Use `ModuleInfo` to get app metadata (`NavApp.GetCurrentModuleInfo(Me)`)  
✅ Add rooms by enum value, not codeunit instance  
✅ Use `NavApp.GetResource()` for PNG images from Resources folder  

### Room Implementation
✅ Use `ModuleInfo` for venue/room metadata  
✅ Use `NavApp.GetResourceAsText()` for HTML descriptions  
✅ Add tasks by enum value, not codeunit instance  
✅ Store HTML/image files in Resources folder  
✅ Use numbered folders: "1. Introduction", "2. Configuration"  

### Task Validation
✅ **Event Subscribers**: Return `false` from `IsValid()`, use event to call `SetStatusCompleted()`  
✅ **Polling**: Return `true` from `IsValid()` when complete  
✅ **Always check room status** in event subscribers  
✅ Use `SingleInstance = true` for event subscriber codeunits  
✅ Use RecordRef/FieldRef for dynamic field checking  
✅ Use List to track inserted/modified records  
✅ Provide helpful notifications with `EscapeRoomNotifications` codeunit  

### Project Organization
✅ Numbered folders for rooms ("1. Introduction", "2. Configuration")  
✅ Separate Tasks/Implementations subfolders  
✅ Resources folder for HTML descriptions and solution files  
✅ One enum extension per category (venue, rooms, tasks per room)  
✅ Consistent naming: RoomName + Task + Dev1/Dev2 suffix  

---
```

---

## Step 7: Add Resources (Optional)

### 7.1 HTML Descriptions

Create HTML files for rich room descriptions:

```
Resources/
  ├── IntroRoomDescription.html
  ├── IntroRoomSolution.html
  ├── ChallengeRoomDescription.html
  └── ChallengeRoomSolution.html
```

### 7.2 Load HTML from Resources

```al
local procedure GetRoomDescriptionHTML(): Text
var
    TempBlob: Codeunit "Temp Blob";
    InStr: InStream;
    Result: Text;
begin
    // Load from resource file
    NavApp.LoadPackageData(Database::"Escape Room", TempBlob);
    TempBlob.CreateInStream(InStr, TextEncoding::UTF8);
    InStr.ReadText(Result);
    exit(Result);
end;
```

---

## Step 8: Testing

### 8.1 Test Venue Registration

1. **Publish your app** to BC environment
2. **Open Escape Room Venue List**
3. **Verify your venue appears** in the list
4. **Check all rooms are created** with correct sequence

### 8.2 Test Task Validation

1. **Open your venue**
2. **Navigate to first room**
3. **View task list**
4. **Complete the challenge**
5. **Run validation** (manually or automatically)
6. **Verify task marked complete**

### 8.3 Test Room Completion

1. **Complete all tasks** in a room
2. **Verify room status** changes to Completed
3. **Check next room opens** automatically
4. **Verify telemetry logged** in Application Insights

---

## Best Practices

### Design Guidelines

**1. Progressive Difficulty**
- Start with simple tasks in early rooms
- Gradually increase complexity
- Provide clear learning progression

**2. Clear Instructions**
- Write detailed room descriptions
- Include specific objectives
- Provide context for business scenarios

**3. Helpful Hints**
- Don't give away solutions completely
- Guide toward the right approach
- Reference BC documentation

**4. Reliable Validation**
- Test validation in clean environments
- Handle edge cases gracefully
- Provide clear feedback on failure

**5. Appropriate Timing**
- Estimate realistic completion times
- Set solution delays appropriately (10-15 minutes minimum)
- Test with real users to calibrate

### Content Guidelines

**Room Descriptions Should Include:**
- Learning objectives
- Business context/scenario
- High-level approach
- Prerequisites from previous rooms
- Estimated time to complete

**Task Descriptions Should Include:**
- Specific goal to accomplish
- What will be validated
- Where to find relevant information
- Any special considerations

**Hints Should Include:**
- Directional guidance
- References to BC patterns/examples
- Troubleshooting tips
- NOT complete solutions

---

## Common Patterns

### Pattern: Sequential Learning

```al
// Room 1: Basics
Tasks: Create Table, Create Page

// Room 2: Business Logic  
Tasks: Add Validation, Implement Posting

// Room 3: Integration
Tasks: Create API, Add Events
```

### Pattern: Feature Implementation

```al
// Each room implements one complete feature
Room 1: Setup Infrastructure
Room 2: Core Functionality
Room 3: UI/UX
Room 4: Testing & Validation
```

### Pattern: Problem Solving

```al
// Rooms present problems to solve
Room 1: "Sales order won't post - fix validation"
Room 2: "Performance is slow - add keys"
Room 3: "Data isn't copying - add events"
```

---

## Troubleshooting

### Venue Not Appearing

**Check:**
- Install codeunit executing `UpdateVenue()`
- Enum extension published
- No errors in Event Log
- Venue ID unique

### Rooms in Wrong Order

**Check:**
- Sequence numbers in GetRooms() list order
- Not setting sequence manually in GetRoomRec()
- Framework assigns sequence automatically

### Tasks Not Validating

**Check:**
- TestCodeunitId correct
- Validation interface implemented properly
- TaskCode() returns correct value
- ValidateTask() logic working
- Permissions on tables being queried

### HTML Not Displaying

**Check:**
- BLOB field populated correctly
- OutStream uses UTF8 encoding
- HTML is valid
- No script/style restrictions

---

## Next Steps

Once your venue is created:

1. **Document your rooms** - Create user-facing documentation
2. **Test thoroughly** - Run through all scenarios
3. **Gather feedback** - Have colleagues test
4. **Refine tasks** - Adjust based on feedback
5. **Monitor telemetry** - Check completion rates after events

---

## Related Documentation

- [Architecture Overview](Architecture.md) - Framework design patterns
- [Task Validation System](Task-Validation.md) - Validation details
- [API Reference](../Dev/API-Reference.md) - Interface specifications
- [Telemetry Integration](Telemetry-Integration.md) - Tracking and scoring

---

**Last Updated:** January 7, 2026
