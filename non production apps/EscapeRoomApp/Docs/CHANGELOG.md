# Changelog

All notable changes to the BCTalent.EscapeRoom framework are documented here.

---

## [1.4.1.0] - 2026-09-14

### Fixed

- **Two rooms could be opened / venue closed prematurely** — `UpdateStatus()` on an already *Completed* room re-ran `OpenNextRoom()` unconditionally. Because that procedure looked for the next *Locked* room, it skipped the room that was already in progress and opened the one after it; once no locked room was left it stopped the venue even though rooms were still in progress. Pressing "Update Status", "Get Hint" or "Solve" on a completed room card (reachable from the room list and from the task-completed notification) triggered it.
  - `OpenNextRoom()` is now idempotent: it exits when a later room is already InProgress, and closes the venue only via `CloseVenueIfCompleted()` (all rooms Completed).
  - Room, task and venue status transitions re-read the row under an update lock and re-check the status before modifying, closing the race where the same completion ran in several sessions at once (e.g. event subscribers fired from the concurrency simulations' background sessions).
  - Room/venue state is committed before the completion image is shown, so an interrupted session can no longer leave the next room locked.
- Renamed a local variable named `Key` in codeunit 73925 "Escape Room Telemetry" (reserved word in current AL compilers, blocked the build).

### Changed

- `"Escape Room Venue".Stop()` and `CloseVenueIfCompleted()` now return `Boolean` (true when the venue got completed by that call) and no longer show the completion image themselves; `"Escape Room".Stop()` shows it.

---

## [1.4.0.0] - 2026-06-10

### Added

- **Custom Telemetry Events API** — New public `LogCustomEvent` overloads on codeunit 73925 "Escape Room Telemetry" allow room extension apps to emit custom scoring and diagnostic events.
  - Task-scoped and room-scoped variants
  - Optional `ExtraDimensions` parameter for caller-provided custom dimensions
  - Score clamped to -5..+5 to keep leaderboards balanced
  - All custom events use the fixed event name `EscapeRoomCustomEvent` with `EventSource = Custom` dimension
- **Custom Events Overview** KQL query in `LeaderboardQueries.kql` for facilitator auditing
- All scoring KQL queries and dashboard tiles now include `EscapeRoomCustomEvent`

---

## [1.3.x] - Previous releases

Initial framework with seven built-in telemetry events, interface-based venue/room/task system, leaderboard KQL queries, and Azure Data Explorer dashboard.
