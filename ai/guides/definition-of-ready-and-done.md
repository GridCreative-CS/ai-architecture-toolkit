# Definition of Ready and Definition of Done

## Purpose

This document defines when a slice, feature specification, or implementation
part is ready to move forward and when it can be considered done.

---

## Definition of Ready

A slice or feature is ready when all of the following are true:

### Architecture readiness

- The final architecture exists.
- Relevant ADRs exist or the need for them is explicitly known.
- The slice fits inside approved architectural boundaries.
- Known architecture risks are documented.

### Delivery readiness

- The slice appears in the delivery plan.
- Dependencies are identified.
- A preferred implementation order is known.
- Cross-slice impacts are understood.
- The slice passes the verticality test: it includes the minimal human workflow
  surface if the architecture specifies human interaction for this capability
  (see `ai/guides/vertical-slice-definition.md`).

### Specification readiness

- A feature specification exists or the scope is otherwise explicit.
- Scope in and scope out are defined.
- Acceptance criteria are defined.
- If a design system exists (`architecture/design-system.md`), UI acceptance
  criteria reference it (§11b of the feature spec).
- Security and observability requirements are identified.
- Test implications are identified.

### Execution readiness

- The target files or modules are known.
- A concrete execution handoff exists for the selected slice:
  `architecture/feature-specs/<slice-name>.md` and `ai-parts/PXX-*.md`.
- The decomposition target is small enough to be executed safely.
- Verification strategy is known.
- Open questions are either resolved or explicitly recorded.

---

## Definition of Done

A slice, feature, or part is done when all of the following are true:

### Implementation completeness

- The intended behavior is implemented.
- The implementation respects architecture and ADRs.
- No unauthorized architectural drift was introduced.
- If the slice includes a human workflow surface, a user-facing verification
  confirms the end-to-end capability, not just API or integration tests.

### Testing completeness

- Required automated tests are added or updated.
- TDD expectations are satisfied where applicable.
- Regression risk is addressed.
- Golden scenario validation is completed where relevant.

### Quality completeness

- Acceptance criteria are met.
- Verification commands pass.
- No partial refactors are left behind.
- No hidden TODO hacks are introduced.
- If a design system exists, UI surfaces conform to the approved design
  system (tokens, components, patterns, accessibility baseline).

### Review completeness

- Architecture compliance is checked for significant changes.
- Integration review is completed where cross-slice interaction exists.
- Outstanding issues are either fixed or explicitly accepted.
- Architecture-sourced doc comments and test descriptions cite the specific
  owning document (e.g., architecture-final.md section reference, specific ADR
  number). Generic claims like "per architecture" or "per ADR" without a
  traceable citation are not acceptable.

### Operational completeness

- Required logging, metrics, tracing, or monitoring hooks are present.
- Security and authorization constraints are respected.
- Deployment or configuration implications are documented when relevant.
- If the slice adds or modifies an EF Core migration: a `Dockerfile.migrate`
  exists, the migration service is present in `docker-compose.yml` with
  `restart: "no"`, and the API service depends on it with
  `condition: service_completed_successfully`.

---

## Glossary Reference

For precise definitions of terms used in this document — including
"decomposition-ready," "independently verifiable," "scope creep," and
"architecture drift" — see `ai/guides/glossary.md`.
