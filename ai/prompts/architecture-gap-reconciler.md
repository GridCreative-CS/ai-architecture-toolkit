# Architecture Gap Reconciler Prompt

Act as the **Lead Enterprise Architect responsible for the final architecture
specification**.

## Inputs

- existing architecture document
- architecture review report

There is no prototype in this mode.

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

- do not expand the architecture beyond what the review identified — this is
  reconciliation, not redesign
- do not remove sections that the review did not flag
- limit new content to filling identified gaps and resolving identified issues

## Required Behavior

- do not merely summarize
- do not simply merge documents
- make decisions explicit
- fill missing but necessary architectural sections
- remove duplication
- preserve a single coherent architecture narrative

## Output

Write to:

- `architecture/architecture-final.md`

## References

- Blueprint template: `ai/templates/architecture-blueprint-template.md`
- Glossary: `ai/guides/glossary.md`
