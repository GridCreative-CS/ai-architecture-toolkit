# Engineering Workflow — Feature Spec Aware

## Purpose

This workflow drives implementation slice by slice. Feature specifications are
a concrete input to decomposition and implementation — not optional
documentation.

**The step numbers in this file are canonical.** Every other toolkit file that
refers to an engineering workflow step (e.g., "Step 4a", "Step 6b") means the
numbering below. Do not renumber steps when summarizing this workflow.

## Preconditions

- `architecture/architecture-final.md` and `architecture/adr/*.md` exist as
  real content (see `ai/workflows/architecture-workflow.md`, finalization gate).
- `ai/project-context.md` is filled in.

If a precondition is missing, stop and complete the architecture phase first.
Do not improvise an architecture from the codebase.

## Naming conventions used below

- **Slice ID** — the identifier from the delivery plan (e.g., `S1.1`, `S2.6`).
  Phases use `phase-<n><letter>` (e.g., `phase-1a`).
- **Feature spec file** — `architecture/feature-specs/<slice-id>-<slice-name>.md`
  in kebab-case (e.g., `S2.6-structured-mse-session-comparison.md`).
- **Parts folder** — `ai-parts/<slice-id>/`, matching the delivery plan's
  slice ID casing exactly (e.g., `ai-parts/S2.6/`, `ai-parts/phase-1a/`).
  One folder per slice; never mix Parts from two slices in one folder.

A project may define a different consistent scheme in `ai/project-context.md`;
if it does, that scheme wins. Whatever the scheme, one slice = one spec file =
one parts folder = one compliance report set.

## Step 0b — UI Foundation (Conditional)

**Mandatory when** the project includes human-facing UI and no
`architecture/design-system.md` exists yet. **Skip when** the project has no
UI, or a design system already exists.

- **Greenfield:** follow `ai/workflows/ui-foundation-workflow.md` to create
  `architecture/design-system.md` before delivery planning.
- **Retrofit:** follow `ai/workflows/ui-retrofit-workflow.md` to inventory
  existing UI and derive a design system.

## Step 1 — Delivery Planning

Use:

- `ai/prompts/delivery-planner.md`

Write:

- `architecture/delivery-plan.md`

## Step 1b — Validate Delivery Plan Verticality

Before proceeding to slice selection, validate the delivery plan against
`ai/guides/vertical-slice-definition.md`.

For each slice in the plan, apply the verticality test:

1. Does this slice deliver a capability a user/operator can exercise or observe?
2. If the architecture specifies human-in-the-loop for this capability, does the
   slice include the minimal UI to prove that loop?
3. Can this slice be called "done" with a user-facing verification, not just an
   integration test?

If any slice is a horizontal layer (all-frontend, all-backend without human
workflow), restructure the delivery plan before proceeding.

This validation is **mandatory** for the initial delivery plan and after any
delivery plan restructuring. It does not need to be repeated at each slice
selection once the plan has been validated.

## Step 2 — Select the Next Slice

Choose the next implementation slice from the delivery plan.

The selected slice must be:

- meaningful — delivers observable value on its own
- bounded — scope fits the delivery plan entry, no bundled extras
- implementation-ready — no unresolved architectural unknowns
- dependency-clear — all declared dependencies are **verified present in the
  codebase** (check the code; do not assume). If a dependency is missing, stop
  and report it — do not silently widen the slice to build it.

> To run Steps 2–5 for one slice in a single agent session, use
> `ai/prompts/slice-preparation-runner.md`. It stops before Step 6.

## Step 3 — Generate the Feature Spec for That Slice

Use:

- `ai/prompts/feature-spec-generator.md`
- `ai/templates/feature-spec-template.md`

Write:

- `architecture/feature-specs/<slice-id>-<slice-name>.md`

## Step 3b — Golden Dataset (Conditional)

**Mandatory when** the slice contains AI decision paths (deterministic or
probabilistic) or business rules with financial, compliance, or safety impact.
**Skip otherwise.**

Use:

- `ai/prompts/golden-dataset-generator.md`
- `ai/templates/golden-dataset-template.md`

Write:

- `architecture/golden-datasets/<slice-id>-<topic>.md` (and data files as the
  format recommendation dictates)

## Step 4 — Run Architecture Compliance Check (Mandatory)

Run this check for **every slice and phase** before decomposition, at one of
two levels.

### Level selection (binary — answer all six)

Run the **full** check if ANY of the following is true; otherwise the
**lightweight** check is sufficient:

1. The slice touches authentication, authorization, consent, or secrets.
2. The slice adds or changes a contract consumed by another slice, module, or
   external system.
3. The slice changes data ownership, adds a new aggregate or table, or
   includes a schema migration.
4. The slice contains AI decision paths.
5. The slice adds a new module or a new cross-module dependency.
6. The slice is a phase, a remediation slice, or the first slice of a
   milestone.

The lightweight check covers only: module/boundary placement, the verticality
assessment, and consistency of any touched contract with the architecture. It
is defined in `ai/prompts/architecture-compliance.md` ("Lightweight mode").
Record the six answers in the report — if any answer was wrong, the full
check is owed before the slice is done.

Use:

- `ai/prompts/architecture-compliance.md`
- `ai/templates/compliance-report-template.md`

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- `architecture/feature-specs/<slice-id>-<slice-name>.md`

Write:

- `architecture/compliance-reports/<slice-id>-<slice-name>.md`

## Step 4a — Run UI Compliance Check (Mandatory for UI Slices)

If the slice includes human workflow surfaces (as identified in the feature
spec §5b), this step is **mandatory**.

When `architecture/design-system.md` exists, run the full design system
compliance check:

- `ai/prompts/ui-compliance-check.md`

When no design system exists, run the reduced UI compliance check defined in
that same prompt (state handling, layout consistency, interactive element
functionality, accessibility baseline).

Inputs:

- `architecture/design-system.md` (when present)
- `architecture/feature-specs/<slice-id>-<slice-name>.md`
- the planned or implemented UI for the slice

Write:

- `architecture/compliance-reports/<slice-id>-<slice-name>-ui.md`

Findings classified as Critical must be resolved before marking the slice done.

Skip this step only when the slice has no human workflow surfaces and §5b
of the feature spec explicitly confirms this with an architecture citation.

## Step 4b — Reconcile Feature Spec Against Compliance Findings

If either compliance report contains findings or required corrections, use:

- `ai/prompts/feature-spec-reconciler.md`
- `ai/prompts/feature-spec-reconciler-quickversion.md` (for 1–3 narrow,
  low-severity findings)

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- `architecture/compliance-reports/<slice-id>-<slice-name>.md` (and `-ui.md`)
- `architecture/feature-specs/<slice-id>-<slice-name>.md`

Write:

- `architecture/feature-specs/<slice-id>-<slice-name>.md` (updated in place)

If there are no findings that require spec changes, proceed directly to
decomposition.

## Step 5 — Decompose the Slice

Use:

- `.github/skills/plan-decomposer/SKILL.md`

Inputs:

- `architecture/feature-specs/<slice-id>-<slice-name>.md` (reconciled when
  compliance findings existed)
- `architecture/delivery-plan.md`
- `architecture/architecture-final.md`
- `architecture/adr/*.md`

The feature spec guides the decomposition for the slice more precisely than
the high-level delivery plan.

Write:

- `ai-parts/<slice-id>/OVERVIEW.md`
- `ai-parts/<slice-id>/PXX-*.md`

Implementation does **not** start from `architecture/delivery-plan.md` alone.
Execution starts only after the selected slice has a concrete implementation
handoff:

- `architecture/feature-specs/<slice-id>-<slice-name>.md`
- `ai-parts/<slice-id>/OVERVIEW.md`
- `ai-parts/<slice-id>/PXX-*.md`

## Step 6 — Execute One Part at a Time

Use:

- `.github/skills/part-executor-tdd/SKILL.md`

Input:

- one Part from `ai-parts/<slice-id>/`

Execute exactly one Part at a time using strict TDD.

This is the point where implementation begins. The delivery plan tells you
**what** to build next, the feature spec defines **one selected slice**, and the
Part file is the execution-ready artifact that tells the agent exactly what to
implement and verify.

**If implementation reality conflicts with the architecture or the feature
spec** (a boundary cannot be respected, a contract cannot be implemented as
specified), stop the Part, report the conflict as a compliance finding or open
question, and wait for a decision. Do not resolve architecture conflicts inside
a Part.

## Step 6b — Integrated Slice Verification (Mandatory for UI Slices)

After all Parts in a slice are executed, verify the slice works correctly in
the running application before proceeding.

**This step is mandatory for slices with human workflow surfaces.** For
API-only or automated slices, this step is recommended but not required.

Use:

- `ai/templates/slice-verification-checklist-template.md`

Procedure:

1. Start the full application (all services, database, frontend).
2. Execute the user flow described in the feature spec §5 and §5b end-to-end
   in a browser (or via browser-based E2E tests).
3. Walk through the Slice Completion Verification Checklist.
4. Verify all acceptance criteria from §11 and §11b against the running
   application.
5. Check that previously completed slices still render and function correctly.

If any checklist item fails, the slice is **not done**. Fix the issue and
re-verify before proceeding.

Write:

- `architecture/slice-verification/<slice-id>-<slice-name>.md` — the completed
  checklist with pass/fail per criterion, commands run, and observations. This
  file is the single location for verification evidence (do not scatter it
  across `ai-parts/`).

### When to skip

Skip this step only when:

- The feature spec §5b explicitly confirms no human workflow surfaces,
  citing the architecture or an ADR.
- The slice is a pure backend/data slice with no rendered UI.

## Step 7 — Use Specialist Agents Where Helpful

Use specialist agents only after the slice is defined and decomposed.

Possible agents (see `ai/agents/`):

- backend
- frontend (**mandatory** for UI slices)
- AI
- QA
- AI testing
- DevOps
- integration reviewer (when the slice touches cross-slice or cross-module
  boundaries)

## Step 8 — Repeat Per Slice

Repeat the sequence per slice:

```text
Delivery Plan
→ Select Slice (dependencies verified in code)
→ Feature Spec
→ Golden Dataset (when AI/critical rules)
→ Architecture Compliance Check (always)
→ UI Compliance Check (mandatory for UI slices)
→ Feature Spec Reconciliation (if findings)
→ Decomposition (ai-parts/<slice-id>/)
→ TDD Execution
→ Integrated Slice Verification (mandatory for UI slices)
→ Next Slice
```

## Executing Phases

Phases (infrastructure bootstrap, production hardening — see
`ai/guides/glossary.md`) are not slices, but they use the same machinery:

1. Write a **phase spec** using `ai/templates/feature-spec-template.md`, with
   §5b, §11b, and §12b marked "N/A — phase, no human workflow surfaces" and
   the phase labeled as a phase (not a slice).
2. Run Step 4 (architecture compliance) on the phase spec.
3. Decompose into `ai-parts/<phase-id>/` and execute with TDD (Steps 5–6).
4. Steps 1b, 4a, and 6b do not apply to phases.

## Missing-input handling

- No `architecture/delivery-plan.md` → go to Step 1; do not select slices from
  memory or from the architecture directly.
- No feature spec for the selected slice → go to Step 3; do not decompose from
  the delivery plan alone.
- No `ai-parts/<slice-id>/` handoff → go to Step 5; do not implement from the
  feature spec alone.
- No `architecture/design-system.md` on a UI slice → Step 4a runs in reduced
  mode; consider Step 0b before further UI slices.
- Feature spec exists but is not decomposition-ready (open architectural
  unknowns in §13) → resolve or escalate the unknowns first.
