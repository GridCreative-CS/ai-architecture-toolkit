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

Write each feature spec to:

- `architecture/feature-specs/<slice-id>-<slice-name>.md` — slice ID from the
  delivery plan plus the slice name in kebab-case (e.g.,
  `S2.6-structured-mse-session-comparison.md`)

Use the structure from `ai/templates/feature-spec-template.md`. For a
**phase** (not a slice), use the same template with §5b, §11b, and §12b marked
"N/A — phase, no human workflow surfaces" and name the file
`phase-<id>-<name>.md`.

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
12b. Browser Verification Steps
13. Open Questions / Assumptions

For criteria in §6, §9, §11, and §11b, emit a stable append-only identifier:
`DR-nn`, `SEC-nn`, `AC-nn`, or `UIAC-nn` respectively. Assign IDs once, never
renumber or reuse them, and keep a withdrawn criterion in place as
`WITHDRAWN — <reason>`.

## Section 5b Instructions

Section 5b (Human Workflow Surfaces) is **mandatory**. If the architecture
defines human interaction for this capability and the spec omits a UI surface,
flag it as a gap. If the slice is purely automated with no human interaction,
cite the architecture or ADR that confirms this.

## Section 6 and Section 9 Instructions

Prefix every domain rule with the next free `DR-nn` ID and every
security/authorization constraint with the next free `SEC-nn` ID. Make each
rule binary and testable, and do not reuse an ID from an earlier version of the
spec.

## Section 11b Instructions

Section 11b (UI/UX Acceptance Criteria) is **mandatory** for every slice
with human workflow surfaces identified in §5b, regardless of whether
`architecture/design-system.md` exists.

When a design system exists, §11b must specify:

- which design system components the slice uses
- which design tokens apply (colors, typography, spacing)
- which layout patterns are followed
- that all four states (loading, success, error, empty) use design system
  state patterns
- that the accessibility baseline from the design system is met

When no design system exists, §11b must specify:

- which screens/pages are involved and their expected structure
- which interactive elements exist and their expected behavior
- that all four states are handled (loading, success, error, empty)
- expected responsive behavior (minimum: mobile and desktop viewports)
- accessibility baseline (semantic HTML, keyboard navigation, labels,
  sufficient contrast)

Prefix every criterion in §11b with the next free `UIAC-nn` ID.

If the slice has no human workflow surfaces and §5b confirms this with an
architecture or ADR citation, state: "N/A — this slice has no human workflow
surfaces per [citation]."

## Acceptance Criteria Guidance

Acceptance criteria in section 11 must be:

- **Binary** — each criterion is either met or not met (no subjective judgment)
- **Testable** — each criterion maps to at least one automated or manual test
- **Specific** — no vague language like "should work correctly" or "handles
  errors gracefully"
- **Stable** — prefix each criterion with the next free `AC-nn` ID; IDs are
  append-only, never renumbered or reused

## Decomposition Readiness

A feature spec is decomposition-ready when (see `ai/guides/glossary.md`):

- scope is bounded (Scope In / Scope Out are explicit)
- acceptance criteria are binary
- target files or modules are identifiable from the spec
- no unresolved architectural unknowns remain in Open Questions
- a verification strategy is clear from Test Implications

If the spec is not yet decomposition-ready, note the blockers in section 13.

## Section 12b Instructions

Section 12b (Browser Verification Steps) is **mandatory** for every slice
with human workflow surfaces. It must describe:

- how to start the application
- the URL and steps to exercise the primary user flow in a browser
- expected visible outcomes at each step
- viewport sizes to verify (minimum: mobile and desktop)
- cross-slice navigation checks

This section becomes the input for the Integrated Slice Verification step
(engineering workflow Step 6b) and the terminal verification Part during
decomposition.

If the slice has no human workflow surfaces, state:
"N/A — this slice has no human workflow surfaces."

## Rules

- one feature spec per slice — do not combine multiple slices
- reference contracts from `ai/guides/contract-definition.md` when defining
  section 7
- do not widen scope beyond what the delivery plan assigns to this slice

## References

- Feature spec template: `ai/templates/feature-spec-template.md`
- Contract definition: `ai/guides/contract-definition.md`
- Glossary: `ai/guides/glossary.md`
