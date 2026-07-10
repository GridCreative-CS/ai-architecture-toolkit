# Architecture Workflow — Prototype Only (Mode A)

Use this workflow when you have:

- a prototype
- no existing architecture document worth validating

If any required input is missing, see "Missing inputs" in
`ai/workflows/architecture-workflow.md`.

## Step 1 — Project context

Create or update project context using:

- `ai/templates/project-context-template.md`

Output:

- `ai/project-context.md` (filled in)

## Step 2 — Prototype analysis

Use:

- `ai/prompts/prototype-analyzer.md`

Output:

- `architecture/prototype-analysis.md`

## Step 3 — Architecture design

Use:

- `ai/prompts/architecture-designer.md`
- `ai/templates/architecture-blueprint-template.md`

Input:

- `architecture/prototype-analysis.md`

Output:

- `architecture/architecture-blueprint.md`

## Step 4 — Architecture review

Use:

- `ai/prompts/architecture-reviewer.md`

Output:

- `architecture/review-report.md`

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

## Done — finalization gate

`architecture/architecture-final.md` and `architecture/adr/*.md` now exist and
are authoritative. Continue per "After the gate" in
`ai/workflows/architecture-workflow.md`: UI foundation first for UI-inclusive
projects, then `ai/workflows/engineering-workflow.md` Step 1 (delivery
planning).
