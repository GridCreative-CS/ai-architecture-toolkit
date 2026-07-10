---
name: part-executor-tdd
description: Executes a single decomposed Part (PART_SPEC) at a time using mandatory TDD (red-green-refactor). Enforces quality gates, requires runnable verification commands, and produces a structured completion report including TDD evidence. Consumes the Part Handoff Contract emitted by plan-decomposer.
license: MIT
compatibility: Designed for skills-compatible coding agents, including Claude Code and GitHub Copilot (VS Code). Assumes write access to the repository workspace and ability to run tests/build commands.
metadata:
  author: Gridcreative Holding B.V.  by Jursley Koots
  version: "1.3.0"
---

# Part Executor (TDD)
*(Execute one PART_SPEC → read nearby code → Red/Green/Refactor → verify → quality report)*

## What this skill does
Given exactly **one Part** in the **Part Handoff Contract** format, you will:
- confirm preconditions
- read the nearby existing code and tests before writing anything
- implement the part using **mandatory TDD**, following the project's
  existing patterns (`ai/guides/code-quality-standard.md`)
- keep the repo green
- deliver a **Part Quality Report** with verification commands, TDD evidence,
  and an explicit contract-surface declaration
  (`ai/templates/code-quality-checklist-template.md`)


## Feature Spec Awareness

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
- `architecture/feature-specs/<slice-id>-<slice-name>.md`

### Priority rule
The `PART_SPEC` JSON remains the immediate execution contract.
However, if a relevant feature spec exists, it should be used to validate that the Part is
being implemented in the intended slice context and not drifting from the approved slice design.

### Glossary reference
For definitions of "TDD," "scope creep," "contract," and other key terms, see
`ai/guides/glossary.md`.


---

## Input contract (required)
You only accept Parts provided as:

- A heading: `# Part PNN — <title>` (one to three leading `#` accepted —
  `plan-decomposer` writes Part files with a single `#`)
- A `Status:` line (TODO | IN_PROGRESS | DONE | BLOCKED)
- A `PART_SPEC` JSON block

If the heading or the PART_SPEC is missing or malformed:
- stop and request a corrected Part definition (do not guess)

If the Part's `Status` is already `DONE`, stop and ask which Part to execute
instead. When you start executing, set the Part file's Status to
`IN_PROGRESS`; when the Part Quality Report is delivered and all gates pass,
set it to `DONE` and update the matching row in
`ai-parts/<slice-id>/OVERVIEW.md`. If the Part code review (engineering
workflow Step 6a) returns `REJECTED — MUST FIX`, set the Status back to
`IN_PROGRESS`, apply the required fixes within this Part (keeping TDD
discipline), regenerate the quality report, and hand back for re-review — the
Part is `DONE` only after an `APPROVED` or `APPROVED WITH NOTES` verdict.

The JSON is the source of truth.

If a relevant feature spec is available, use it to interpret the Part safely.

---

## Non-negotiable rules
- **Mandatory TDD for behavioral changes**: Red → Green → Refactor.
- **Do not write production code before tests** for behavioral changes.
- **Do not execute multiple Parts at once**.
- **No skipped verification**: run the commands from `verify`.
- Do not violate the active slice feature spec if one exists.
- **Read before write**: inspect nearby existing implementation and tests
  before writing any code, and follow the project's established patterns over
  generic model-generated patterns (`ai/guides/code-quality-standard.md` §1).
- **If the existing pattern is unclear or inconsistent, stop** and list the
  ambiguity as an open question — do not invent a new style.
- **No new libraries/packages** without explicit justification recorded in the
  quality report (code-quality standard §3).
- **No abstractions this Part does not need** — no speculative interfaces,
  base classes, wrappers, or configuration (code-quality standard §4).
- **No silent contract changes**: any change to a public API, database schema,
  event, or UI contract surface must be declared in the quality report §7 and
  covered by the feature spec (code-quality standard §12).
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

### 3) Read before write (required)
Before writing any test or production code:
- Open at least two existing files that do the same kind of work as this Part
  (same layer, same artifact type), **and their tests**. Start from the
  PART_SPEC `file_touch_points`, the `existing_patterns` field if present, and
  the OVERVIEW Preflight pattern inventory.
- Record the observed patterns you will follow: file placement/naming, error
  handling and error identifiers, validation split, logging/metrics/tracing,
  async + cancellation propagation, doc-comment style, test naming and
  assertion style. This list goes into the quality report §6.
- Follow `ai/guides/code-quality-standard.md` §§1–2: existing project patterns
  beat model defaults; when sources conflict, apply the precedence order; when
  the pattern is unclear or inconsistent, stop and list the ambiguity.
- If no comparable code exists (first slice, new layer), derive the pattern
  from the architecture, ADRs, and project instruction files, and state in the
  quality report that this Part establishes a new pattern.

### 4) TDD workflow (required)
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
- no commented-out hacks / untracked TODOs
- no prohibited outputs (code-quality standard §11): no placeholders, stub
  bodies, or fake implementations in production paths; no dead/unused code;
  no suppressed warnings without recorded justification
- tests prove behavior, not implementation: no test passes purely by
  verifying mocks were called or by mirroring internal steps; TDD claims are
  backed by recorded red evidence (command + observed failure)
- no existing test was weakened, deleted, or skipped to get to green (a
  legitimately obsolete test is removed with justification in the quality
  report)
- all four contract surfaces (public API, database/schema, events/messages,
  UI behavior) are explicitly declared **changed** (with spec coverage) or
  **unchanged** in the quality report §7
- any new dependency is justified in the quality report §8 (none added
  otherwise)
- architecture / layer-dependency tests are comprehensive: if the Part adds or modifies
  cross-layer boundaries, verify that architecture tests cover **all** prohibited dependency
  directions (e.g. Domain must not reference Api, Worker, Application, Infrastructure — not
  just a subset). Check the existing architecture tests and add any missing guardrails.
- immutability contracts are enforced: value objects / types documented as immutable must
  defensively copy mutable inputs and expose read-only views. Add tests that prove external
  mutation of source data does not affect the constructed instance.
- for the **final Part of a UI slice** (or a dedicated verification Part):
  the running application must be started and the user flow must be verified
  in a browser or via E2E browser tests. Passing component-level or unit
  tests alone is insufficient for slice-level UI verification. Record the
  completed checklist as verification evidence in
  `architecture/slice-verification/<slice-id>-<slice-name>.md`
  (engineering workflow Step 6b) — not scattered across `ai-parts/`.
- if `e2e_verify` is present in the PART_SPEC, those commands must be
  executed and pass.
If a gate fails:
- fix it now, or
- roll back and propose a smaller Part/spike.

---

## Required completion report — the Part Quality Report
At the end of **every** Part, produce the **Part Quality Report** using
`ai/templates/code-quality-checklist-template.md`, and write it to:

- `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`

(create the `reviews/` folder if missing). Also output it in the response.
A Part without a completed quality report is **not done**. Every field is
required — write "none" or "N/A — <reason>" rather than omitting a field.

The report must contain (see the template for the full structure):

1. **Part executed** — what was implemented
2. **Files changed** — every file, with change type and purpose
3. **Tests added or updated** — plus TDD evidence: red observed (command +
   exact failure) and green achieved (command + result)
4. **Checks run** — every command actually executed (`verify`, `e2e_verify`,
   build, linters) with its real result — not intentions
5. **Architecture rules verified** — boundaries touched, dependency direction,
   ADRs applied (cited by number), architecture-test coverage of new boundaries
6. **Existing patterns followed** — the nearby files read in step 3 and the
   patterns adopted
7. **Contract surfaces** — public API / database-schema / events / UI
   behavior, each explicitly CHANGED (with details + spec reference) or
   UNCHANGED
8. **Dependencies** — new libraries added: NONE or name + justification
9. **Deviations from existing patterns** — each with the reason, or "none"
10. **Remaining risks** — or "none" only if true
11. **Prohibited-output check** — PASS/FAIL lines per code-quality standard §11
12. **Verdict** — explicit **Part status: DONE / NOT DONE** statement

Additionally include (from the PART_SPEC):
- **Acceptance criteria check** — each criterion: PASS/FAIL
- **Feature spec alignment** — in scope / contracts respected / security &
  observability respected: PASS/FAIL each
- **Integrated UI verification** (when applicable) — app started, flow
  verified in browser, cross-slice regression, responsive check, E2E tests:
  YES/NO/N-A each
- **Rollback** (commands/steps from PART_SPEC)
- **Next part** (name only; do not start it)

After delivering the report, the Part goes to the Part code review
(engineering workflow Step 6a, `ai/prompts/code-quality-reviewer.md`). Do not
start the next Part until the review verdict is `APPROVED` or
`APPROVED WITH NOTES`.

---

## Example prompts this skill expects
- “Execute this Part only using PART_SPEC and strict TDD: <paste Part>”
- “Consume the PART_SPEC below and implement it; then give the completion report.”


## When a Feature Spec Exists

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
