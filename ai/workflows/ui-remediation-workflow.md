# UI Remediation Workflow

## Purpose

This workflow revalidates and fixes slices that were completed under an
older toolkit version that lacked mandatory browser-based UI verification.
It brings an existing project up to the current toolkit standard before
resuming normal slice execution.

This workflow addresses **functional correctness** — does the UI actually
work in the running application? For visual consistency and design system
adoption, use `ai/workflows/ui-retrofit-workflow.md` after remediation.

## Prerequisites

- at least one slice has been implemented with UI surfaces
- `architecture/architecture-final.md` exists
- `architecture/delivery-plan.md` exists
- `architecture/feature-specs/*.md` exist for completed slices

## When to Use

Use this workflow when:

- slices were marked "done" but UI was never verified in a running application
- browser-level errors, broken layouts, non-functional interactions, or
  integration issues were discovered after slices were supposedly complete
- the project was built with a toolkit version that allowed completing UI
  slices without browser verification

Do NOT use this workflow when:

- the project has no UI (API-only, batch processing)
- UI issues are purely visual/cosmetic (use the retrofit workflow instead)
- the project has not yet started implementation

## Step 1 — Audit Completed Slices

For each completed slice with human workflow surfaces:

1. Start the full application.
2. Walk through the user flow described in the slice's feature spec §5/§5b.
3. Run the Slice Completion Verification Checklist
   (`ai/templates/slice-verification-checklist-template.md`).
4. Record the status of each checklist item.

Write:

- `architecture/remediation-audit.md` (summary of all slice statuses)

### Audit Format

| Slice | Feature Spec | Has UI | Functional Status | Critical Issues | Degraded Issues | Cosmetic Issues |
|-------|-------------|--------|-------------------|-----------------|-----------------|-----------------|
| | | | PASS / FAIL / PARTIAL | count | count | count |

## Step 2 — Triage Findings

Classify each issue:

| Severity | Definition | Action |
|----------|------------|--------|
| **Blocking** | Flow does not complete, page crashes, critical interaction broken, data loss | Fix immediately before any new slices |
| **Degraded** | Layout broken at a viewport, states not handled, shared layout incorrect, navigation broken | Fix before new slices |
| **Cosmetic** | Spacing inconsistency, minor visual glitch, missing polish | Batch into retrofit phase or fix opportunistically |

## Step 3 — Create Remediation Specs

For each slice with blocking or degraded issues, create a remediation spec:

Use:

- `ai/templates/remediation-spec-template.md`

Inputs:

- `architecture/remediation-audit.md`
- `architecture/feature-specs/<slice-id>-<slice-name>.md`
- the checklist results for this slice

Write:

- `architecture/feature-specs/remediation-<slice-name>.md`

### Scope rule

Remediation specs fix identified issues only. No refactoring, no redesign,
no new features. Do not combine functional fixes with visual/design-system
migration.

## Step 4 — Execute Remediation Slices

Each remediation slice executes through the standard engineering workflow
(`ai/workflows/engineering-workflow.md`) with mandatory Step 6b:

1. Use the remediation spec as the feature spec for decomposition
2. Decompose with `plan-decomposer`
3. Execute parts with `part-executor-tdd`
4. **Mandatory:** Run Step 6b (Integrated Slice Verification) with full checklist
5. Add E2E browser tests for every remediated flow to prevent regression

### No behavioral changes

Remediation Parts fix UI integration issues. They do not change backend
behavior, API contracts, or business logic.

## Step 5 — Verify Cross-Slice Integration

After all remediation slices are complete:

1. Start the full application
2. Walk through every completed slice's user flow
3. Verify cross-slice navigation works end-to-end
4. Verify shared layout renders correctly across all slices
5. Run all E2E browser tests

Write:

- Updated `architecture/remediation-audit.md` with final pass/fail status

## Step 6 — Resume Normal Execution

After remediation, resume the standard engineering workflow. All future slices
must follow the improved workflow with mandatory Step 6b.

If visual consistency is also needed, proceed to
`ai/workflows/ui-retrofit-workflow.md`.

## Flow Summary

```text
Existing Project (slices completed under weaker toolkit)
  → Step 1: Audit Completed Slices → architecture/remediation-audit.md
  → Step 2: Triage (blocking / degraded / cosmetic)
  → Step 3: Create Remediation Specs → architecture/feature-specs/remediation-<slice>.md
  → Step 4: Execute Remediation Slices (standard workflow + mandatory Step 6b + E2E tests)
  → Step 5: Cross-Slice Integration Verification → updated remediation-audit.md
  → Step 6: Resume Normal Execution (optional: proceed to ui-retrofit-workflow.md)
```

## References

- Slice verification checklist: `ai/templates/slice-verification-checklist-template.md`
- Remediation spec template: `ai/templates/remediation-spec-template.md`
- Engineering workflow: `ai/workflows/engineering-workflow.md`
- UI retrofit workflow: `ai/workflows/ui-retrofit-workflow.md`
- Definition of Ready/Done: `ai/guides/definition-of-ready-and-done.md`
- Glossary: `ai/guides/glossary.md`
