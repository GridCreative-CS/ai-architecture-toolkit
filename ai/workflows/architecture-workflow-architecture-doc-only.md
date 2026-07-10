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

## Step 3b — Architecture-final quality gate

Use (in a fresh agent session/subagent — never the session that wrote the
final document):

- `ai/prompts/architecture-final-quality-gate.md`

Output:

- `architecture/architecture-final-gate.md`

Verdict `APPROVED` or `APPROVED WITH NOTES` → proceed to Step 4.
Verdict `REJECTED — MUST FIX` → return to Step 3 with the gate report as
additional input, then re-run this gate on the revised document.

## Step 4 — ADR generation

Use:

- `ai/prompts/adr-generator.md`
- `ai/templates/adr-template.md`

Output:

- `architecture/adr/*.md`

## Done — finalization gate

`architecture/architecture-final.md` (gate verdict `APPROVED` or
`APPROVED WITH NOTES` in `architecture/architecture-final-gate.md`) and
`architecture/adr/*.md` now exist and are authoritative. Continue per "After the gate" in
`ai/workflows/architecture-workflow.md`: UI foundation first for UI-inclusive
projects, then `ai/workflows/engineering-workflow.md` Step 1 (delivery
planning).
