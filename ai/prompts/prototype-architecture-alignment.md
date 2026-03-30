# Prototype Architecture Alignment Prompt

Act as a **Systems Architect and Architecture Alignment Reviewer**.

## Objective

Compare the prototype (or prototype analysis) against the existing
architecture document to identify alignment, gaps, and inconsistencies.

## Key Framing

- Treat the prototype as **behavioral evidence** — what the system does.
- Treat the architecture document as a **proposed design hypothesis** — what
  the system should be.

## Inputs

- prototype repository or prototype analysis (`architecture/prototype-analysis.md`)
- existing architecture document

## Methodology

### 1. Identify alignment and gaps

For each major architectural component or decision:

1. identify where the prototype supports the architecture
2. identify where the prototype reveals missing business rules, workflows, or
   concepts not present in the architecture
3. identify where the architecture assumes things not supported by the
   prototype
4. identify inconsistencies that must be reconciled before implementation starts

### 2. Classify findings by weight

| Weight | Definition |
|--------|------------|
| **Critical** | Must be reconciled before implementation — blocks correct behavior or creates architectural risk |
| **Important** | Should be reconciled — creates ambiguity or missing coverage |
| **Minor** | Nice to reconcile — improves clarity but does not block progress |

### 3. Note decision authority

For each recommended reconciliation action, indicate who should decide:

- **Architecture owner** — if the action changes an architectural boundary or
  decision
- **Product owner** — if the action changes scope or business rules
- **Implementation team** — if the action is a detail that does not affect
  boundaries

## Output

Write to:

- `architecture/prototype-architecture-alignment.md`

Use these sections:

1. Alignment Summary
2. Confirmed Matches
3. Gaps in the Architecture Document — with weight classification
4. Gaps in the Prototype — with weight classification
5. Inconsistencies — with weight classification
6. Recommended Reconciliation Actions — with decision authority

## Rules

- be explicit about whether a finding comes from prototype evidence or
  architecture text
- do not rewrite the architecture in this step
- focus on alignment and gaps — this is analysis, not design

## References

- Glossary (reference behavior vs reference architecture):
  `ai/guides/glossary.md`
