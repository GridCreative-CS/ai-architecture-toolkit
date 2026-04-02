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
4. **UI compliance check** — optionally run `ai/prompts/ui-compliance-check.md`
   per slice to verify design system conformance.

## Step 4 — Evolve the Design System

The design system is a living document. Update it when:

- a new slice needs a component not yet in the catalog
- user testing reveals needed changes to tokens or patterns
- accessibility testing identifies baseline gaps

To add a component: add it to `architecture/design-system.md` first, then
implement it in the slice. Do not create ad-hoc components.

## Flow Summary

```text
Architecture Finalization
  → architecture-final.md ★
  → adr/*.md
  ↓
UI Foundation (this workflow)
  → design-system.md
  ↓
Delivery Planning
  → delivery-plan.md (informed by design-system.md)
  ↓
Slice Execution Loop (engineering-workflow.md)
  → Feature Spec (with §11b UI/UX acceptance criteria)
  → Compliance Check (with Design System dimension)
  → Decompose → TDD Execute
  → UI Compliance Check (optional per slice)
  → Next Slice
```

## References

- Design system generator: `ai/prompts/design-system-generator.md`
- Design system template: `ai/templates/design-system-template.md`
- Engineering workflow: `ai/workflows/engineering-workflow.md`
- UI compliance check: `ai/prompts/ui-compliance-check.md`
- Feature spec template: `ai/templates/feature-spec-template.md`
- Glossary: `ai/guides/glossary.md`
