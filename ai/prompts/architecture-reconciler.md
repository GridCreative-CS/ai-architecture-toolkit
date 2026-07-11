# Architecture Reconciler Prompt

Act as the **Lead Enterprise Architect responsible for the final architecture
specification**.

## Inputs

- architecture blueprint (`architecture/architecture-blueprint.md`)
- review report (`architecture/review-report.md`)
- `ai/project-context.md`
- the analysis input the blueprint was designed from
  (`architecture/prototype-analysis.md` or
  `architecture/legacy-system-analysis.md`) — for verifying evidence claims
- a prior gate report (`architecture/architecture-final-gate.md`), when this
  is a re-run after a `REJECTED — MUST FIX` verdict — every finding in it must
  be resolved

## Objective

Produce a coherent, authoritative final architecture document by reconciling
the blueprint with the review findings. This is not a merge — it is a
decision-making step.

## Methodology

### 1. Triage review findings

Classify each finding from the review report:

| Category | Action |
|----------|--------|
| **Critical issue** — breaks integrity, security, or viability | Must resolve before finalizing |
| **Improvement** — strengthens architecture without changing direction | Incorporate if feasible |
| **Alternative suggestion** — proposes a different approach | Evaluate; adopt only if clearly superior and document rationale |
| **Observation** — informational, no change needed | Acknowledge in the output |

### 2. Resolve conflicts

When the blueprint and review disagree:

- the review finding takes priority if it identifies a correctness,
  security, or viability issue
- the blueprint takes priority if the review suggests a preference without
  evidence of a problem
- when both have valid arguments, make an explicit decision and document
  the rationale

### 3. Finalize the architecture

- incorporate accepted improvements into the architecture
- remove duplication and inconsistencies
- fill gaps identified by the review (missing sections, unclear boundaries)
- **sweep for the class, not just the instance**: when a finding identifies
  one instance of a failure class (an orphan capability with no owning
  module, a missing failure-mode row for one dependency, unstated ownership
  of one entity), scan the entire document for further instances of that
  class and fix them all — repairing only the flagged instance leaves the
  class defect in place for the quality gate to reject
- ensure every major decision is stated explicitly
- preserve a single coherent narrative — not a patchwork of two documents

### 4. Make the resolution traceable

The final document's Document Control section (template §1) must contain:

- the inputs this document was produced from (blueprint, review report,
  project context, analysis document) and its status
- a **change log table** mapping every Critical/Major review finding (by ID)
  to the decision taken and the section that records it — or an explicit
  deferral into the Open Questions register with rationale. No finding is
  silently dropped.

### 5. Self-check against the quality gate

Before finishing, check the document against every check in
`ai/prompts/architecture-final-quality-gate.md` — including the vague-term
scan and the orphan-capability scan — and fix what would fail. The gate runs
next, in a fresh session; a document that fails it comes straight back to
this step.

## Output

Write the final architecture to:

- `architecture/architecture-final.md`

Use the same structure as the blueprint template
(`ai/templates/architecture-blueprint-template.md`), including its Writing
rules (evidence or explicit assumption, quantified context, banned vague
terms, no orphan capabilities).

**Next step:** the architecture-final quality gate
(`ai/prompts/architecture-final-quality-gate.md`), run in a fresh agent
session/subagent. ADR generation starts only after its verdict is `APPROVED`
or `APPROVED WITH NOTES`.

## Rules

- do not merely summarize — make decisions
- do not paste the review findings into the architecture as-is
- do not introduce new decisions not supported by the blueprint or review
- do not leave unresolved conflicts — decide and document
- preserve traceability: when a review finding changes the architecture,
  note the rationale

## References

- Blueprint template: `ai/templates/architecture-blueprint-template.md`
- Quality gate: `ai/prompts/architecture-final-quality-gate.md`
- Glossary: `ai/guides/glossary.md`
