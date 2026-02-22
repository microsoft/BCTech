# Room Content: HTML Description and Solution Files

## Overview

Every room has two HTML files that are loaded and displayed inside Business Central:

- **Description file** — Presents the challenge. Tells participants WHAT to fix and WHERE to look, without revealing HOW.
- **Solution file** — Provides the complete, step-by-step answer after the solution delay expires.

These files are loaded via the `GetRoomDescription()` and `GetRoomSolution()` methods on `iEscapeRoom`, or embedded as BLOB fields on the Escape Room record. They render in the **Rich Text Box Page** inside BC.

> **BC HTML Viewer Constraints:** No JavaScript, no external CSS, no emoji or emoticons. Use inline styles only. Plain HTML with `<strong>`, `<em>`, `<code>`, `<pre>`, `<ul>`, `<ol>`, `<table>`, and `<div>` with inline `style`.

---

## Description Files

### The One Rule That Overrides All Others

**Descriptions present the MYSTERY. Solutions reveal the ANSWER.**

Participants must figure out HOW to solve the challenge — that is the entire point. The description exists to make the problem tangible and send them to the right place. It must never save them the thinking.

**The self-test:** "Could they copy-paste something from this description to solve the task?" If yes, the description is too revealing. Move it to the Solution file.

---

### Tone and Language

| Do | Do NOT |
|---|---|
| "Operations are slow" | "Record-by-record loops cause slowness" |
| "We test your ability to..." | "You will learn about..." |
| "Find a better approach" | "Use SetLoadFields to fix this" |
| "Something about how the data is loaded..." | "CalcFields fires a SQL query per record" |
| "Research what AL offers for bulk operations" | "Use DataTransfer for this scenario" |

- **Tone = "we test your skills"**, not "you will learn"
- Describe the **symptom**, not the cause
- Describe the **problem**, not the technique that fixes it
- Never name the AL method, class, or pattern that IS the solution

---

### Required Structure (7 sections, in this order)

#### 1. HTML Shell + H1

```html
<!DOCTYPE html>
<html>

<head>
    <title>Room X: Short Title</title>
</head>

<body>
    <h1>Room X: Short Title</h1>
    <!-- sections 2-7 here -->
</body>

</html>
```

Use a clear, descriptive title. Examples: `Room 2: Bulk Data Operations`, `Room 3: AL Profiler`.

---

#### 2. TL;DR (H2)

2-3 sentences maximum. The elevator pitch: what's broken, what you'll do, how you prove it's fixed. Participants who read nothing else should still know what the room is about.

```html
<h2>TL;DR</h2>
<p>Fix the slow upgrade in <strong>OptimAL.PTE</strong> by finding better approaches for bulk table
    transfers and field updates. Prove your optimization uses dramatically fewer database operations.</p>
```

---

#### 3. The Challenge (H2)

ONE short paragraph (2-4 sentences) that sets the scene. If the problem was experienced in a previous room, reference it. End with a single-sentence "why this matters" statement woven into the paragraph. **Do NOT list learning objectives, future rooms, or skills here.**

```html
<h2>The Challenge</h2>
<p>In Room 1, you experienced slow data operations taking far too long for 10,000 records. There
    must be a better way to transfer and update large amounts of data. Slow migrations directly
    impact upgrade windows and user downtime.</p>
```

**Rules:**
- ONE paragraph — never split into sub-sections
- "Why this matters" = ONE sentence, woven in, not a separate section or subsection
- Reference previous room context if applicable
- Do NOT write "This room teaches you...", "By the end of this room you will understand...", or any similar learning-objective framing

---

#### 4. Your Mission (H2)

The core of the description. Each mission item gets an **H3 with a descriptive title** — never task numbers ("Task 1:", "Task 2:"). Items may be grouped or split based on what makes logical sense, not one-to-one with validators.

Each mission item contains (as flowing paragraphs, NOT separate sub-sections):

- **What to do** — the goal, the problem to solve (described mysteriously)
- **Where to look** — exact object name, procedure name, page action (inline, as part of the prose)
- **How to trigger/reproduce** — numbered `<ol>` steps only when navigating BC UI
- **Hint** — a research nudge (use `<strong>Hint:</strong>`)
- **Validation** — how the system confirms success (use `<em>`)

Hints must be **VAGUE about technique** and **PRECISE about location**. When referencing a BC UI action, use the **exact label as it appears in Business Central, case-sensitive**. For non-toolbar actions, include the full navigation path:

```html
<p><strong>Hint:</strong> Navigate to <strong>Actions &rarr; Analytics &amp; Reporting</strong> and run 
the <strong>Customer Order Analytics</strong> action. Something about how it retrieves data might 
surprise you.</p>
```

```html
<h2>Your Mission</h2>

<h3>Optimize Table Transfer</h3>
<p>The upgrade code in <strong>Codeunit 74391 "Upgrade OptimAL PTE"</strong> copies records from
    Performance Test Customer to a Customer Archive table in a way that is extremely slow. Find a
    better approach that can transfer all records much more efficiently.</p>
<p><strong>Hint:</strong> AL must have better tools for data migration scenarios. Research what is
    available.</p>
<p><em>Republish OptimAL.PTE (bump version, F5) and check the Performance Measurements page for
    the upgrade entry. The system validates that total database operations drop below 100.</em></p>
```

**Rules for Your Mission:**
- H3 headings are **descriptive names** — never "Task 1:", "Task 2:", "Step 1:", etc.
- Never build separate "Where to Look", "Validation", "Key Concepts", or "Research Topics" sections — all of that content is inline inside the mission items
- Each item is self-contained: goal + location + hint + validation in one place
- Keep numbered `<ol>` steps ONLY for BC UI navigation (how to reproduce the problem)

---

#### 5. Warning / Gotcha Box (OPTIONAL)

Only for critical setup issues that would waste significant time if missed. Maximum one per room.

```html
<div style="background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0;">
    <p><strong>Important:</strong> Make sure your <strong>launch.json</strong> includes
        <code>"dependencyPublishingOption": "Ignore"</code> before pressing F5.</p>
</div>
```

---

#### 6. Update Status Reminder

Every description must end with this (just before "What's Next", or at the very end for the final room):

```html
<p><em><strong>Update Status:</strong> Not all steps are captured automatically. Hit the
    <strong>Update Status</strong> button on the room page to check if you have completed
    steps that were not registered yet.</em></p>
```

---

#### 7. What's Next (H2)

ONE sentence about the next room only. Omit for the final room in the venue.

```html
<h2>What's Next</h2>
<p><strong>Room 3:</strong> Learn to use profiling tools to discover performance bottlenecks yourself.</p>
```

---

### Sections FORBIDDEN in Descriptions

These sections create redundancy or reveal the answer. Their content either belongs inline in mission items, belongs only in Solution files, or should be removed entirely:

| Forbidden Section | Why |
|---|---|
| "How Task Validation Works" | Too much explanation — breaks mystery |
| "Business Impact" | Redundant filler |
| "Performance Results" / "Performance Baseline" table | Belongs in Solution files only |
| "Profiler Comparisons" | Belongs in Solution files only |
| "Skills Tested / Learning Objectives" | The mission items ARE the skills being tested |
| "Key Concepts / Research Topics" | Fold hints and research nudges into mission items |
| "Tips for Success" | Fold into mission items |
| "Real-World Application" | Filler — remove entirely |
| "Where to Look" as a standalone section | Put inline in mission items |
| "Validation" as a standalone section | Put inline in mission items |
| "Why This Room Matters" as a standalone section | One sentence in The Challenge paragraph |
| "What's Next" listing all remaining rooms | Only mention the next room, one sentence |
| Task numbers ("Task 1:", "Task 2:") in any heading | Use descriptive H3 names instead |
| "You will learn" / "This will teach you" language | Tone is "we test your" — not educational |
| Emoticons / emojis | BC HTML viewer cannot display them |
| Naming the solution technique | Keeps challenge mysterious |

---

## Solution Files

### Purpose

Solution files provide **exact, copy-pasteable, step-by-step instructions** for completing all tasks. They teach participants exactly how to solve the challenge while explaining WHY each step matters.

**Solution = EXACT HOW + WHY.** No ambiguity. No "try something like this." Every code block must be complete and working.

---

### Required Structure

#### 1. Main Heading

```html
<h1>Solution: Room X - Task Title</h1>
```

#### 2. Introduction (optional)

Brief paragraph explaining the overall approach.

#### 3. Task Sections

Each task gets its own section:

```html
<h2>Optimizing Table Transfer</h2>
<p>Replace the slow record-by-record loop with a bulk operation.</p>

<h3>Step 1: Locate the Code</h3>
<ol>
    <li>Open <strong>Codeunit 74390 "Upgrade OptimAL PTE"</strong></li>
    <li>Find the <code>TransferDataRecordByRecord()</code> procedure</li>
</ol>

<h3>Step 2: Implement the Solution</h3>
<pre>
local procedure TransferDataBulk()
var
    DataTransfer: DataTransfer;
begin
    DataTransfer.SetTables(Database::"Performance Test Customer", Database::"Customer Archive");
    DataTransfer.CopyFields();
    DataTransfer.CopyRows();
end;
</pre>

<h4>Why This Matters:</h4>
<ul>
    <li>Uses a set-based operation instead of a loop</li>
    <li>Reduces 10,000 queries to 1</li>
    <li>Dramatically improves performance at scale</li>
</ul>
```

**Section heading rules:**
- Use descriptive H2 names — never "Task 1:", "Task 2:", etc.
- Sub-steps use H3; explanations use H4
- Show "Why This Matters" after each major code block

---

#### 4. Forbidden Content in Solutions

| Forbidden | Why |
|---|---|
| "Performance Results" comparison tables | Removed in practice — numbers vary by environment |
| "Profiler Comparisons" sections | Removed in practice — misleading without live data |
| Task number references in headings | Use descriptive names |

---

#### 5. Code Examples

- **Always complete and working** — participants should be able to copy-paste
- Use `<pre>` for code blocks
- Include full procedure signatures, not fragments
- Include inline code comments for complex logic
- Show "before" (the problem) and "after" (the solution) where helpful

---

#### 6. Alternative Approaches

When multiple approaches are valid, show the recommended one first:

```html
<h3>Option 1: Bulk Transfer with DataTransfer (Recommended)</h3>
<pre>...</pre>

<h3>Option 2: Manual Loop with Commit</h3>
<pre>...</pre>
```

---

#### 7. Warning Boxes in Solutions

```html
<div style="background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0;">
    <p><strong>Security Warning:</strong> Hardcoding API keys is <strong>NOT</strong> a
        production pattern. This is a training shortcut only.</p>
</div>
```

---

#### 8. Verification Section

```html
<h2>Verification</h2>
<p>After completing all tasks:</p>
<ul>
    <li>The Performance Measurements page shows the upgrade entry with fewer than 100 DB operations</li>
    <li>Both duration and SQL statement count have dropped significantly</li>
</ul>
<p><strong>Important:</strong> Don't forget to click the <strong>Update Status</strong> button!</p>
```

---

#### 9. What You've Accomplished

```html
<h2>What You've Accomplished</h2>
<p>By completing this room, you have:</p>
<ul>
    <li><strong>Replaced record-by-record loops</strong> with set-based operations</li>
    <li><strong>Proven the improvement</strong> using built-in measurement tooling</li>
    <li><strong>Learned to recognise</strong> this class of performance problem in real code</li>
</ul>
<p><strong>Ready for Room 3:</strong> Now you will use the AL Profiler to discover bottlenecks yourself.</p>
```

---

## Tables

For comparisons, metrics, or structured data in solutions:

```html
<table border="1" cellpadding="8" style="border-collapse: collapse; width: 100%; margin: 20px 0;">
    <tr>
        <th style="text-align: left;">Operation</th>
        <th style="text-align: left;">Before</th>
        <th style="text-align: left;">After</th>
        <th style="text-align: left;">Improvement</th>
    </tr>
    <tr>
        <td>Transfer 10,000 records</td>
        <td>~120 seconds</td>
        <td>&lt;5 seconds</td>
        <td>24x faster</td>
    </tr>
</table>
```

---

## File Naming Convention

- Description: `Room[N][Topic]Description.html` — e.g., `Room2DataTransferDescription.html`
- Solution: `Room[N][Topic]Solution.html` — e.g., `Room2DataTransferSolution.html`

---

## Quality Checklists

### Description File Checklist

- [ ] Complete HTML structure with DOCTYPE
- [ ] Clear H1 title with room number
- [ ] TL;DR (2-3 sentences max)
- [ ] The Challenge (ONE paragraph, "why this matters" woven in as one sentence)
- [ ] Your Mission with descriptive H3 headings (no task numbers)
- [ ] Each mission item is self-contained: goal + location + hint + validation inline
- [ ] Hints use the exact BC UI action label (case-sensitive); non-toolbar actions include full nav path
- [ ] No standalone Skills Tested, Key Concepts, Research Topics, Validation, or Where to Look sections
- [ ] No Performance Results or Baseline tables
- [ ] Update Status reminder present (just before What's Next)
- [ ] What's Next mentions only the next room (one sentence)
- [ ] Problem described ONCE — no repetition across sections
- [ ] Tone is "we test your skills" — no "you will learn" language
- [ ] MYSTERIOUS — no method names, no code solutions, no technical terms that reveal the answer
- [ ] No emoticons or emojis

### Solution File Checklist

- [ ] Clear H1 with "Solution: Room X - Title"
- [ ] Step-by-step instructions for each task
- [ ] Complete, copy-pasteable code examples
- [ ] "Why This Matters" explanations after each code block
- [ ] Verification checklist
- [ ] "What You've Accomplished" summary
- [ ] Tutorial-style, explanatory tone
- [ ] Specific object names, procedure names, and exact UI navigation
- [ ] No Performance Results comparison tables
- [ ] No Profiler Comparisons sections
- [ ] No task number references in headings (use descriptive names)
- [ ] No emoticons or emojis
