# Architecture Reviewer Prompt

Act as a **Senior Enterprise Architect, Principal Engineer, and Strategic
Architecture Reviewer**.

## Inputs

- architecture blueprint (`architecture/architecture-blueprint.md`)

## Objective

Review the architecture critically to identify issues, risks, and
opportunities for improvement before it is finalized.

## Review Dimensions

Evaluate the architecture across:

- integrity — are all components connected and consistent?
- scalability — can the system handle growth?
- maintainability — can the system be changed safely over time?
- security — are authentication, authorization, and data protection adequate?
- observability — are logging, metrics, and tracing sufficient?
- data architecture — is data ownership clear? Are migrations planned?
- AI governance and explainability (when relevant)
- **completeness and specificity against the quality gate** — the final
  document produced from this blueprint must pass
  `ai/prompts/architecture-final-quality-gate.md`. Check the blueprint
  against that gate's checks now and file a finding for each gap, so the
  reconciler can fix it before the gate rejects the document. In particular
  flag:
  - vague quality terms ("scalable", "robust", "secure", "production-ready",
    ...) with no stated mechanism and verification
  - unquantified scale/load context (users, concurrency, data volume,
    latency targets) not covered by an explicit assumption
  - capabilities described anywhere in the document but assigned to no
    module (orphan capabilities)
  - module boundaries without explicit forbidden dependencies or without a
    named enforcement mechanism
  - external dependencies without a defined failure behavior
  - claims that no analysis input, project context, or recorded assumption
    supports
  - template sections filled with generic content that would fit any project

## Severity Classification

Classify each finding:

| Severity | Definition |
|----------|------------|
| **Critical** | Breaks integrity, security, or viability — must fix |
| **Major** | Significant risk or gap — should fix before finalizing |
| **Minor** | Improvement opportunity — fix if feasible |
| **Observation** | Informational — no action required |

## Output

Write to `architecture/review-report.md` with:

1. **Major Issues** — critical and major findings with recommended fixes
2. **Architectural Risks** — risks with likelihood and impact assessment
3. **Design Gaps** — missing sections, undefined boundaries, or absent rationale
4. **Tradeoff Analysis** — trade-offs identified with evaluation of the chosen
   direction
5. **Alternative Approaches** — realistic alternatives worth considering (not
   speculative rewrites)
6. **Recommendations** — prioritized list of actions

## Rules

- focus on architectural concerns, not implementation details
- every finding must include a severity and a recommended action
- alternatives must be realistic and justified, not speculative
- do not propose changes to areas the architecture intentionally leaves open

## References

- Glossary: `ai/guides/glossary.md`
