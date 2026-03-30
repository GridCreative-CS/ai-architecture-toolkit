# AI Explainability Guide

How to design, implement, and verify explainability in systems that include AI
components.

## When This Guide Applies

Use this guide when the system includes any component whose output depends on a
model, algorithm, or learned rule — not just "AI/ML" features. This includes:

- machine learning models (classification, regression, ranking)
- LLM-generated content (summaries, recommendations, decisions)
- rule engines with complex or opaque rule sets
- scoring algorithms where the logic is not immediately obvious to the user

If the system is purely deterministic with transparent logic, this guide does
not apply.

---

## Core Principle

Every AI decision the system surfaces to a user, operator, or auditor must be
accompanied by an explanation appropriate to the audience and the domain.

"Appropriate" is defined by the **explainability tier** (see below).

---

## Explainability Tiers

Not all explanations need the same depth. The required tier depends on the
domain, the audience, and the regulatory environment.

| Tier | Name | What It Includes | When to Use |
|------|------|------------------|-------------|
| 1 | **Audit trail** | Full input snapshot, model version, feature weights, confidence score, reasoning trace, timestamp | Regulated domains (healthcare, finance, legal), any decision subject to external audit |
| 2 | **Decision summary** | Inputs used, rules applied, confidence score, model version | Operational domains where operators need to understand and potentially override decisions |
| 3 | **User-facing explanation** | Plain-language summary of why the decision was made, key factors | Consumer-facing features where trust matters but full audit is not required |
| 4 | **Metadata only** | Model version, confidence score, timestamp | Internal-only decisions with no user visibility and no regulatory requirement |

### Choosing a Tier

1. Start from the **highest tier required by regulation** for this domain.
2. If no regulation applies, choose based on **who sees the decision**:
   - external user → Tier 3 minimum
   - internal operator → Tier 2 minimum
   - no human sees it → Tier 4
3. If the decision can be **overridden by a human**, add one tier (e.g., Tier 4
   becomes Tier 3) so the human has enough context to decide.

---

## Explanation Components

Each explanation should include a subset of these components, depending on the
tier:

| Component | Description | Tier 1 | Tier 2 | Tier 3 | Tier 4 |
|-----------|-------------|--------|--------|--------|--------|
| **Inputs used** | What data the model received | ✅ | ✅ | ✅ | — |
| **Rules applied** | Which rules or model logic produced the output | ✅ | ✅ | Simplified | — |
| **Confidence score** | How confident the model is in the output | ✅ | ✅ | Optional | ✅ |
| **Model version** | Which model/algorithm version produced the output | ✅ | ✅ | — | ✅ |
| **Reasoning trace** | Step-by-step logic chain from input to output | ✅ | Optional | — | — |
| **Feature weights** | Which input features contributed most to the output | ✅ | Optional | — | — |
| **Timestamp** | When the decision was made | ✅ | ✅ | — | ✅ |
| **Plain-language summary** | Human-readable explanation of the decision | ✅ | Optional | ✅ | — |

---

## Architecture Patterns

### Pattern 1: Explanation as a Side Effect

The AI component produces the explanation alongside the decision. Both are
stored together.

```
[Input] → [AI Component] → [Decision + Explanation] → [Store Both]
```

**Use when:** The AI component can produce explanations natively (e.g.,
feature importance from a tree model, chain-of-thought from an LLM).

**Advantage:** Explanation is always consistent with the decision.

### Pattern 2: Post-Hoc Explanation Service

The AI component produces the decision. A separate explanation service
reconstructs the explanation from the input and decision.

```
[Input] → [AI Component] → [Decision] → [Store]
                                      → [Explanation Service] → [Store]
```

**Use when:** The AI component cannot produce explanations natively (e.g.,
black-box model, third-party API).

**Risk:** The explanation may not accurately reflect the actual reasoning.
Mitigate with golden dataset validation.

### Pattern 3: Explanation Cache

Explanations are computed once and cached for repeated access.

**Use when:** Explanations are expensive to compute and the decision is stable
(not re-evaluated on each access).

---

## Storage and Retrieval

### What to Store

- the explanation payload (matching the tier requirements)
- the decision ID it explains
- the model version that produced it
- the timestamp

### Where to Store

- **Same table/document as the decision** — simplest; use when the explanation
  is always retrieved with the decision.
- **Separate explanation store** — use when explanations are large, accessed
  independently, or have different retention requirements.

### Retention

- regulated domains: retain explanations for the legally required period
- operational domains: retain for at least the audit window (typically 90 days)
- internal-only: retain as long as the decision is queryable

---

## Testing Explainability

### In Golden Datasets

For each golden scenario that involves an AI decision, include the expected
explanation (or key components of it) as part of the expected output.

### In Contract Tests

If the explanation is part of the API contract, test that the response includes
the required explanation fields.

### In Integration Tests

Verify that:

- explanations are stored when decisions are made
- explanations can be retrieved by decision ID
- explanation tier matches the configured tier for the feature

---

## Common Mistakes

| Mistake | Why It's Wrong | Fix |
|---------|---------------|-----|
| Generating explanations only in the UI layer | Explanations are lost if the UI changes or if the decision is accessed via API | Generate and store explanations in the backend |
| Using a different model version for explanation than for decision | Explanation may not match actual reasoning | Always use the same model version; store version with both |
| Explaining what the model did instead of why | Users need actionable context, not technical narration | Focus on factors that influenced the outcome, not the algorithm steps |
| Treating explainability as a Phase 2 feature | Retrofitting audit trails is expensive and error-prone | Design explainability into the architecture from the start |

---

## References

- Glossary (explainability, deterministic vs probabilistic):
  `ai/guides/glossary.md`
- Golden dataset template: `ai/templates/golden-dataset-template.md`
- Contract definition: `ai/guides/contract-definition.md`
