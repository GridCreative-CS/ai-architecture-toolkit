# Architecture Gap Reconciler Prompt

Act as the **Lead Enterprise Architect responsible for the final architecture
specification**.

## Inputs

- existing architecture document
- architecture review report (`architecture/existing-architecture-review.md`)
- **Mode B only:** the alignment report
  (`architecture/prototype-architecture-alignment.md`) — treat its Critical
  and Important findings as gaps/inconsistencies to reconcile, with the
  prototype evidence weighed as behavioral fact
- `ai/project-context.md`
- a prior gate report (`architecture/architecture-final-gate.md`), when this
  is a re-run after a `REJECTED — MUST FIX` verdict — every finding in it must
  be resolved

In Mode C there is no prototype; work from the document and review alone.

## Objective

Reconcile review findings into the existing architecture to produce a
coherent, authoritative final architecture document.

## Methodology

### 1. Triage findings

Classify each review finding before acting on it:

| Category | Action |
|----------|--------|
| **Gap** — missing section, undefined boundary, absent rationale | Fill with explicit content; do not leave as a TODO |
| **Inconsistency** — contradicts another section | Resolve in favor of the architecturally sounder position; document rationale |
| **Weakness** — rationale exists but is thin or unconvincing | Strengthen with additional context, constraints, or alternatives considered |
| **Observation** — informational, no change needed | Acknowledge; do not force a rewrite |

### 2. Resolve conflicts

When the existing architecture and the review disagree:

- if the review identifies a correctness, security, or viability issue, the
  review finding takes priority
- if the review suggests a preference without evidence of a problem, preserve
  the existing architecture
- in ambiguous cases, make an explicit decision and document why

### 3. Scope limits

- do not expand the architecture beyond what the review, the alignment report,
  or a gate report identified — this is reconciliation, not redesign
- content areas required by `ai/prompts/architecture-final-quality-gate.md`
  that the document lacks count as identified gaps (the reviewer checks
  completeness against the gate) — filling them is in scope
- do not remove sections that the review did not flag
- limit new content to filling identified gaps and resolving identified issues

### 4. Make the resolution traceable

The final document must state (in a Document Control section or equivalent):

- the inputs it was produced from (existing document, review, alignment
  report, project context) and its status
- a **change log table** mapping every Critical/Major finding (by ID) to the
  decision taken and the section that records it — or an explicit deferral
  into the Open Questions register with rationale. No finding is silently
  dropped.

### 5. Self-check against the quality gate

Before finishing, check the document against every check in
`ai/prompts/architecture-final-quality-gate.md` — including the vague-term
scan and the orphan-capability scan — and fix what would fail within the
scope limits above. The gate runs next, in a fresh session; a document that
fails it comes straight back to this step.

## Required Behavior

- do not merely summarize
- do not simply merge documents
- make decisions explicit
- fill missing but necessary architectural sections
- remove duplication
- preserve a single coherent architecture narrative
- keep the existing document's structure where it is sound — the gate judges
  content coverage, not section layout; use the blueprint template's Writing
  rules (evidence or explicit assumption, quantified context, banned vague
  terms, no orphan capabilities) for everything you write

## Output

Write to:

- `architecture/architecture-final.md`

**Next step:** the architecture-final quality gate
(`ai/prompts/architecture-final-quality-gate.md`), run in a fresh agent
session/subagent. ADR generation starts only after its verdict is `APPROVED`
or `APPROVED WITH NOTES`.

## References

- Blueprint template: `ai/templates/architecture-blueprint-template.md`
- Quality gate: `ai/prompts/architecture-final-quality-gate.md`
- Glossary: `ai/guides/glossary.md`
