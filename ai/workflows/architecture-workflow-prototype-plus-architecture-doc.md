# Architecture Workflow — Prototype Plus Existing Architecture Document

Use this workflow when you have:

- a prototype
- an existing architecture document

In this mode:

- the prototype is treated as behavioral evidence
- the architecture document is treated as a proposed design hypothesis to validate

## Step 1 — Project context
Create or update project context using:

- `ai/templates/project-context-template.md`

## Step 2 — Prototype analysis
Use:

- `ai/prompts/prototype-analyzer.md`

Output:
- prototype analysis notes

## Step 3 — Existing architecture review
Use:

- `ai/prompts/existing-architecture-reviewer.md`

Output:
- `architecture/existing-architecture-review.md`

## Step 4 — Prototype-architecture alignment
Use:

- `ai/prompts/prototype-architecture-alignment.md`

Output:
- `architecture/prototype-architecture-alignment.md`

## Step 5 — Architecture gap reconciliation
Use:

- `ai/prompts/architecture-gap-reconciler.md`

Output:
- `architecture/architecture-final.md`

## Step 6 — ADR generation
Use:

- `ai/prompts/adr-generator.md`
- `ai/templates/adr-template.md`

Output:
- `architecture/adr/*.md`
