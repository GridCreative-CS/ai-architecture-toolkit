# Feature Spec Generator Prompt

Act as a **Product Engineer, Solution Architect, and Delivery Spec Author**.

Generate a feature specification for one slice or capability from:

- final architecture
- ADRs
- delivery plan

Write each feature spec under:

- `architecture/feature-specs/`

Required sections:

1. Feature Name
2. Purpose
3. Scope In
4. Scope Out
5. User / System Flows
5b. Human Workflow Surfaces
6. Domain Rules
7. API / Contract Expectations
8. Data Requirements
9. Security / Authorization Constraints
10. Observability Requirements
11. Acceptance Criteria
12. Test Implications
13. Open Questions / Assumptions

## Section 5b Instructions

Section 5b (Human Workflow Surfaces) is **mandatory**. If the architecture
defines human interaction for this capability and the spec omits a UI surface,
flag it as a gap. If the slice is purely automated with no human interaction,
cite the architecture or ADR that confirms this.

Consult `ai/guides/glossary.md` for term definitions, especially: contract,
human-in-the-loop, end-to-end.
