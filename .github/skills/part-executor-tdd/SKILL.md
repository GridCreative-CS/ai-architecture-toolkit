---
name: part-executor-tdd
description: Executes a single decomposed Part (PART_SPEC) at a time using mandatory TDD (red-green-refactor). Enforces quality gates, requires runnable verification commands, and produces a structured completion report including TDD evidence. Consumes the Part Handoff Contract emitted by plan-decomposer.
license: MIT
compatibility: Designed for skills-compatible coding agents, including Claude Code and GitHub Copilot (VS Code). Assumes write access to the repository workspace and ability to run tests/build commands.
metadata:
  author: Gridcreative Holding B.V.  by Jursley Koots
  version: "1.1.0"
---

# Part Executor (TDD)
*(Execute one PART_SPEC → Red/Green/Refactor → verify → report)*

## What this skill does
Given exactly **one Part** in the **Part Handoff Contract** format, you will:
- confirm preconditions
- implement the part using **mandatory TDD**
- keep the repo green
- deliver a completion report with verification commands and TDD evidence


## Feature Spec Awareness  ⬅ NEW

This skill may also receive or be expected to consult a **slice-level feature specification**
for the Part being executed.

If a feature spec exists for the selected slice, treat it as an additional source of truth
together with the Part definition, especially for:

- scope boundaries
- acceptance criteria interpretation
- API / contract expectations
- security and authorization constraints
- observability requirements
- test implications

### Typical feature spec location
- `architecture/feature-specs/<slice-name>.md`

### Priority rule
The `PART_SPEC` JSON remains the immediate execution contract.
However, if a relevant feature spec exists, it should be used to validate that the Part is
being implemented in the intended slice context and not drifting from the approved slice design.


---

## Input contract (required)
You only accept Parts provided as:

- A heading: `### Part PNN — <title>`
- A `PART_SPEC` JSON block

If either is missing or malformed:
- stop and request a corrected Part definition (do not guess)

The JSON is the source of truth.

If a relevant feature spec is available, use it to interpret the Part safely.

---

## Non-negotiable rules
- **Mandatory TDD for behavioral changes**: Red → Green → Refactor.
- **Do not write production code before tests** for behavioral changes.
- **Do not execute multiple Parts at once**.
- **No skipped verification**: run the commands from `verify`.
- Do not violate the active slice feature spec if one exists.
- No scope creep: if the Part is unclear, list assumptions and implement the smallest safe choice.

---

## Execution protocol (repeat for each Part)
### 1) Parse PART_SPEC (required)
- Read and restate:
  - `part_id`, `title`, `goal`
  - `tests_first`
  - `acceptance_criteria`
  - `verify`
  - `rollback`
- Identify dependencies; if unmet, stop and ask for the required preceding Part(s).
- If a relevant slice feature spec exists, restate the most relevant:
  - scope boundaries
  - API / contract expectations
  - security / authorization constraints
  - observability requirements
  - test implications

### 2) Preconditions (required)
- Ensure baseline build/tests are green.
- If baseline is broken, stop and propose **a Baseline Fix Part** (do not proceed).
- If a relevant feature spec exists, verify the Part still fits the intended slice scope.

### 3) TDD workflow (required)
1) **Red**
   - Implement/adjust tests listed in `tests_first.test_files`
   - Ensure at least one test fails consistent with `tests_first.expected_red`
2) **Green**
   - Minimal production changes to pass tests
   - Re-run the relevant test suite
3) **Refactor**
   - Improve structure while staying green
   - Re-run tests

### If tests are not feasible
You must justify why and use the best alternative verification:
- integration/contract tests
- snapshot tests
- type-level tests
- lint/static analysis checks
- manual QA checklist (last resort)

---

## Quality gates (must pass)
A Part is “Done” only if:
- tests are green (or approved alternative verification completed)
- build is green
- acceptance criteria met
- no conflict with the relevant feature spec remains unresolved
- no partial refactors left behind
- no commented-out hacks / untracked TODOs- architecture / layer-dependency tests are comprehensive: if the Part adds or modifies
  cross-layer boundaries, verify that architecture tests cover **all** prohibited dependency
  directions (e.g. Domain must not reference Api, Worker, Application, Infrastructure — not
  just a subset). Check the existing architecture tests and add any missing guardrails.
- immutability contracts are enforced: value objects / types documented as immutable must
  defensively copy mutable inputs and expose read-only views. Add tests that prove external
  mutation of source data does not affect the constructed instance.
If a gate fails:
- fix it now, or
- roll back and propose a smaller Part/spike.

---

## Required completion report
At the end of the Part, output exactly:

## Part Complete — <part_id>: <title>
- **TDD evidence**
  - Tests added/updated:
  - Red observed (command + what failed):
  - Green achieved (command + what passed):
- **Acceptance criteria check**
  - <criterion>: PASS/FAIL
- **Feature spec alignment**
  - In scope for selected slice: PASS/FAIL
  - Contract expectations respected: PASS/FAIL
  - Security / observability expectations respected: PASS/FAIL
- **What changed**
- **Files changed**
- **How to verify** (commands from PART_SPEC)
- **Rollback** (commands/steps from PART_SPEC)
- **Notes**
- **Next part** (name only; do not start it)

---

## Example prompts this skill expects
- “Execute this Part only using PART_SPEC and strict TDD: <paste Part>”
- “Consume the PART_SPEC below and implement it; then give the completion report.”


## When a Feature Spec Exists  ⬅ NEW

If a relevant feature spec exists for the selected slice, execution must remain aligned with it.

### Required behavior
- Use the feature spec to interpret ambiguous acceptance criteria safely.
- Do not widen the Part scope beyond the selected slice.
- Ensure implementation respects stated API, data, security, and observability expectations.
- Use feature-spec test implications to strengthen test choices where relevant.
- If the Part appears to conflict with the feature spec, stop and report the inconsistency.

### Escalation rule
If the `PART_SPEC` and the feature spec disagree in a meaningful way:
- do not guess
- identify the conflict explicitly
- implement the smallest safe choice only if unambiguous
- otherwise stop and request clarification or a corrected Part
