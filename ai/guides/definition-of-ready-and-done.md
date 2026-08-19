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
  `architecture/feature-specs/<slice-id>-<slice-name>.md`,
  `ai-parts/<slice-id>/OVERVIEW.md`, and `ai-parts/<slice-id>/PXX-*.md`.
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
  **in a running application** confirms the end-to-end capability, not just
  API, component, or integration tests. See UI completeness below.

### Testing completeness

- Required automated tests are added or updated.
- TDD expectations are satisfied where applicable.
- Regression risk is addressed.
- Golden scenario validation is completed where relevant.
- For slices with human workflow surfaces, at least one browser-based test
  or documented browser walkthrough confirms the slice works end-to-end in
  the running application.

### Quality completeness

- Acceptance criteria are met.
- Verification commands pass.
- No partial refactors are left behind.
- No hidden TODO hacks are introduced.
- The code follows `ai/guides/code-quality-standard.md`: nearby code and
  tests were read before implementing; existing project patterns were
  followed (or deviations justified); no unneeded dependencies or
  abstractions were added; error handling, validation, logging, and
  async/cancellation match the project's established patterns.
- No prohibited outputs exist (code-quality standard §11): no placeholders,
  stubs, fake implementations, dead/unused code, or commented-out code.
- Tests prove observable behavior — no test passes purely by verifying mocks
  or implementation details; TDD claims are backed by recorded red evidence.
- Tests are behavioral, not structural: each would fail if the implementation
  it covers were removed. Where the slice implements an authorization guard,
  cache invalidation, cancellation, or error→message mapping, a mutation
  check proves it (code-quality standard §10).
- Every acceptance criterion is traced in the Part Quality Report §3b
  requirement coverage matrix — implementation location, positive test,
  negative/edge test, verification evidence, and status. No criterion is
  marked covered on implementation inspection alone.
- All four contract surfaces (public API, database/schema, events/messages,
  UI behavior) are explicitly declared changed or unchanged — no silent
  contract changes.
- If a design system exists, UI surfaces conform to the approved design
  system (tokens, components, patterns, accessibility baseline).

### UI completeness (mandatory for slices with human workflow surfaces)

- The application starts and runs with all required services.
- The user flow from the feature spec §5/§5b is executable in a running
  application via a browser or browser-based test.
- All four interaction states are handled and visible: loading, success,
  error, empty.
- Interactive elements function correctly: buttons, links, forms, navigation.
- The shared layout (header, sidebar, navigation) renders correctly with the
  new slice integrated.
- The slice renders correctly at a minimum of two viewport sizes (mobile and
  desktop).
- Previously completed slices still render and function correctly (no visual
  regression).
- If a design system exists, the UI compliance check passes with no critical
  findings.
- Browser verification evidence is documented in
  `architecture/slice-verification/<slice-id>-<slice-name>.md`.

### Review completeness

- Architecture compliance is checked for significant changes.
- Every executed Part has a completed Part Quality Report
  (`ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`, per
  `ai/templates/code-quality-checklist-template.md`) ending in an explicit
  DONE / NOT DONE statement.
- Every executed Part has a Part code review (engineering workflow Step 6a,
  `ai/prompts/code-quality-reviewer.md`) covering all twelve checks — the ten
  defect checks plus the dimension audit and the requirement coverage audit —
  with a verdict of `APPROVED` or `APPROVED WITH NOTES`. A Part with
  `REJECTED — MUST FIX` is not done until the required fixes are applied and
  re-review approves.
- The review and the quality report describe the same frozen snapshot (base
  commit, committed diff, worktree diff, generated/untracked Part files).
- Every criterion in the slice's feature spec has an owning Part in
  `ai-parts/<slice-id>/OVERVIEW.md`, and no criterion remains `NOT-YET` after
  the slice's final Part.
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
