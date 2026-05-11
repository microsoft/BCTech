# Sales Validation Agent — Tests

Automation tests that validate the **Sales Validation Agent** sample app. The tests run on the Business Central **AI Test Toolkit (AIT)** framework and demonstrate how a partner can author end-to-end accuracy tests for a third-party agent — exercising real agent runs against curated sales-order scenarios and asserting the resulting release decisions.

## Overview

These tests drive the agent through shipment-date validation scenarios using data-driven datasets. Each test creates sales orders with specific configurations (shipping advice, reservation state, shipment dates), submits a task to the agent, waits for it to complete, and validates that the correct orders were released.

The dataset/library separation follows the contract documented in the official documentation:

- **YAML datasets** (`.resources/`) declare *what* to test — orders to create, queries to send, expectations to validate.
- **AL test code** (`Tests/`, `Libraries/`) implements *how* — translating YAML actions into Business Central data and running the agent turn loop.

---

## Project Structure

```
test/
├── Libraries/                 # Test helpers — data setup and validation
├── Setup/                     # Install codeunit that loads datasets & suites
├── Tests/                     # [Test] codeunits that drive the agent
└── .resources/
    ├── eval_suite_setup/      # Shared per-suite master data (locations, customers)
    ├── datasets/              # Per-test scenarios (one YAML file per category)
    │   └── variations/        # Parallel variants (different customers / dates / qtys)
    └── configuration/         # XML suite definitions wiring datasets to test codeunits
```

### Tests

`SVA Accuracy Test` (codeunit `53740`) is the single `[Test]` entry point used by every dataset. The codeunit follows the recommended `RunTurnAndWait` + `FinalizeTurn` loop:

1. `Initialize()` — resolves the agent user, runs per-suite setup once, ensures the agent is active, and clears prior sales orders so each test starts clean.
2. For each turn:
   - `SetupTurnData()` reads `turn_setup.setup_actions` and creates the sales orders described in YAML.
   - `LibraryAgent.RunTurnAndWait` submits the YAML's `query` (task input or intervention) and blocks until the agent finishes the turn.
   - `SVATestLibrary.ValidateSalesOrdersReleased` checks that every order classified as expected-released is `Released` and every order classified as expected-non-released is `Open`.
   - `LibraryAgent.FinalizeTurn` writes the turn output, validates intervention expectations declared in `expected_data`, and advances to the next turn.

The `SVA Event Handler` codeunit suppresses the platform's "shipment date before work date" check so tests can exercise back-dated scenarios that mimic real customer data.

### Libraries

`SVA Test Library` is the dispatcher that consumes the opaque `turn_setup` / `eval_suite_setup` JSON. It recognises three `action_type` values:

| `action_type`         | YAML location          | Effect                                                                                                                                                  |
| --------------------- | ---------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `create_location`     | suite setup            | Creates a warehouse location with inventory posting setup, named per the YAML.                                                                          |
| `create_customers`    | suite setup            | Creates customer master records keyed by `No.`.                                                                                                         |
| `create_sales_order`  | per-turn setup         | Creates a sales header with the specified shipping advice and shipment date, plus lines that may stock inventory and auto-reserve.                      |

Per-line keys `quantity_in_inventory` and `reserve` are application-specific extensions used by the dispatcher — not part of any AL field. They control whether item stock is posted and whether the line is reserved before the agent runs.

The library also classifies each created order as expected-released or expected-non-released based on the same rules the agent should apply (Shipping Advice + reserved quantity vs. line quantity), then validates the actual outcome against `expected_data.agent_released_orders` (the count) after the turn completes.

### Setup

`SVA Tests Install` is an `Install`-subtype codeunit. On install it enumerates `*.yaml` and `*.xml` resources and registers them with the AIT Test Toolkit:

- YAML files become **Test Inputs** — datasets and suite-setup groups discoverable by name.
- XML files become **AIT Suites** — bindings of dataset → test codeunit → frequency.

Recursion into subfolders is automatic; folder names are convention only.

### Resources

#### `eval_suite_setup/SVA-SETUP.yaml`

Shared master data installed once per suite run: one warehouse location and five customer records. Datasets reference this group via `suite_setup: SVA-SETUP`.

#### `datasets/`

Each file is a category of accuracy scenarios. Splitting tests into meaningful groups makes results easier to triage and lets you run subsets independently — for example, the **P0** group is the must-pass core suite that any change to the agent (instructions, tooling, model) is expected to keep green, and is therefore kept as a single dedicated dataset rather than scattered across other files.

| Dataset                            | What it covers                                                                                                |
| ---------------------------------- | ------------------------------------------------------------------------------------------------------------- |
| `SVA-P0.yaml`                      | Single-order happy path — every Shipping Advice × reservation state combination. Must-pass baseline.         |
| `SVA-MULTIORDER.yaml`              | Multiple orders in a single task with mixed release outcomes.                                                 |
| `SVA-MULTILINE.yaml`               | Single order with multiple lines — tests release decisions when only some lines are reserved.                 |
| `SVA-USERINTERVENTION.yaml`        | Multi-turn — agent must pause for user intervention when the user omits the shipment date.                    |
| `SVA-NATURALLANGUAGE-DATES.yaml`   | The user expresses the shipment date in natural language ("today", "yesterday", …) instead of an ISO date.   |

The `variations/` subfolder holds parallel datasets that re-run the same logical scenarios with slightly different inputs — different customers, shipment-date formulas, or quantities. They are valuable because LLM-driven agents can be sensitive to small phrasing or data shifts that a single fixed dataset would not surface; running variations alongside the primary datasets gives a more honest picture of accuracy and helps catch regressions that only appear under specific conditions. Adding a new variation is cheap: copy an existing dataset, tweak the inputs, and add a `<Line>` for it in the suite XML.

Shipment dates are written as `$DateFormula-<...>$` placeholders so tests stay relative to `WorkDate()` and never drift out of date. See §9 of the official documentation for the full placeholder grammar.

#### `configuration/SVA-ACCUR.xml`

The AIT suite definition. Encoded as **UTF-16** (a framework requirement) and wires every dataset to codeunit `53740` (`SVA Accuracy Test`) under suite code `SVA-ACCR`. `TestRunnerId="130451"` selects the isolation-disabled test runner — agent tasks span transactions, so this is required.

---

## How It Works

1. **Install** — installing the test app fires `SVA Tests Install`, which loads every `*.yaml` dataset and the `SVA-ACCR` suite into the AIT Test Toolkit.
2. **Run the suite** — open the **AIT Test Suites** page, select `SVA-ACCR`, and start a run. The toolkit iterates each `<Line>` in the XML suite, executing codeunit `53740` once per dataset row.
3. **Per-test execution** — for each test in a dataset, `SVA Accuracy Test`:
   - Restores baseline state (deletes prior sales orders).
   - Creates the orders described by `turn_setup`.
   - Submits the YAML `query` to the agent and waits.
   - Validates the resulting sales-order statuses against the rules encoded by the test library and the count declared in `expected_data.agent_released_orders`.
4. **Multi-turn flow** — datasets like `SVA-USERINTERVENTION.yaml` declare additional turns under `turns:`. After the first turn, follow-up turns send `intervention.instruction` payloads that resume a paused agent task.
5. **Results** — pass/fail status and the captured agent output are written back to the AIT Test Toolkit for evaluation.

---

## Authoring New Tests

To add a new scenario:

1. Add a test case to an existing dataset under `.resources/datasets/`, or create a new dataset file referencing `suite_setup: SVA-SETUP`.
2. If the new dataset is a new file, add a `<Line>` for it in `.resources/configuration/SVA-ACCUR.xml`.
3. If the scenario needs a new kind of data setup, extend `SVA Test Library` with a new `action_type` handler — the YAML stays declarative and the dispatcher pattern keeps test code reusable.

Refer to the official documentation for the full YAML schema, placeholder grammar, and `Library - Agent` API surface.