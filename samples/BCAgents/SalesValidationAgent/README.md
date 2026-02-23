# Sales Validation Agent

A sample **third-party agent** extension for Microsoft Dynamics 365 Business Central that validates and processes sales orders by checking inventory reservation and releasing eligible orders to the warehouse. It's purpose is to demonstrate how the sample agent in Business Central can be productized into an app.

## Overview

The Sales Validation Agent is a Copilot-powered agent that automates the routine work of reviewing open sales orders for a given shipment date, verifying that inventory is reserved, and releasing qualifying orders. It is built on the Business Central **Agent** framework and is intended as a reference implementation for partners building their own agents.

---

## Project Structure

```
app/
├── Integration/          # Copilot capability registration & install logic
├── Interaction/          # User-facing pages and page extensions
└── Setup/                # Agent configuration, metadata, KPIs, and profile
    ├── KPI/              # Performance tracking (orders released)
    ├── Metadata/         # IAgentFactory / IAgentMetadata implementations
    └── Profile/          # Role Center profile & page customizations
```

### Integration

Contains the install codeunit and enum extension that register the **Sales Validation Agent** as a Copilot capability. On first install the capability is registered with the platform; on subsequent installs or upgrades the agent's instructions are refreshed.

### Interaction

Provides the user-facing entry point for the agent. A page extension on the **Sales Order List** adds a *Validate with Agent* action that verifies the agent is active, prompts the user for a shipment date via a dialog page, and creates an `Agent Task` with the selected date.

### Setup

Holds the core configuration for the agent:

- **Setup table & page** – A lightweight table stores the agent's User Security ID (company-independent), and a `ConfigurationDialog` page lets admins provision or update the agent user with the correct profile and permissions. The Copilot capability must be enabled before the setup page can be opened.
- **Setup codeunit** – Central helper that resolves the agent user, supplies default profile and access controls (`D365 READ` + `D365 SALES`), loads instructions from a resource file, and ensures the setup record exists.

#### KPI

Tracks agent performance metrics. A KPI table records counters per agent user (currently *Orders Released*), exposed through a `CardPart` summary page. An event subscriber on `OnAfterReleaseSalesDoc` automatically increments the counter each time the agent releases a sales order, so metrics stay up to date without any manual bookkeeping.

#### Metadata

Implements the `IAgentFactory` and `IAgentMetadata` interfaces required by the Business Central Agent framework. The factory provides default initials, the setup page, the Copilot capability, the default profile, and access control templates. The `ShowCanCreateAgent` method controls UI visibility for agent creation (single-instance by convention); if the app needs to strictly enforce one agent, the Setup table/page must also prevent duplicates. The metadata codeunit supplies page IDs for setup, summary, and task message cards. An enum extension on `Agent Metadata Provider` wires these implementations into the platform.

#### Profile

Defines a dedicated **Sales Validation Agent (Copilot)** profile based on the *Order Processor Role Center*. Accompanying page customizations tailor the Role Center, Sales Order card, Sales Order List, Sales Order Statistics, Sales Order Subform, and SO Processor Activities pages to present only the information the agent needs.

---

## How It Works

1. **Create the agent** – The *Sales Validation Agent* Copilot capability is registered as **Preview** and is therefore enabled by default. Open the *Sales Val. Agent Setup* configuration dialog (accessible from the agent avatar in the Role Center) to provision the agent user with the correct profile and permissions.
2. **Assign a task** – From the **Sales Order List**, choose *Validate with Agent*, pick a shipment date, and a task is created for the agent.
3. **Agent processes orders** – The agent reads its instructions (loaded from `Instructions/InstructionsV1.txt`), validates open sales orders for the specified shipment date, checks inventory reservation, and releases eligible orders.
4. **KPIs are tracked** – Each time the agent releases an order, the `OnAfterReleaseSalesDoc` event subscriber increments the *Orders Released* counter, visible on the agent's summary page.

---

## Prerequisites

- Business Central **application version 27.4** or later
- The **Copilot & AI Capabilities** feature enabled in the environment
- Appropriate licensing for Copilot / Agent functionality
