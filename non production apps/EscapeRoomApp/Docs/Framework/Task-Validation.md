# Task Validation System

## Overview

The task validation system determines when a participant has successfully completed a challenge. The framework supports **three validation patterns**, each suited for different scenarios:

1. **Polling Pattern** - Simple `IsValid()` checks (field existence, configuration)
2. **Event Subscriber Pattern** - Monitor BC events (table inserts, field changes)
3. **Test Codeunit Pattern** - Run automated AL tests for complex UI/workflow validation

---

## Pattern 1: Polling Pattern 📊

Tasks return `true` from `IsValid()` when complete. Framework calls periodically via `UpdateStatus()`.

### When to Use
- ✅ Check object existence (tables, pages, fields)
- ✅ Verify configuration settings
- ✅ Validate app installations
- ✅ Simple Boolean checks

### Implementation

```al
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
        // Framework calls this periodically
        Field.SetRange(TableNo, Database::Item);
        Field.SetRange(FieldName, 'Information');
        exit(not Field.IsEmpty());  // True when field exists
    end;

    procedure GetHint(): Text
    begin
        exit('Create an "Information" field on the Item table.');
    end;
}
```

### Common Polling Checks

**Field Existence:**
```al
procedure IsValid(): Boolean
var
    Field: Record Field;
begin
    Field.SetRange(TableNo, Database::"Sales Line");
    Field.SetRange(FieldName, 'Information');
    exit(not Field.IsEmpty());
end;
```

**Object Existence:**
```al
procedure IsValid(): Boolean
var
    AllObjWithCaption: Record AllObjWithCaption;
begin
    AllObjWithCaption.SetRange("Object Type", AllObjWithCaption."Object Type"::Table);
    AllObjWithCaption.SetFilter("Object Name", '*Copilot Translation Language*');
    exit(not AllObjWithCaption.IsEmpty());
end;
```

**App Installation:**
```al
procedure IsValid(): Boolean
var
    NavAppInstalled: Record "NAV App Installed App";
begin
    NavAppInstalled.SetRange(Name, 'Beach House Heist');
    exit(not NavAppInstalled.IsEmpty);
end;
```

### Best Practices
✅ Keep validation fast and efficient (called frequently)  
✅ Use system tables: Field, AllObjWithCaption, NAV App Installed App  
✅ Return `true` when task is complete  
✅ No room status checking needed (framework handles context)  

---

## Pattern 2: Event Subscriber Pattern 🎯

Tasks return `false` from `IsValid()` and use event subscribers to monitor participant actions.

### When to Use
- ✅ Monitor table insert/modify/delete events
- ✅ Validate field value changes
- ✅ React to business logic execution
- ✅ Track specific user interactions
- ✅ Validate data flow between tables

### Implementation

```al
codeunit 74012 "CreateCustomer Dev1" implements iEscapeRoomTask
{
    SingleInstance = true;  // Required for event subscribers

    var
        Room: Codeunit "IntroductionRoom Dev1";
        InsertedCustomers: List of [Text];  // Track created records

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
        exit(false);  // Event subscriber handles validation
    end;

    procedure GetHint(): Text
    begin
        exit('Navigate to the Customer Card page.');
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, OnAfterInsertEvent, '', false, false)]
    local procedure CustomerOnAfterInsert(var Rec: Record Customer)
    begin
        // Always check room status first
        if Room.GetRoomRec().GetStatus() <> Enum::"Escape Room Status"::InProgress then
            exit;

        InsertedCustomers.Add(Rec."No.");

        if Rec.Name <> 'BC Talent' then
            exit;

        // Task complete!
        this.GetTaskRec().SetStatusCompleted();
    end;

    [EventSubscriber(ObjectType::Table, Database::Customer, OnAfterModifyEvent, '', false, false)]
    local procedure CustomerOnAfterModify(var Rec: Record Customer)
    begin
        if Room.GetRoomRec().GetStatus() <> Enum::"Escape Room Status"::InProgress then
            exit;

        // Only check customers created during this room
        if not InsertedCustomers.Contains(Rec."No.") then
            exit;

        if Rec.Name <> 'BC Talent' then
            exit;

        this.GetTaskRec().SetStatusCompleted();
    end;
}
```

### RecordRef/FieldRef Pattern (Dynamic Field Validation)

For validating field copies or business logic:

```al
codeunit 74038 "Sales Line Success Dev1" implements iEscapeRoomTask
{
    var
        Room: Codeunit "ImplementBusinessLogic Dev1";

    procedure IsValid(): Boolean
    begin
        exit(false);  // Event subscriber handles validation
    end;

    [EventSubscriber(ObjectType::Table, Database::"Sales Line", OnAfterValidateEvent, "No.", false, false)]
    local procedure CheckFieldCopy(var Rec: Record "Sales Line")
    var
        RecRef: RecordRef;
        FldRef: FieldRef;
        Field: Record Field;
        Item: Record Item;
        ItemInfo: Text;
        SalesInfo: Text;
    begin
        // Check room status
        if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
            exit;

        if Rec.Type <> Rec.Type::Item then
            exit;

        // Get Item Information field using RecordRef/FieldRef
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

        // Get Sales Line Information field
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
}
```

### Page Event Pattern (UI Interaction)

```al
[EventSubscriber(ObjectType::Page, Page::"Escape Room", OnAfterGetCurrRecordEvent, '', false, false)]
local procedure CheckPageOpened(var Rec: Record "Escape Room")
begin
    if Room.GetRoomRec().GetStatus() <> enum::"Escape Room Status"::InProgress then
        exit;

    if Rec.Name = Room.GetRoomRec().Name then
        this.GetTaskRec().SetStatusCompleted();
end;
```

### Best Practices
✅ Use `SingleInstance = true` on codeunit  
✅ **Always** check room status in event subscribers  
✅ Return `false` from `IsValid()`  
✅ Call `this.GetTaskRec().SetStatusCompleted()` when condition met  
✅ Use List to track inserted/modified records  
✅ Use RecordRef/FieldRef for dynamic field access  
✅ Provide helpful notifications with `EscapeRoomNotifications`  

---

## Pattern 3: Test Codeunit Pattern 🧪

Tasks run automated AL tests (Subtype = Test) to validate complex scenarios like UI workflows, page handlers, and multi-step interactions.

### When to Use
- ✅ Validate UI actions (button clicks, page handlers)
- ✅ Complex workflows requiring TestPage
- ✅ Multi-step interactions
- ✅ Copilot/AI interactions
- ✅ Dialog/modal validation

### Implementation

**Step 1: Task Implementation**
```al
codeunit 74104 "VerifyTranslatAction Dev2" implements iEscapeRoomTask
{
    var
        Room: Codeunit "UserInterfaceRoom Dev2";

    procedure GetTaskRec() EscapeRoomTask: Record "Escape Room Task"
    var
        Me: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(Me);

        EscapeRoomTask."Venue Id" := Me.Name;
        EscapeRoomTask."Room Name" := 'User Interface Preparation';
        EscapeRoomTask.Name := Format(this.GetTask());
        EscapeRoomTask.Description := 'Verify the "Translate with Copilot" action opens dialog.';
    end;

    procedure GetTask(): Enum "Escape Room Task"
    begin
        exit(Enum::"Escape Room Task"::AddTranslateActionDev2);
    end;

    procedure GetHint(): Text
    begin
        exit('The action needs to open page 60200. Update the OnAction trigger.');
    end;

    procedure IsValid(): Boolean
    var
        TestQueue: Record "Test Queue";
        TaskValidationTestRunner: Codeunit "Task Validation Test Runner";
        TestCodeunitId: Integer;
        EscapeRoomNotifications: Codeunit EscapeRoomNotifications;
    begin
        TestCodeunitId := Codeunit::"VerifyTranslateActionTest Dev2";

        // Clean up any existing test queue record
        if TestQueue.Get(TestCodeunitId) then
            TestQueue.Delete();

        // Set up the test queue
        TestQueue.Init();
        TestQueue."Codeunit Id" := TestCodeunitId;
        TestQueue.Success := false;
        TestQueue.Insert();

        // Run the test
        Commit();
        TaskValidationTestRunner.Run(TestQueue);

        // Get the result - must reload after test run
        SelectLatestVersion();
        TestQueue.Get(TestCodeunitId);

        // Provide feedback on failure
        if not TestQueue.Success then begin
            if GetLastErrorText.Contains('TranslationPromptDialogHandler') then
                EscapeRoomNotifications.Warning('Dialog not opened from Item list yet.')
            else
                EscapeRoomNotifications.Warning(GetLastErrorText);
        end;

        exit(TestQueue.Success);
    end;
}
```

**Step 2: Test Codeunit**
```al
codeunit 74150 "VerifyTranslateActionTest Dev2"
{
    Subtype = Test;
    TestPermissions = NonRestrictive;

    [Test]
    [HandlerFunctions('TranslationPromptDialogHandler')]
    procedure TestTranslateActionOpensPromptDialog()
    var
        Item: Record Item;
        ItemListPage: TestPage "Item List";
    begin
        // [GIVEN] An item exists
        if not Item.FindFirst() then
            Error('No items found to test with');

        // [WHEN] Opening Item List and clicking action
        ItemListPage.OpenView();
        ItemListPage.GoToRecord(Item);
        ItemListPage.TranslateWithCopilot.Invoke();

        // [THEN] Dialog opens (verified by page handler)
    end;

    [ModalPageHandler]
    procedure TranslationPromptDialogHandler(var TranslationPromptDialog: TestPage "Translation Prompt Dialog")
    begin
        // Dialog opened successfully - test passes
    end;
}
```

### How Test Codeunit Pattern Works

```
┌─────────────────────────────────────────────────────────────────┐
│ Framework calls IsValid()                                       │
│   └─> Task creates Test Queue record                           │
│   └─> Task runs TaskValidationTestRunner                       │
│       └─> TestRunner.OnRun() executes test codeunit            │
│       └─> TestRunner.OnAfterTestRun() updates TestQueue.Success│
│   └─> Task reads TestQueue.Success                             │
│   └─> Returns true/false based on test result                  │
└─────────────────────────────────────────────────────────────────┘
```

### Framework Components

**Task Validation Test Runner (Built-in)**
```al
codeunit 73920 "Task Validation Test Runner"
{
    Subtype = TestRunner;
    TestIsolation = Codeunit;
    TableNo = "Test Queue";

    trigger OnRun()
    begin
        codeunit.run(Rec."Codeunit Id");
    end;

    trigger OnAfterTestRun(CodeunitId: Integer; CodeunitName: Text; 
        FunctionName: Text; Permissions: TestPermissions; Success: Boolean)
    var
        TestQueue: Record "Test Queue";
    begin
        if FunctionName <> '' then exit; // Only full codeunit tests supported

        TestQueue.ReadIsolation := IsolationLevel::UpdLock;
        TestQueue.Get(CodeunitId);
        TestQueue.Success := Success;
        TestQueue.Modify();
        Commit;
    end;
}
```

### Test Queue Table

Framework automatically manages the "Test Queue" table:
- **Codeunit Id**: Test codeunit to run
- **Success**: Result of test execution

### Best Practices
✅ Use `Subtype = Test` on test codeunit  
✅ Use `TestPermissions = NonRestrictive` to avoid permission issues  
✅ Clean up TestQueue before running: `if TestQueue.Get(Id) then TestQueue.Delete()`  
✅ Always `Commit()` before running test  
✅ Use `SelectLatestVersion()` before reading result  
✅ Provide helpful error messages from GetLastErrorText  
✅ Use page handlers for modal dialogs: `[HandlerFunctions('MyHandler')]`  
✅ Test full user workflows with TestPage  

### When NOT to Use
❌ Simple field existence checks (use Polling)  
❌ Monitoring continuous events (use Event Subscriber)  
❌ When you can validate without UI interaction  
❌ Performance-sensitive scenarios (tests are slower)  

---

## Pattern Comparison

| Pattern | Use Case | Returns from IsValid() | Complexity | Performance |
|---------|----------|----------------------|------------|-------------|
| **Polling** | Object/field existence | `true` when complete | ⭐ Simple | ⚡ Fast |
| **Event Subscriber** | Monitor BC events | `false` (event calls SetStatusCompleted) | ⭐⭐ Medium | ⚡⚡ Very Fast |
| **Test Codeunit** | UI workflows, dialogs | `true` when test passes | ⭐⭐⭐ Complex | 🐌 Slower |

---

## Decision Tree

```
Need to validate task completion?
│
├─ Is it a simple check?
│  └─ Field exists? → Polling Pattern
│  └─ Object exists? → Polling Pattern
│  └─ App installed? → Polling Pattern
│
├─ Need to monitor user actions?
│  └─ Table insert/modify? → Event Subscriber Pattern
│  └─ Field value changes? → Event Subscriber Pattern
│  └─ Business logic execution? → Event Subscriber Pattern
│
└─ Need to test UI interaction?
   └─ Button clicks? → Test Codeunit Pattern
   └─ Page handlers? → Test Codeunit Pattern
   └─ Multi-step workflow? → Test Codeunit Pattern
```

---

## Real-World Examples from Development.1 & Development.2

### Polling: Field Existence (Development.1)
✅ Check if "Information" field exists on Item table  
✅ Check if field exists on Sales Line, Sales Invoice Line, etc.  

### Event Subscriber: Record Creation (Development.1)
✅ Monitor Customer creation with specific name  
✅ Track Sales Line field population from Item  
✅ Validate business logic during posting  

### Event Subscriber: Page Interaction (Development.1)
✅ Detect when participant opens room description page  

### Test Codeunit: UI Workflow (Development.2)
✅ Verify "Translate with Copilot" action opens dialog  
✅ Test Copilot translation workflow  
✅ Validate page handlers for modal dialogs  

---

## Common Mistakes to Avoid

❌ **Mixing patterns**: Don't use event subscribers AND return true from IsValid()  
❌ **Missing room status check**: Always check in event subscribers  
❌ **Forgetting SingleInstance**: Event subscribers need this  
❌ **Not committing before test**: Test codeunit pattern requires Commit()  
❌ **Ignoring TestQueue cleanup**: Delete before creating new  
❌ **Heavy polling validation**: Keep IsValid() fast  

---
    Rec."Stop DateTime" := CurrentDateTime;
    Rec.Modify();
    
    EscapeRoomTelemetry.LogFinishedTask(Rec);
    
    if Room.Get(Rec."Venue Id", Rec."Room Name") then
        Room.CloseRoomIfCompleted();
end;
```

---

## Validation Patterns

### Common Validation Types

#### 1. **Field Existence Check**

Verify a custom field has been added:

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
var
    Field: Record Field;
begin
    Field.SetRange(TableNo, Database::"Sales Line");
    Field.SetRange(FieldName, 'Information');
    exit(Field.FindFirst());
end;
```

#### 2. **Record Existence Check**

Verify specific data has been created:

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
var
    MySetupTable: Record "My Setup Table";
begin
    MySetupTable.SetFilter(Code, '<>%1', '');
    exit(not MySetupTable.IsEmpty);
end;
```

#### 3. **Configuration Check**

Verify setup is complete:

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
var
    LanguageSetup: Record "Copilot Translation Languages";
    Languages: Record Language;
begin
    if not LanguageSetup.FindFirst() then
        exit(false);
    
    // Verify language code exists
    exit(Languages.Get(LanguageSetup.Code));
end;
```

#### 4. **Functionality Check**

Verify business logic is working:

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
var
    SalesLine: Record "Sales Line";
    Item: Record Item;
begin
    // Check if Information field gets populated from Item
    SalesLine.SetFilter(Information, '<>%1', '');
    if not SalesLine.FindFirst() then
        exit(false);
    
    // Verify it matches item description
    if Item.Get(SalesLine."No.") then
        exit(SalesLine.Information = Item.Description);
end;
```

#### 5. **Code Extension Check**

Verify event subscribers are implemented:

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
var
    EventSubscription: Record "Event Subscription";
begin
    EventSubscription.SetRange("Subscriber Codeunit ID", Codeunit::"My Event Subscriber");
    EventSubscription.SetRange("Event Type", EventSubscription."Event Type"::OnAfterEvent);
    exit(not EventSubscription.IsEmpty);
end;
```

---

## Validation Best Practices

### Design Guidelines

**1. Make Validation Deterministic**
- Same input → same output every time
- Don't rely on timestamps or random data
- Avoid external dependencies that may change

**2. Validate End Results, Not Implementation**
- Check WHAT was accomplished, not HOW
- Allow multiple solution approaches
- Focus on observable behavior

**3. Provide Clear Feedback**
- `GetValidationDescription()` should explain what's being checked
- Consider adding error messages for common failures
- Use hints to guide participants

**4. Keep Validation Fast**
- Avoid complex queries or loops when possible
- Use filtered searches with specific criteria
- Don't validate more than necessary

**5. Test Edge Cases**
- Empty data scenarios
- Partial completion
- Multiple concurrent users
- Clean environment (no pre-existing data)

---

### Error Handling

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
var
    MyTable: Record "My Table";
begin
    // Use TRY to prevent errors from breaking validation
    if not MyTable.ReadPermission then
        exit(false);
    
    // Safe queries with filters
    MyTable.SetLoadFields("Key Field"); // Performance optimization
    MyTable.SetRange(Status, MyTable.Status::Active);
    
    exit(MyTable.FindFirst());
end;
```

---

## Manual vs. Automatic Validation

### Automatic Validation (Recommended)

Tasks with `TestCodeunitId` set:
- Framework can auto-check validation
- Immediate feedback when complete
- No manual intervention needed

**Use When:**
- Validation logic is deterministic
- Checking BC data/configuration
- Task completion is objective

### Manual Validation

Tasks without `TestCodeunitId`:
- User manually marks complete
- Facilitator verification may be needed
- More subjective completion criteria

**Use When:**
- Validation requires external tools
- Subjective judgment needed
- Checking non-BC systems

---

## Validation Triggers

### When Validation Runs

**Framework-Triggered:**
1. **User clicks "Validate" action** - Manual trigger
2. **Task list refresh** - Periodic check (if implemented)
3. **Room status update** - When checking completion

**Implementation Example:**
```al
action(ValidateTask)
{
    Caption = 'Validate Task';
    Image = Approve;
    
    trigger OnAction()
    var
        TaskValidation: Codeunit "Task Validation Runner";
    begin
        if TaskValidation.RunValidation(Rec) then
            Message('Task completed successfully!')
        else
            Message('Task not yet complete. Keep working!');
    end;
}
```

---

## Advanced Patterns

### Multi-Step Validation

For complex tasks requiring multiple checks:

```al
procedure ValidateTask(TaskCode: Code[20]): Boolean
begin
    // All steps must pass
    if not CheckStep1() then exit(false);
    if not CheckStep2() then exit(false);
    if not CheckStep3() then exit(false);
    
    exit(true);
end;

local procedure CheckStep1(): Boolean
var
    // Step 1 logic
begin
    // Return true if step 1 complete
end;
```

### Partial Credit (Not Implemented in Framework)

Framework currently uses Boolean (complete/not complete), but you could extend:

```al
// Future enhancement idea
procedure GetCompletionPercentage(TaskCode: Code[20]): Decimal
begin
    // Calculate 0-100% completion
    if CheckStep1() then exit(33);
    if CheckStep2() then exit(66);
    if CheckStep3() then exit(100);
    exit(0);
end;
```

---

## Testing Validation Logic

### Test Codeunit Pattern

```al
codeunit 50199 "Test My Task Validation"
{
    Subtype = Test;
    
    [Test]
    procedure TestValidationPasses()
    var
        TaskValidation: Codeunit "My Task Validator";
    begin
        // Arrange: Set up data
        CreateTestData();
        
        // Act: Run validation
        Assert.IsTrue(
            TaskValidation.ValidateTask('MYTASK'),
            'Validation should pass with correct data'
        );
    end;
    
    [Test]
    procedure TestValidationFails()
    var
        TaskValidation: Codeunit "My Task Validator";
    begin
        // Arrange: Don't set up data
        
        // Act: Run validation
        Assert.IsFalse(
            TaskValidation.ValidateTask('MYTASK'),
            'Validation should fail without data'
        );
    end;
}
```

---

## Troubleshooting Validation

### Common Issues

**Validation Always Returns False:**
- Check permission on tables being queried
- Verify filter criteria is not too restrictive
- Use debugger to step through logic
- Check for typos in field/table names

**Validation Errors:**
- Add error handling with TRY functions
- Check that all tables/fields exist
- Verify record permissions
- Test in clean environment

**Inconsistent Results:**
- Remove dependencies on timestamps/random data
- Ensure queries are filtered properly
- Check for data from previous test runs
- Verify multi-user scenarios

---

## Related Documentation

- [Architecture Overview](Architecture.md) - How validation fits into framework
- [Creating New Rooms](Creating-Rooms.md) - Implementing tasks with validation
- [Telemetry Integration](Telemetry-Integration.md) - How validation affects scoring
- [API Reference](../Dev/API-Reference.md) - Complete interface specifications

---

**Last Updated:** January 7, 2026
