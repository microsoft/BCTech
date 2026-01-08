# Permission Sets

Security configuration and permission requirements for the BCTalent.EscapeRoom framework.

---

## Overview

The framework uses AL permission objects to control access to escape room functionality. Permission sets are designed to enable educational scenarios where participants can complete tasks without accessing sensitive business data.

---

## Permission Set Architecture

### Framework Permission Set

**Permission Set:** Escape Room Admin (73920)

**Purpose:** Full access to escape room framework

**Scope:** Internal (framework-defined)

**Included Permissions:**
- Read/Write to Escape Room Venue table
- Read/Write to Escape Room table
- Read/Write to Escape Room Task table
- Execute framework codeunits
- Access to all escape room pages

---

## Permission Set Details

### Escape Room Admin (73920)

**File:** `EscapeRoomAdmin.PermissionSet.al`

**Permissions:**

```al
permissionset 73920 "Escape Room Admin"
{
    Caption = 'Escape Room Admin';
    Assignable = true;

    Permissions =
        table "Escape Room Venue" = X,
        tabledata "Escape Room Venue" = RIMD,
        table "Escape Room" = X,
        tabledata "Escape Room" = RIMD,
        table "Escape Room Task" = X,
        tabledata "Escape Room Task" = RIMD,
        
        codeunit "Escape Room" = X,
        codeunit "Escape Room Telemetry" = X,
        codeunit "Escape Room Notifications" = X,
        
        page "Escape Room Venue List" = X,
        page "Escape Room Venue Card" = X,
        page "Escape Room Page" = X,
        page "Escape Room Task List" = X,
        page "Picture Page" = X,
        page "Rich Text Box Page" = X;
}
```

**Permission Levels:**
- `X` = Execute (for objects)
- `R` = Read
- `I` = Insert  
- `M` = Modify
- `D` = Delete

**Assignment:**
- Assignable directly to users
- Typically assigned to all event participants
- No business data access included

---

## Permission Set Extensions

### D365 BUSINESS PREMIUM Extension

**File:** `BusPremiumExt.PermissionSetExt.al`

**Purpose:** Add escape room permissions to Business Premium license

**Code:**
```al
permissionsetextension 73920 "Bus. Premium Ext." extends "D365 BUSINESS PREMIUM"
{
    Permissions =
        permissionset "Escape Room Admin" = X;
}
```

**Effect:**
- Users with Business Premium license automatically get Escape Room Admin
- Simplifies setup for events
- Participants don't need manual permission assignment

**Use Case:** Event scenarios where all participants have Premium licenses

---

### SUPER Extension

**File:** `FullAdminExt.PermissionSetExt.al`

**Purpose:** Ensure SUPER users have escape room access

**Code:**
```al
permissionsetextension 73921 "Full Admin Ext." extends SUPER
{
    Permissions =
        permissionset "Escape Room Admin" = X;
}
```

**Effect:**
- Super users automatically have escape room access
- Facilitators and admins get access
- Useful for troubleshooting

**Use Case:** Admin and facilitator access

---

## Participant Permission Requirements

### Minimum Required Permissions

For participants to complete escape rooms:

**Framework Permissions:**
- ✅ Escape Room Admin permission set (includes all below)

**Alternative Granular Permissions:**
- Read: Escape Room Venue, Escape Room, Escape Room Task
- Write: Escape Room Venue (Full Name, Partner Name fields)
- Write: Escape Room Task (Status field)
- Execute: Escape Room codeunit, validation codeunits

**Extension-Specific Permissions:**
- Varies by room content
- Examples:
  - Customer/Item tables (read-only) for "Beach House Heist"
  - Page/Table metadata access for design challenges
  - API page access for integration tasks

---

## Facilitator Permission Requirements

### Event Organizer Permissions

For facilitators running events:

**Framework Permissions:**
- ✅ Escape Room Admin (full access)
- ✅ SUPER (for environment setup)

**Additional Access:**
- User management (create/delete users)
- Security group management
- App publishing rights
- Application Insights access
- Azure AD admin (for sandbox creation)

**Setup Scripts Require:**
- Azure subscription owner/contributor
- BC Admin Center access
- PowerShell execution policy

---

## Extension App Permissions

### Permissions in Venue Apps

Extension apps (Development.1, Development.2, Consultant.1) define their own permissions for:
- Custom tables they create
- Custom pages they add
- Validation logic access

**Pattern:**
```al
permissionset 74000 "My Venue Access"
{
    Assignable = true;
    
    Permissions =
        table "My Custom Table" = X,
        tabledata "My Custom Table" = RIMD,
        codeunit "My Validation Logic" = X;
}
```

**Best Practice:**
- Create venue-specific permission set
- Include only what participants need
- Extend Business Premium if appropriate
- Document required licenses

---

## License Requirements

### Business Central Licenses

**Participants:**
- ✅ Essential license (minimum)
- ✅ Premium license (recommended)
- ✅ Team Member (insufficient - read-only)

**Facilitators:**
- ✅ Premium license (minimum)
- ✅ Admin rights for setup

### Azure Requirements

**For Telemetry:**
- Application Insights resource (free tier sufficient for small events)
- Azure subscription (consumption-based pricing)

**Cost Estimate:**
- ~$2-5 per event (100 participants)
- ~5-10 MB telemetry per event
- First 5 GB/month free in Application Insights

---

## Security Considerations

### Data Isolation

**Participant Data:**
- Each participant's progress isolated
- No access to other participants' data
- Venue/Room/Task records scoped by participant

**Business Data:**
- Framework has NO access to business data by default
- Extension apps MAY grant read access (e.g., Customer table)
- Never grant write access to business tables in educational scenarios

### Telemetry Privacy

**What's Logged:**
- Participant names (Full Name, Partner Name)
- Task completion timestamps
- Hint/solution requests
- Room/venue progress

**What's NOT Logged:**
- Passwords
- Business data
- Other user activity
- Session tokens

**GDPR Compliance:**
- Participants voluntarily enter data
- Event organizer is data controller
- Can anonymize post-event
- Retention managed in Application Insights (default 90 days)

---

## Common Permission Issues

### Issue 1: "You do not have permission to access Escape Room"

**Cause:** User doesn't have Escape Room Admin permission set

**Solution:**
```al
// In BC, assign permission set to user:
1. Search "Users"
2. Select participant
3. Add "Escape Room Admin" permission set
```

**Alternative:** Extend D365 BUSINESS PREMIUM (if licensed)

---

### Issue 2: "You do not have permission to run validation"

**Cause:** Validation codeunit not accessible to user

**Solution:**
- Ensure extension app's permission set assigned
- Check codeunit included in permission set
- Verify TestCodeunitId correct in task

---

### Issue 3: Telemetry not logging

**Cause:** Not a permission issue, but often confused with permissions

**Actual Issues:**
- Application Insights connection string missing
- Network blocked
- Extension telemetry disabled in settings

**Verification:**
```al
// Check if telemetry enabled
Session.LogMessage('0001', 'Test', Verbosity::Normal, DataClassification::SystemMetadata, 
    TelemetryScope::ExtensionPublisher, 'Test', 'Test');
// Should appear in Application Insights if configured
```

---

### Issue 4: Participant can't modify business data (by design)

**Cause:** Escape Room Admin only grants framework access

**Explanation:** Working as intended for security

**For Educational Scenarios:**
- Grant read-only to specific tables if needed
- Never grant write access to business tables
- Use sandbox environments
- Consider test data only

---

## Permission Testing Checklist

Before an event, verify:

### Framework Permissions
- [ ] All participants have Escape Room Admin assigned
- [ ] Facilitators have SUPER assigned
- [ ] Permission sets visible in permission list
- [ ] No permission errors when opening venues

### Extension Permissions
- [ ] Venue-specific permission sets assigned
- [ ] Validation codeunits accessible
- [ ] Custom pages/tables accessible
- [ ] No compilation errors

### License Verification
- [ ] All participants have Essential or Premium licenses
- [ ] Licenses activated in environment
- [ ] User count within license limit
- [ ] Sandbox environment properly licensed

### Telemetry Access
- [ ] Application Insights connection string set
- [ ] Facilitators can access AI portal
- [ ] KQL queries return data
- [ ] Dashboard visible

---

## Permission Set Lifecycle

### Development Phase

**Creating Permission Sets:**
```al
permissionset [ID] "[Name]"
{
    Caption = '[Display Name]';
    Assignable = true;  // Allow direct assignment
    
    Permissions =
        // Add objects here
        table "My Table" = X,
        tabledata "My Table" = RIMD;
}
```

**Testing:**
- Create test user with ONLY your permission set
- Attempt all participant actions
- Fix missing permissions iteratively

---

### Deployment Phase

**Permission Set Deployment:**
1. ✅ Included in app package automatically
2. ✅ Available after app publish
3. ✅ Assign via BC admin UI or PowerShell
4. ✅ Changes require app upgrade

**Bulk Assignment:**
```powershell
# PowerShell example for bulk assignment
$users = Get-BCUser -Environment "MyEnvironment"
foreach ($user in $users) {
    Add-BCPermissionSet -User $user.Id -PermissionSet "Escape Room Admin"
}
```

---

### Maintenance Phase

**Adding Permissions:**
- Extend existing permission set (minor version)
- Create new permission set (major changes)
- Document changes in CHANGELOG.md

**Removing Permissions:**
- Deprecate in current version
- Remove in next major version
- Provide migration guide

---

## API Page Permissions

### Aut. Security Groups API (73930)

**Purpose:** Automation API for security group management

**Permissions Required:**
- Execute permission on page
- Read/Write to underlying tables
- Typically SUPER only

**Usage:**
- Event setup automation
- PowerShell scripts for user management
- Not for participants

**Security:**
- Not exposed to participants
- Admin-only access
- Requires authentication token

---

## Best Practices

### Permission Set Design

**Do:**
- ✅ Use descriptive names
- ✅ Make sets assignable
- ✅ Grant minimum required permissions
- ✅ Document in permission set caption
- ✅ Test with restricted user

**Don't:**
- ❌ Grant SUPER to participants
- ❌ Include business data write access
- ❌ Use indirect permissions unnecessarily
- ❌ Forget to test permission changes
- ❌ Mix framework and extension permissions

### Security Principles

**Least Privilege:**
- Grant only what's needed for tasks
- Separate participant and facilitator permissions
- Use read-only where possible

**Defense in Depth:**
- Framework permissions + extension permissions
- Sandbox environments for events
- Monitoring via telemetry
- Regular permission audits

**Documentation:**
- Document permission requirements in room descriptions
- Include in setup guides
- Warn if business data access needed
- Explain why each permission needed

---

## Troubleshooting Tools

### Check User Permissions

```al
// In BC, check effective permissions
1. Search "Effective Permissions"
2. Select user
3. Select object type (Table/Page/Codeunit)
4. Find escape room objects
5. Verify Read/Insert/Modify/Delete/Execute
```

### Permission Recorder

```al
// Record actual permission usage
1. Search "Recording Permissions"
2. Start recording
3. Have user perform actions
4. Stop recording
5. Review actual permissions used
6. Generate permission set XML
```

### Event Viewer

```al
// Check for permission errors
1. Search "Event Log"
2. Filter by user
3. Filter by date range
4. Look for "Permission" errors
5. Note table/page/codeunit
6. Grant missing permissions
```

---

## Related Documentation

- [Framework Architecture](../Framework/Architecture.md) - System design
- [Event Setup](../Facilitator/Event-Setup.md) - Permission assignment in setup
- [Developer Guide](README.md) - Framework development
- [API Reference](API-Reference.md) - Permission-protected objects

---

**Last Updated:** January 7, 2026
