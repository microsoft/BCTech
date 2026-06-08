# 📤 Stage 6 — Export & Import

> Export your agent definition to XML and import it into a new agent.

**Stage 6 of 10** · ← [Stage 5: Make It Your Own](./05-publishing.md) · [Overview](../README.md) · Next → [Stage 7: Agent in AL](./07-al-agent.md)

---

## Overview

Agents can be exported as XML and imported back in — handy for backups, sharing configs, or promoting between environments.

---

## Part 1 — Export the Agent

### Task 1 — Open your agent card

Go to the **Agents (preview)** page and open the agent you configured in the earlier stages.

### Task 2 — Export the agent definition

From the action bar, choose **Export agent definition**. BC generates and downloads an XML file with the full agent config — name, instructions, profile, permissions. Save it somewhere you can find it.

### Task 3 — Inspect the XML

Open it in a text editor (Notepad is fine) and have a look at the structure — can you spot the instructions you wrote? The profile name? This is the portable representation of your agent.

---

## Part 2 — Import the Agent

### Task 4 — Start a new import

Back on the **Agents (preview)** page, choose the **Import agent definition** action (you'll find it in the same area as the create button).

### Task 5 — Select the XML file

Select the XML file you exported in Part 1 — BC will read it and create a new agent from it.

### Task 6 — Verify the imported agent

Open the new agent and check that the instructions, profile and permissions match the original. Run a quick task to confirm it behaves the same way.

---

> [!TIP]
> **💡 What this enables** — this is how you move an agent between environments — sandbox to production — without recreating it by hand. The XML is readable enough to store in source control alongside your AL code too.

---

## ✅ Stage complete when…

- [ ] You have an exported XML file from your agent
- [ ] You have a successfully imported agent that matches the original

---

← [Stage 5: Make It Your Own 🛠️](./05-publishing.md) · [Overview](../README.md) · Next → [Stage 7: Agent in AL 💻](./07-al-agent.md)
