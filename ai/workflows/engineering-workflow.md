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

## Step 4a — Run UI Compliance Check (Mandatory for UI Slices)

If the slice includes human workflow surfaces (as identified in the feature
spec §5b), this step is **mandatory**.

When `architecture/design-system.md` exists, run the full design system
compliance check:

- `ai/prompts/ui-compliance-check.md`

When no design system exists, run a reduced UI compliance check covering:

- State handling (loading, success, error, empty)
- Layout consistency with existing slices
- Interactive element functionality
- Accessibility baseline (semantic HTML, keyboard navigation, labels)

Inputs:

- `architecture/design-system.md` (when present)
- `architecture/feature-specs/<slice-name>.md`
- the implemented UI code for the slice

This check verifies token usage, component usage, layout conformance, state
handling, and accessibility against the design system (or baseline standards
when no design system exists). Findings classified as Critical must be
resolved before marking the slice done.

Skip this step only when the slice has no human workflow surfaces and §5b
of the feature spec explicitly confirms this with an architecture citation.

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

Implementation does **not** start from `architecture/delivery-plan.md` alone.
Execution starts only after the selected slice has a concrete implementation
handoff:

- `architecture/feature-specs/<slice-name>.md`
- `ai-parts/OVERVIEW.md`
- `ai-parts/PXX-*.md`

## Step 6 — Execute One Part at a Time

Use:

- `skills/part-executor-tdd/SKILL.md`

Input:

- one Part from `ai-parts/`

Execute exactly one Part at a time using strict TDD.

This is the point where implementation begins. The delivery plan tells you
**what** to build next, the feature spec defines **one selected slice**, and the
Part file is the execution-ready artifact that tells the agent exactly what to
implement and verify.
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

- Verification evidence in the slice's completion notes (pass/fail per
  criterion, commands run, observations).

### When to skip

Skip this step only when:

- The feature spec §5b explicitly confirms no human workflow surfaces,
  citing the architecture or an ADR.
- The slice is a pure backend/data slice with no rendered UI.

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
→ Compliance Check (UI compliance mandatory for UI slices)
→ Feature Spec Reconciliation (if findings)
→ Decomposition
→ TDD Execution
→ Integrated Slice Verification (mandatory for UI slices)
→ Next Slice
```
