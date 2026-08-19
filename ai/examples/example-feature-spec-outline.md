# Feature Specification Example Outline

## 1. Feature Name

Recommendation Slice

## 2. Purpose

Generate and expose recommendations for a validated decision workflow.

## 3. Scope In

- backend endpoint
- domain decision mapping
- explainability summary
- UI read path

## 4. Scope Out

- model retraining
- admin controls
- unrelated reporting

## 5. User / System Flows

- clinician requests recommendation
- system evaluates inputs
- system returns recommendation and explanation summary

## 6. Domain Rules

- DR-01: recommendation must include an explanation summary
- DR-02: unsupported states must return a domain-safe error

## 7. API / Contract Expectations

- request and response schema must remain stable
- explanation fields are required

## 8. Data Requirements

- read relevant assessment data
- store audit event where required

## 9. Security / Authorization Constraints

- SEC-01: only authorized roles may access the endpoint
- SEC-02: denied roles receive 403 and no audit event is written on their behalf

## 10. Observability Requirements

- request metric
- latency metric
- structured error logging

## 11. Acceptance Criteria

- AC-01: endpoint returns 200 with the approved contract shape for valid input
- AC-02: the explanation summary is present in every successful response
- AC-03: unauthorized users receive 403 and no recommendation is produced

## 12. Test Implications

- contract tests
- authorization tests
- golden scenario validation

## 13. Open Questions / Assumptions

- final naming of explanation fields
