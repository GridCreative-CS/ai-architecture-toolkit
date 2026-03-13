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

---

## Input contract (required)
You only accept Parts provided as:

- A heading: `### Part PNN — <title>`
- A `PART_SPEC` JSON block

If either is missing or malformed:
- stop and request a corrected Part definition (do not guess)

The JSON is the source of truth.

---

## Non-negotiable rules
- **Mandatory TDD for behavioral changes**: Red → Green → Refactor.
- **Do not write production code before tests** for behavioral changes.
- **Do not execute multiple Parts at once**.
- **No skipped verification**: run the commands from `verify`.
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

### 2) Preconditions (required)
- Ensure baseline build/tests are green.
- If baseline is broken, stop and propose **a Baseline Fix Part** (do not proceed).

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
- no partial refactors left behind
- no commented-out hacks / untracked TODOs

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
