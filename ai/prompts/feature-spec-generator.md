# Feature Spec Generator Prompt

Act as a **Product Engineer, Solution Architect, and Delivery Spec Author**.

## Objective

Generate a feature specification for one slice or capability.

## Inputs

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- `architecture/design-system.md` (when present)

## Output

Write each feature spec under:

- `architecture/feature-specs/`

## Required Sections

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
11b. UI/UX Acceptance Criteria
12. Test Implications
13. Open Questions / Assumptions

## Section 5b Instructions

Section 5b (Human Workflow Surfaces) is **mandatory**. If the architecture
defines human interaction for this capability and the spec omits a UI surface,
flag it as a gap. If the slice is purely automated with no human interaction,
cite the architecture or ADR that confirms this.

## Section 11b Instructions

Section 11b (UI/UX Acceptance Criteria) is **mandatory** when
`architecture/design-system.md` exists. It must specify:

- which design system components the slice uses
- which design tokens apply (colors, typography, spacing)
- which layout patterns are followed
- that all four states (loading, success, error, empty) use design system
  state patterns
- that the accessibility baseline from the design system is met

If no design system exists, state "N/A — no design system adopted for this
project."

## Acceptance Criteria Guidance

Acceptance criteria in section 11 must be:

- **Binary** — each criterion is either met or not met (no subjective judgment)
- **Testable** — each criterion maps to at least one automated or manual test
- **Specific** — no vague language like "should work correctly" or "handles
  errors gracefully"

## Decomposition Readiness

A feature spec is decomposition-ready when (see `ai/guides/glossary.md`):

- scope is bounded (Scope In / Scope Out are explicit)
- acceptance criteria are binary
- target files or modules are identifiable from the spec
- no unresolved architectural unknowns remain in Open Questions
- a verification strategy is clear from Test Implications

If the spec is not yet decomposition-ready, note the blockers in section 13.

## Rules

- one feature spec per slice — do not combine multiple slices
- reference contracts from `ai/guides/contract-definition.md` when defining
  section 7
- do not widen scope beyond what the delivery plan assigns to this slice

## References

- Feature spec template: `ai/templates/feature-spec-template.md`
- Contract definition: `ai/guides/contract-definition.md`
- Glossary: `ai/guides/glossary.md`
