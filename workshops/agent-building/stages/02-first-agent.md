# 🤖 Stage 2 — Your First Agent

> Create a UI-driven custom agent and trigger its first task — no code required.

**Stage 2 of 10** · ← [Stage 1: Introduction](./01-introduction.md) · [Overview](../README.md) · Next → [Stage 3: Profile Customization](./03-profile-customization.md)

---

## Overview

Everything in this stage is UI only — no AL needed. By the end you'll have a working agent that's run its first task.

> [!NOTE]
> **Before you start** — make sure the **Custom Agent** capability is enabled in **Copilot & AI Capabilities** and that your user has the **AGENT - ADMIN** permission set assigned. If the agent icon is missing from the navigation bar after completing Stage 1, check these two settings first.

---

## Part 1 — Create the Agent

### Task 1 — Open the agent designer

In the top-right navigation bar of your Role Centre, look for the **+** (plus) icon. Select it, then choose **Agent** → **Create**.

<img src="../assets/images/create_icon.png" alt="The plus icon used to create a new agent" width="320" />

### Task 2 — Choose a starting point

In the **Create agent** wizard, select **Create agent from scratch**, then choose **Next**.

### Task 3 — Give your agent an identity

Fill in the following fields:

- **Name** — a short internal name, e.g. `WorkshopAgent01`
- **Display Name** — what users will see, e.g. `My First Agent`
- **Initials** — suggested automatically, but feel free to change it
- **Description** — optional; briefly describe what the agent does

Select the arrow to continue to the next page.

### Task 4 — Set the agent's profile

Under **Profile (role)**, select **Setup profile** and choose **Business Manager**. This determines which UI pages and actions the agent can see and interact with.

### Task 5 — Set permissions

Under **Permissions**, select **Manage permissions** and add the `SUPER` permission set. This gives the agent access to everything in the environment for the purposes of this workshop.

> [!WARNING]
> **Workshop only** — `SUPER` is fine for a sandbox. In production, assign only what the agent actually needs.

### Task 6 — Write the agent's instructions

Under **Instructions for the agent**, select **Edit instructions** and enter the following:

```text
You are a helpful Business Central assistant.
When given a task, look up the requested information
and provide a clear, concise summary to the user.
```

Keep them short and specific — you'll refine them in later stages.

### Task 7 — Activate and save

Turn on the **Active** toggle, then select **Update** to complete the setup.

The agent icon in the navigation bar should change to reflect this.

> [!TIP]
> **You're here when…** the agent icon in the top navigation changes and your agent appears in the **Agents (preview)** page with a status of **Active**.

---

## Part 2 — Trigger Your First Task

### Task 8 — Run a task

Select the **Run task** action. In the dialog, enter a simple task for the agent — for example:

```text
List the top 5 customers by name.
```

Confirm and hit **Start** — watch the agent pick it up.

### Task 9 — Watch it run

The task pane on the right updates in real time. Watch it work through the steps and check you see a result.

---

<details>
<summary>🛟 <strong>Stuck?</strong></summary>

- If you see **"Something went wrong"** in the task pane, do a force refresh (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>R</kbd> on Windows, <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>R</kbd> on Mac) and try running the task again

</details>

---

## ✅ Stage complete when…

- [ ] Your agent appears on the **Agents (preview)** page with status **Active**
- [ ] You've run at least one task and can see its output in the task log

---

← [Stage 1: Introduction 🚀](./01-introduction.md) · [Overview](../README.md) · Next → [Stage 3: Profile Customization 🎨](./03-profile-customization.md)
