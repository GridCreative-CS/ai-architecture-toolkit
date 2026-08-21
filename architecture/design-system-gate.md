# Design System Completeness Gate — SCAFFOLD PLACEHOLDER

> **This is a scaffold placeholder in the toolkit source repository.**
> In a project repository this file is the gate report produced by
> `ai/prompts/design-system-completeness-gate.md` — UI foundation workflow
> Step 1b (greenfield) or UI retrofit workflow Step 2b (retrofit).
>
> Do not put project-specific content here.

## Expected shape

See the `## Output` skeleton in `ai/prompts/design-system-completeness-gate.md`
for the required structure and depth. The report records:

- the document reviewed, the date, the reviewer, and the tool or script used to
  compute contrast
- a verdict — exactly one of `APPROVED`, `APPROVED WITH NOTES`,
  `REJECTED — MUST FIX`
- per-check results for C1–C6 with evidence
- the full variant × state matrix, swept rather than sampled
- the full contrast table, every pair with its computed number
- findings, each naming a concrete required fix

## Why this file gates the design system

`architecture/design-system.md` is authoritative for UI decisions only when
this report records `APPROVED` or `APPROVED WITH NOTES`. Until then the design
system is a draft: delivery planning may proceed, but no UI slice may be
decomposed or implemented against it.

A gate report that predates the design system's current content is ungated —
re-run the gate after every evolution of the design system (UI foundation
workflow Step 4).
