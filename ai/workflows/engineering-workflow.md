# Engineering Workflow — Feature Spec Aware

## Purpose

This workflow makes feature specifications a concrete input to decomposition and
implementation.

## Step 0b — UI Foundation (Optional)

If the project includes human-facing UI and no design system exists yet:

- **Greenfield:** follow `ai/workflows/ui-foundation-workflow.md` to create
  `architecture/design-system.md` before delivery planning.
- **Retrofit:** follow `ai/workflows/ui-retrofit-workflow.md` to inventory
  existing UI and derive a design system.

This step is optional. If the project has no UI, or a design system already
exists, skip to Step 1.

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

This validation is **mandatory** for the initial delivery plan. It is optional
for subsequent slice selections once the plan has been validated.

## Step 2 — Select the Next Slice

Choose the next implementation slice from the delivery plan.

The selected slice should be:

- meaningful
- bounded
- implementation-ready
- aligned with current priorities and dependencies

## Step 3 — Generate the Feature Spec for That Slice

Use:

- `ai/prompts/feature-spec-generator.md`
- `ai/templates/feature-spec-template.md`

Write:

- `architecture/feature-specs/<slice-name>.md`

## Step 4 — Run Architecture Compliance Check

If the slice is sensitive, cross-cutting, or high-risk, use:

- `ai/prompts/architecture-compliance.md`
- `ai/templates/compliance-report-template.md`

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- `architecture/feature-specs/<slice-name>.md`

Write:

- `architecture/compliance-reports/<slice-name>.md`

## Step 4a — Run UI Compliance Check (Optional)

If `architecture/design-system.md` exists and the slice includes UI surfaces,
optionally run a UI-specific compliance check:

- `ai/prompts/ui-compliance-check.md`

Inputs:

- `architecture/design-system.md`
- `architecture/feature-specs/<slice-name>.md`
- the implemented UI code for the slice

This check verifies token usage, component usage, layout conformance, state
handling, and accessibility against the design system. Findings are classified
by severity and should be addressed before marking the slice done.

## Step 4b — Reconcile Feature Spec Against Compliance Findings

If the compliance report contains findings or required corrections, use:

- `ai/prompts/feature-spec-reconciler.md`
- `ai/prompts/feature-spec-reconciler-quickversion.md` (optional for low-severity or narrow updates)

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- `architecture/compliance-reports/<slice-name>.md`
- `architecture/feature-specs/<slice-name>.md`

Write:

- `architecture/feature-specs/<slice-name>.md` (updated)

If there are no findings that require spec changes, proceed directly to decomposition.

## Step 5 — Decompose the Slice

Use:

- `skills/plan-decomposer/SKILL.md`

Inputs:

- `architecture/delivery-plan.md`
- `architecture/feature-specs/<slice-name>.md` (reconciled when compliance findings exist)

If both exist, the feature spec should guide the decomposition for that slice
more precisely than the high-level delivery plan.

Write:

- `ai-parts/OVERVIEW.md`
- `ai-parts/PXX-*.md`

## Step 6 — Execute One Part at a Time

Use:

- `skills/part-executor-tdd/SKILL.md`

Input:

- one Part from `ai-parts/`

Execute exactly one Part at a time using strict TDD.

## Step 7 — Use Specialist Agents Where Helpful

Use specialist agents only after the slice is defined and decomposed.

Possible agents:

- backend
- frontend
- AI
- QA
- AI testing
- DevOps
- integration reviewer

## Step 8 — Repeat Per Slice

Repeat the sequence per slice:

```text
Delivery Plan
→ Select Slice
→ Feature Spec
→ Compliance Check
→ Feature Spec Reconciliation (if findings)
→ Decomposition
→ TDD Execution
→ Next Slice
```
