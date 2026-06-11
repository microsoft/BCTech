# Record Link Migration Tool

Migrates record links and notes from an on-premises Business Central SQL database to Business Central Online (SaaS). Data is replicated into a buffer table via the Custom Migration Provider framework, then transferred to the system Record Link table with user mapping, duplicate detection, and per-company progress tracking.

## Architecture

**Integration layer** (`src/Integration/`):
- Implements the `"Custom Migration Provider"` interface
- Registered via enum extension on `"Custom Migration Provider"` enum (value 57500)
- `SetupReplicationTableMappings()` maps source `Record Link` system table → `RL Migration Buffer` (table 57500)
- Framework calls this automatically when migration is enabled or companies change

**Record Link Management layer** (`src/RecordLinkManagement/`):
- Core migration logic, user mapping, duplicate detection, progress tracking
- All tables, pages, and management codeunits

## Data Flow

```
On-Prem SQL "Record Link" table
        │
        ▼ (Replication via Custom Migration framework)
RL Migration Buffer (table 57500)
        │
        ├─► Define User Mappings (on-prem → SaaS usernames)
        ├─► Apply Mapping (updates User ID / To User ID in buffer)
        ├─► Identify Duplicates (optional, marks existing matches)
        ├─► Verify Records Exist (optional, warns about orphans)
        │
        ▼ (Transfer action)
System "Record Link" table (new Link IDs assigned)
        │
        ▼
RL Migration Mapping (table 57501) — tracks Source ID → Target ID
```

## Objects

| ID | Type | Name | Purpose |
|----|------|------|---------|
| 57500 | Table | RL Migration Buffer | Staging for replicated record links. DataPerCompany=false, ReplicateData=true |
| 57501 | Table | RL Migration Mapping | Source→Target ID mapping + content fields for duplicate detection |
| 57502 | Table | RL Migration User Mapping | On-prem user → SaaS user mapping |
| 57503 | Table | RL Migration Progress | Per-company: total, migrated, last processed ID, status, errors |
| 57500 | Codeunit | RL Migration Provider | Custom Migration Provider interface implementation |
| 57500 | EnumExt | RL Migration Provider | Extends "Custom Migration Provider" enum |
| 57501 | Codeunit | RL Migration Mgt. | Transfer logic, user mapping, progress dialog, batch commits |
| 57502 | Codeunit | RL Migration Duplicate Finder | Content-based duplicate detection |
| 57500 | Page | RL Migration Dashboard | Progress view + actions (Transfer, Verify, Duplicates, User Mapping, Delete Buffer) |
| 57501 | Page | RL Migration User Mapping | Define/edit user mappings with Populate, Apply, Clear actions |
| 57502 | Page | RL Migration Duplicates | Review detected duplicates with Skip/Overwrite options |

## Key Design Decisions

- **Buffer persists** after migration — users delete manually (warning about losing duplicate detection)
- **No Install/Upgrade codeunit** — mappings registered by the framework via interface call
- **InherentPermissions/Entitlements** — no separate permission set objects
- **Duplicate detection is a separate step** — not done during transfer; uses mapping table (Record ID + Type + Description + first 2048 chars of Note)
- **User mapping applied to buffer** — usernames are translated in the buffer before transfer, not during
- **Resume support** — tracks last processed Link ID per company; batch commits every 1000 records
- **New Link IDs always assigned** — never reuses source IDs to avoid conflicts with existing data
- **Blank-company records** always migrated (per-database links not tied to any company)
- **Non-existent target records** — warned but not blocked (orphaned links still transferable)

## Dependencies

- `Intelligent Cloud Base` (Microsoft, ID `58623bfa-0559-4bc2-ae1c-0979c29fd9e0`) — provides Custom Migration Provider interface, Replication Table Mapping, Hybrid Company
- **NOT** dependent on HybridBaseDeployment

## Usage Workflow

1. **Enable Custom Migration** — select "Record Link Migration" as the Custom Migration Provider in Cloud Migration Setup
2. **Run replication** — the framework replicates the on-prem `Record Link` table into the buffer
3. **Define User Mappings** — open the Dashboard → "Define User Mappings" → "Populate Users" to auto-detect users from the buffer and copy from existing cloud migration user mappings (matches by email where possible). Review and edit as needed.
4. **Apply Mapping** — click "Apply Mapping" to update User ID and To User ID in the buffer with the mapped SaaS usernames
5. **Identify Duplicates** (optional) — scan buffer against existing Record Links; review and mark as Skip/Overwrite
6. **Verify Records Exist** (optional) — check that target records still exist; warns about orphaned links
7. **Transfer to Record Link** — creates system Record Link entries from the buffer, assigns new Link IDs, stores mapping for future runs

Steps 3–6 can be repeated. Transfer is resumable — if interrupted, re-running picks up from the last processed Link ID.

## User Mapping Details

The "Populate Users" action pre-populates the user mapping table from two sources:
1. **Existing cloud migration user mappings** — copies from the `User Mapping Source` table (if previously defined in standard cloud migration) and auto-matches by `Authentication Email`
2. **Buffer contents** — extracts all distinct `User ID` and `To User ID` values from the buffer that aren't already mapped

If the user mapping table already has data, the admin is prompted to confirm before adding new entries. A "Clear All Mappings" action is available to start fresh.
