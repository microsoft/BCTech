# 🎨 Stage 3 — Customizing the Agent's Profile

> Use profile customization to control what your agent can see and access.

**Stage 3 of 10** · ← [Stage 2: Your First Agent](./02-first-agent.md) · [Overview](../README.md) · Next → [Stage 4: Inspecting Task Logs](./04-testing.md)

---

## Overview

An agent can only interact with what its profile exposes. Here we'll hide an action from its role centre and see how that changes what it can do.

---

## Part 1 — Customize the Profile

### Task 1 — Open the agent's design page

Go to the **Agents (preview)** page and open your agent from Stage 2.

### Task 2 — Open profile customization

In the agent page, select **Design** → **Customize profile**. This opens the Business Central personalization experience in a new window, scoped to your agent's profile.

> [!NOTE]
> **What you're seeing** — the **Customizing** banner at the top of the new window confirms you're editing the agent's profile, not your own personal view. Changes apply to all users on this profile.

### Task 3 — Hide the Customer List action

On the Role Centre, locate the **Customers** action (the link that navigates to the Customer List).

1. Point to the action — it will highlight with an arrowhead
2. Select the arrowhead, then choose **Hide**
3. The action will appear greyed out with italic text, confirming it is hidden

### Task 4 — Save the customization

Select **Done** in the **Customizing** banner to save and close the personalization window.

> [!TIP]
> **You're here when…** the customization window closes and you're back on the agent page.

### Task 5 — Deactivate and reactivate the agent

Profile changes don't take effect until the agent is restarted. On the agent page, turn the **Active** toggle **off**, then turn it back **on** again.

> [!NOTE]
> **Why this is needed** — the agent caches its profile on activation. Toggling it off and on forces it to pick up the updated customization.

---

## Part 2 — Test the Restriction

### Task 6 — Re-run the original task

Run a new task with the same prompt as Stage 2:

```text
List the top 5 customers by name.
```

### Task 7 — Observe the result

Watch the task pane — this time the agent shouldn't be able to get to the Customer List. It's a good illustration of how the profile acts as a guardrail.

---

## ✅ Stage complete when…

- [ ] The Customer List action is hidden in the agent's profile
- [ ] The agent fails (or returns a "cannot access" response) on the same task it completed in Stage 2

---

← [Stage 2: Your First Agent 🤖](./02-first-agent.md) · [Overview](../README.md) · Next → [Stage 4: Inspecting Task Logs 🔍](./04-testing.md)
