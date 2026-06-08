# ⚙️ Stage 8 — Customizing the Setup Page

> Add per-environment configuration to your agent using a custom setup page.

**Stage 8 of 10** · ← [Stage 7: Agent in AL](./07-al-agent.md) · [Overview](../README.md) · Next → [Stage 9: Programmatic Tasks](./09-programmatic-tasks.md)

---

## Overview

Once your agent is deployed as an extension, different environments will likely need different configuration — company name, tone, maybe an API endpoint. Hardcoding those means a recompile each time. The setup page solves that — admins can change values without touching code.

Here we'll add a custom field and inject it into the instructions dynamically.

> [!TIP]
> 📖 **Docs reference** — [Agent setup pages (preview)](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/ai/ai-agent-sdk-setup-page)

---

## Part 1 — Add a Custom Field to the Setup Table

### Task 1 — Add a field to your setup table

Open your agent's setup table (the one with `User Security ID` as the primary key) and add a field for the per-environment value. For example, a description of what product the agent is supporting.

```al
field(20; "Product Description"; Text[250])
{
    Caption = 'Product Description';
    DataClassification = CustomerContent;
}
```

---

## Part 2 — Expose the Field on the Setup Page

### Task 2 — Add the field to the setup page layout

Open the setup page and add a group for your new field below the standard `Agent Setup Part`. Setting `IsUpdated := true` on validate activates the **Update** button when the value changes.

```al
group(AdditionalConfiguration)
{
    Caption = 'Additional Configuration';

    field(ProductDescription; Rec."Product Description")
    {
        ApplicationArea = All;
        ToolTip = 'Describe the product this agent supports. This is injected into the agent instructions.';

        trigger OnValidate()
        begin
            IsUpdated := true;
        end;
    }
}
```

### Task 3 — Save the custom field in SaveCustomProperties

In `SaveCustomProperties`, make sure the new field gets written to the database alongside the existing ones.

```al
MyAgentSetupRecord."Product Description" := TempMyAgentSetup."Product Description";
```

---

## Part 3 — Inject the Value into the Instructions

### Task 4 — Switch from static to dynamic instructions

In `SaveSetupRecord`, replace the static instructions with a dynamic version that reads the custom field and merges it in:

```al
local procedure GetInstructions(ProductDescription: Text): Text
var
    InstructionsTxt: Label 'You are an agent for a Business Central customer.\\Product context: %1\\Your role is to help users navigate and operate the system efficiently.', Comment = '%1 = product description';
begin
    exit(StrSubstNo(InstructionsTxt, ProductDescription));
end;
```

Then pass the value when calling `Agent.SetInstructions`:

```al
if IsNewAgent then
    Agent.SetInstructions(TempMyAgentSetup."User Security ID",
        GetInstructions(TempMyAgentSetup."Product Description"));
```

---

## Part 4 — Test the Setup Page End-to-End

### Task 5 — Publish and delete the existing agent instance

Publish (<kbd>F5</kbd>), then delete the agent instance from Stage 7 on the **Agents (preview)** page so you can create a fresh one through the setup dialog.

### Task 6 — Create a new instance and fill in the custom field

Create a new instance. When the setup dialog opens you should see your **Product Description** field — fill it in and click **Update**.

### Task 7 — Verify the instructions reflect the configured value

Open the agent card and check that your description appears in the **Instructions** field. Run a task to make sure it's behaving accordingly.

---

> [!TIP]
> **💡 Why this pattern matters** — the setup page is the only place an admin touches your agent's config. Keep the instructions template in AL, the variable parts in the setup table, and you ship one `.app` that works everywhere — no per-customer forks.

---

## ✅ Stage complete when…

- [ ] Your setup page shows the custom **Product Description** field
- [ ] A new agent instance picks up the value you entered
- [ ] The instructions stored on the agent contain that value

---

← [Stage 7: Agent in AL 💻](./07-al-agent.md) · [Overview](../README.md) · Next → [Stage 9: Programmatic Tasks ⚡](./09-programmatic-tasks.md)
