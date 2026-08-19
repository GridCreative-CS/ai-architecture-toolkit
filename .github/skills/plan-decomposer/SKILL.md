---
name: plan-decomposer
user-invocable: true
description: Performs repo-aware preflight and decomposes an implementation plan into small, independently verifiable Parts. Writes each Part into its own Markdown file plus an OVERVIEW.md index file that references all Parts. Includes a Status line in each Part file and emits a strict, parseable PART_SPEC JSON per Part so output is guaranteed consumable by the executor skill.
license: MIT
compatibility: Designed for skills-compatible coding agents, including Claude Code and GitHub Copilot (VS Code). Assumes read/write access to the repository workspace.
metadata:
  author: Gridcreative Holding B.V. by Jursley Koots
  version: "2.4.0"
---

# Plan Decomposer
*(Plan → preflight → Part files + overview index)*

## What this skill does
When the user provides an implementation plan, you will:
1) **Preflight** the repository and plan for mismatches and risks
2) **Decompose** the plan into small ordered **Parts**
3) **Write files** (one folder per slice — see Output location):
   - `./ai-parts/<slice-id>/OVERVIEW.md` (index of all parts for this slice)
   - `./ai-parts/<slice-id>/P01-<slug>.md`, `P02-<slug>.md`, ... (one per part)
4) Ensure each part contains:
   - a **Status line** (machine-scannable)
   - strict, parseable `PART_SPEC` JSON (file-based handoff contract)


## Feature Spec Awareness

This skill may also receive a **slice-level feature specification** in addition to
the broader implementation or delivery plan.

If a feature spec exists for the selected slice, treat it as a primary input for
that slice together with:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`

### Priority rule
For the selected slice, prefer the feature spec over vague or broader delivery-plan
wording when defining scope, acceptance criteria, contracts, test implications, and
touch points.

### Glossary reference
For definitions of "independently verifiable," "scope creep,"
"decomposition-ready," and other key terms, see `ai/guides/glossary.md`.

### Typical feature spec location
- `architecture/feature-specs/<slice-id>-<slice-name>.md`


---

## Reasoning Mode & Agent Scope

Apply the architectural, testing, and system-design principles of the
**expert-dotnet-software-engineer** agent while decomposing the plan.

Scope the agent’s influence carefully:

- **`backend` and `infrastructure` Parts (domain, API, data, infra)**
  - Apply the agent fully (SOLID, TDD, boundaries, contracts, risks).

- **`shared-contract` Parts (DTOs, API schemas, auth flows)**
  - Apply the agent fully for correctness, validation, and versioning.

- **`frontend` Parts (e.g. Next.js / React)**
  - Apply the agent *only* at the architectural level:
    - data boundaries
    - API contracts
    - error handling
    - testability
  - Do **NOT** impose .NET patterns or backend abstractions on frontend implementation details.

For each Part, classify it as one of:
- `backend`
- `frontend`
- `shared-contract`
- `infrastructure`

Apply the agent according to the rules above, and **record the classification
explicitly** — in the PART_SPEC `part_type` field and the Parts Index Type
column. Step 6a uses it to decide which review dimensions apply to the Part,
so leaving the classification implicit pushes a guess onto the reviewer.

---

## Non-negotiable rules
- Every behavioral change must be **TDD-first** (tests before production code).
- Every Part must be **independently verifiable** and include:
  - tests-first description (Red)
  - acceptance criteria
  - verification commands
  - rollback steps
- Parts must be **small** and **ordered**.
- If a feature spec exists for the selected slice, use it to tighten decomposition scope.
- **Every feature spec criterion gets an owning Part.** Build the Requirement
  Coverage Map and check it before finishing: a criterion (§6 `DR-nn`, §9
  `SEC-nn`, §11 `AC-nn`, §11b `UIAC-nn`) that no Part owns is a decomposition
  defect, not an open question. Decomposition is not complete until the map
  has no unowned criterion.
- No scope creep: unclear items go under **Open Questions / Assumptions** in the overview.

---

## Output location (required)
Write all decomposition outputs into a per-slice folder:

- `./ai-parts/<slice-id>/` — the slice ID from the delivery plan, matching
  its casing exactly (e.g., `./ai-parts/S2.6/`, `./ai-parts/phase-1a/`). If
  the project already uses a different consistent per-slice folder scheme
  (e.g., `slice2.6`), match the existing scheme.

Never mix Parts from two slices in one folder — one slice = one folder.
If the folder does not exist, create it.

Every path below is written out in full. There is no `./ai-parts/<file>`
shorthand: a path without `<slice-id>` is wrong.

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
3) If a slice-level feature spec is provided, compare it against:
   - the delivery plan
   - relevant architecture constraints
   - repo reality and touch points
4) **Build a pattern inventory** (see `ai/guides/code-quality-standard.md` §1):
   for each kind of artifact the slice will add (handler, endpoint, entity,
   migration, component, API module, test class), name **concrete existing
   files** that exemplify the project's current pattern — error handling and
   error identifiers, validation split, logging/metrics/tracing, async +
   cancellation usage, naming, test naming and assertion style. Executors must
   read these files before writing code. If the project has no comparable code
   yet, say so explicitly. If two existing files show conflicting patterns for
   the same thing, record that as an open question — do not pick silently.

Then write preflight results into:
- `./ai-parts/<slice-id>/OVERVIEW.md` (in the Preflight section, including a
  `### Pattern Inventory` subsection)

---

# File-based Handoff Contract (required)
The decomposition MUST be represented by files so that an executor can consume them without ambiguity.

## Required files
All decomposition output lives in a **per-slice folder**,
`./ai-parts/<slice-id>/` (see Output location):
1) `./ai-parts/<slice-id>/OVERVIEW.md`
2) One file per part:
   - `./ai-parts/<slice-id>/P01-<slug>.md`
   - `./ai-parts/<slice-id>/P02-<slug>.md`
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
- `part_type` (string; one of `backend`, `frontend`, `shared-contract`,
  `infrastructure` — the classification from **Reasoning Mode & Agent Scope**,
  made explicit. Step 6a uses it to decide which review dimensions apply, so
  the reviewer does not have to guess. **Emit it on every Part.** When absent,
  the reviewer classifies from `file_touch_points` and records what it used.)
- `criteria_covered` (array of criterion IDs from the feature spec —
  `AC-nn`, `UIAC-nn`, `SEC-nn`, `DR-nn` — that this Part is responsible for
  satisfying. Must agree with the OVERVIEW Requirement Coverage Map. The
  executor's quality report §3b marks these `COVERED-THIS-PART` or explains
  why not.)
- `e2e_verify` (array of strings; browser-based verification commands — e.g.,
  Playwright test commands, Cypress commands, or documented manual browser
  walkthrough steps. **Required for the final Part of any UI slice.**)
- `existing_patterns` (array of strings; concrete existing files from the
  Preflight pattern inventory the executor must read before writing code —
  **include this whenever comparable code exists in the repo**)
- `contracts_touched` (array of strings; which of the four contract surfaces —
  `public-api`, `database-schema`, `events`, `ui-behavior` — this Part is
  expected to change, or `["none"]`. The executor's quality report §7 must be
  consistent with this or explain the difference.)

---

## Required structure: OVERVIEW.md
`./ai-parts/<slice-id>/OVERVIEW.md` MUST include:

1) `# AI Parts Overview`
2) `## Preflight` (Plan summary, repo reality check, risks, open questions,
   `### Pattern Inventory`)
3) `## Requirement Coverage Map` (every feature spec criterion → owning Part)
4) `## Parts Index` (a table listing all parts)
5) `## Execution Order` (ordered list of part IDs)
6) `## How to Execute` (instructions to feed this overview to the executor skill)

### Parts Index table columns (required)
- Part ID
- Title
- File
- Type (backend | frontend | shared-contract | infrastructure)
- Dependencies
- Status

Status values:
- TODO | IN_PROGRESS | DONE | BLOCKED

### Requirement Coverage Map (required when a feature spec exists)

One row per criterion in the slice's feature spec — every §6 (`DR-nn`), §9
(`SEC-nn`), §11 (`AC-nn`), and §11b (`UIAC-nn`) entry:

| Criterion | Text (short) | Owning Part(s) | Verified at |
| --- | --- | --- | --- |
| AC-01 | … | P03 | P03 tests |
| UIAC-04 | … | P07 | Step 6b browser verification |

Rules:

- **Every criterion has an owner.** A criterion no Part owns is a
  decomposition defect — fix the decomposition; do not write "TBD". Step 5 is
  not complete while any criterion is unowned.
- `Verified at` names where the proof lands: a Part's tests, or a named later
  step (Step 6b) for what only browser verification can show.
- The map is what the executor's quality report §3b fills its owner column
  from, and what the Step 6a reviewer audits statuses against.
- If the feature spec predates criterion IDs, key rows as
  `§<section> "<verbatim text>"`.

Keep the map current: if a Part is inserted (`P09b`) or re-scoped, update the
owning Part here in the same pass.

---

## Writing the decomposition files (required procedure)

### 1) Create OVERVIEW.md skeleton first
Write `./ai-parts/<slice-id>/OVERVIEW.md` with:
- Preflight section filled out
- Parts Index table header
- Execution Order list (initially can be empty if you still need to write parts)

### 2) Create Part files
For each Part:
- Choose `part_id`: P01..PNN
- Choose a short slug for filename
- Write file: `./ai-parts/<slice-id>/P<NN>-<slug>.md`

If a Part must be **inserted later** between existing Parts (discovered scope
that cannot wait), suffix a letter instead of renumbering: `P09b-<slug>.md`
with `part_id` "P09b", dependencies on P09, and a new row + execution-order
entry in OVERVIEW.md. Never renumber existing Part files.

Each Part file MUST include:
- `Status: TODO` at the top section
- Summary bullets (Goal, Scope, Touch points)
- Tests-first bullets (what test(s) you will write first and what should fail)
- Verify commands
- Rollback steps
- `PART_SPEC` JSON matching the schema

### 3) Populate OVERVIEW.md index
After creating Part files:
- Fill Parts Index table rows with correct file paths and each Part's Type
- Fill Execution Order list in correct order
- Ensure Status column matches each Part file’s Status (initially TODO)

### 4) Fill and check the Requirement Coverage Map
- One row per feature spec criterion (§6 `DR-nn`, §9 `SEC-nn`, §11 `AC-nn`,
  §11b `UIAC-nn`), with its owning Part and where it will be verified
- Cross-check both directions: every criterion has an owner, and every Part's
  `criteria_covered` appears in the map
- **A criterion with no owner blocks completion** — add or re-scope a Part.
  Do not hand the gap to the executor to discover

---

## Part file template (required)
Use this exact structure in each part file:

# Part PNN — <title>
Status: TODO

## Summary
- Goal:
- Type: backend | frontend | shared-contract | infrastructure
- Criteria covered: <criterion IDs from the feature spec, e.g. AC-03, SEC-01>
- Scope In:
- Scope Out:
- Expected file touch points:

## Tests First (TDD: Red)
- Test files:
- Test cases: <include the negative/edge case for each criterion this Part
  owns, not only the happy path>
- Expected Red (what fails and why):

## ProjectReference Checklist (required for test project parts — .NET default; substitute the project's stack equivalent)
- [ ] Each `tests/<Module>.Tests.csproj` references `../../src/<Module>/<Module>.csproj`
- [ ] `dotnet build <solution>` passes (full solution, not just target project)

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

## Terminal Verification Part (required for UI slices)

For slices with human workflow surfaces, the **last Part** in the decomposition
must be a dedicated verification Part that:

1. Has no production code changes (verification only)
2. Runs the full application
3. Executes the Slice Completion Verification Checklist
   (`ai/templates/slice-verification-checklist-template.md`)
4. Runs any E2E browser tests
5. Verifies cross-slice navigation and shared layout integrity
6. Documents verification evidence

This Part's PART_SPEC must include:

- `e2e_verify` — browser-based verification commands
- `acceptance_criteria` — all §11 and §11b criteria from the feature spec
- `verify` — commands to start the app + run E2E tests

---

## Overview file template (required)
Use this exact structure in `./ai-parts/<slice-id>/OVERVIEW.md`:

# AI Parts Overview

## Preflight
### Plan Summary
- ...

### Repo Reality Check
- ...

### Pattern Inventory
- <artifact kind>: follow `<existing file path>` (and its tests: `<test file path>`)
- ...

### Top Risks
- ...

### Open Questions / Assumptions
- ...

## Requirement Coverage Map
| Criterion | Text (short) | Owning Part(s) | Verified at |
|---|---|---|---|
| AC-01 | ... | P01 | P01 tests |
| UIAC-01 | ... | P04 | Step 6b browser verification |

(Every §6/§9/§11/§11b criterion of the feature spec appears here. No criterion
may be left without an owner.)

## Parts Index
| Part ID | Title | File | Type | Dependencies | Status |
|---|---|---|---|---|---|
| P01 | ... | ./ai-parts/<slice-id>/P01-....md | backend | (none) | TODO |

## Execution Order
1. P01
2. P02
...

## How to Execute
- Use the `part-executor-tdd` skill.
- Execute exactly **one Part per run**: take the next non-DONE Part from the
  Execution Order, open its file, and execute it strictly using TDD.
- Use the selected slice feature spec if it exists.
- Read the Pattern Inventory files before writing code
  (`ai/guides/code-quality-standard.md` §1).
- Each Part ends with a Part Quality Report
  (`ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`) followed by the
  Part code review (engineering workflow Step 6a). Do not start the next Part
  until the review verdict is APPROVED or APPROVED WITH NOTES.
- After each Part completes, update its Status here and in the Part file
  before starting the next Part.

---

## Communication style
- Structured Markdown
- Short bullets, checklists
- Be precise and file-path oriented


## When a Feature Spec Is Provided

If a slice-level feature spec is provided, the decomposition must reflect it.

### Required behavior
- Decompose only the selected slice described by the feature spec.
- Use feature-spec acceptance criteria to shape Part acceptance criteria, and
  record which criterion IDs each Part owns in `criteria_covered`.
- Build the **Requirement Coverage Map** in OVERVIEW.md from §6/§9/§11/§11b —
  every criterion owned by exactly one Part (or explicitly by a later
  verification step). This map is what the executor's quality report §3b and
  the Step 6a coverage audit are checked against.
- Give the **negative and edge cases** their own `test_cases` entries where a
  criterion has one — a denied role issuing no request, an async source
  failing independently, a reset or unmount path, a locale-specific rendering.
  A Part whose tests only cover the happy path leaves its criterion unproven.
- Use feature-spec test implications to strengthen `tests_first`.
- Use feature-spec API, data, security, and observability notes to avoid vague Parts.
- Keep Parts aligned with both the feature spec and the broader architecture.

### Practical input set
For slice-level decomposition, the preferred input set is:

- `architecture/feature-specs/<slice-id>-<slice-name>.md`
- `architecture/delivery-plan.md`
- `architecture/architecture-final.md`
- `architecture/adr/*.md`
