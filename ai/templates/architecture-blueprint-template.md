# Architecture Blueprint / Final Architecture Template

<!-- This template structures a production-grade architecture document. It is  -->
<!-- used twice in the architecture phase:                                      -->
<!--   architecture/architecture-blueprint.md — the designed architecture       -->
<!--     (Modes A and D, written by ai/prompts/architecture-designer.md)        -->
<!--   architecture/architecture-final.md — the reconciled, authoritative       -->
<!--     version (all modes; the items marked FINAL ONLY in §1 become           -->
<!--     mandatory there)                                                       -->
<!-- In Modes B and C the final document may keep the existing document's       -->
<!-- structure — the content requirements below still apply and are enforced    -->
<!-- by ai/prompts/architecture-final-quality-gate.md.                          -->
<!-- Fill every section. If a section does not apply, write                     -->
<!-- "Not applicable — [reason]" rather than deleting it.                       -->
<!-- Reference: ai/guides/glossary.md for term definitions.                     -->

## Writing rules (apply to every section)

The architecture-final quality gate (`ai/prompts/architecture-final-quality-gate.md`)
enforces these rules on the final document. Violating them in the blueprint
only defers the failure.

- **Project-specific, not generic.** Every section is about THIS system. A
  paragraph that could be pasted unchanged into another project's architecture
  document is not done.
- **Evidence or explicit assumption.** Every material claim traces to an input
  (the analysis document, `ai/project-context.md`, the existing architecture
  document, or an explicit user statement) or is recorded in §4 as an
  assumption with an ID. Never silently invent domain facts, constraints, or
  stakeholder preferences.
- **Quantify.** State numbers for the context that shapes the architecture:
  users, concurrency, data volume, latency targets, team size and skills,
  budget, timeline. Unknown numbers become identified assumptions in §4, not
  omissions.
- **Banned vague terms.** "Scalable", "robust", "maintainable", "flexible",
  "secure", "performant", "production-ready", "highly available",
  "enterprise-grade", "best-practice", and similar quality adjectives are
  banned unless the same passage states (a) the mechanism that achieves the
  quality and (b) how it is verified. Example: not "the API is scalable" but
  "the API serves 50 concurrent users at p99 < 500 ms, verified by a load test
  at the Phase 2 validation gate."
- **No orphan capabilities.** Every capability or feature described anywhere
  in this document is assigned to exactly one module in §7. A capability with
  no owning module is a defect.
- **Decisions, not option lists.** Where alternatives exist, choose one,
  record the rejected alternatives and rationale in §18, and write the rest of
  the document as if the decision is made (because it is).

## 1. Document Control

<!-- Table: document type, version, date, status (Draft for the blueprint;     -->
<!-- Final for architecture-final.md), target audience, and Inputs — the exact -->
<!-- files this document was produced from (analysis docs, project context,    -->
<!-- existing architecture doc, review reports).                               -->
<!-- FINAL ONLY — Change log: a table mapping every Critical/Major review      -->
<!-- finding (by ID) to the decision taken and the section that records it.    -->
<!-- No finding may be silently dropped: resolve it, or defer it explicitly    -->
<!-- into §19 Open Questions with rationale.                                   -->

## 2. Executive Summary

<!-- 3–5 sentences: what the system does, for whom, and the key architectural  -->
<!-- approach. Follow with a strategic decision summary table — one row per    -->
<!-- major decision (architecture style, stack, persistence, AI approach,      -->
<!-- deployment, ...) with a pointer to the §18 decision entry (and the ADR,   -->
<!-- once ADRs exist).                                                         -->

## 3. Business Context

<!-- 3.1 Problem statement — the concrete business/domain problem, in domain   -->
<!--     terms, with the pain quantified where possible.                       -->
<!-- 3.2 Business goals — table: goal, description, measurable success         -->
<!--     criterion. Goals without a success criterion are not done.            -->
<!-- 3.3 Users and stakeholders — table: stakeholder, role, primary concern.   -->
<!--     Include indirect stakeholders (e.g., data subjects, auditors).        -->
<!-- 3.4 Regulatory and compliance context — which regulations apply and what  -->
<!--     they force architecturally; or "Not applicable — [reason]".           -->

## 4. Evidence Base, Assumptions, Constraints, and Exclusions

<!-- 4.1 Evidence base — what was learned from the analysis input (prototype,  -->
<!--     legacy system, or existing doc): the business rules, workflows, data  -->
<!--     flows, and domain concepts that must be preserved. Cite the analysis  -->
<!--     document sections. If designing from requirements only, say so.       -->
<!-- 4.2 Assumptions — table with IDs (A1, A2, ...): every assumption this     -->
<!--     document rests on that no input confirms. Downstream work treats      -->
<!--     these as revisit triggers.                                            -->
<!-- 4.3 Constraints — table with IDs (C1, C2, ...): budget, team, timeline,   -->
<!--     technology mandates, data residency — with their source.              -->
<!-- 4.4 Exclusions — what this system deliberately does NOT do, so scope      -->
<!--     cannot silently grow back.                                            -->

## 5. System Context and Boundaries

<!-- What is inside the system; what is outside (users, external services,     -->
<!-- data sources); every actor and external system named, with the direction  -->
<!-- and purpose of each interaction. A context diagram or an equivalent       -->
<!-- textual actor/interface list.                                             -->

## 6. Core Workflows

<!-- The concrete user and system workflows the architecture must serve, one   -->
<!-- subsection each, in domain language: trigger, steps, decision points,     -->
<!-- outcome. These come from the evidence base (§4.1) — not invented.        -->

## 7. Module Decomposition

<!-- The heart of the document. Reference:                                     -->
<!-- ai/guides/modular-monolith-definition.md for boundary rules.              -->
<!-- 7.1 Modules/bounded contexts — table: module, responsibility, owned data  -->
<!--     (aggregates/entities), key events it emits.                           -->
<!-- 7.2 Capability map — every capability from §3/§6 assigned to exactly one  -->
<!--     module. No orphans.                                                   -->
<!-- 7.3 Dependency rules — which modules may depend on which (and via what:   -->
<!--     events, contracts, shared kernel), and the FORBIDDEN dependencies     -->
<!--     stated explicitly. Name the enforcement mechanism (e.g., architecture -->
<!--     tests such as NetArchTest/ArchUnit in CI, project references,         -->
<!--     lint rules) — an unenforced boundary rule is a wish, not a rule.      -->
<!-- 7.4 Cross-module communication — the events/contracts between modules:    -->
<!--     name, producer, consumer, payload summary, sync/async.                -->

## 8. Data Architecture

<!-- 8.1 Data ownership — which module owns each key entity; other modules     -->
<!--     access it only via that module's contract.                            -->
<!-- 8.2 Persistence — the store(s) chosen, and the key entities/tables at     -->
<!--     least named per module with their most important columns/fields.      -->
<!-- 8.3 Schema evolution — how migrations are created, tested, executed, and  -->
<!--     rolled back, and when they run relative to deployment.                -->
<!-- 8.4 Data protection — encryption at rest/in transit, retention, deletion/ -->
<!--     erasure strategy where personal or sensitive data exists; or          -->
<!--     "Not applicable — [reason]".                                          -->

## 9. APIs, Events, and Integrations

<!-- Reference: ai/guides/contract-definition.md for contract structure.       -->
<!-- 9.1 API surface — style (REST/gRPC/...), base paths, and the versioning   -->
<!--     policy including WHAT triggers a new version (breaking-change         -->
<!--     criteria).                                                            -->
<!-- 9.2 Error contract — the wire format for errors (e.g., RFC 9457 problem   -->
<!--     details) and stable error identifiers.                                -->
<!-- 9.3 External integrations — table per integration: system, purpose,       -->
<!--     direction, protocol, and behavior when it is DOWN (see §13).          -->
<!-- 9.4 Eventing — broker/in-process choice, delivery guarantees, and schema  -->
<!--     versioning for events; or "Not applicable — [reason]".                -->

## 10. AI Decision Architecture

<!-- Where AI decisions occur; deterministic vs probabilistic responsibilities -->
<!-- separated; confidence thresholds and how they are calibrated; fallback    -->
<!-- paths when the model fails or is not confident; human override points;    -->
<!-- model/prompt versioning and pinning; explainability: what explanation is  -->
<!-- produced per decision, where it is stored, how it is surfaced; audit      -->
<!-- trail requirements; governance: bias monitoring, human-in-the-loop tiers, -->
<!-- update/rollback authority.                                                -->
<!-- References: ai/guides/ai-explainability-guide.md,                         -->
<!-- ai/guides/ai-governance-checklist.md.                                     -->
<!-- If no AI components exist: "Not applicable — no AI components."           -->

## 11. Security Architecture

<!-- Authentication (protocol, provider), authorization model (roles/policies/ -->
<!-- attributes — name them), secrets management, data protection measures     -->
<!-- (cross-reference §8.4), and the main threats considered with their        -->
<!-- mitigations. Every mechanism named concretely — "industry-standard        -->
<!-- security" is a banned phrase.                                             -->

## 12. Frontend Architecture and Frontend/Backend Boundary

<!-- For systems with human-facing UI:                                         -->
<!-- frontend stack; how the frontend consumes backend contracts (e.g., typed  -->
<!-- client generated from OpenAPI); state/session/token handling; which       -->
<!-- logic is FORBIDDEN client-side (e.g., business rules, authorization       -->
<!-- decisions, anything trusting client input); degraded-mode and error UX    -->
<!-- expectations.                                                             -->
<!-- If the system has no UI: "Not applicable — no human-facing UI."           -->

## 13. Error Handling and Resilience

<!-- The application-level error handling architecture (exception strategy,    -->
<!-- result types, validation split) and a failure-mode table: one row per     -->
<!-- external dependency and critical internal component — failure, retry      -->
<!-- policy, fallback behavior, user-visible effect. "Handle errors           -->
<!-- gracefully" is a banned phrase; each row states WHAT happens.             -->

## 14. Observability

<!-- Logging, metrics, and tracing: tools, what is measured (key signals per   -->
<!-- module/endpoint), alerting. Where sensitive data exists: a telemetry      -->
<!-- data classification — what is allowed vs prohibited in logs/metrics/      -->
<!-- traces.                                                                   -->

## 15. Deployment and Operations

<!-- Deployment target and topology (concrete: what runs where), environments, -->
<!-- release/rollback approach, backup and recovery (with verification         -->
<!-- procedure and recovery target) where stateful, scaling approach with the  -->
<!-- numbers from §4 it is sized for, and operational constraints.             -->

## 16. Testing and Verification Strategy

<!-- Test layers (unit/integration/contract/E2E) and what each proves;         -->
<!-- architecture-boundary enforcement tests (cross-reference §7.3); golden    -->
<!-- datasets for AI decision paths (or N/A); per-phase or per-milestone       -->
<!-- validation gates with binary pass criteria.                               -->

## 17. Technology Stack

<!-- Table: layer/concern, chosen technology (with version), rationale tied to -->
<!-- this project's constraints (§4.3) — not generic praise. Include           -->
<!-- language/framework, database, messaging, CI/CD, observability tools,      -->
<!-- identity provider.                                                        -->

## 18. Key Decisions and Trade-offs

<!-- One entry per major decision — these seed the ADRs. For each: the         -->
<!-- decision, the realistic alternatives considered, why they were rejected,  -->
<!-- the accepted trade-off/cost, and the conditions under which the decision  -->
<!-- should be revisited. An entry whose alternatives or costs are missing is  -->
<!-- not ADR-ready.                                                            -->

## 19. Risks and Open Questions

<!-- 19.1 Risk register — table with IDs (R1, R2, ...): risk, impact,          -->
<!--      likelihood, mitigation, and which §18 decision or §4 assumption it   -->
<!--      relates to. Distinguish deployment-blocking risks from future        -->
<!--      concerns.                                                            -->
<!-- 19.2 Open questions — table with IDs (Q1, Q2, ...): question, impact if   -->
<!--      unresolved, resolution path (who decides, by when/which phase).      -->
<!--      An open question is honest; a silent gap is a defect.                -->

## 20. Delivery Considerations

<!-- How the system will be built: vertical-slice delivery                     -->
<!-- (ai/guides/vertical-slice-definition.md), build order rationale (riskiest -->
<!-- assumptions first), team/capacity constraints from §4.3, and what each    -->
<!-- phase/milestone must prove. Detailed planning belongs to                  -->
<!-- architecture/delivery-plan.md — this section records only what the        -->
<!-- architecture imposes on delivery.                                         -->
