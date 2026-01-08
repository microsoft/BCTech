# Changelog - BCTalent.EscapeRoom Framework

All notable changes to the EscapeRoomApp framework are documented here.

Changes are grouped by ISO week number (YYYY.WW format) and include all commits affecting the framework across the development lifecycle.

---

## 2026.01 (January 2026)

- [a28a5eb](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/a28a5eb) – Add new dashboard configuration with multiple visualizations and queries for enhanced leaderboard tracking (waldo)

## 2025.44 (October - November 2025)

- [697df90](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/697df90) – Update app versions and modify script parameters (Arend-Jan Kauffmann)
- [ae9c3cd](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/ae9c3cd) – Update build numbers in app.json files (Arend-Jan Kauffmann)

## 2025.43 (October 2025)

- [d14900a](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/d14900a) – Add Suggest Sales Lines room and enhance venue refresh functionality (Arend-Jan Kauffmann)
- [a26d81d](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/a26d81d) – Refactor escape room task implementations to resolve circular dependencies and enhance task retrieval logic (waldo) [See: Framework/Task-Validation.md](Framework/Task-Validation.md)

## 2025.40 (October 2025)

- [9ce773a](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/9ce773a) – Enhance leaderboard queries to include fastest venue completion times and improve data summarization (waldo1001) [See: Facilitator/Leaderboard-Setup.md](Facilitator/Leaderboard-Setup.md)
- [22b1f21](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/22b1f21) – Refactor leaderboard queries to improve clarity and accuracy in scoring calculations (waldo1001)
- [2c8343f](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/2c8343f) – Update telemetry dashboard configuration (waldo1001)
- [e3feb4a](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/e3feb4a) – Update environment range in sandbox scripts to limit operations (Arend-Jan Kauffmann)
- [367149a](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/367149a) – Update example appfile paths in publish-apps script (waldo1001)
- [ab0cf73](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/ab0cf73) – Add telemetry dashboard for BCTalent Escape Rooms (waldo1001)
- [3c4b7b3](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/3c4b7b3) – Add scripts for sandbox environment management, user/group creation, App Insights configuration, and security groups (Arend-Jan Kauffmann) [See: Facilitator/Event-Setup.md](Facilitator/Event-Setup.md)
- [39509543](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/3950954) – Refactor AddScorePoints calls to remove event identifiers for score adjustments (waldo) [See: Framework/Telemetry-Integration.md](Framework/Telemetry-Integration.md)
- [daacc9d](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/daacc9d) – Enhance Escape Room telemetry with scoring functionality and add leaderboard queries documentation (waldo)
- [164db15](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/164db15) – Remove download section from ReadMe (waldo1001)

## 2025.39 (September 2025)

- [96c218f](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/96c218f) – Remove unused TestCiellos files and clean up workspace configuration (waldo)
- [44d86e0](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/44d86e0) – Add .gitignore for BCCopilotGuidance files (waldo1001)

## 2025.38 (September 2025)

- [7a4e00e](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/7a4e00e) – Refactor IsValid procedure to properly validate Sales Header records (Arend-Jan Kauffmann)
- [4dd14f4](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/4dd14f4) – Remove submodule BCCopilotGuidance (waldo1001)
- [4ebb645](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/4ebb645) – Add BCCopilotGuidance submodule configuration (waldo1001)

## 2025.35 (August 2025)

- [20e5ae0](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/20e5ae0) – Rename testapp for Development.1 (waldo)

## 2025.24 (June 2025)

- [0e9e22d](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/0e9e22d) – Update room description and enhance sales order validation; bump version to 1.0.0.2 (Arend-Jan Kauffmann)
- [0978546](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/0978546) – Add decent badge images for room completion (waldo1001)

## 2025.23 (June 2025)

- [42f5f8c](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/42f5f8c) – Implement "Test and See If Fail" escape room with tasks for validating Information field copying (waldo)
- [3a6523e](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/3a6523e) – Add hints for extending tables and codeunits in assignment description (waldo)
- [618bbf8](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/618bbf8) – Update dashboard schema and eTag for Escape Rooms telemetry (waldo)
- [f25577b](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/f25577b) – Update Application Insights connection string for improved monitoring (waldo)
- [c285f19](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c285f19) – Refactor sales quote and order processing tasks (Arend-Jan Kauffmann)

## 2025.22 (May - June 2025)

- [d323d7b](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/d323d7b) – Refactor Escape Room task handling by removing unused customer cache codeunit and cleaning up TODO list (waldo)
- [8f73f51](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/8f73f51) – Add "Solve" method, move tasks to bonus room, introduce Rich Text Box solution description (waldo)
- [2ea346a](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/2ea346a) – Enhance Escape Room functionality with new procedures for solving rooms and add solution delay field (waldo1001)

## 2025.21 (May 2025)

- [e87935c](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/e87935c) – Update task description for Sales Order and enhance TODO list (waldo1001)
- [a13c0f9](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/a13c0f9) – Refactor telemetry logging to correctly capture TaskName (waldo1001) [See: Framework/Telemetry-Integration.md](Framework/Telemetry-Integration.md)
- [abe2c0c](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/abe2c0c) – Update app version to 1.0.0.2 (waldo1001)
- [66e1978](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/66e1978) – Add BusPremiumExt permissionset extension to include EscapeRoomAdmin (waldo1001) [See: Dev/Permissions.md](Dev/Permissions.md)
- [13dbd57](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/13dbd57) – Add EscapeRoomAdmin permissionset and FullAdminExt extension; update app version to 1.0.0.1 (waldo1001) [See: Dev/Permissions.md](Dev/Permissions.md)
- [c405f33](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c405f33) – Comment out unused room entries in Escape Room Venue (waldo1001)
- [cf919f2](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/cf919f2) – Comment out unused enum values in EscapeRooms extension (Arend-Jan Kauffmann)
- [88c91bd](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/88c91bd) – Add VAT setup, purchase, sales, and invoice rooms (Arend-Jan Kauffmann)
- [14569e0](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/14569e0) – Enhance Sales Line functionality: Add event subscriber to copy Item information to Sales Line (waldo1001)
- [e533da3](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/e533da3) – Add AdminPermissions permissionset: Define permissions for Escape Room entities and tasks (waldo1001) [See: Dev/Permissions.md](Dev/Permissions.md)
- [c64ea08](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c64ea08) – Add ReadMe file: Documentation for Escape Rooms project (waldo1001)
- [90afd7f](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/90afd7f) – Add ZipAppsAndCopyToDropbox script (waldo1001)

## 2025.20 (May 2025)

- [d000e23](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/d000e23) – Add CloseVenueIfCompleted procedure to handle venue completion logic (waldo1001) [See: Framework/Architecture.md](Framework/Architecture.md)
- [ace7b34](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/ace7b34) – Implement CloseRoomIfCompleted procedure and integrate with SetStatusCompleted (waldo1001) [See: Framework/Architecture.md](Framework/Architecture.md)
- [421ea5c](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/421ea5c) – Refactor OpenNextRoom procedure to set current key and ascending order (waldo1001)
- [f76dd1a](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/f76dd1a) – Add current key setting for NextRoom in OpenNextRoom procedure (waldo1001)
- [b97ff96](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/b97ff96) – Add implementation for "Copy to Item Ledger Entry" task (waldo1001)
- [e857784](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/e857784) – Add "Copy to Posted Docs" task implementation and update enum values (waldo1001)
- [0e6c738](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/0e6c738) – Add implementation for "Copy to Posted Tables" task (waldo1001)
- [d7e283f](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/d7e283f) – Add telemetry logging for Escape Room events and create dashboard configuration (waldo) [See: Framework/Telemetry-Integration.md](Framework/Telemetry-Integration.md)
- [0e8e641](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/0e8e641) – Update Stop DateTime to use CurrentDateTime and enhance telemetry logging for room status (waldo)
- [fd63601](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/fd63601) – Fix typo in telemetry logging for task stop date time (waldo)
- [2cc0372](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/2cc0372) – Add Escape Room Venues action to Business Manager Role Center (waldo)
- [50bc30b](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/50bc30b) – Update Introduction Room tasks and telemetry logging; rename tasks (waldo)
- [2df9903](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/2df9903) – Refactor Introduction Room description and enhance telemetry logging for room start events (waldo)
- [55b3923](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/55b3923) – Add introduction room to list of available rooms in venue (waldo)
- [3cb9a4c](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/3cb9a4c) – Add application insights connection string to app configuration (waldo)
- [bc64293](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/bc64293) – Add telemetry logging for room and task events, enhancing notification tracking (waldo) [See: Framework/Telemetry-Integration.md](Framework/Telemetry-Integration.md)
- [6879f03](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/6879f03) – Add introduction room to framework (waldo)
- [bbd57f4](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/bbd57f4) – Add introduction room with tasks and refactor venue code (Arend-Jan Kauffmann)
- [a6bb26e](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/a6bb26e) – Add room completion and notifications to TODO list (waldo)
- [11288ee](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/11288ee) – Refactor status check to use enum for room status in task implementations (waldo)
- [c199ef9](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c199ef9) – Fix logic in Escape Room Codeunit for proper insertion and refreshing (Arend-Jan Kauffmann) [See: Framework/Architecture.md](Framework/Architecture.md)
- [cee2657](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/cee2657) – Update task names to include "Dev1" suffix for clarity (Arend-Jan Kauffmann)
- [f5244b9](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/f5244b9) – Update room descriptions to load from external resources (Arend-Jan Kauffmann)
- [f1cab24](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/f1cab24) – Update application versions and add resource folders (Arend-Jan Kauffmann)
- [8593f84](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/8593f84) – Update gitignore (Arend-Jan Kauffmann)
- [286e301](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/286e301) – Enhance Escape Room functionality: notifications, new tasks, table/page extensions (waldo1001) [See: Framework/Architecture.md](Framework/Architecture.md)
- [62d3386](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/62d3386) – Update TODO list: add tasks for room notifications, documentation, and telemetry dashboard (waldo1001)

## 2025.19 (May 2025)

- [1f01f7b](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/1f01f7b) – Add hints to room descriptions (waldo1001)
- [c7a53bb](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c7a53bb) – Update room description for Copy Fields While Posting (waldo1001)
- [4b0c029](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/4b0c029) – Refactor task implementations: rename and update descriptions, add new tasks (waldo1001)
- [375ffa7](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/375ffa7) – Update instructions for adding custom fields and clarify posting process (waldo1001)
- [347cdd4](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/347cdd4) – Restructure resource files (waldo1001)
- [cd187ce](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/cd187ce) – Create HTML documentation files and remove outdated markdown (waldo1001)
- [87105c9](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/87105c9) – Refactor Escape Room Card page: remove ApplicationArea, add actions for status updates (waldo1001)
- [b5f0b25](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/b5f0b25) – Refactor Escape Room Card and Task List pages: rename Tasks group, update filtering (waldo1001)
- [3e59351](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/3e59351) – Remove initial status assignment and make DateTime fields non-editable (waldo1001)
- [5102ecc](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/5102ecc) – Add Start/Stop DateTime fields to Escape Room pages and update task list filtering (waldo1001)
- [d4ef96f](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/d4ef96f) – Add Start/Stop timing fields (waldo1001)
- [7e6b2c2](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/7e6b2c2) – Create data OnInstall (waldo1001)
- [51f60c2](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/51f60c2) – Add new Escape Room and Vendor Credit Limit functionalities (waldo1001)
- [71546129](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/7154612) – Rename GetVenue to GetVenueRec for clarity; add GetVenue method (waldo1001) [See: Framework/API-Reference.md](Dev/API-Reference.md)
- [15e3e7e](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/15e3e7e) – Add new tasks for Copy Fields While Posting; implement logic for default values (waldo1001)
- [442af60](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/442af60) – Enhance Escape Room functionality: add tasks, update descriptions, refactor sequence handling (waldo)
- [710f932](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/710f932) – Add GetRoomDescription method and update Escape Room structure (waldo)
- [c665c10](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c665c10) – Add test codeunit and validation for Escape Room tasks (waldo) [See: Framework/Task-Validation.md](Framework/Task-Validation.md)
- [0ebe8e0](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/0ebe8e0) – First successful task validation implementation (waldo) [See: Framework/Task-Validation.md](Framework/Task-Validation.md)
- [c4f6e61](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/c4f6e61) – Update runtime to 15.0 and refactor Escape Room functionality with new interfaces (waldo) [See: Framework/Architecture.md](Framework/Architecture.md)
- [dad601d](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/dad601d) – Add interfaces for extensibility (waldo) [See: Framework/API-Reference.md](Dev/API-Reference.md)
- [1ad079d](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/1ad079d) – Add TestPTE for validation testing (waldo)
- [aa6090c](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/aa6090c) – Refactor Escape Room Venue fields: use 'Id' instead of 'Code', add Full Name and Partner Name (waldo) [See: Framework/Architecture.md](Framework/Architecture.md)
- [fdbd90d](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/fdbd90d) – Add Escape Room Venue functionality and update dependencies (waldo1001) [See: Framework/Architecture.md](Framework/Architecture.md)
- [3f681fb](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/3f681fb) – Add interface for task validation with necessary procedures (waldo1001) [See: Framework/Task-Validation.md](Framework/Task-Validation.md)
- [da1b671](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/da1b671) – Update app names and publishers for Consultant and Development versions (waldo1001)
- [fd645b5](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/fd645b5) – Update project structure (waldo1001)
- [34dd222](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/34dd222) – Project cleanup (waldo1001)
- [ca022b3](https://github.com/waldo1001/BCTalent.EscapeRoom/commit/ca022b3) – Initial framework commit (waldo) [See: Framework/Architecture.md](Framework/Architecture.md)

---

**Note:** This changelog covers the EscapeRoomApp framework only. For venue-specific changes, see:
- Development.1/Docs/CHANGELOG.md
- Development.2/Docs/CHANGELOG.md  
- Consultant.1/Docs/CHANGELOG.md
