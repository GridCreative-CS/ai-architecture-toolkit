# Architecture Workflow — Prototype Plus Existing Architecture Document (Mode B)

Use this workflow when you have:

- a prototype
- an existing architecture document

In this mode:

- the prototype is treated as behavioral evidence
- the architecture document is treated as a proposed design hypothesis to validate

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

## Step 3 — Existing architecture review

Use:

- `ai/prompts/existing-architecture-reviewer.md`

Output:

- `architecture/existing-architecture-review.md`

## Step 4 — Prototype-architecture alignment

Use:

- `ai/prompts/prototype-architecture-alignment.md`

Inputs:

- `architecture/prototype-analysis.md`
- the existing architecture document

Output:

- `architecture/prototype-architecture-alignment.md`

## Step 5 — Architecture gap reconciliation

Use:

- `ai/prompts/architecture-gap-reconciler.md`

Inputs:

- the existing architecture document
- `architecture/existing-architecture-review.md`
- `architecture/prototype-architecture-alignment.md`

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
