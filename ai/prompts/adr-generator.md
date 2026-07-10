# ADR Generator Prompt

Act as a **Principal Software Architect responsible for documenting
architectural decisions**.

## Inputs

- `architecture/architecture-final.md`
- `architecture/adr/*.md` (existing ADRs, to avoid duplication)

## Objective

Generate Architecture Decision Records (ADRs) from the final architecture.
Each ADR captures one major decision, its context, alternatives, and
consequences.

## Methodology

### 1. Identify decisions

Scan the final architecture for significant decisions across these areas:

- system architecture style (e.g., modular monolith, microservices)
- module or slice strategy
- persistence strategy (database selection, schema ownership)
- API strategy (REST, gRPC, versioning approach)
- eventing strategy (sync vs async, message broker selection)
- security model (authentication, authorization, secrets management)
- observability approach (logging, metrics, tracing)
- AI explainability approach (if relevant)

### 2. Deduplicate

Check existing ADRs. Do not create a new ADR for a decision already
documented. If an existing ADR needs updating, note the required change in the
new ADR with a `Supersedes: ADR-XXX` reference.

### 3. Write one ADR per decision

Use the template at `ai/templates/adr-template.md` for the output structure.

Each ADR must include:

- **Status** — one of: Proposed, Accepted, Deprecated, Superseded
- **Context** — the architectural forces and constraints that led to the
  decision
- **Decision** — the chosen approach, stated clearly and unambiguously
- **Alternatives Considered** — at least two realistic alternatives with
  reasons for rejection
- **Consequences** — positive, negative, and neutral impacts

### 4. Cross-reference

When one ADR depends on or constrains another, include an explicit reference
(e.g., "This decision depends on ADR-001 (modular monolith)").

## Output

Write each ADR to `architecture/adr/` using the naming convention:

- `ADR-001-<topic>.md`
- `ADR-002-<topic>.md`
- etc.

If the project already has ADRs, match the existing filename casing and
numbering instead of introducing a second scheme.

## Rules

- one major decision per ADR — do not bundle unrelated decisions
- alternatives must be realistic, not straw-man options
- consequences must include at least one negative or trade-off
- do not duplicate decisions already captured in existing ADRs
- do not invent decisions not present in the final architecture

## References

- ADR template: `ai/templates/adr-template.md`
- Glossary: `ai/guides/glossary.md`
