# Architecture Workflow — Architecture Document Only (Mode C)

Use this workflow when you have:

- an existing architecture document
- no prototype

If any required input is missing, see "Missing inputs" in
`ai/workflows/architecture-workflow.md`.

## Step 1 — Project context

Create or update project context using:

- `ai/templates/project-context-template.md`

Output:

- `ai/project-context.md` (filled in)

## Step 2 — Existing architecture review

Use:

- `ai/prompts/existing-architecture-reviewer.md`

Output:

- `architecture/existing-architecture-review.md`

## Step 3 — Architecture gap reconciliation

Use:

- `ai/prompts/architecture-gap-reconciler.md`

Inputs:

- the existing architecture document
- `architecture/existing-architecture-review.md`

Output:

- `architecture/architecture-final.md`

## Step 4 — ADR generation

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
