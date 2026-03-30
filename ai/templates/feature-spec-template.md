# Feature Specification

<!-- This template defines ONE slice. One feature spec = one slice.            -->
<!-- If this spec covers multiple slices, split it.                            -->
<!-- Reference: ai/guides/glossary.md for term definitions.                    -->
<!-- Reference: ai/guides/how-feature-specs-are-used.md for the workflow.      -->

## 1. Feature Name

<!-- Short name matching the slice name in the delivery plan. -->

## 2. Purpose

<!-- 2–3 sentences: what capability this slice delivers and why it matters.    -->

## 3. Scope In

<!-- Bullet list of what is included in this slice.                            -->

## 4. Scope Out

<!-- Bullet list of what is explicitly excluded. This prevents scope creep.    -->

## 5. User / System Flows

<!-- Describe the end-to-end flow: who triggers it, what steps occur, what    -->
<!-- the outcome is. Include error paths.                                      -->

## 5b. Human Workflow Surfaces

<!-- MANDATORY section.                                                        -->
<!-- Answer these questions:                                                    -->
<!-- - Which UI surfaces, operator flows, approval flows, or override flows   -->
<!--   does this slice include?                                                -->
<!-- - If the slice is purely automated with no human interaction and the      -->
<!--   architecture agrees, state that explicitly with citation to the         -->
<!--   relevant ADR or architecture section.                                   -->
<!-- - If the architecture specifies human-in-the-loop for this capability    -->
<!--   but the spec omits a UI surface, flag as a gap.                        -->
<!-- Reference: glossary.md → Human-in-the-Loop tiers.                        -->

## 6. Domain Rules

<!-- List business rules that this slice must enforce. Each rule should be     -->
<!-- testable. Example: "Recommendation must include an explanation summary."  -->

## 7. API / Contract Expectations

<!-- Define the contract: request shape, response shape, error codes,         -->
<!-- idempotency, versioning.                                                  -->
<!-- Reference: ai/guides/contract-definition.md for the three contract       -->
<!-- layers (schema, behavior, non-functional).                                -->

## 8. Data Requirements

<!-- What data does this slice read? What does it write? What data does it    -->
<!-- own vs. read from another module?                                         -->

## 9. Security / Authorization Constraints

<!-- Who can access this capability? What roles or policies apply? How are    -->
<!-- secrets managed?                                                          -->

## 10. Observability Requirements

<!-- What metrics, logs, and traces are required? Reference the operational   -->
<!-- architecture for standards.                                               -->

## 11. Acceptance Criteria

<!-- Each criterion must be:                                                   -->
<!-- - Binary: met or not met (no subjective judgment)                        -->
<!-- - Testable: maps to at least one automated or manual test                -->
<!-- - Specific: no vague language ("should work correctly")                   -->
<!-- Example: "Endpoint returns 200 with approved contract shape for valid    -->
<!-- input." "Unauthorized users receive 403."                                 -->

## 12. Test Implications

<!-- What types of tests are required? (unit, integration, contract, golden   -->
<!-- dataset, authorization) What test infrastructure is needed?               -->

## 13. Open Questions / Assumptions

<!-- List unresolved questions and working assumptions. Each assumption        -->
<!-- should note what would change if the assumption is wrong.                 -->
<!-- If no open questions remain, this spec is decomposition-ready.            -->
