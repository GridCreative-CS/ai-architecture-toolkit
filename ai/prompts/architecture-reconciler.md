# Architecture Reconciler Prompt

Act as the **Lead Enterprise Architect responsible for the final architecture
specification**.

## Inputs

- architecture blueprint (`architecture/architecture-blueprint.md`)
- review report (`architecture/review-report.md`)

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
- ensure every major decision is stated explicitly
- preserve a single coherent narrative — not a patchwork of two documents

## Output

Write the final architecture to:

- `architecture/architecture-final.md`

Use the same structure as the blueprint template
(`ai/templates/architecture-blueprint-template.md`).

## Rules

- do not merely summarize — make decisions
- do not paste the review findings into the architecture as-is
- do not introduce new decisions not supported by the blueprint or review
- do not leave unresolved conflicts — decide and document
- preserve traceability: when a review finding changes the architecture,
  note the rationale

## References

- Blueprint template: `ai/templates/architecture-blueprint-template.md`
- Glossary: `ai/guides/glossary.md`
