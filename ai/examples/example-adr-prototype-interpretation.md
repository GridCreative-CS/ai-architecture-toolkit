# Example ADR — Treat Prototype as Reference Behavior, Not Reference Architecture

<!-- EXAMPLE ONLY. This is a sample ADR showing the expected shape and depth. -->
<!-- It is NOT a decision for your project. Real ADRs are generated per       -->
<!-- project via ai/prompts/adr-generator.md into architecture/adr/ and       -->
<!-- become authoritative only there.                                          -->

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
