# Escape Room Leaderboard Setup

This document explains how to set up leaderboards and telemetry dashboards for the Escape Room application using Application Insights and Azure Data Explorer.

## Quick Setup Options

### Option 1: Import Pre-built Dashboard (Recommended)
1. **Use the dashboard JSON file**: `dashboard-BCTalent.EscapeRooms.json`
2. **Import into Azure Data Explorer** (Kusto) for instant dashboard setup
3. **Configure data source** to point to your Application Insights instance
4. **Adjust time range and venue filters** as needed

### Option 2: Manual Setup with KQL Queries
1. **Copy KQL queries** from `LeaderboardQueries.kql`
2. **Create custom dashboards** in Application Insights
3. **Build tiles manually** using the provided queries

## Telemetry Data Structure

The escape room telemetry uses these key custom dimensions:

### Core Event Data
- `eventId`: Event type (ALEscapeRoomTaskFinished, ALEscapeRoomRoomCompleted, etc.)
- `alScorePoints`: Score points for the event
- `alVenueUserName`: Attendee name
- `alVenuePartner`: Partner/consulting firm name
- `alVenueName`: Venue name
- `alRoomName`: Room name
- `alTaskName`: Task name

### Timing Data
- `alTaskStopDateTime`: Task completion timestamp
- `alRoomStartDateTime`: Room start timestamp
- `alRoomStopDateTime`: Room completion timestamp
- `alHintDateTime`: Hint request timestamp

### Additional Context
- `companyName`: Company name from BC
- `alVenueId`: Venue identifier
- `alHint`: Hint text (for hint requests)
- `alDescription`: Event description

## Dashboard Setup (Using JSON Import)

### Prerequisites
1. **Azure Data Explorer (Kusto)** access or Application Insights access
2. **BCTelemetry database** connected to your Application Insights data
3. **Permission** to create/import dashboards

### Import Steps
1. **Open Azure Data Explorer** (https://dataexplorer.azure.com)
2. **Navigate to Dashboards** section
3. **Click "Import Dashboard"** 
4. **Upload** the `dashboard-BCTalent.EscapeRooms.json` file
5. **Configure data source**:
   - **Cluster URI**: `https://ade.applicationinsights.io/subscriptions/{your-subscription-id}`
   - **Database**: `BCTelemetry`
6. **Adjust parameters**:
   - **Time range**: Default is last 6 hours
   - **Venue filter**: Select specific venues or "All"
   - **Room filter**: Select specific rooms or "All"

### Dashboard Features
The imported dashboard includes 4 pages with 14 tiles:

#### Page 1: Base Tables
- **Last 1000 Events**: Recent telemetry activity
- **Hints**: All hint requests with details
- **Tasks**: Task completion events
- **Rooms**: Room start/completion events  
- **Attendees**: Partner-attendee relationships

#### Page 2: Rooms
- **Started Rooms**: Currently active rooms by attendee
- **Finished Rooms**: Completed rooms with timing and partner info
- **Finished Rooms per User**: Bar chart of completion counts

#### Page 3: Tasks  
- **Finished Tasks**: Completed tasks with scores
- **Finished Tasks per User**: Bar chart of task completion counts
- **Hints (Newest First)**: Recent hints with context

#### Page 4: Leaderboard
- **Overall Leaderboard**: Top 5 users with comprehensive stats
- **Fastest Venue Completions**: Top 5 fastest completions with times
- **Score Breakdown Analysis**: Detailed scoring analysis for top 10 users

## Available KQL Queries

Based on the `LeaderboardQueries.kql` file, you can use these queries for custom dashboards:

### 1. Overall Leaderboard (Top 5 All Time)
Shows the top 5 users across all venues with:
- **Total score** aggregated from all events
- **Tasks/rooms/venues completed** counts
- **Hints used, solutions used** counts with penalties
- **Last activity date** for recency tracking

**Key Features**: Filters for non-empty attendee, score, and partner data. Orders by total score descending.

### 2. Leaderboard by Partner (Top 5 per Partner)
Breaks down rankings by venue partner to show top performers per consulting firm:
- **Partner-specific rankings** (Top 5 per partner)
- **Cross-partner comparison** capabilities
- **Same metrics as overall** but grouped by partner

### 3. Fastest Venue Completions (Top 5 by completion time)
Shows users who completed entire venues fastest:
- **Completion time** in readable format (e.g., "2h 15m")
- **Duration in minutes** for precise sorting
- **Final score achieved** at venue completion
- **Completion date** timestamp

**Key Features**: Uses `ALEscapeRoomVenueCompleted` events, calculates duration from start/stop times.

### 4. Most Recent Activity (Last 7 days)
Shows most active users in the past week:
- **Activity within last 7 days** only
- **List of recent activities** by venue/room
- **Unique venues/rooms visited** counts
- **Score from recent activity** period

### 5. Score Breakdown Analysis (Top 10)
Detailed analysis showing comprehensive scoring:
- **Task Points**: Points from completed tasks
- **Room Bonus Points**: Bonus points from room completions  
- **Venue Bonus Points**: Bonus points from venue completions
- **Hint Penalty**: Negative points from hints
- **Solution Penalty**: Negative points from solutions
- **Score Efficiency**: Score per activity ratio
- **Activity counts** for each category

**Key Features**: Uses conditional aggregation (`sumif`, `countif`) to separate different point sources.

### 6. Company/Team Leaderboard (Top 10)
Aggregated scores by company/organization:
- **Total company score** from all users
- **Average score per user** within company
- **Number of unique users** participating
- **Company-level activity statistics**
- **Ranking by total company performance**

## Manual Query Usage

If you prefer creating custom dashboards instead of importing the JSON:

1. **Open Application Insights** in Azure Portal or **Azure Data Explorer**
2. **Go to Logs/Query section**
3. **Use the base query structure** that provides `EscapeRoomData` dataset:
   ```kql
   traces
   | where timestamp between (_startTime .. _endTime)
   | where customDimensions.eventId startswith "ALEscapeRoom"
   | extend [all custom dimensions as shown in KQL file]
   | where venueName has_any (_venue)
   | where roomName has_any (_room)
   ```
4. **Add specific analysis** from the 6 query templates
5. **Adjust parameters**:
   - **Top N results**: Modify `where Rank <= 5` to desired number
   - **Time ranges**: Modify `ago(7d)` for different periods  
   - **Venue/room filters**: Add specific filtering criteria
6. **Pin to dashboard** for regular monitoring

## Data Structure & Event Types

The queries work with these primary event types:

### Event Types
- **`ALEscapeRoomTaskFinished`**: Task completion with points
- **`ALEscapeRoomRoomCompleted`**: Room completion with bonus
- **`ALEscapeRoomVenueCompleted`**: Full venue completion with timing
- **`ALEscapeRoomHintRequested`**: Hint usage with penalty
- **`ALEscapeRoomSolutionRequested`**: Solution usage with penalty
- **`ALEscapeRoomStarted`**: Room/venue start tracking

### Calculated Fields  
- **`durationMinutes`**: Calculated from start/stop timestamps
- **Various aggregations**: `sumif()`, `countif()`, `dcount()` for metrics
- **Ranking functions**: `row_number()` for leaderboard positions

## Customization Options

### Query Modifications
You can customize the provided queries:
- **Results count**: Change `where Rank <= 5` to show more/fewer results
- **Time periods**: Modify `ago(7d)` for different activity windows
- **Score thresholds**: Add minimum score requirements
- **Venue/room filtering**: Create location-specific leaderboards
- **Company filtering**: Focus on specific organizations
- **Event type filtering**: Analyze specific activity types

### Additional Analysis Ideas
- **Trend analysis**: Score progression over time
- **Completion rate analysis**: Task/room success rates
- **Hint usage patterns**: Most requested hints by room/task
- **Peak activity times**: When users are most active
- **Partner performance comparison**: Side-by-side partner metrics
- **Venue difficulty analysis**: Completion times by venue

## Dashboard Best Practices

### Recommended Dashboard Layout
1. **Executive Summary Tile**: Key metrics at a glance
2. **Live Leaderboard**: Current top performers with auto-refresh
3. **Recent Activity Feed**: Latest completions and achievements
4. **Performance Trends**: Score/completion charts over time
5. **Partner Comparison**: Side-by-side partner performance
6. **Venue Analytics**: Completion rates and average times per venue

### Refresh Intervals
- **Leaderboards**: Every 15-30 minutes during events
- **Activity feeds**: Every 5-10 minutes
- **Trend charts**: Hourly or daily depending on event duration
- **Summary stats**: Daily for historical analysis

## Data Retention & Archival

Application Insights and Azure Data Explorer retain data based on your service tier:
- **Application Insights**: 90 days to 2 years depending on plan
- **Azure Data Explorer**: Configurable retention policies
- **Long-term storage**: Consider exporting to Azure Storage for historical analysis

### Archival Recommendations
- **Export monthly snapshots** of leaderboard data
- **Create historical summary tables** for year-over-year comparisons  
- **Archive detailed event data** while keeping aggregated metrics
- **Implement data lifecycle policies** for automated cleanup

## Integration Options

### Power BI Integration
- **Connect Power BI** to Application Insights or Azure Data Explorer
- **Create interactive reports** with drill-down capabilities
- **Set up automated email reports** for stakeholders
- **Build mobile-friendly dashboards** for real-time monitoring

### Real-time Updates
- **Use Azure Data Explorer streaming** for near real-time updates
- **Implement webhooks** from Business Central for instant notifications
- **Create alerts** for leaderboard position changes
- **Set up automated social media** posts for achievements

## Future Enhancements

Consider these additional features:
- **Achievement badges/milestones** based on score thresholds
- **Venue-specific difficulty ratings** based on completion statistics
- **Predictive analytics** for completion likelihood
- **Team/group challenges** with collaborative scoring
- **Integration with external systems** (CRM, HRM, etc.)
- **Mobile app integration** for push notifications
- **Gamification elements** like streaks and challenges
- **Historical trend analysis** and seasonal patterns
