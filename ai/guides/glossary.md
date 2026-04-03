# Glossary

Single source of truth for load-bearing terms used across this toolkit.
Every prompt, agent, skill, workflow, and guide should interpret these terms
consistently.

---

## Architecture & Structure

### Modular Monolith

A single deployable unit with explicitly defined internal module boundaries that
enforce encapsulation. Modules communicate through well-defined interfaces.
Data ownership is per-module. The system is deployed as one unit but organized as
if modules could be extracted.

**Key distinction:** A modular monolith is NOT a ball-of-mud monolith (no
boundaries) and NOT microservices (multiple deployments). It sits between the
two.

See [`ai/guides/modular-monolith-definition.md`](modular-monolith-definition.md)
for full guidance.

### Bounded Context / Domain Boundary

A boundary around a cohesive area of domain responsibility. Defined by three
properties: domain responsibility (what business capability it owns), data
ownership (what data it is the single source of truth for), and communication
pattern (how it exposes its capabilities to the outside).

**Key distinction:** A boundary is NOT a layer, a file location, or a framework
concern (e.g., "the controller layer" is not a bounded context).

### Cross-Cutting Concern

A concern that spans multiple modules or slices but is not owned by any single
one. Enumerated list: security, observability, error handling, validation,
caching, configuration.

**Handling guidance:** Cross-cutting concerns are delivered as shared
infrastructure (middleware, base classes, interceptors) but configured or
integrated per-module/per-slice. They are legitimate targets for **phases**, not
slices.

### Production-Grade

Minimum qualities a system must possess before it can serve real users:
scalability, security, observability, resilience, documentation, and monitoring.
A system is NOT production-grade if any of these are absent or untested.

---

## Delivery & Decomposition

### Milestone

A container for multiple slices that marks a release boundary, review boundary,
or stakeholder checkpoint. A milestone is a scheduling and communication unit,
not a work unit.

**Key distinction:** A milestone contains slices; a milestone is not a slice.

### Slice (Vertical Slice)

An end-to-end capability that proves a user or operator workflow through all
necessary layers — data, backend logic, API surface, and minimal UI — within a
single deliverable unit.

See [`ai/guides/vertical-slice-definition.md`](vertical-slice-definition.md) for
the full definition, verticality test, and anti-patterns.

### Feature Spec

A detailed specification of ONE slice. The bridge between the delivery plan and
decomposition. Defines scope, acceptance criteria, contracts, flows, data
requirements, security, observability, and test implications for a single
capability.

**Key distinction:** A feature spec covers exactly one slice. If it covers
multiple slices, split it.

### Part

The smallest independently verifiable unit of work within a slice. The TDD
execution target consumed by `part-executor-tdd`.

**Key distinction:** A Part is smaller than a slice. A slice decomposes into
multiple Parts. Each Part produces a working, testable increment.

### Phase

Precondition work (infrastructure, hardening) that enables slices but does not
directly serve users. Examples: CI/CD setup, database provisioning, security
hardening, monitoring dashboards.

**Key distinction:** A phase is NOT a slice. Phases do not pass the verticality
test and should be labelled as phases, not slices.

### Decomposition-Ready

A slice or feature spec is decomposition-ready when: scope is bounded, acceptance
criteria are binary (pass/fail), target files are known, estimated effort is 1–3
sessions per Part, no architectural unknowns remain, and a verification strategy
is defined.

### Independently Verifiable

A Part is independently verifiable when: it can be verified once its declared
prior dependencies are met, it does not depend on future Parts, its tests can run
without manual setup, and it makes no implicit state assumptions.

### Scope Creep

Additions not in the original PART_SPEC or feature spec.

**Distinguished from:**
- Bug fixes discovered during implementation — not creep; fix and document.
- Edge cases already implied by acceptance criteria — not creep.
- New requirements — creep; escalate, do not implement.

---

## Contracts & Integration

### Contract / API Contract

The complete, testable agreement between a producer and a consumer. Includes
schema (request/response shapes), behavior (expected outcomes, error codes,
idempotency), and non-functional expectations (latency, availability).

See [`ai/guides/contract-definition.md`](contract-definition.md) for full
guidance.

### Contract Test

A test that validates an implementation against its declared contract. Runs in
CI. Covers both schema correctness (types, required fields) and behavioral
correctness (valid/invalid input handling, error codes, idempotency).

### Architecture Compliance

Explicit verification that work conforms to the approved architecture and ADRs.
Performed during review. A compliance failure is a detected violation.

### Architecture Drift

Unintended, undocumented movement away from the approved architecture. A
compliance failure is a detected violation; drift is the gradual, undetected
accumulation of many small violations.

**Key distinction:** Drift ≠ a single failure. Drift is the pattern of many
undetected failures over time.

---

## Human Interaction

### Human-in-the-Loop

A human decision is required before the system proceeds. Three tiers:

| Tier | Description | Examples |
|---|---|---|
| **Mandatory (same-slice UI required)** | Approval/override decisions, emergency controls, compliance actions | Approve job posting, override AI recommendation, trigger emergency stop |
| **Context-dependent** | Monitoring dashboards, alert triage, review queues | Review flagged content, triage alerts, inspect analytics |
| **Not in-the-loop** | Read-only reporting, async notifications, batch summaries | Email digest, weekly report PDF, notification badge |

### End-to-End

For slices with UI: user interaction through to persistence and back, including
error paths. For automated slices: external trigger through to observable
outcome. Always includes error paths.

**Key distinction:** End-to-end is not "API responds 200." It is the full round
trip including persistence, side effects, and error handling.

---

## Testing & Quality

### Golden Dataset

A curated collection of (input, expected output) pairs for validating AI or
business logic behavior. Version-controlled. CI-enforced.

### Golden Scenario

A single case within a golden dataset. One input/output pair with acceptance
criteria.

### TDD (Test-Driven Development)

Red-green-refactor cycle applied to all behavioral changes.

**Behavioral change:** any code that changes observable output, return values,
side effects, or error behavior.

**NOT behavioral:** formatting, renaming, import reordering, configuration
changes.

### Integrated Slice Verification

Mandatory browser-based check after all Parts in a UI slice complete. Confirms
the slice works end-to-end in the running application, not just in tests.
Performed during engineering workflow Step 6b using the Slice Completion
Verification Checklist (`ai/templates/slice-verification-checklist-template.md`).

### Browser-Based Verification

Verification requiring the application to be running and accessed through a
browser or browser automation tool (e.g., Playwright, Cypress). Distinguished
from component tests and integration tests, which do not require a running
application or real browser rendering.

### Visual Regression

Unintended changes to the appearance or layout of previously completed UI
surfaces caused by new code. Detected during cross-slice regression checks
in the Integrated Slice Verification step.

**"Not feasible" exceptions:** UI layout, third-party SDK behavior,
infrastructure config. Must be documented and replaced with alternative
verification.

### Acceptable Risk

Risk mitigated to a documented level, owned by a named stakeholder, and recorded
in a risk register or ADR.

**Unresolved critical risk:** a risk that could cause production failure, data
loss, or compliance violation and has no documented mitigation. Must be resolved
before proceeding.

---

## AI-Specific

### Explainability

Minimum viable explanation of an AI decision: inputs used, rules applied,
confidence score, model version, reasoning trace. Scope varies by domain —
regulated domains require full audit trail; operational domains require
sufficient trace for debugging.

### Deterministic vs Probabilistic

- **Deterministic:** same input always produces same output. Examples: rule
  engines, calculations, threshold checks. Testing: exact assertion.
- **Probabilistic:** output varies. Examples: ML models, LLM generation.
  Testing: confidence bounds, regression thresholds, statistical tests.

### Reference Behavior

What a prototype DOES — workflows, data flows, business rules, algorithms.
Extracted as evidence during architecture work, not adopted as architecture.

**Key distinction:** Reference behavior is observed fact. It is not a design
decision.

### Reference Architecture

How a system IS ORGANIZED — components, boundaries, communication patterns,
technology choices. Designed through architecture work, not extracted from
prototype code.

**Key distinction:** Reference architecture is a design decision. It is not
observed fact.

---

## UI & Design System

### Design System

A shared visual vocabulary that defines the tokens (colors, typography,
spacing, breakpoints), components (buttons, forms, cards, navigation,
feedback), layout patterns, state patterns, and accessibility baseline for
a project's UI surfaces. Documented in `architecture/design-system.md`.

**Key distinction:** A design system is a project output, not a toolkit
constraint. It is created per project — either greenfield
(`ai/workflows/ui-foundation-workflow.md`) or derived from existing code
(`ai/workflows/ui-retrofit-workflow.md`).

### Design Token

An atomic visual value (color hex code, font size in px, spacing value,
breakpoint width) that is named and reused across UI components. Tokens are
the smallest unit of the design system.

**Key distinction:** A token is a named value, not a CSS variable or
implementation detail. The name is semantic (e.g., `--color-primary`), not
descriptive (e.g., `--blue-500`).

### UI Inventory

A comprehensive catalog of all existing UI surfaces, components, styling
patterns, and design token values in a project's codebase. Used as the
starting input for the retrofit track.

**Key distinction:** An inventory reports what exists. It does not propose
changes.

### Retrofit Slice

A vertical slice whose purpose is to migrate an existing UI surface from
ad-hoc styling to the approved design system. Retrofit slices are
behavior-preserving — all existing tests must pass unchanged after migration.

**Key distinction:** A retrofit slice changes appearance, not behavior. If
behavior must change, that is a separate feature slice.

### Behavior-Preserving Migration

A change to UI implementation (tokens, components, layout) that does not
alter observable behavior. Verified by green-to-green TDD: existing tests
pass before and after each migration step.

**Key distinction:** If a test needs modification to accommodate the change,
the change is not behavior-preserving and should be handled as a feature
change, not a retrofit.
