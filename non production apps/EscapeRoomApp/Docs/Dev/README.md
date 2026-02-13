# Developer Documentation

## Overview

Technical documentation for developers working on the BCTalent.EscapeRoom framework itself (not extension developers creating rooms - see [Framework Documentation](../Framework/) for that).

This guide covers framework maintenance, core architecture modifications, and contribution guidelines.

---

## Quick Navigation

### Framework Documentation (For Extension Developers)
If you're creating new escape room venues, rooms, or tasks, see:
- [Framework Architecture](../Framework/Architecture.md)
- [Creating New Rooms](../Framework/Creating-Rooms.md)
- [Task Validation System](../Framework/Task-Validation.md)
- [Telemetry Integration](../Framework/Telemetry-Integration.md)

### Core Developer Reference (This Section)
For framework maintainers and contributors:
- [API Reference](API-Reference.md) - Complete interface specifications
- [Permission Sets](Permissions.md) - Security configuration
- [Dependencies](#dependencies) - Required apps and libraries
- [Contributing](#contributing-to-framework) - How to contribute
- [Testing](#testing-framework-changes) - Validation procedures

---

## Framework Structure

### Folder Organization

```
EscapeRoomApp/
├── Src/
│   ├── 1.Venue/              # Venue-related objects
│   │   ├── EscapeRoomVenue.Table.al
│   │   ├── EscapeRoomVenueCard.Page.al
│   │   ├── EscapeRoomVenueList.Page.al
│   │   └── Interface/
│   │       ├── iEscapeRoomVenue.Interface.al
│   │       └── EscapeRoomVenue.Enum.al
│   ├── 2.Room/                # Room-related objects
│   │   ├── EscapeRoom.Table.al
│   │   ├── EscapeRoom.Page.al
│   │   ├── EscapeRoomList.Page.al
│   │   ├── EscapeRoomStatus.Enum.al
│   │   ├── Hints/
│   │   └── Interface/
│   │       └── iEscapeRoom.Interface.al
│   ├── 3.Task/                # Task-related objects
│   │   ├── EscapeRoomTask.Table.al
│   │   ├── EscapeRoomTaskList.Page.al
│   │   ├── EscapeRoomTaskStatus.Enum.al
│   │   ├── IEscapeRoomTaskValidation.Interface.al
│   │   ├── Interface/
│   │   └── RunTests/
│   ├── Telemetry/             # Telemetry integration
│   │   ├── EscapeRoomTelemetry.Codeunit.al
│   │   └── TelemetryLogger.Codeunit.al
│   ├── APIs/                  # API pages
│   │   └── AutSecurityGroups.Page.al
│   ├── EscapeRoom.Codeunit.al          # Core framework logic
│   ├── EscapeRoomNotifications.Codeunit.al
│   ├── BusinessManagerRCExt.PageExt.al  # Role Center extension
│   ├── PicturePage.Page.al
│   └── RichTextBoxPage.Page.al
├── Docs/                      # Documentation (you are here!)
├── Leaderboard/               # Telemetry dashboards and queries
│   ├── dashboard-BCTalent.EscapeRooms.json
│   └── LeaderboardQueries.kql
├── EscapeRoomAdmin.PermissionSet.al
├── BusPremiumExt.PermissionSetExt.al
├── FullAdminExt.PermissionSetExt.al
└── app.json
```

---

## Dependencies

### Framework Has NO Dependencies

The framework is designed to be standalone with no dependencies on other apps. This ensures:
- Extension apps only need framework as dependency
- Framework can be deployed independently
- No circular dependencies
- Clear separation of concerns

### Extension Apps Depend on Framework

All venue apps depend on:
```json
{
  "dependencies": [
    {
      "id": "f03c0f0c-d887-4279-b226-dea59737ecf8",
      "name": "BCTalent.EscapeRoom",
      "publisher": "waldo & AJ",
      "version": "1.0.0.0"
    }
  ]
}
```

---

## Core Components

### Tables

| Object | ID | Purpose |
|--------|-----|---------|
| Escape Room Venue | 73926 | Stores venue records |
| Escape Room | 73920 | Stores room records |
| Escape Room Task | 73922 | Stores task records |

### Pages

| Object | ID | Type | Purpose |
|--------|-----|------|---------|
| Escape Room Venue List | 73927 | List | Browse venues |
| Escape Room Venue Card | 73926 | Card | Venue details + rooms |
| Escape Room Page | 73920 | Card | Room details + tasks |
| Escape Room Task List | 73923 | ListPart | Task subpage |
| Picture Page | 73928 | Card | Display badge images |
| Rich Text Box Page | 73929 | Card | Display HTML descriptions |

### Codeunits

| Object | ID | Purpose |
|--------|-----|---------|
| Escape Room | 73922 | Core framework logic |
| Escape Room Notifications | (TBD) | Notification management |
| Escape Room Telemetry | 73925 | Telemetry logging |
| Telemetry Logger | (TBD) | Low-level telemetry |

### Enums

| Object | Purpose | Extensible |
|--------|---------|-----------|
| Escape Room Venue | Venue identification | Yes ✅ |
| Escape Room | Room identification | Yes ✅ |
| Escape Room Task | Task identification | Yes ✅ |
| Escape Room Status | Room status tracking | No ❌ |
| Escape Room Task Status | Task status tracking | No ❌ |

### Interfaces

| Object | Purpose | Implemented By |
|--------|---------|----------------|
| iEscapeRoomVenue | Venue behavior | Extension apps |
| iEscapeRoom | Room behavior | Extension apps |
| iEscapeRoomTask | Task behavior | Extension apps |
| IEscape Room Task Validation | Task validation | Extension apps |

---

## Object ID Ranges

### Current Range: 73920-73999 (80 IDs)

**Allocation:**
- 73920-73929: Core tables and pages
- 73930-73939: Reserved for future tables
- 73940-73959: Reserved for future pages
- 73960-73979: Reserved for codeunits
- 73980-73999: Reserved for enums/interfaces

**Usage as of v1.3:**
- Tables: 73920, 73922, 73926 (3 used)
- Pages: 73920, 73923, 73926, 73927, 73928, 73929 (6 used)
- Codeunits: 73922, 73925 (2 used)
- Enums: 5 used (IDs not in range - system assigned)
- Interfaces: 4 used (IDs not in range - system assigned)

**Capacity:** Plenty of room for expansion

---

## Contributing to Framework

### Development Workflow

**1. Clone Repository**
```bash
git clone https://github.com/waldo1001/BCTalent.EscapeRoom.git
cd BCTalent.EscapeRoom/EscapeRoomApp
```

**2. Create Feature Branch**
```bash
git checkout -b feature/your-feature-name
```

**3. Make Changes**
- Follow AL coding standards
- Update documentation
- Add/update tests
- Maintain backward compatibility

**4. Test Thoroughly**
- Test with all existing venue apps
- Verify telemetry still works
- Check interface contracts preserved
- Validate permission sets

**5. Submit Pull Request**
- Descriptive PR title and description
- Reference any related issues
- Include testing notes
- Update CHANGELOG.md

### Code Standards

**AL Coding Conventions:**
- Use meaningful names
- No hardcoded IDs (use object references)
- Comment complex logic
- Handle errors gracefully
- Use proper data classifications

**Interface Changes:**
- **NEVER break existing interfaces** without major version bump
- Add optional parameters at end
- Deprecate old methods, don't remove
- Document breaking changes clearly

**Telemetry Changes:**
- Don't change event names (breaks queries)
- Add new dimensions carefully
- Maintain ScorePoints consistency
- Test queries after changes

---
### Test Scenarios

**Core Functionality:**
- [ ] Venue registration works
- [ ] Rooms created in correct sequence
- [ ] Tasks created and linked properly
- [ ] Status tracking functions
- [ ] Telemetry logged correctly
- [ ] Scoring calculated accurately

**Interface Compatibility:**
- [ ] Existing venue apps still work
- [ ] No compilation errors
- [ ] Runtime behavior unchanged
- [ ] Telemetry preserved

**Permission Tests:**
- [ ] Users can access venues with EscapeRoomAdmin
- [ ] Non-admin users blocked appropriately
- [ ] Permission extensions work

**Telemetry Validation:**
- [ ] Events appear in Application Insights
- [ ] Custom dimensions correct
- [ ] KQL queries return data
- [ ] Dashboard displays correctly

---

## Versioning Strategy

### Semantic Versioning

Format: `MAJOR.MINOR.PATCH.BUILD`

**MAJOR:** Breaking changes (interface modifications)
**MINOR:** New features (backward compatible)
**PATCH:** Bug fixes (no new features)
**BUILD:** Build number (auto-increment)

**Example:** `1.3.10026.0`
- Major: 1 (original framework)
- Minor: 3 (feature updates)
- Patch: 10026 (incremental)
- Build: 0

**Guarantee:** Any 1.x framework version works with any venue app built for 1.0+

---

## Performance Considerations

### Database Queries

**Optimized:**
- FlowFields for task counts (calculated on demand)
- Filtered queries in validation
- Indexed primary keys

**Potential Issues:**
- Many tasks in one room (>50)
- Many concurrent participants (>100)
- Complex validation queries

**Mitigation:**
- Use LoadFields for specific fields
- Set appropriate filters before FindSet
- Limit validation query complexity
- Consider caching in validation codeunits

### Telemetry Impact

**Minimal Overhead:**
- Async logging to Application Insights
- No blocking on telemetry calls
- Batched where possible

**Cost Considerations:**
- ~1-2 KB per event
- 50-100 events per completed venue
- 5-10 MB per 100 participants
- ~$2-5 per event in AI costs

---

## Security Considerations

### Permission Model

**Framework Permissions:**
- Read: Venues, Rooms, Tasks (all participants)
- Write: Task status updates (participants)
- Admin: Venue setup, room configuration (facilitators)

**Extension Permissions:**
- Defined by extension app
- Framework doesn't restrict validation logic
- Validation queries need appropriate permissions

### Data Classification

All tables use `DataClassification = CustomerContent`:
- Participant names (PII)
- Progress data (personal)
- Telemetry linked to individuals

**GDPR Compliance:**
- Participants enter own data voluntarily
- No automatic PII collection
- Event organizers control data
- Can be anonymized post-event

---

## Troubleshooting Framework Issues

### Common Development Issues

**Interface Not Found:**
- Check casing (AL is case-sensitive)
- Verify interface defined in framework
- Check app dependencies
- Rebuild symbols

**Enum Value Conflict:**
- Use unique ID ranges per extension
- Don't reuse framework enum values
- Coordinate with other extension developers

**Validation Not Triggering:**
- Check TestCodeunitId set correctly
- Verify interface implementation
- Check procedure signatures match
- Look for errors in Event Log

**Telemetry Missing:**
- Verify Application Insights connection
- Check network connectivity
- Test with manual event
- Review Event Log

---

## Roadmap & Future Enhancements

### Planned Features

**Framework:**
- [ ] Partial task completion tracking
- [ ] Time-based challenges
- [ ] Multi-language support
- [ ] Enhanced hint system
- [ ] Team collaboration features

**Telemetry:**
- [ ] Real-time leaderboard API
- [ ] Enhanced dashboard widgets
- [ ] Participant progress notifications
- [ ] Automated reporting

**Development:**
- [ ] Visual room designer
- [ ] Validation testing framework
- [ ] Template repositories
- [ ] Enhanced debugging tools

### Contribution Ideas

Want to contribute? Consider:
- New UI features for participant experience
- Enhanced telemetry visualizations
- Automated testing tools
- Documentation improvements
- Performance optimizations
- Accessibility enhancements

---

## Related Documentation

- [API Reference](API-Reference.md) - Complete interface specs
- [Permission Sets](Permissions.md) - Security configuration
- [Framework Architecture](../Framework/Architecture.md) - Design patterns
- [Changelog](../CHANGELOG.md) - Version history

---

**Last Updated:** January 7, 2026
