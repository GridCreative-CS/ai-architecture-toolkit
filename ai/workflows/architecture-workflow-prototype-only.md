# Architecture Workflow — Prototype Only

Use this workflow when you have:

- a prototype
- no existing architecture document worth validating

## Step 1 — Project context
Create or update project context using:

- `ai/templates/project-context-template.md`

## Step 2 — Prototype analysis
Use:

- `ai/prompts/prototype-analyzer.md`

Output:
- prototype analysis notes
- extracted domain and workflow insights

## Step 3 — Architecture design
Use:

- `ai/prompts/architecture-designer.md`
- `ai/templates/architecture-blueprint-template.md`

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
