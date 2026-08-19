# Architecture-Final Quality Gate Report

The quality gate review of `architecture/architecture-final.md`, produced by
`ai/prompts/architecture-final-quality-gate.md` in a fresh agent session
after reconciliation and before ADR generation.

## Expected Structure

- Document reviewed, date, reviewer
- Verdict: `APPROVED` | `APPROVED WITH NOTES` | `REJECTED — MUST FIX`
- Check results — one row per gate check with PASS/FAIL/N/A and evidence
- Findings — each with a concrete required fix
- Notes (APPROVED WITH NOTES only) — what is open and where it is resolved
- Verdict justification

The exact output skeleton (heading, metadata bullets, and both tables) is
defined in the `## Output` section of the prompt; see
`ai/examples/example-architecture-final-gate-report.md` for the expected shape
and depth of a filled report.

A `REJECTED — MUST FIX` verdict returns the document to the mode's
reconciliation step; ADR generation must not start until the verdict is
`APPROVED` or `APPROVED WITH NOTES`.
