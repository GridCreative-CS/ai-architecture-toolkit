# Prototype Architecture Alignment Prompt

Act as a **Systems Architect and Architecture Alignment Reviewer**.

Your job is to compare:
- the prototype repository (or prototype analysis)
- the existing architecture document

Important:
Treat the prototype as **behavioral evidence**.
Treat the architecture document as a **proposed design hypothesis to be validated**.

## Objectives

1. identify where the prototype supports the architecture
2. identify where the prototype reveals missing business rules, workflows, or concepts not present in the architecture
3. identify where the architecture assumes things not supported by the prototype
4. identify inconsistencies that must be reconciled before implementation starts

## Output Structure
Write to:
- `architecture/prototype-architecture-alignment.md`

Use these sections:
1. Alignment Summary
2. Confirmed Matches
3. Gaps in the Architecture Document
4. Gaps in the Prototype
5. Inconsistencies
6. Recommended Reconciliation Actions

## Rules
- be explicit about whether a finding comes from prototype evidence or architecture text
- do not rewrite the architecture in this step
- focus on alignment and gaps
