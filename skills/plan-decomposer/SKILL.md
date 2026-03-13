---
name: plan-decomposer
user-invocable: true
description: Performs repo-aware preflight and decomposes an implementation plan into small, independently verifiable Parts. Writes each Part into its own Markdown file plus an OVERVIEW.md index file that references all Parts. Includes a Status line in each Part file and emits a strict, parseable PART_SPEC JSON per Part so output is guaranteed consumable by the executor skill.
license: MIT
compatibility: Designed for skills-compatible coding agents, including Claude Code and GitHub Copilot (VS Code). Assumes read/write access to the repository workspace.
metadata:
  author: Gridcreative Holding B.V. by Jursley Koots
  version: "2.1.0"
---

# Plan Decomposer
*(Plan → preflight → Part files + overview index)*

## What this skill does
When the user provides an implementation plan, you will:
1) **Preflight** the repository and plan for mismatches and risks
2) **Decompose** the plan into small ordered **Parts**
3) **Write files**:
   - `./ai-parts/OVERVIEW.md` (index of all parts)
   - `./ai-parts/P01-<slug>.md`, `P02-<slug>.md`, ... (one per part)
4) Ensure each part contains:
   - a **Status line** (machine-scannable)
   - strict, parseable `PART_SPEC` JSON (file-based handoff contract)

---

## Reasoning Mode & Agent Scope  ⬅ NEW

Apply the architectural, testing, and system-design principles of the
**expert-dotnet-software-engineer** agent while decomposing the plan.

Scope the agent’s influence carefully:

- **Backend / Domain / API / Data / Infrastructure**
  - Apply the agent fully (SOLID, TDD, boundaries, contracts, risks).

- **Shared / Contract Parts (DTOs, API schemas, auth flows)**
  - Apply the agent fully for correctness, validation, and versioning.

- **Frontend (Next.js / React) Parts**
  - Apply the agent *only* at the architectural level:
    - data boundaries
    - API contracts
    - error handling
    - testability
  - Do **NOT** impose .NET patterns or backend abstractions on frontend implementation details.

For each Part, implicitly classify it as:
- Backend
- Frontend
- Shared / Contract
- Infrastructure

And apply the agent according to the rules above.

---

## Non-negotiable rules
- Every behavioral change must be **TDD-first** (tests before production code).
- Every Part must be **independently verifiable** and include:
  - tests-first description (Red)
  - acceptance criteria
  - verification commands
  - rollback steps
- Parts must be **small** and **ordered**.
- No scope creep: unclear items go under **Open Questions / Assumptions** in the overview.

---

## Output location (required)
Write all decomposition outputs into:
- `./ai-parts/`

If `./ai-parts/` does not exist, create it.

---

## Step 0 — Repo + Plan Preflight (required)
Before writing any Part files:
1) Scan the repo quickly:
   - layout (solutions/projects/packages)
   - test runner + build tooling
   - CI scripts or common commands
2) Compare plan assumptions to repo reality:
   - referenced files/modules that don’t exist
   - stack mismatches
   - missing prerequisites

Then write preflight results into:
- `./ai-parts/OVERVIEW.md` (in the Preflight section)

---

# File-based Handoff Contract (required)
The decomposition MUST be represented by files so that an executor can consume them without ambiguity.

## Required files
1) `./ai-parts/OVERVIEW.md`
2) One file per part:
   - `./ai-parts/P01-<slug>.md`
   - `./ai-parts/P02-<slug>.md`
   - ...

## Required Part Status line (new requirement)
Each Part file MUST include a single Status line near the top:

- `Status: TODO`

Allowed values:
- TODO | IN_PROGRESS | DONE | BLOCKED

The Status line must be easy to find (top section) and must be exactly one of the allowed values.

---

## Required structure: each Part file
Each Part file MUST contain:
- A heading: `# Part PNN — <title>`
- A Status line: `Status: TODO`
- A `PART_SPEC` section containing **strict JSON** (double quotes, no trailing commas)
- A short execution checklist (tests-first, verify, rollback)

### PART_SPEC schema (required)
REQUIRED fields:
- `part_id` (string, e.g. "P01")
- `title` (string)
- `goal` (string)
- `scope` (object with `in` array and `out` array)
- `file_touch_points` (array of strings)
- `tests_first` (object; see below)
- `acceptance_criteria` (array of strings)
- `verify` (array of strings; runnable commands)
- `rollback` (array of strings)

`tests_first` REQUIRED fields:
- `test_files` (array of strings)
- `test_cases` (array of strings; names/descriptions)
- `expected_red` (string)

OPTIONAL fields:
- `dependencies` (array of part_id strings)
- `risks` (array of strings)
- `notes` (array of strings)
- `definition_of_done` (array of strings)

---

## Required structure: OVERVIEW.md
`./ai-parts/OVERVIEW.md` MUST include:

1) `# AI Parts Overview`
2) `## Preflight` (Plan summary, repo reality check, risks, open questions)
3) `## Parts Index` (a table listing all parts)
4) `## Execution Order` (ordered list of part IDs)
5) `## How to Execute` (instructions to feed this overview to the executor skill)

### Parts Index table columns (required)
- Part ID
- Title
- File
- Dependencies
- Status

Status values:
- TODO | IN_PROGRESS | DONE | BLOCKED

---

## Writing the decomposition files (required procedure)

### 1) Create OVERVIEW.md skeleton first
Write `./ai-parts/OVERVIEW.md` with:
- Preflight section filled out
- Parts Index table header
- Execution Order list (initially can be empty if you still need to write parts)

### 2) Create Part files
For each Part:
- Choose `part_id`: P01..PNN
- Choose a short slug for filename
- Write file: `./ai-parts/P<NN>-<slug>.md`

Each Part file MUST include:
- `Status: TODO` at the top section
- Summary bullets (Goal, Scope, Touch points)
- Tests-first bullets (what test(s) you will write first and what should fail)
- Verify commands
- Rollback steps
- `PART_SPEC` JSON matching the schema

### 3) Populate OVERVIEW.md index
After creating Part files:
- Fill Parts Index table rows with correct file paths
- Fill Execution Order list in correct order
- Ensure Status column matches each Part file’s Status (initially TODO)

---

## Part file template (required)
Use this exact structure in each part file:

# Part PNN — <title>
Status: TODO

## Summary
- Goal:
- Scope In:
- Scope Out:
- Expected file touch points:

## Tests First (TDD: Red)
- Test files:
- Test cases:
- Expected Red (what fails and why):

## Acceptance Criteria
- ...

## Verify
- ...

## Rollback
- ...

## PART_SPEC
(Strict JSON per schema.)

## Notes (optional)
- ...

---

## Overview file template (required)
Use this exact structure in `./ai-parts/OVERVIEW.md`:

# AI Parts Overview

## Preflight
### Plan Summary
- ...

### Repo Reality Check
- ...

### Top Risks
- ...

### Open Questions / Assumptions
- ...

## Parts Index
| Part ID | Title | File | Dependencies | Status |
|---|---|---|---|---|
| P01 | ... | ./ai-parts/P01-....md | (none) | TODO |

## Execution Order
1. P01
2. P02
...

## How to Execute
- Use the `part-executor-tdd` skill.
- Provide this OVERVIEW.md as input.
- The executor will iterate Parts in order, open each referenced file, and execute it strictly using TDD.

---

## Communication style
- Structured Markdown
- Short bullets, checklists
- Be precise and file-path oriented
