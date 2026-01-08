# Event Setup Guide

## Overview

This guide explains how to prepare Business Central environments for escape room events. It covers sandbox provisioning, app publishing, user setup, and environment configuration.

---

## Prerequisites

- Azure subscription with BC sandbox capability
- Admin access to Business Central Admin Center
- PowerShell with BC modules installed
- Compiled app files (.app) for all escape room apps
- Application Insights resource (for telemetry)

---

## Step 1: Generate User Credentials

### Using Password Generation Script

Located in `/Scripts/0-generate-passwords.ps1`:

```powershell
# Generates secure passwords for all participant accounts
.\0-generate-passwords.ps1
```

**What it does:**
- Creates unique passwords for each environment
- Stores passwords securely
- Generates credential export for distribution

**Output:**
- `passwords.txt` - Credential list for distribution
- Used by subsequent scripts for automated setup

---

## Step 2: Create Users and Security Groups

### Using User Creation Script

Located in `/Scripts/1-create-users-and-groups.ps1`:

```powershell
# Creates Microsoft 365 users and security groups
.\1-create-users-and-groups.ps1
```

**What it does:**
- Creates M365 users for each participant
- Creates security groups for access control
- Assigns users to appropriate groups
- Sets up Azure AD structure

**Parameters:**
- User naming convention
- Group naming convention
- Number of environments
- Domain information

---

## Step 3: Delete Old Sandbox Environments (Optional)

### Using Deletion Script

Located in `/Scripts/2-delete-sandbox-environments.ps1`:

```powershell
# Removes old sandbox environments
.\2-delete-sandbox-environments.ps1
```

**What it does:**
- Connects to BC Admin Center
- Deletes specified sandbox environments
- Cleans up old test data
- Frees up sandbox quota

**Use When:**
- Starting fresh for new event
- Cleaning up after previous event
- Resolving environment issues

---

## Step 4: Create Sandbox Environments

### Using Environment Creation Script

Located in `/Scripts/3-create-sandbox-environments.ps1`:

```powershell
# Creates new sandbox environments for event
.\3-create-sandbox-environments.ps1
```

**What it does:**
- Creates sandbox environment for each participant/team
- Configures environment settings
- Sets appropriate Business Central version
- Applies naming conventions

**Configuration:**
```powershell
$EnvironmentPrefix = "BCTalent-EscapeRoom"
$EnvironmentRange = 21..40  # Creates 20 environments
$BCVersion = "26.0"  # BC version
$CountryCode = "US"
```

**Output:**
- 20 sandbox environments (configurable)
- Named: `BCTalent-EscapeRoom-21` through `BCTalent-EscapeRoom-40`
- Ready for app publishing

---

## Step 5: Configure Application Insights

### Using App Insights Script

Located in `/Scripts/4-set-appinsights-key.ps1`:

```powershell
# Configures Application Insights connection for telemetry
.\4-set-appinsights-key.ps1
```

**What it does:**
- Sets Application Insights connection string
- Configures telemetry settings
- Enables event logging
- Connects to leaderboard infrastructure

**Configuration:**
- Application Insights connection string from `app.json`
- Applied to all sandbox environments
- Enables real-time telemetry

---

## Step 6: Assign Security Groups

### Using Security Group Assignment Script

Located in `/Scripts/5-set-security-group.ps1`:

```powershell
# Assigns security groups to environments
.\5-set-security-group.ps1
```

**What it does:**
- Maps security groups to environments
- Controls access per environment
- Ensures proper isolation
- Assigns appropriate permissions

**Configuration:**
- Security group IDs from Step 2
- Environment mappings
- Access levels

---

## Step 7: Publish Apps

### Using App Publishing Script

Located in `/Scripts/6-publish-apps.ps1`:

```powershell
# Publishes escape room apps to all environments
.\6-publish-apps.ps1
```

**What it does:**
- Publishes framework app (BCTalent.EscapeRoom)
- Publishes venue apps 
- Publishes test apps (if applicable)
- Installs and synchronizes all apps

**Publishing Order (Important):**
1. Framework app first (BCTalent.EscapeRoom)
2. Venue apps second 
3. Test apps last (if applicable)

### Manual Publishing Alternative

If scripts don't work, publish manually via BC Admin Center:

1. **Navigate to environment**
2. **Extension Management**
3. **Upload Extension**
4. **Select .app file**
5. **Publish → Install → Synchronize**
6. **Repeat for each app** (maintain order!)

---

## Step 8: Configure Security Group Permissions

### Using Permission Script

Located in `/Scripts/7-set-security-group-permissions.ps1`:

```powershell
# Sets appropriate permissions for security groups
.\7-set-security-group-permissions.ps1
```

**What it does:**
- Assigns Escape Room Admin permission set
- Grants necessary BC permissions
- Configures user access levels
- Ensures participants can complete tasks

**Permission Sets:**
- `ESCAPEROOM ADMIN` - Full access to escape room features
- `D365 BUSINESS PREMIUM` extension - Include escape room permissions
- Standard BC permissions as needed

---

## Step 9: Verify Setup

### Verification Checklist

For **each environment**, verify:

✅ **Environment Created**
- Sandbox active and accessible
- Correct BC version
- Named correctly

✅ **Apps Published**
- BCTalent.EscapeRoom installed
- Venue apps installed 
- No installation errors
- All apps synchronized

✅ **Telemetry Working**
- Application Insights configured
- Connection string set
- Test event logged successfully

✅ **Users Configured**
- M365 users created
- Security groups assigned
- Users can access environment
- Correct permissions applied

✅ **Escape Rooms Accessible**
- Escape Room Venues action visible
- Venues appear in list
- Rooms created correctly
- Tasks loaded successfully

### Quick Test Procedure

1. **Log in as test user**
2. **Search for "Escape Room Venues"**
3. **Open a venue**
4. **Enter Full Name and Partner Name**
5. **Open first room**
6. **Complete a simple task**
7. **Verify task marked complete**
8. **Check telemetry in Application Insights**

---

## Environment Specifications

### Recommended Configuration

**Per Environment:**
- **Type:** Sandbox
- **Version:** BC 26.0 or later
- **Country:** US (or event location)
- **Users:** 1-2 participants
- **Apps:** 4 (Framework + 3 venues)
- **Telemetry:** Shared Application Insights

**Capacity Planning:**
- **20 environments** = 20-40 participants (1-2 per environment)
- **40 environments** = 40-80 participants
- Adjust based on event size

### Naming Conventions

**Environments:**
```
BCTalent-EscapeRoom-01
BCTalent-EscapeRoom-02
...
BCTalent-EscapeRoom-40
```

**Security Groups:**
```
EscapeRoom-Group-01
EscapeRoom-Group-02
...
```

**Users:**
```
escaperoom01@yourdomain.com
escaperoom02@yourdomain.com
...
```

---

## Troubleshooting Setup

### Apps Won't Publish

**Check:**
- Correct publishing order (Framework first)
- App dependencies satisfied
- Version compatibility
- No conflicting apps
- Sufficient environment capacity

### Users Can't Access

**Check:**
- Security group assignments
- Permission sets applied
- M365 licenses assigned
- User synchronization complete
- Correct environment URL

### Telemetry Not Working

**Check:**
- Application Insights connection string
- Connection string in environment
- Network connectivity to Azure
- Event log for errors
- Test with manual event

### Venues Not Appearing

**Check:**
- Apps fully installed (not just published)
- Install codeunits executed
- No errors in Event Log
- Database synchronization complete
- User permissions sufficient

---

## Event Day Checklist

### Before Participants Arrive

✅ All environments accessible
✅ All apps published and working
✅ Telemetry dashboard functional
✅ User credentials prepared
✅ Leaderboard queries tested
✅ Facilitator access confirmed
✅ Backup plan prepared

### During Event

✅ Monitor telemetry real-time
✅ Watch for stuck participants
✅ Check for environment issues
✅ Track completion rates
✅ Be ready to assist

### After Event

✅ Generate final leaderboards
✅ Export telemetry data
✅ Collect participant feedback
✅ Document issues encountered
✅ Clean up environments (optional)
✅ Archive credentials securely

---

## Scaling for Large Events

### For 100+ Participants

**Consider:**
1. **Multiple Application Insights resources** - Avoid throttling
2. **Staggered environment creation** - Avoid quota issues
3. **Load testing** - Test with pilot group first
4. **Support team** - Multiple facilitators
5. **Backup environments** - For technical issues

### Automation Tips

**Create Master Script:**
```powershell
# Full setup automation
.\0-generate-passwords.ps1
.\1-create-users-and-groups.ps1
.\3-create-sandbox-environments.ps1
.\4-set-appinsights-key.ps1
.\5-set-security-group.ps1
.\6-publish-apps.ps1
.\7-set-security-group-permissions.ps1
```

**Parallel Execution:**
- Publish apps to multiple environments in parallel
- Use PowerShell workflows
- Monitor for errors carefully

---

## Cost Considerations

### Azure Costs

**Per Event:**
- **BC Sandbox Environments:** Included in BC license (quota limits)
- **Application Insights:** ~$2-5 per event (depends on volume)
- **M365 Users:** Temporary licenses (plan accordingly)
- **Data Storage:** Minimal for telemetry

**Cost Optimization:**
- Delete sandboxes after event
- Use shared Application Insights
- Archive telemetry data
- Reuse environments when possible

---

## Related Documentation

- [Leaderboard Setup](Leaderboard-Setup.md) - Configure dashboards
- [Participant Management](Participant-Management.md) - User onboarding
- [Architecture Overview](../Framework/Architecture.md) - Framework design
- [Telemetry Integration](../Framework/Telemetry-Integration.md) - Event tracking

---

**Last Updated:** January 7, 2026
