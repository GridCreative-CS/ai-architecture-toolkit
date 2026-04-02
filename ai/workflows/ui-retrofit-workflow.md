# UI Retrofit Workflow

## Purpose

This workflow adds a unified design system to a project where vertical slices
have already been implemented without one. It provides a structured path from
UI inventory through design system derivation to behavior-preserving migration.

## Prerequisites

- `architecture/architecture-final.md` exists
- at least one slice has been implemented with UI surfaces
- no `architecture/design-system.md` exists (or it is incomplete)
- `architecture/delivery-plan.md` (when present — required for Step 3 onward,
  but Steps 1–2 can run before delivery planning)

## When to Use

Use this workflow when:

- a project has implemented slices with inconsistent or ad-hoc UI
- the team wants to introduce visual consistency without restarting
- UI debt has accumulated across multiple slices

Do NOT use this workflow when:

- starting a new project with no implemented UI — use
  `ai/workflows/ui-foundation-workflow.md` instead
- a design system already exists and is up to date

## Step 1 — Inventory Existing UI

Use:

- `ai/prompts/ui-inventory.md`
- `ai/templates/ui-inventory-template.md`

Inputs:

- the project's source code (all UI-related files)
- `architecture/architecture-final.md`
- `ai/project-context.md`

Write:

- `architecture/ui-inventory.md`

### Completeness gate

Before proceeding, verify the completeness checklist in the UI inventory
template. An incomplete inventory leads to an incomplete design system and
missed migration targets.

## Step 2 — Derive Design System from Inventory

Use:

- `ai/prompts/design-system-from-inventory.md`
- `ai/templates/design-system-template.md`

Inputs:

- `architecture/ui-inventory.md`
- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `ai/project-context.md`

Write:

- `architecture/design-system.md`

### Conflict resolution

If the inventory contains genuinely conflicting patterns, the design system
derivation prompt will flag them. Resolve conflicts before proceeding to
migration planning.

## Step 3 — Plan Retrofit Migration

> **Note:** `architecture/delivery-plan.md` must exist before this step.
> Steps 1–2 (inventory and design system derivation) can run before delivery
> planning, but migration planning requires the delivery plan and its feature
> specs.

For each existing slice with UI surfaces that do not conform to the new design
system, create a retrofit specification:

Use:

- `ai/templates/retrofit-spec-template.md`

Inputs:

- `architecture/design-system.md`
- `architecture/ui-inventory.md`
- `architecture/feature-specs/<slice>.md` (existing feature spec for the slice)

Write:

- `architecture/feature-specs/retrofit-<slice-name>.md` (one per slice)

### Prioritization

Prioritize retrofit slices by:

1. **High visibility** — screens seen by most users
2. **High inconsistency** — screens with the most anomalies per the inventory
3. **Low risk** — screens with good test coverage (safer to migrate)

## Step 4 — Execute Retrofit Slices

Each retrofit slice executes through the standard engineering workflow
(`ai/workflows/engineering-workflow.md`):

1. Use the retrofit spec as the feature spec for decomposition
2. Decompose with `plan-decomposer`
3. Execute parts with `part-executor-tdd` using **green-to-green** TDD:
   - existing tests must pass before migration (green)
   - existing tests must pass after each migration step (still green)
   - new design-system conformance tests are added
4. Run `ai/prompts/ui-compliance-check.md` after each slice

### Behavior-preserving guarantee

- No behavioral changes are bundled with styling changes in the same Part
- Token swaps happen before component replacements
- Component replacements happen before layout adjustments
- Each step is independently verifiable and revertible

## Step 5 — Lock Down

After retrofit migration is complete:

1. The Design System Compliance dimension in
   `ai/prompts/architecture-compliance.md` applies to all future slices.
2. The UI compliance check (`ai/prompts/ui-compliance-check.md`) should be
   run for every slice that includes UI changes.
3. Ad-hoc tokens and components are no longer acceptable — all UI must
   reference the design system.

## Flow Summary

```text
Existing Project (slices implemented without design system)
  ↓
Step 1: UI Inventory                          ← can run before delivery planning
  → architecture/ui-inventory.md
  ↓
Step 2: Derive Design System                  ← can run before delivery planning
  → architecture/design-system.md
  ↓
─── delivery-plan.md required from here ───
  ↓
Step 3: Plan Retrofit
  → architecture/feature-specs/retrofit-<slice>.md (one per slice)
  ↓
Step 4: Execute Retrofit Slices
  → Standard engineering workflow with green-to-green TDD
  → UI compliance check after each slice
  ↓
Step 5: Lock Down
  → Design System Compliance enforced for all future slices
  ↓
Continue with new slices (design system now available)
```

## References

- UI inventory prompt: `ai/prompts/ui-inventory.md`
- Design system from inventory: `ai/prompts/design-system-from-inventory.md`
- UI compliance check: `ai/prompts/ui-compliance-check.md`
- Retrofit spec template: `ai/templates/retrofit-spec-template.md`
- Design system template: `ai/templates/design-system-template.md`
- Engineering workflow: `ai/workflows/engineering-workflow.md`
- Glossary: `ai/guides/glossary.md`
