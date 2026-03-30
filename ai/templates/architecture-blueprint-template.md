# Architecture Blueprint

<!-- This template structures a production-grade architecture document.        -->
<!-- Fill each section. If a section is not applicable (e.g., no AI            -->
<!-- components), write "Not applicable — [reason]" rather than deleting it.   -->
<!-- Reference: ai/guides/glossary.md for term definitions.                    -->

## 1. Executive Summary

<!-- 3–5 sentence overview: what the system does, who it serves, and the key  -->
<!-- architectural approach (e.g., modular monolith, event-driven).           -->

## 2. Prototype Insights & Extracted Domain Concepts

<!-- Summarize what was learned from the prototype (if one exists).            -->
<!-- List: business rules, workflows, data flows, and domain concepts that    -->
<!-- must be preserved. Cite the prototype analysis if available.              -->
<!-- If no prototype exists, write "No prototype — architecture designed from -->
<!-- requirements."                                                            -->

## 3. System Context and Boundaries

<!-- Describe the system boundary: what is inside the system, what is         -->
<!-- external (users, third-party services, data sources).                     -->
<!-- Include a context diagram or a textual description of actors and          -->
<!-- interfaces.                                                               -->

## 4. Domain Model

<!-- Define the core domain entities, value objects, and their relationships.  -->
<!-- Identify bounded contexts and data ownership per module.                  -->
<!-- Reference: ai/guides/modular-monolith-definition.md for boundary rules.  -->

## 5. Production System Architecture

<!-- Describe the module structure, communication patterns, and deployment     -->
<!-- model. For each module: name, responsibility, exposed contracts, and      -->
<!-- data it owns.                                                             -->
<!-- Reference: ai/guides/contract-definition.md for contract structure.       -->

## 6. AI Decision Architecture

<!-- Describe where AI decisions occur, confidence thresholds, fallback paths, -->
<!-- and human override points. Separate deterministic logic from              -->
<!-- probabilistic model output.                                               -->
<!-- If no AI components exist, write "Not applicable — no AI components."     -->

## 7. Data Architecture

<!-- Describe data stores, schema ownership, migration strategy, and data     -->
<!-- flow between modules. Identify which module owns each data entity.        -->

## 8. Explainability Architecture

<!-- Describe what explanations are produced, where they are stored, and how  -->
<!-- they are surfaced. Define audit trail requirements by domain tier.        -->
<!-- Reference: ai/guides/ai-explainability-guide.md (if available).           -->
<!-- If no AI components exist, write "Not applicable — no AI components."     -->

## 9. Governance & Risk Model

<!-- Describe regulatory constraints, bias monitoring, human-in-the-loop      -->
<!-- controls, and risk thresholds. Reference the glossary for                 -->
<!-- human-in-the-loop tiers.                                                  -->

## 10. Operational Architecture

<!-- Describe deployment topology, health checks, scaling strategy,           -->
<!-- monitoring, alerting, and disaster recovery.                              -->

## 11. Technology Stack Recommendation

<!-- List chosen technologies with rationale. Include: language/framework,    -->
<!-- database, message broker, CI/CD, observability tools.                     -->

## 12. Engineering Delivery Model

<!-- Describe how the system will be built: vertical slices, TDD approach,    -->
<!-- deployment cadence, and team structure assumptions.                        -->
<!-- Reference: ai/guides/vertical-slice-definition.md.                        -->

## 13. Key Architectural Decisions & Trade-offs

<!-- Summarize major decisions and trade-offs. Each should become an ADR.      -->
<!-- For each: state the decision, the trade-off, and the rationale.          -->

## 14. Risks and Mitigation Strategies

<!-- List known risks with likelihood, impact, and mitigation plan.           -->
<!-- Distinguish deployment-blocking risks from future concerns.               -->
