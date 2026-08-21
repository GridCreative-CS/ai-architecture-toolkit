# UI Foundation Workflow — Greenfield

## Purpose

This workflow establishes a design system for new projects that incorporate
UI from the start. It runs **after architecture finalization and before (or
alongside) delivery planning**.

## Prerequisites

- `architecture/architecture-final.md` exists
- `architecture/adr/*.md` exist
- `ai/project-context.md` is filled in

## When to Use

Use this workflow when:

- starting a new project that includes human-facing UI surfaces
- the architecture specifies human-in-the-loop interactions
- no design system exists yet

Do NOT use this workflow when:

- the project has no UI (API-only, batch processing, etc.)
- a design system already exists — update it directly instead
- the project already has implemented UI surfaces — use
  `ai/workflows/ui-retrofit-workflow.md` instead

## Step 1 — Generate Design System v1

Use:

- `ai/prompts/design-system-generator.md`
- `ai/templates/design-system-template.md`

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `ai/project-context.md`

Write:

- `architecture/design-system.md`

### Scope constraint

The design system v1 should cover only what the first 2–3 slices need.
Do not attempt a comprehensive component library. The design system grows
iteratively as slices are implemented.

## Step 1b — Design System Completeness Gate

**Mandatory.** A design system is not authoritative until it passes this gate.

Use:

- `ai/prompts/design-system-completeness-gate.md`

Run it in a **fresh session** — the gate must not inherit the reasoning that
produced the document.

Write:

- `architecture/design-system-gate.md`

The gate establishes two properties, both mechanical:

1. **Renderability** — every component variant in every state is specified to
   the point where it could be drawn from the document alone, without a
   further decision.
2. **Computed conformance** — every colour pair the document specifies meets
   its contrast floor, as a calculated number rather than an assertion.

Verdict `APPROVED` or `APPROVED WITH NOTES` → proceed. `REJECTED — MUST FIX` →
fix the findings in `architecture/design-system.md` and re-run the gate. A
contrast pair below its floor is always a FAIL, never a note.

Delivery planning may proceed on a rejected design system, but **no slice may
be decomposed or implemented against it** until the gate passes.

### Why this step exists

It was added after a pilot run on a greenfield project found six findings that
survived into real changes to a design system the toolkit's own generator had
just produced — most from the enumeration sweep (three component states that
could not be drawn at all, one of which cascaded into four computed
text-contrast failures), the next largest share from computed contrast, and one
token collision. The credited mechanism is **enumeration and computation**:
sweeping every variant × state cell until each is concretely renderable, then
computing every pair. Visual inspection of a rendering was not what found them
— the pilot published a board and never looked at it — so that half remains
unevidenced.

Specify the gate accordingly — enumerate and compute, do not render and look.

### Optional — visual reference

Producing a rendered board (for example via a design canvas tool) is
**optional and carries no gate weight**. It is a communication aid, not a
verification mechanism. If one is produced, record it in design system §9 with
the document revision it reflects.

The design system document is authoritative. A visual reference is derived,
may lag, and no token value, variant, or state may exist only in it. Where the
two disagree, the document wins.

Skip freely: if no canvas tool is available — for instance when working under
different tooling — `architecture/design-system.md` plus a passing gate fully
satisfies this workflow. Nothing downstream consumes a visual reference.

## Step 2 — Proceed to Delivery Planning

Once the design system exists, proceed to delivery planning as defined in
`ai/workflows/engineering-workflow.md`.

The delivery planner can now reference `architecture/design-system.md` as an
additional input. Slices with UI surfaces should note which design system
components and tokens they will use.

## Step 3 — Integrate with Slice Execution

During slice execution, the design system integrates at these points:

1. **Feature spec generation** — each feature spec includes §11b (UI/UX
   Acceptance Criteria) referencing the design system.
2. **Architecture compliance** — the compliance check includes a Design System
   Compliance dimension.
3. **Frontend agent** — the frontend agent consumes
   `architecture/design-system.md` as an authoritative input.
4. **UI compliance check** — run `ai/prompts/ui-compliance-check.md` for every
   slice with human workflow surfaces (engineering workflow Step 4a —
   **mandatory** for UI slices) to verify design system conformance.

## Step 4 — Evolve the Design System

The design system is a living document. Update it when:

- a new slice needs a component not yet in the catalog
- user testing reveals needed changes to tokens or patterns
- accessibility testing identifies baseline gaps

To add a component: add it to `architecture/design-system.md` first, then
implement it in the slice. Do not create ad-hoc components.

**Re-run the completeness gate (Step 1b) after every evolution.** Scope it to
the components that changed — except when a token in §2a changes value, which
changes the contrast context of every pair drawn on or against it and requires
a full re-run. Update `architecture/design-system-gate.md` with the new
verdict; a design system whose gate report predates its current content is
ungated.

## Flow Summary

```text
Architecture Finalization
  → architecture-final.md ★
  → adr/*.md
  ↓
UI Foundation (this workflow)
  → design-system.md
  → design-system-gate.md ★ (Step 1b — mandatory)
  ↓
Delivery Planning
  → delivery-plan.md (informed by design-system.md)
  ↓
Slice Execution Loop (engineering-workflow.md)
  → Feature Spec (with §11b UI/UX acceptance criteria)
  → Compliance Check (with Design System dimension)
  → UI Compliance Check (mandatory for UI slices — Step 4a)
  → Decompose → TDD Execute
  → Integrated Slice Verification (mandatory for UI slices — Step 6b)
  → Next Slice
```

## References

- Design system generator: `ai/prompts/design-system-generator.md`
- Design system completeness gate: `ai/prompts/design-system-completeness-gate.md`
- Design system template: `ai/templates/design-system-template.md`
- Engineering workflow: `ai/workflows/engineering-workflow.md`
- UI compliance check: `ai/prompts/ui-compliance-check.md`
- Feature spec template: `ai/templates/feature-spec-template.md`
- Glossary: `ai/guides/glossary.md`
