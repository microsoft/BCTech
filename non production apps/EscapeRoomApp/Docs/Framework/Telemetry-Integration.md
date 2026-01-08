# Telemetry Integration

## Overview

The BCTalent.EscapeRoom framework includes comprehensive telemetry integration with **Azure Application Insights**, enabling real-time tracking of participant progress, scoring, and leaderboard generation. All events are automatically logged with rich context for analysis.

---

## Application Insights Configuration

## Telemetry Events

### Event Types

The framework logs seven distinct event types:

#### 1. **EscapeRoomTaskFinished** 🎯

**When:** Task validation passes and task is marked complete  
**Score Impact:** +3 points  
**Custom Dimensions:**
- VenueId, VenueName
- PartnerName, FullName (participant info)
- RoomName
- TaskName
- ScorePoints: 3

**Logged By:** `LogFinishedTask(Task)`

---

#### 2. **EscapeRoomHintRequested** 💡

**When:** User views hint for a task  
**Score Impact:** -1 point  
**Custom Dimensions:**
- VenueId, VenueName
- PartnerName, FullName
- RoomName
- TaskName
- ScorePoints: -1

**Logged By:** `LogHintRequested(Task)`

---

#### 3. **EscapeRoomSolutionRequested** 📖

**When:** User views solution for a room  
**Score Impact:** -3 points  
**Custom Dimensions:**
- VenueId, VenueName
- PartnerName, FullName
- RoomName
- ScorePoints: -3

**Logged By:** `LogSolutionRequested(Room)`

---

#### 4. **EscapeRoomStarted** 🚀

**When:** Room status changes to InProgress  
**Score Impact:** 0 points  
**Custom Dimensions:**
- VenueId, VenueName
- PartnerName, FullName
- RoomName
- ScorePoints: 0

**Logged By:** `LogRoomStarted(Room)`

---

#### 5. **EscapeRoomCompleted** ✅

**When:** All tasks in a room are completed  
**Score Impact:** +5 bonus points  
**Custom Dimensions:**
- VenueId, VenueName
- PartnerName, FullName
- RoomName
- ScorePoints: 5

**Logged By:** `LogRoomCompleted(Room)`

---

#### 6. **EscapeRoomVenueCompleted** 🏆

**When:** All rooms in a venue are completed  
**Score Impact:** +10 bonus points  
**Custom Dimensions:**
- VenueId, VenueName
- PartnerName, FullName
- ScorePoints: 10

**Logged By:** `LogVenueCompleted(Venue)`

---

#### 7. **EscapeRoomNotification** 📢

**When:** Framework sends notification to user  
**Score Impact:** 0 points  
**Custom Dimensions:**
- NotificationText

**Logged By:** `LogNotification(NotificationText)`

---

## Scoring System

### Point Values

| Action | Points | Event |
|--------|--------|-------|
| Complete a task | **+3** | EscapeRoomTaskFinished |
| Complete a room (bonus) | **+5** | EscapeRoomCompleted |
| Complete entire venue (bonus) | **+10** | EscapeRoomVenueCompleted |
| Request hint | **-1** | EscapeRoomHintRequested |
| View solution | **-3** | EscapeRoomSolutionRequested |
| Start room | **0** | EscapeRoomStarted |

### Scoring Examples

**Perfect Run (5 tasks, 3 rooms, 1 venue):**
```
5 tasks × 3 points = 15 points
3 rooms × 5 points = 15 points
1 venue × 10 points = 10 points
Total: 40 points
```

**With Hints/Solutions:**
```
5 tasks × 3 points = 15 points
2 hints × -1 point = -2 points
1 solution × -3 points = -3 points
3 rooms × 5 points = 15 points
1 venue × 10 points = 10 points
Total: 35 points
```

---

## Telemetry Codeunit Structure

### Codeunit 73925 "Escape Room Telemetry"

**Key Procedures:**

```al
procedure LogFinishedTask(var Task: Record "Escape Room Task")
procedure LogHintRequested(var Task: Record "Escape Room Task")
procedure LogSolutionRequested(var Room: record "Escape Room")
procedure LogRoomStarted(var Room: Record "Escape Room")
procedure LogRoomCompleted(var Room: Record "Escape Room")
procedure LogVenueCompleted(var Venue: Record "Escape Room Venue")
procedure LogNotification(NotificationText: Text)
```

**Internal Procedures:**

```al
local procedure GetCustomDimensionsForTask(var Task, var CustomDimensions)
local procedure GetCustomDimensionsForRoom(var Room, var CustomDimensions)
local procedure GetCustomDimensionsForVenue(var Venue, var CustomDimensions)
local procedure AddScorePoints(var CustomDimensions, ScorePoints: Integer)
local procedure LogMessage(EventName: Text, Message: Text, CustomDimensions)
```

---

## Custom Dimensions

### Data Captured

Every telemetry event includes rich context:

**Venue-Level Dimensions:**
```json
{
  "VenueId": "DEV1",
  "VenueName": "Development Track",
  "PartnerName": "Microsoft",
  "FullName": "John Doe"
}
```

**Room-Level Dimensions (includes venue):**
```json
{
  "VenueId": "DEV1",
  "VenueName": "Development Track",
  "PartnerName": "Microsoft",
  "FullName": "John Doe",
  "RoomName": "Introduction Room"
}
```

**Task-Level Dimensions (includes venue + room):**
```json
{
  "VenueId": "DEV1",
  "VenueName": "Development Track",
  "PartnerName": "Microsoft",
  "FullName": "John Doe",
  "RoomName": "Introduction Room",
  "TaskName": "Create Translation Copilot App"
}
```

**Scoring Dimension (all events):**
```json
{
  "ScorePoints": "3"  // or -1, -3, 5, 10, 0
}
```

---

## Leaderboard Queries

### KQL Queries

Located in `Leaderboard/LeaderboardQueries.kql`.

**Example: Top Scores by Venue**

```kql
traces
| where message == "EscapeRoomTaskFinished" 
    or message == "EscapeRoomCompleted" 
    or message == "EscapeRoomVenueCompleted"
    or message == "EscapeRoomHintRequested"
    or message == "EscapeRoomSolutionRequested"
| extend VenueId = tostring(customDimensions.VenueId)
| extend FullName = tostring(customDimensions.FullName)
| extend PartnerName = tostring(customDimensions.PartnerName)
| extend ScorePoints = toint(customDimensions.ScorePoints)
| summarize TotalScore = sum(ScorePoints) by VenueId, FullName, PartnerName
| order by TotalScore desc
```

**Example: Fastest Completions**

```kql
traces
| where message == "EscapeRoomVenueCompleted"
| extend VenueId = tostring(customDimensions.VenueId)
| extend FullName = tostring(customDimensions.FullName)
| extend Duration = datetime_diff('minute', timestamp, 
    toscalar(traces 
    | where message == "EscapeRoomStarted" 
    | extend VenueIdStart = tostring(customDimensions.VenueId)
    | where VenueIdStart == VenueId
    | summarize min(timestamp)))
| project VenueId, FullName, Duration, timestamp
| order by Duration asc
```

---

## Dashboard Configuration

### Dashboard JSON

Located in `Leaderboard/dashboard-BCTalent.EscapeRooms.json`.

**Key Visualizations:**

1. **Total Participants** - Count of unique FullName values
2. **Venue Completions** - Count of EscapeRoomVenueCompleted events
3. **Top Scores** - Leaderboard by total points
4. **Completion Times** - Fastest venue completions
5. **Event Timeline** - Activity over time
6. **Hint/Solution Usage** - Track help requests

### Importing Dashboard

See [Leaderboard Setup Guide](../Facilitator/Leaderboard-Setup.md) for detailed instructions.

---

## Implementation Details

### When Telemetry is Logged

**Task Completion:**
```al
procedure SetStatusCompleted()
var
    EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
begin
    Rec.Status := Rec.Status::Completed;
    Rec."Stop DateTime" := CurrentDateTime;
    Rec.Modify();
    
    EscapeRoomTelemetry.LogFinishedTask(Rec);  // ← Telemetry logged here
    
    // Triggers room completion check
end;
```

**Room Completion:**
```al
procedure CloseRoomIfCompleted()
var
    EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
begin
    if Rec."No. of Uncompleted Tasks" = 0 then begin
        Rec.Status := Rec.Status::Completed;
        Rec."Stop DateTime" := CurrentDateTime;
        Rec.Modify();
        
        EscapeRoomTelemetry.LogRoomCompleted(Rec);  // ← Telemetry logged here
        
        OpenNextRoom();
        CloseVenueIfCompleted();
    end;
end;
```

**Venue Completion:**
```al
procedure CloseVenueIfCompleted()
var
    EscapeRoomTelemetry: Codeunit "Escape Room Telemetry";
begin
    // Check all rooms complete
    if AllRoomsCompleted() then begin
        Rec."Stop DateTime" := CurrentDateTime;
        Rec.Modify();
        
        EscapeRoomTelemetry.LogVenueCompleted(Rec);  // ← Telemetry logged here
    end;
end;
```

---

## Telemetry Best Practices

### For Framework Maintainers

**1. Maintain Event Consistency**
- Don't change event names - breaks existing queries
- Keep custom dimensions consistent
- Add new dimensions carefully (won't break old queries)

**2. Include Participant Context**
- Always include FullName and PartnerName when available
- Enables filtering by participant in queries
- Required for accurate leaderboards

**3. Score Points Accurately**
- Every scoring event must include ScorePoints dimension
- Use consistent point values across framework
- Document point values in telemetry and user docs

**4. Log at Right Time**
- Log after record modifications (Modify() before logging)
- Ensures telemetry reflects actual database state
- Prevents phantom events from failed operations

### For Event Organizers

**1. Test Telemetry Before Events**
- Verify events appear in Application Insights
- Test KQL queries return correct data
- Confirm dashboard displays properly

**2. Monitor During Events**
- Watch real-time event stream
- Check for anomalies or errors
- Identify stuck participants

**3. Analyze After Events**
- Generate final leaderboards
- Review completion rates
- Identify challenging tasks/rooms

---

## Privacy Considerations

### Participant Data

**Collected:**
- Full Name (self-entered)
- Partner Name (self-entered)
- Completion times
- Hint/solution usage
- Score

**NOT Collected:**
- User IDs
- Email addresses
- IP addresses
- Detailed session data

### GDPR Compliance

- Participants enter their own names voluntarily
- No PII collected beyond what users provide
- Telemetry used only for leaderboard/analytics
- Event organizers control Application Insights access

---

## Troubleshooting Telemetry

### Events Not Appearing

**Check:**
1. Application Insights connection string valid
2. Network connectivity to Azure
3. Permissions on Application Insights resource
4. Event name spelled correctly in queries

### Incorrect Scores

**Check:**
1. ScorePoints dimension present on all scoring events
2. Point values match documented system
3. No duplicate events being logged
4. Query logic sums correctly

### Missing Custom Dimensions

**Check:**
1. Venue/Room/Task records have required fields populated
2. FullName and PartnerName entered on venue
3. CustomDimensions dictionary populated before logging
4. No null/empty values breaking queries

---

## Related Documentation

- [Architecture Overview](Architecture.md) - How telemetry fits into framework
- [Leaderboard Setup](../Facilitator/Leaderboard-Setup.md) - Configuring dashboards
- [Event Setup Guide](../Facilitator/Event-Setup.md) - Preparing for events

---

**Last Updated:** January 7, 2026
