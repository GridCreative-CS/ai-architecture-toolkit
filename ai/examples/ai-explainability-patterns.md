# AI Explainability Patterns — Good and Bad Examples

Concrete examples of explainability patterns applied to a recommendation system.
Use these as reference when designing explainability into your own system.

---

## Scenario

A system generates risk assessments for job applicants. The assessment
includes a risk level (LOW, MEDIUM, HIGH) and an explanation of why that level
was assigned.

---

## Pattern 1: Explanation as a Side Effect

### ✅ Good: Explanation produced alongside the decision

```json
{
  "decision_id": "D-2026-0042",
  "risk_level": "HIGH",
  "confidence": 0.87,
  "model_version": "risk-v2.3.1",
  "timestamp": "2026-03-15T14:22:00Z",
  "explanation": {
    "summary": "High risk due to incomplete work history and flagged reference.",
    "factors": [
      {
        "factor": "work_history_gaps",
        "weight": 0.45,
        "description": "Employment gaps exceeding 12 months in the last 5 years."
      },
      {
        "factor": "reference_flag",
        "weight": 0.35,
        "description": "One reference returned a negative verification result."
      }
    ],
    "inputs_used": ["work_history", "reference_checks", "education_verification"],
    "tier": 2
  }
}
```

**Why this is good:**

- Explanation is generated at decision time with the same model version
- Factors include weights so the operator can see what mattered most
- `inputs_used` documents what data was considered
- `tier` makes the explainability level explicit

### ❌ Bad: Decision without explanation

```json
{
  "risk_level": "HIGH",
  "confidence": 0.87
}
```

**Why this is bad:**

- No explanation — operator cannot understand or challenge the decision
- No model version — impossible to reproduce or audit
- No timestamp — impossible to correlate with input data
- No decision ID — impossible to track through the system

---

## Pattern 2: Post-Hoc Explanation

### ✅ Good: Separate explanation service with consistency validation

```json
{
  "decision_id": "D-2026-0042",
  "explanation_source": "post-hoc",
  "explanation": {
    "summary": "High risk primarily driven by work history gaps.",
    "factors": [
      { "factor": "work_history_gaps", "contribution": "primary" },
      { "factor": "reference_flag", "contribution": "secondary" }
    ],
    "model_version_at_decision": "risk-v2.3.1",
    "explanation_model_version": "explain-v1.2.0",
    "consistency_check": "passed"
  }
}
```

**Why this is good:**

- Explicitly labels the explanation as post-hoc
- Records both the decision model version and the explanation model version
- Includes a consistency check result so consumers know if the explanation
  was validated against the actual decision

### ❌ Bad: Post-hoc explanation without version tracking

```json
{
  "decision_id": "D-2026-0042",
  "explanation": "The applicant was flagged as high risk."
}
```

**Why this is bad:**

- No model version — explanation may not match the actual decision
- Vague narrative instead of structured factors
- No consistency check — no way to know if the explanation is accurate

---

## Pattern 3: User-Facing Explanation (Tier 3)

### ✅ Good: Plain-language summary with key factors

```text
Your application was assessed as high risk. The main reasons were:
1. Gaps in your employment history exceeding 12 months.
2. One of your references could not be verified.

If you believe this assessment is incorrect, you can request a review
by contacting your case manager.
```

**Why this is good:**

- Plain language — no technical jargon
- Lists the specific factors that drove the decision
- Provides an actionable next step (request review)
- Does not expose model internals (weights, version)

### ❌ Bad: Technical dump presented to a user

```text
risk_model_v2.3.1 output: HIGH (0.87 confidence)
feature_weights: {work_history_gaps: 0.45, reference_flag: 0.35,
education_score: 0.12, location_risk: 0.08}
```

**Why this is bad:**

- Incomprehensible to a non-technical user
- Exposes model internals unnecessarily
- No actionable information
- No plain-language summary

---

## Pattern 4: Audit Trail (Tier 1)

### ✅ Good: Full audit record

```json
{
  "audit_id": "AUD-2026-0042",
  "decision_id": "D-2026-0042",
  "timestamp": "2026-03-15T14:22:00Z",
  "model_version": "risk-v2.3.1",
  "input_snapshot": {
    "applicant_id": "A-1234",
    "work_history": ["2020-2022: Employer A", "2024-present: Employer B"],
    "reference_checks": [
      { "name": "Ref 1", "status": "verified" },
      { "name": "Ref 2", "status": "flagged" }
    ],
    "education": { "degree": "BSc", "verified": true }
  },
  "output": {
    "risk_level": "HIGH",
    "confidence": 0.87
  },
  "reasoning_trace": [
    "Step 1: Evaluated work history — 2-year gap detected (2022-2024)",
    "Step 2: Evaluated references — 1 of 2 flagged",
    "Step 3: Combined score exceeds HIGH threshold (0.80)"
  ],
  "feature_weights": {
    "work_history_gaps": 0.45,
    "reference_flag": 0.35,
    "education_score": 0.12,
    "location_risk": 0.08
  },
  "explanation_summary": "High risk due to employment gap and flagged reference.",
  "operator_action": null,
  "retention_policy": "7 years (regulatory requirement)"
}
```

**Why this is good:**

- Full input snapshot enables reproduction
- Reasoning trace shows step-by-step logic
- Feature weights show contribution of each factor
- Retention policy is explicitly stated
- Operator action field supports override tracking

---

## Anti-Patterns Summary

| Anti-Pattern | Problem | Fix |
|-------------|---------|-----|
| Decision without any explanation | Operators and users cannot understand or challenge decisions | Always generate an explanation at the required tier |
| Technical explanation shown to non-technical users | Confusing, erosion of trust | Use Tier 3 (plain language) for user-facing explanations |
| Post-hoc explanation without consistency check | Explanation may not match actual reasoning | Validate post-hoc explanations against the decision |
| Explanation without model version | Cannot reproduce or audit | Always record model version with both decision and explanation |
| Explanation generated in the UI layer only | Lost on API access, lost on UI change | Generate and store explanations in the backend |
| "Phase 2" explainability | Retrofitting audit trails is expensive and error-prone | Design explainability into the architecture from the start |

---

## References

- Explainability guide: `ai/guides/ai-explainability-guide.md`
- Governance checklist: `ai/guides/ai-governance-checklist.md`
- Glossary: `ai/guides/glossary.md`
