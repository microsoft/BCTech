# 🔌 Stage 10 — Bring Your Own App

> Integrate everything you've built with your own AL extension.

**Stage 10 of 10** · ← [Stage 9: Programmatic Tasks](./09-programmatic-tasks.md) · [Overview](../README.md)

---

## Overview

Your sandbox. Agent in AL, setup page for config, programmatic task creation — now point it at your own app.

No set path here — find a meaningful integration point and go.

---

## Getting Started

### Task 1 — Add your app as a dependency

In `app.json`, add your app as a dependency:

```json
"dependencies": [
  {
    "id": "<your-app-id>",
    "name": "<Your App Name>",
    "publisher": "<Your Publisher>",
    "version": "1.0.0.0"
  }
]
```

Then run **AL: Download Symbols** to pull in your app's symbols so you can reference its objects directly.

### Task 2 — Pick an integration point

Think about where the agent adds most value in your workflow. Some ideas:

- **Action on a key page** — add a "Send to Agent" button on your app's main list or card page, passing relevant record context in the task message
- **Business event trigger** — subscribe to an event your app already raises (e.g. a custom document being posted or a status change) and auto-create a task
- **Setup page field** — expose a config value from your app in the agent setup page so the agent knows about your app's data structure
- **Profile customization** — use the agent's profile to hide or show pages from your app, scoping what the agent can see

### Task 3 — Write the task message with useful context

The richer the task message, the better the agent performs. Include record IDs, relevant field values, and a clear instruction:

```al
AgentTaskMessageBuilder
    .Initialize(
        'System',
        'A new ' + Rec.TableCaption() + ' has been created: ' + Rec."No." +
        '. Description: ' + Rec.Description +
        '. Please review and take appropriate action.')
    .SetRequiresReview(false);
```

### Task 4 — Update the agent instructions to know about your app

Add context about your app's pages and data to the agent instructions — via the setup page field from Stage 8, or directly in the resource file. The more it understands your app's structure, the more accurately it navigates.

### Task 5 — Publish, trigger, and inspect

Publish both extensions, trigger your integration point, and watch it work. Use the task log from Stage 4 to see what pages it hit and what decisions it made. Iterate on the instructions and message until it handles the workflow reliably.

---

> [!TIP]
> **💡 Things worth experimenting with**
> - Give the agent an ambiguous task and see how it interprets it — then tighten the instructions
> - Intentionally restrict the agent's profile (Stage 3 approach) so it can't access a page — observe the failure in the task log
> - Add a second custom setup field to hold something app-specific (an account code, a series number) and inject it into the task message dynamically
> - Store the task ID on your record and surface its status back in your app's UI

---

## ✅ Stage complete when…

- [ ] You've triggered at least one agent task that touches a page or record from your own app
- [ ] The task log shows the agent successfully navigating your extension's UI

---

## 🎉 Workshop Complete!

You've built and deployed an agent from scratch — through the UI, then as a packaged AL extension with custom configuration and programmatic triggers, wired into your own app.

Want to keep going? The [appendix](../appendix.md) has every Microsoft Learn article referenced across the workshop, plus a link to the [Sales Validation Agent sample](https://github.com/microsoft/BCTech/tree/master/samples/BCAgents/SalesValidationAgent) — a fully-fleshed-out reference implementation that uses the same patterns at production scale.

---

← [Stage 9: Programmatic Tasks ⚡](./09-programmatic-tasks.md) · [Overview](../README.md) · [Appendix 📖](../appendix.md)
