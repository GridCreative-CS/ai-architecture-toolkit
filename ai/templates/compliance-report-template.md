# Architecture Compliance Report

<!-- This template structures a compliance review for a feature spec, part     -->
<!-- definition, or code change against the approved architecture.             -->
<!-- Reference: ai/prompts/architecture-compliance.md for the review prompt.   -->

## 1. Review Metadata

- **Review target:** <!-- What is being reviewed (feature spec, PR, etc.) -->
- **Review date:**
- **Reviewer:**
- **Level:** Full / Lightweight <!-- per the six trigger questions in engineering workflow Step 4 -->
- **Trigger answers:** <!-- 1..6: yes/no. Lightweight is permitted only when all six are "no". -->
- **Inputs reviewed:**
  - <!-- List each input document reviewed (architecture, ADRs, specs) -->
- **Related ADRs:** <!-- ADR numbers that apply to this review -->
- **Related feature or slice:** <!-- Slice ID + name from the delivery plan -->

## 2. Compliance Summary

<!-- 2–3 sentence assessment of overall alignment with the approved            -->
<!-- architecture. State whether the artifact is broadly compliant or has      -->
<!-- significant deviations.                                                    -->

## 3. Conforming Decisions

<!-- List choices that align with the architecture. Group by source:           -->
<!-- - Final architecture                                                      -->
<!-- - ADRs                                                                    -->
<!-- - Delivery plan                                                           -->
<!-- - Feature specification                                                   -->

## 4. Violations Detected

<!-- For each violation, use this structure:                                    -->

<!-- Use severity levels:                                                      -->
<!-- - Critical: breaks a core constraint, security, or ADR — must fix        -->
<!-- - Warning: deviates from intent but not a hard constraint — should fix    -->
<!-- - Info: minor inconsistency — fix if convenient                           -->

- **Identifier:** <!-- e.g., CV-001 — compliance violation. Do not use the AC- prefix: that namespace belongs to feature spec §11 acceptance criteria. -->
- **Description:**
- **Violated source:** <!-- Which architecture section or ADR? -->
- **Severity:** Critical / Warning / Info
- **Impact:**
- **Recommended correction:**

## 5. Risks Introduced

<!-- For each new risk the artifact introduces:                                -->

- **Risk:**
- **Cause:**
- **Likelihood:** High / Medium / Low
- **Impact:** High / Medium / Low
- **Mitigation:**

## 6. Verticality Assessment

<!-- Does the slice include the human workflow required by the architecture?   -->
<!-- If the architecture requires human-in-the-loop and the slice omits UI,   -->
<!-- this is a compliance violation.                                           -->
<!-- Reference: ai/guides/vertical-slice-definition.md for the verticality    -->
<!-- test.                                                                     -->

- **Verticality test passed:** Yes / No
- **Details:** <!-- Explain the assessment -->

## 7. Required Corrections

<!-- Numbered list of actions required before approval. Each correction        -->
<!-- should reference the violation identifier.                                -->

## 8. Approval Status

<!-- Choose exactly one:                                                       -->
<!-- - APPROVED — no violations, all conforming                               -->
<!-- - APPROVED WITH CHANGES — violations exist but are correctable           -->
<!-- - REJECTED — critical violations that require redesign                    -->

## 9. Re-review Requirement

- **Re-review required:** Yes / No
- **Re-review trigger:** <!-- What must happen before re-review? -->
