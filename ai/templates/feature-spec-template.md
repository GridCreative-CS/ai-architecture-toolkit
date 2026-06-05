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

## 11b. UI/UX Acceptance Criteria

<!-- MANDATORY section for slices with human workflow surfaces (§5b).          -->
<!--                                                                           -->
<!-- When architecture/design-system.md exists, list:                           -->
<!-- - Which design system components are used?                                -->
<!-- - Which design tokens (colors, typography, spacing) apply?                -->
<!-- - Which layout patterns from the design system are followed?              -->
<!-- - Are all four states handled per the design system state patterns        -->
<!--   (loading, success, error, empty)?                                       -->
<!-- - Does the UI meet the design system's accessibility baseline (§6)?       -->
<!--                                                                           -->
<!-- When NO design system exists, list:                                        -->
<!-- - Which screens/pages are involved and their expected structure?           -->
<!-- - Which interactive elements exist (buttons, forms, links, navigation)?   -->
<!-- - Are all four states handled (loading, success, error, empty)?            -->
<!-- - Expected responsive behavior (minimum: mobile and desktop)?             -->
<!-- - Accessibility baseline met (semantic HTML, keyboard navigation,         -->
<!--   labels, sufficient contrast)?                                            -->
<!--                                                                           -->
<!-- If the slice has no human workflow surfaces (§5b confirms this with an    -->
<!-- architecture citation), state:                                             -->
<!-- "N/A — this slice has no human workflow surfaces per [citation]."          -->
<!--                                                                           -->
<!-- Each criterion must be binary, testable, and specific — same rules as     -->
<!-- §11.                                                                       -->
<!-- Reference: architecture/design-system.md                                  -->
<!-- Reference: ai/templates/design-system-template.md                         -->

## 12. Test Implications

<!-- What types of tests are required? (unit, integration, contract, golden   -->
<!-- dataset, authorization) What test infrastructure is needed?               -->

## 12b. Browser Verification Steps

<!-- MANDATORY section for slices with human workflow surfaces (§5b).          -->
<!-- Describe the browser-based verification that confirms the slice works     -->
<!-- end-to-end in the running application. This section becomes the basis     -->
<!-- for the Integrated Slice Verification (engineering workflow Step 6b).     -->
<!--                                                                           -->
<!-- Include:                                                                   -->
<!-- - Application startup commands (e.g., docker compose up)                  -->
<!-- - The URL to access                                                        -->
<!-- - Step-by-step click/navigate/input sequence for the primary flow         -->
<!-- - Expected visible outcomes at each step                                   -->
<!-- - Error flow walkthrough                                                   -->
<!-- - Viewport sizes to check (minimum: mobile ≤480px, desktop ≥1024px)      -->
<!-- - Cross-slice navigation check (navigate away and back)                   -->
<!--                                                                           -->
<!-- If automated E2E tests exist or should be created, list the test          -->
<!-- commands (e.g., npx playwright test).                                      -->
<!--                                                                           -->
<!-- If the slice has no human workflow surfaces, state:                        -->
<!-- "N/A — this slice has no human workflow surfaces."                         -->

## 13. Open Questions / Assumptions

<!-- List unresolved questions and working assumptions. Each assumption        -->
<!-- should note what would change if the assumption is wrong.                 -->
<!-- If no open questions remain, this spec is decomposition-ready.            -->
