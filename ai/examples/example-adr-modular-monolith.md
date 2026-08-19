# Example ADR — Modular Monolith with Vertical Slice Architecture

<!-- EXAMPLE ONLY. This is a sample ADR showing the expected shape and depth. -->
<!-- It is NOT a decision for your project. Real ADRs are generated per       -->
<!-- project via ai/prompts/adr-generator.md into architecture/adr/ and       -->
<!-- become authoritative only there.                                          -->

## Status
Accepted

## Context
The system needs to evolve from prototype to product while keeping operational complexity manageable.

## Decision
Adopt a modular monolith architecture with vertical slice organization by default.

## Alternatives Considered
- Microservices
- Large layered monolith

## Consequences

### Positive
- lower operational complexity
- stronger feature ownership
- better fit for incremental delivery

### Negative
- requires discipline to preserve boundaries
