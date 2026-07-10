# Architecture Workflow — Legacy System Replacement (Mode D)

Use this workflow when you have:

- a legacy system
- no trustworthy running prototype
- no existing architecture document worth validating
- a goal of designing and building a new replacement system rather than
  repairing the legacy implementation

If any required input is missing, see "Missing inputs" in
`ai/workflows/architecture-workflow.md`.

## Step 1 — Project context

Create or update project context using:

- `ai/templates/project-context-template.md`

Output:

- `ai/project-context.md` (filled in). For Mode D, record explicitly: which
  legacy behaviors and integrations must be preserved, which are consciously
  dropped, and any migration/coexistence constraints (data migration, parallel
  run, cutover expectations).

## Step 2 — Legacy system analysis

Use:

- `ai/prompts/legacy-system-analyzer.md`

Output:

- `architecture/legacy-system-analysis.md`

## Step 3 — Architecture design

Use:

- `ai/prompts/architecture-designer.md`
- `ai/templates/architecture-blueprint-template.md`

Input:

- `architecture/legacy-system-analysis.md` (in place of a prototype analysis —
  treat it as reference intent and constraints, not reference architecture)

Output:

- `architecture/architecture-blueprint.md`

The blueprint must address the replacement-specific concerns captured in the
legacy analysis: external integration compatibility, data migration ownership,
and any coexistence/cutover constraints from the project context.

## Step 4 — Architecture review

Use:

- `ai/prompts/architecture-reviewer.md`

Output:

- `architecture/review-report.md`

The review must verify that every High-priority constraint from
`architecture/legacy-system-analysis.md` is either honored by the blueprint or
explicitly and deliberately dropped with rationale.

## Step 5 — Architecture reconciliation

Use:

- `ai/prompts/architecture-reconciler.md`

Output:

- `architecture/architecture-final.md`

## Step 6 — ADR generation

Use:

- `ai/prompts/adr-generator.md`
- `ai/templates/adr-template.md`

Output:

- `architecture/adr/*.md`

For Mode D, expect ADRs covering at least: the replacement strategy (big-bang
vs incremental cutover), data migration approach, and how legacy integrations
are preserved or replaced.

## Done — finalization gate

`architecture/architecture-final.md` and `architecture/adr/*.md` now exist and
are authoritative. Continue per "After the gate" in
`ai/workflows/architecture-workflow.md`: UI foundation first for UI-inclusive
projects, then `ai/workflows/engineering-workflow.md` Step 1 (delivery
planning). In Mode D, the delivery plan should sequence slices so that
legacy-compatibility risks (integrations, data migration) are proven early.
