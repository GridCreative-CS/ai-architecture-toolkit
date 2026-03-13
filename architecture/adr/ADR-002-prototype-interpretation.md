# ADR-002: Treat Prototype as Reference Behavior, Not Reference Architecture

## Status
Accepted

## Context
Prototype repositories often validate ideas but are not good production architecture references.

## Decision
Use prototypes to extract behavior, domain concepts, rules, and workflows.
Do not copy prototype technical structure into production by default.

## Consequences

### Positive
- preserves valuable behavior
- avoids prototype technical debt

### Negative
- requires explicit analysis and architecture work
