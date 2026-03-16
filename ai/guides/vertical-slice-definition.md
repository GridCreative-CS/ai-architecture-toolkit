# Vertical Slice Definition

## Purpose

This document is the single source of truth for what constitutes a vertical
slice in this toolkit. All delivery planning, feature specification, compliance
review, and orchestration must conform to this definition.

---

## What Is a Vertical Slice?

A vertical slice is an **end-to-end capability** that proves a user or operator
workflow through all necessary layers — data, backend logic, API surface, and
minimal UI — within a single deliverable unit.

A completed vertical slice can be **demonstrated to a stakeholder** using a
human-facing workflow, not just verified through automated tests.

---

## What a Vertical Slice Is NOT

- A pipeline stage (e.g., "set up database," "build API layer," "add frontend")
- A backend service without its human-facing workflow
- A database or infrastructure layer delivered in isolation
- A frontend shell delivered separately from its backing capability
- A horizontal layer that spans many capabilities at one tier

---

## The Verticality Test

Every slice in a delivery plan must answer **YES** to all three questions:

1. **User-observable capability** — Does this slice deliver a capability that a
   user, operator, or stakeholder can exercise or observe?
2. **Human-in-the-loop completeness** — If the architecture specifies human
   interaction for this capability (approval, override, review, emergency
   control), does the slice include the minimal UI to prove that loop?
3. **User-facing verification** — Can this slice be called "done" with a
   user-facing verification, not just an API or integration test?

If any answer is NO, the slice must be restructured before proceeding.

---

## Exceptions: Phases vs. Slices

Not all work is a vertical slice. The following are legitimate **phases**, not
slices:

- **Infrastructure bootstrap** — CI/CD pipelines, container orchestration,
  database provisioning, environment setup
- **Production hardening** — performance tuning, security hardening, monitoring
  dashboards, load testing

Label these explicitly as **phases** in the delivery plan. Do not call them
slices.

---

## Anti-Patterns

| Anti-Pattern | Why It Fails | Fix |
|---|---|---|
| Frontend-as-a-slice | Converts all other slices into horizontal backend layers. UI is disconnected from the capability it serves. | Distribute UI work into each capability slice. |
| API-only slice for a human-facing flow | The capability cannot be demonstrated to a stakeholder. Defers the hardest integration risk. | Include the minimal UI that exercises the API. |
| Backend-first, then UI-later | Creates an integration bottleneck. Backend assumptions go untested against real user flows. | Build the thinnest possible UI alongside the backend in the same slice. |
| Database-layer slice | Pure infrastructure with no user-observable outcome. | Fold data work into the slice that first needs it. |
| "Shared services" slice | Builds horizontal infrastructure without proving any capability. | Deliver shared services incrementally as each slice needs them. |

---

## How This Document Is Used

- **Delivery planner** (`ai/prompts/delivery-planner.md`) — must apply the
  verticality test to every generated slice
- **Feature spec generator** (`ai/prompts/feature-spec-generator.md`) — must
  include Human Workflow Surfaces for each spec
- **Engineering workflow** (`ai/workflows/engineering-workflow.md`) — must
  validate delivery plan verticality before proceeding
- **Definition of Ready** (`ai/guides/definition-of-ready-and-done.md`) —
  includes verticality as a readiness criterion
- **Architecture compliance** (`ai/prompts/architecture-compliance.md`) —
  includes verticality assessment in review output
- **Orchestrator agent** (`ai/agents/orchestrator-agent.md`) — must flag slices
  where Frontend Tasks is empty for human-facing capabilities
