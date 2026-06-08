# 🛠️ Stage 5 — Make It Your Own

> Adapt the agent for your own product and explore what it can do.

**Stage 5 of 10** · ← [Stage 4: Inspecting Task Logs](./04-testing.md) · [Overview](../README.md) · Next → [Stage 6: Export & Import](./06-export-import.md)

---

## Overview

Freeform time. Take what you've built and make it relevant to your product — there's no right answer, just experimentation.

---

## Part 1 — Rewrite the Agent Instructions

### Task 1 — Open your agent in the Designer

Go to the **Agents (preview)** page and open your agent from Stage 2 in the Agent Designer.

### Task 2 — Rewrite the instructions for your product

Replace the placeholder instructions with something meaningful to **your** scenario. Think about:

- What business process does your product support?
- What should the agent be *good* at? What should it avoid?
- What tone or persona fits your product?

The more context you give it, the better it performs.

### Task 3 — Run a task that makes sense for your scenario

Give it a task that reflects a real workflow from your product. Watch how it reasons through it.

---

## Part 2 — Install Your App and Extend the Agent (Optional)

> [!TIP]
> **🔔 This environment has AppSource access** — if you have an app published to AppSource (or a per-tenant extension available), you can install it here and let the agent interact with it.

### Task 4 — Install your app from AppSource

Go to **Extension Management** and install your app, or search for it in AppSource via **Microsoft AppSource** in the Business Central marketplace.

### Task 5 — Update the agent instructions to reference your app

Add context to the instructions so the agent knows your app exists and what it does. For example:

> *"This company uses [Your App Name] for [purpose]. When a user asks about [topic], navigate to [page] in [Your App Name] to find the answer."*

### Task 6 — Give the agent a task that touches your app

Give it something that touches a page from your app. Check the task log (Stage 4) to see how it got there.

---

> [!TIP]
> **💡 Ideas to try**
> - Ask the agent to summarise data from a custom list page in your app
> - Ask it to create a record using your app's data entry page
> - Ask it something it *can't* do — then use the profile customisation from Stage 3 to restrict it further
> - Compare log entries between a task that succeeded and one that failed

---

## ✅ Stage complete when…

- [ ] You've rewritten the agent's instructions for your own scenario
- [ ] You've run at least one task using your own product-specific instructions
- [ ] You have a feel for the agent's strengths and limitations in your domain

---

← [Stage 4: Inspecting Task Logs 🔍](./04-testing.md) · [Overview](../README.md) · Next → [Stage 6: Export & Import 📤](./06-export-import.md)
