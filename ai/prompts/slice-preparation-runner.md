# Slice Preparation Runner Prompt

Act as a **Product Engineer, Solution Architect, Delivery Spec Author, and
Principal Software Architect**.

## Objective

Run engineering workflow **Steps 2 through 5** for exactly one slice, in
order, producing every artifact the slice needs before implementation. **Stop
before Step 6 (TDD execution) — write no implementation code.**

This prompt exists so one agent session can prepare a slice end-to-end without
the user re-explaining the process each time. It follows
`ai/workflows/engineering-workflow.md`; that file's step numbers and output
paths are canonical.

## Inputs

- `architecture/delivery-plan.md` (must exist and be verticality-validated —
  if it does not exist, stop and run Step 1 first)
- `architecture/architecture-final.md` and `architecture/adr/*.md`
  (authoritative)
- `architecture/design-system.md` (when present — authoritative for UI)
- `ai/project-context.md`
- The **selected slice ID** (from the user; if not given, propose the next
  ready slice from the delivery plan and ask for confirmation)
- The **most recent completed slice**, used as structural precedent (find it
  via the delivery plan status or existing files in
  `architecture/feature-specs/` and `ai-parts/`)

## Hard rules for this run

- Follow the working rules in `.github/copilot-instructions.md` and
  `CLAUDE.md`: vertical slices, modular monolith, and **do not introduce new
  architecture** — surface any such need as a compliance finding or open
  question instead of deciding it.
- Do not skip steps. Do not reorder steps. Do not proceed to Step 6.
- Verify claims **against repo reality** — check the code; do not assume.
- Use the precedent slice's artifacts as the structural model for every
  artifact you produce (same section depth, same naming pattern).

## Procedure

### Step 2 — Confirm slice selection

State the selected slice explicitly (ID, name, milestone, bounded context, as
defined in the delivery plan). Confirm it satisfies the selection criteria:
meaningful, bounded, implementation-ready, dependency-clear.

For every dependency the delivery plan declares for this slice, verify **in
the codebase** that the dependency is actually implemented (name the files or
types that prove it). If any dependency is not present, **stop and report it**
— do not widen this slice to build the missing dependency.

### Step 3 — Generate the feature spec

Use `ai/prompts/feature-spec-generator.md` with
`ai/templates/feature-spec-template.md`.

Write `architecture/feature-specs/<slice-id>-<slice-name>.md`.

### Step 3b — Golden dataset (conditional)

If the slice contains AI decision paths or business rules with financial,
compliance, or safety impact, use `ai/prompts/golden-dataset-generator.md` and
write to `architecture/golden-datasets/`. Otherwise state why it is skipped.

### Step 4 — Architecture compliance check

Use `ai/prompts/architecture-compliance.md` with
`ai/templates/compliance-report-template.md`.

Write `architecture/compliance-reports/<slice-id>-<slice-name>.md`.

### Step 4a — UI compliance check (mandatory for UI slices)

If the feature spec §5b identifies human workflow surfaces, use
`ai/prompts/ui-compliance-check.md` (full check when a design system exists,
reduced check otherwise).

Write `architecture/compliance-reports/<slice-id>-<slice-name>-ui.md`.

### Step 4b — Reconcile the feature spec (if findings)

If either compliance report contains findings requiring spec changes, use
`ai/prompts/feature-spec-reconciler.md` (or the quick version for 1–3 narrow
findings) and update the feature spec file in place. If no findings require
changes, state that explicitly.

### Step 5 — Decompose the slice

Use `.github/skills/plan-decomposer/SKILL.md` with the (reconciled) feature
spec as primary input.

Write `ai-parts/<slice-id>/OVERVIEW.md` and `ai-parts/<slice-id>/PXX-*.md`.

For a UI slice, the final Part must be the Terminal Verification Part required
by the skill.

### Stop

Do not execute any Part. End the run with the summary below.

## Required completion summary

- **Slice:** ID + name
- **Dependencies verified:** each dependency → the file/type that proves it
- **Artifacts produced:** every file written, with its path
- **Compliance status:** approval status of each report; Critical findings and
  how the spec was reconciled
- **Open questions:** anything that blocks execution readiness
- **Next action:** "Execute `ai-parts/<slice-id>/P01-*.md` with
  `part-executor-tdd`" (or the blocking question to resolve first)

## References

- Engineering workflow (canonical steps): `ai/workflows/engineering-workflow.md`
- How feature specs are used: `ai/guides/how-feature-specs-are-used.md`
- Glossary: `ai/guides/glossary.md`
