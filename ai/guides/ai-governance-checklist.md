# AI Governance Checklist

A structured checklist for verifying that AI components in the system meet
governance, safety, and accountability requirements.

## When to Use This Checklist

Use this checklist:

- during architecture design — to ensure governance is designed in
- during feature spec generation — to ensure each AI slice addresses governance
- during compliance review — to verify governance requirements are met
- before deployment — as a final governance gate

If the system has no AI components, this checklist does not apply.

---

## 1. Decision Transparency

| # | Check | Status |
|---|-------|--------|
| 1.1 | Every AI decision has an assigned explainability tier (see `ai/guides/ai-explainability-guide.md`) | ☐ |
| 1.2 | Explanations are generated and stored alongside decisions (not reconstructed later) | ☐ |
| 1.3 | The explanation tier matches the domain requirements (regulated → Tier 1, operator-visible → Tier 2, user-visible → Tier 3) | ☐ |
| 1.4 | Model version is recorded with every decision | ☐ |

---

## 2. Human Oversight

| # | Check | Status |
|---|-------|--------|
| 2.1 | Human-in-the-loop controls are defined for high-risk decisions (see glossary → Human-in-the-Loop tiers) | ☐ |
| 2.2 | Override mechanisms exist: an authorized human can override any AI decision before it takes effect | ☐ |
| 2.3 | Emergency stop: the AI component can be disabled without taking the system down | ☐ |
| 2.4 | Fallback behavior is defined: what happens when the AI component is unavailable or returns low-confidence results | ☐ |

---

## 3. Bias and Fairness

| # | Check | Status |
|---|-------|--------|
| 3.1 | Protected attributes are identified (race, gender, age, disability, etc.) | ☐ |
| 3.2 | The model is tested for disparate impact across protected groups | ☐ |
| 3.3 | Monitoring is in place to detect bias drift in production | ☐ |
| 3.4 | A remediation plan exists for when bias is detected | ☐ |

---

## 4. Data Governance

| # | Check | Status |
|---|-------|--------|
| 4.1 | Training data provenance is documented (source, collection method, consent) | ☐ |
| 4.2 | Personal data handling complies with applicable regulations (GDPR, HIPAA, etc.) | ☐ |
| 4.3 | Data retention policies are defined for AI inputs, outputs, and explanations | ☐ |
| 4.4 | Data used for decisions can be reproduced for audit purposes | ☐ |

---

## 5. Model Lifecycle

| # | Check | Status |
|---|-------|--------|
| 5.1 | Model versioning is in place (every deployed model has a version identifier) | ☐ |
| 5.2 | Model deployment requires approval (not auto-deployed on merge) | ☐ |
| 5.3 | Rollback procedure is defined: previous model version can be restored | ☐ |
| 5.4 | Performance monitoring is in place (accuracy, latency, error rate) | ☐ |
| 5.5 | Retraining triggers are defined (schedule, drift threshold, or manual) | ☐ |

---

## 6. Security

| # | Check | Status |
|---|-------|--------|
| 6.1 | AI endpoints are protected by the same authentication and authorization as other system endpoints | ☐ |
| 6.2 | Input validation prevents adversarial inputs (prompt injection, data poisoning) | ☐ |
| 6.3 | Model artifacts (weights, configs) are stored securely and access-controlled | ☐ |
| 6.4 | API rate limiting is applied to AI endpoints to prevent abuse | ☐ |

---

## 7. Observability

| # | Check | Status |
|---|-------|--------|
| 7.1 | AI decision metrics are emitted (decision count, confidence distribution, override rate) | ☐ |
| 7.2 | Latency metrics distinguish model inference time from total request time | ☐ |
| 7.3 | Error logging captures model failures separately from application failures | ☐ |
| 7.4 | Alerting is configured for anomalous AI behavior (confidence drop, error spike, latency increase) | ☐ |

---

## 8. Compliance and Audit

| # | Check | Status |
|---|-------|--------|
| 8.1 | Regulatory requirements for AI are identified and documented | ☐ |
| 8.2 | Audit trail covers the full decision lifecycle (input → model → decision → explanation → action) | ☐ |
| 8.3 | Compliance review has been performed for AI-specific risks | ☐ |
| 8.4 | Acceptable risk levels are documented and owned by a named stakeholder (see glossary → Acceptable Risk) | ☐ |

---

## How to Use

1. **During architecture design:** Walk through each section. For any ☐ that
   cannot be checked, add the requirement to the architecture or flag it as a
   gap.
2. **During feature spec generation:** Include relevant checklist items in the
   Security, Observability, and Acceptance Criteria sections of the feature
   spec.
3. **During compliance review:** Use as a supplementary checklist alongside the
   compliance report template.
4. **Before deployment:** All items must be ☐ → ✅ or explicitly documented as
   accepted risk with a named owner.

---

## References

- Explainability guide: `ai/guides/ai-explainability-guide.md`
- Glossary (human-in-the-loop, acceptable risk, explainability):
  `ai/guides/glossary.md`
- Compliance report template: `ai/templates/compliance-report-template.md`
