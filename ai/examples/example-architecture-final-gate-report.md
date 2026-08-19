# Architecture-Final Quality Gate — Retail Recommendation Platform

<!-- Sample gate report showing the expected shape and depth of              -->
<!-- architecture/architecture-final-gate.md. The project is fictional.      -->
<!-- Produced by ai/prompts/architecture-final-quality-gate.md.              -->

- Document reviewed: architecture/architecture-final.md (v1.0, 2026-03-02)
- Date: 2026-03-03
- Reviewer: architecture-final-quality-gate (fresh session)
- Verdict: REJECTED — MUST FIX

## Check results

| # | Check | Result | Evidence / location in document |
|---|-------|--------|--------------------------------|
| 1 | Traceability and document control | FAIL | §1 lists inputs but has no change log; review findings MI-2 (cart/catalog coupling) and DG-1 (no retention policy) from `review-report.md` are not mapped to any decision and do not appear in Open Questions. |
| 2 | Business context | PASS | §3.2 goals table with measurable criteria ("recommendation acceptance rate ≥ 8% within 3 months"); §3.3 stakeholder map incl. merchandising team and data-privacy officer. |
| 3 | Evidence and assumptions | PASS | §4.1 cites prototype-analysis §2–§4 per preserved rule; assumptions A1–A6 and constraints C1–C4 with sources; §4.4 exclusions ("no real-time inventory sync"). |
| 4 | System boundary | PASS | §5 names all four external systems (PIM, order service, email provider, consent platform) with direction and purpose. |
| 5 | Modules and dependency rules | FAIL | §7.1 defines Catalog, Recommendations, Shopper Profile, Experimentation modules with owned data, and §7.2 maps most capabilities — but "seasonal campaign boosts" (described in §6.3) is assigned to no module, and §7.3 lists allowed dependencies only: no forbidden dependencies and no enforcement mechanism are named. |
| 6 | Data architecture | PASS | §8.1 ownership table; §8.3 EF Core migration governance with CI forward+rollback test; §8.4 pseudonymised clickstream, 13-month retention, erasure via profile-key deletion (personal-data condition applies and is covered). |
| 7 | APIs, events, integrations | FAIL | §9.1 defines `/api/v1/` with breaking-change criteria and §9.4 names `RecommendationServed`/`RecommendationClicked` events with producer/consumer — but the PIM integration (§9.3) has no defined behavior when the PIM is down. |
| 8 | Security | PASS | §11: OIDC code flow + PKCE, three named roles, secrets via key vault, consent checked before profile reads. |
| 9 | Frontend/backend boundary | PASS | §12: React storefront widget consumes a generated typed client; ranking logic and consent decisions are explicitly forbidden client-side. |
| 10 | AI decision architecture | PASS | §10: two-stage ranker (deterministic eligibility filter, then model scoring), pinned model versions, sub-threshold fallback to bestseller list, merchandiser override with reason capture. |
| 11 | Error handling and resilience | FAIL | §13 defines RFC 9457 responses and a failure-mode table for the model service and database, but the table omits the email provider and the consent platform — and §13.2 says the widget must "degrade gracefully" without stating what is rendered. |
| 12 | Observability and deployment | PASS | §14: OpenTelemetry with per-endpoint latency and recommendation-serve counters; telemetry prohibits shopper IDs and profile attributes; §15: two environments, blue/green on AKS, nightly backup with restore test and 4-hour recovery target. |
| 13 | Testing strategy | PASS | §16: unit/integration/contract layers; NetArchTest rules planned for module isolation (must be extended per check 5 fix); golden dataset of 60 ranked baskets for the scoring path; per-milestone binary gates. |
| 14 | Decisions, risks, open questions | PASS | §18 has six ADR-ready decisions with rejected alternatives and costs; §19 risk register R1–R7 with mitigation; open questions Q1–Q3 with owner and phase. |
| 15 | Specificity and language | FAIL | §2 states the platform is "highly scalable and production-ready" with no mechanism or verification anywhere in the passage; scale context elsewhere is quantified (§4: 2,000 concurrent shoppers, p95 < 300 ms), so only the wording fails. One template comment remains in §17. |
| 16 | Downstream sufficiency | PASS | Decisions extractable as ADRs; §7.2 capability map (once the orphan is fixed) supports slice derivation; contracts and error identifiers are concrete enough for feature specs to cite. |

## Findings

| # | Check | Finding | Required fix |
|---|-------|---------|--------------|
| 1 | 1 | Review findings MI-2 and DG-1 are silently dropped — no decision, no deferral. | Add a change-log table to §1 mapping every Critical/Major review finding to a decision and section, or defer each to §19.2 Open Questions with rationale. |
| 2 | 5 | "Seasonal campaign boosts" (§6.3) is an orphan capability owned by no module. | Assign it to exactly one module in §7.2 (Experimentation appears to be the natural owner) and record the owning aggregate in §7.1. |
| 3 | 5 | §7.3 states allowed dependencies only; forbidden dependencies and enforcement are missing. | State the forbidden dependencies explicitly (e.g., Recommendations must not reference Shopper Profile internals; model client only in Infrastructure) and name the enforcement mechanism (NetArchTest rules in CI, per §16). |
| 4 | 7 | The PIM integration has no failure behavior. | Add a row to the §13 failure-mode table: retry policy, fallback (e.g., serve from last-synced catalog snapshot), and user-visible effect when the PIM is down. |
| 5 | 11 | Email provider and consent platform missing from the failure-mode table; "degrade gracefully" (§13.2) does not state what happens. | Add both dependencies to the table. Replace §13.2 with the concrete degraded rendering (e.g., widget renders bestseller list with no personalisation banner when ranking is unavailable). |
| 6 | 15 | "Highly scalable and production-ready" (§2) with no mechanism/verification; leftover template comment in §17. | Replace with the quantified claim already available in §4 ("serves 2,000 concurrent shoppers at p95 < 300 ms, verified by the load test at the M2 gate") and delete the template comment. |

## Verdict justification

Checks 1, 5, 7, 11, and 15 fail. The document is strong on business context,
data, security, and AI governance, but two review findings were silently
dropped, one capability has no owning module, module boundaries are
unenforceable as written, and two external dependencies have undefined failure
behavior. These are exactly the gaps that resurface as rework during delivery
planning and implementation. Return to the reconciliation step
(`ai/prompts/architecture-reconciler.md`) with this report as input; re-run
the gate on the revised document.
