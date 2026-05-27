# ⚡ Stage 9 — Programmatic Tasks

> Trigger agent tasks from page actions and business events using AL code.

**Stage 9 of 10** · ← [Stage 8: Customizing the Setup Page](./08-setup-page.md) · [Overview](../README.md) · Next → [Stage 10: Bring Your Own App](./10-bring-your-own-app.md)

---

## Overview

So far tasks have been triggered manually. In practice you'll want the agent to kick off automatically — a button click, a document posting, a status change. Here we'll wire that up using the `Agent Task Builder` API.

> [!TIP]
> 📖 **Docs reference** — [Managing agent tasks programmatically (preview)](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/ai/ai-agent-sdk-tasks)

---

## Part 1 — Trigger a Task from a Page Action

The simplest integration: a button on a page that sends a task to your agent.

### Task 1 — Add a page extension with an agent action

Create a page extension with a **Send to Agent** action. `AgentUserSecurityId` is the `User Security ID` from your setup table — retrieve it before calling `Initialize`.

```al
pageextension 50200 "Sales Order Agent Action" extends "Sales Order"
{
    actions
    {
        addlast(Processing)
        {
            action(SendToAgent)
            {
                Caption = 'Send to Agent';
                Image = Task;
                ApplicationArea = All;
                ToolTip = 'Ask the agent to review this sales order.';

                trigger OnAction()
                var
                    AgentTaskBuilder: Codeunit "Agent Task Builder";
                    AgentTask: Record "Agent Task";
                    MyAgentSetup: Record "My Agent Setup";
                begin
                    MyAgentSetup.FindFirst();
                    AgentTask := AgentTaskBuilder
                        .Initialize(MyAgentSetup."User Security ID", 'Review Sales Order')
                        .AddTaskMessage(
                            'Sales Team',
                            'Please review sales order ' + Rec."No." +
                            ' for customer ' + Rec."Sell-to Customer Name")
                        .Create();

                    Message('Task created: %1', AgentTask.ID);
                end;
            }
        }
    }
}
```

### Task 2 — Publish and test the action

Publish, open a sales order, and try the **Send to Agent** action. Go to the **Agents (preview)** page and check a task appeared. Approve and run it, then look at the log.

---

## Part 2 — Skip the Approval Step

By default a new task waits for approval before the agent starts. For internal, trusted triggers you can skip that.

### Task 3 — Set RequiresReview to false

Update the action to use `Agent Task Message Builder` directly and call `SetRequiresReview(false)` — the agent will start as soon as the task is created.

```al
var
    AgentTaskBuilder: Codeunit "Agent Task Builder";
    AgentTaskMessageBuilder: Codeunit "Agent Task Message Builder";
    AgentTask: Record "Agent Task";
    MyAgentSetup: Record "My Agent Setup";
begin
    MyAgentSetup.FindFirst();

    AgentTaskMessageBuilder
        .Initialize('Sales Team',
            'Please review sales order ' + Rec."No." +
            ' for customer ' + Rec."Sell-to Customer Name")
        .SetRequiresReview(false);

    AgentTask := AgentTaskBuilder
        .Initialize(MyAgentSetup."User Security ID", 'Review Sales Order')
        .AddTaskMessage(AgentTaskMessageBuilder)
        .Create();
end;
```

> [!WARNING]
> **⚠️ Only skip review for trusted sources** — never set `SetRequiresReview(false)` for messages that originate from external or user-controlled input. Always validate inputs first.

---

## Part 3 — Trigger a Task from a Business Event

Instead of a button, let the system create the task automatically when something happens.

### Task 4 — Subscribe to a posting event

Add an event subscriber that fires after a sales invoice posts. No user action needed — the agent kicks off automatically.

```al
[EventSubscriber(ObjectType::Codeunit, Codeunit::"Sales-Post",
    'OnAfterSalesInvHeaderInsert', '', false, false)]
local procedure OnAfterSalesInvoicePost(var SalesInvHeader: Record "Sales Invoice Header")
var
    AgentTaskBuilder: Codeunit "Agent Task Builder";
    AgentTaskMessageBuilder: Codeunit "Agent Task Message Builder";
    MyAgentSetup: Record "My Agent Setup";
begin
    if not MyAgentSetup.FindFirst() then
        exit;

    AgentTaskMessageBuilder
        .Initialize(
            'System',
            'New invoice posted: ' + SalesInvHeader."No." +
            ' for ' + SalesInvHeader."Sell-to Customer Name" +
            '. Amount: ' + Format(SalesInvHeader.Amount))
        .SetRequiresReview(false);

    AgentTaskBuilder
        .Initialize(MyAgentSetup."User Security ID", 'Review Posted Invoice')
        .AddTaskMessage(AgentTaskMessageBuilder)
        .Create();
end;
```

### Task 5 — Post an invoice and observe the task

Publish, then post a sales invoice. Switch to the **Agents (preview)** page and watch the task appear automatically. Check the log to see how it processed the details.

---

## Part 4 — Track Task Lifecycle

### Task 6 — Store the task ID alongside your record

For proper tracking, add an `Agent Task ID` field (BigInteger) to the relevant table and save the ID on task creation:

```al
AgentTask := AgentTaskBuilder
    .Initialize(MyAgentSetup."User Security ID", 'Review Sales Order')
    .AddTaskMessage(AgentTaskMessageBuilder)
    .Create();

Rec."Agent Task ID" := AgentTask.ID;
Rec.Modify();
```

Later you can check its status:

```al
var
    AgentTaskCU: Codeunit "Agent Task";
    AgentTaskRec: Record "Agent Task";
begin
    AgentTaskRec.Get(Rec."Agent Task ID");

    if AgentTaskCU.IsTaskRunning(AgentTaskRec) then
        Message('Still running...')
    else if AgentTaskCU.IsTaskCompleted(AgentTaskRec) then
        Message('Done!');
end;
```

> [!TIP]
> **💡 External ID vs table-based tracking** — use `SetExternalId` on the task builder for simple one-to-one correlations (e.g. an email thread ID). For anything more complex — multiple records per task, audit trails, querying by business entity — store the task ID in your own table as shown above.

---

## ✅ Stage complete when…

- [ ] You can trigger an agent task from a page action
- [ ] You can trigger an agent task from a business event
- [ ] You can look up the task record in AL to check whether it completed

---

← [Stage 8: Customizing the Setup Page ⚙️](./08-setup-page.md) · [Overview](../README.md) · Next → [Stage 10: Bring Your Own App 🔌](./10-bring-your-own-app.md)
