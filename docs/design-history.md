# Design History

Historical rationale for the AI Architecture Toolkit — the conversation
conclusions that shaped its process. Background reading for toolkit
maintainers, not an operational guide.

## Main problem

You wanted a reusable system for turning prototypes — especially Spark prototypes — into production systems with architecture, planning, and AI-assisted implementation.

## Main conclusion

The process should be framed as:

**prototype → architecture → delivery → feature spec → decomposition → TDD execution**

not as a direct migration workflow.

## Architecture decisions made

- Start with architecture before coding
- Add an explicit review step
- Reconcile the review into a final architecture spec
- Generate ADRs
- Prefer vertical slices over a large horizontal application layer
- Prefer modular monolith by default (see `ai/guides/modular-monolith-definition.md`)
- Keep architecture authoritative for implementation agents

> For precise definitions of "modular monolith," "vertical slice," and other
> key terms, see `ai/guides/glossary.md`.

## Engineering decisions made

- Reuse your existing `plan-decomposer` and `part-executor-tdd`
- Add a delivery-planner step and a per-slice feature-spec step before decomposition
- Use specialist agents (backend/frontend/AI/QA/DevOps) only after the slice is defined and decomposed
- Use an orchestrator and integration review to avoid architectural drift
