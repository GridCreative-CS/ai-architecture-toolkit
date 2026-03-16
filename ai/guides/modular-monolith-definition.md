# Modular Monolith Definition

## Purpose

This document defines what a modular monolith is, what its module boundary rules
are, and when extraction to an independent service is justified. It is the
reference guide for architecture, delivery, and implementation decisions in this
toolkit.

---

## Definition

A **modular monolith** is a single deployable unit with explicitly defined
internal module boundaries that enforce encapsulation. Modules communicate
through well-defined interfaces. Data ownership is per-module. The system is
deployed as one unit but organized as if modules could be extracted.

---

## Module Boundary Rules

1. **Data ownership:** Each module owns its data (tables, schemas). No
   cross-module direct database access. If Module A needs data from Module B,
   Module A calls Module B's public interface.

2. **Public interface only:** Modules expose public interfaces (services, DTOs,
   events). Internal types are not shared. In C#, types are `internal` by
   default; only contracts (interfaces, DTOs, events) are `public`.

3. **In-process communication:** Module-to-module communication is in-process
   via interface calls or in-process events. No HTTP calls between modules
   within the same deployment unit.

4. **Minimal shared kernel:** Shared types (common value objects, base classes)
   are kept to an explicit, minimal shared kernel. This kernel is a dependency
   of all modules, so every addition must be justified.

---

## What Prevents Ball-of-Mud

| Mechanism | How It Helps |
|---|---|
| Module visibility rules | C#: `internal` by default, `public` only for contracts. Prevents accidental coupling. |
| Dependency rules | No circular module references. Enforced by project structure and build. |
| Data ownership | No shared tables between modules. Each module has its own schema or DbContext. |
| Interface-based communication | Modules depend on abstractions (interfaces), not implementations. |

---

## What Prevents Over-Engineering into Microservices

| Constraint | Why It Matters |
|---|---|
| Single deployment | One deployable unit. No container-per-module, no separate deploy pipelines per module. |
| Single database | One database instance with schema-per-module (or DbContext-per-module). Not separate database servers. |
| In-process communication | Interface calls, not HTTP or gRPC between modules. No network latency, no serialization overhead. |

Do not add network boundaries, separate databases, or independent deployments
until scaling evidence demands it. Document the decision in an ADR if you do.

---

## When to Extract a Module to an Independent Service

Extract only when one or more of the following conditions are met:

1. **Independent scaling** — The module needs to scale independently of the rest
   of the system (e.g., it handles 100x the traffic of other modules).
2. **Independent deployment cadence** — The module needs to be deployed on a
   different schedule than the rest of the system.
3. **Different technology stack** — The module requires a different runtime,
   language, or data store that cannot coexist in the monolith.

Document the extraction decision in an ADR. Include the scaling evidence or
deployment cadence data that justifies the extraction.

---

## Relationship to Domain Service Clusters

In the Atlas architecture, the system uses **Domain Service Clusters** (multiple
deployment units). This is NOT a modular monolith at the top level — it is a
bounded-service architecture.

The modular monolith guidance applies **within each cluster**: services within a
cluster are modules in a monolith, not independent microservices. They share a
deployment unit, communicate in-process, and follow the module boundary rules
above.

---

## Anti-Patterns

| Anti-Pattern | Description | Fix |
|---|---|---|
| **Distributed monolith** | Multiple deployments but tightly coupled — one module cannot be deployed without the others. Network calls between modules that were previously in-process. | Merge back into a single deployment, or properly decouple with async events and independent data stores. |
| **Monolithic module** | One module doing everything. No internal boundaries, no separation of concerns. | Identify domain boundaries within the module and split into separate modules with clear ownership. |
| **Shared database without ownership** | All modules read/write the same tables. Schema changes break multiple modules. | Assign table ownership to one module. Other modules access data through the owning module's interface. |
| **Circular dependencies** | Module A depends on Module B, which depends on Module A. | Extract the shared concept into a third module or the shared kernel. Restructure the dependency graph. |

---

## How This Document Is Used

- **Glossary** (`ai/guides/glossary.md`) — references this document for the
  "modular monolith" entry.
- **Architecture designer** (`ai/prompts/architecture-designer.md`) — should
  apply these boundary rules when designing module structure.
- **Backend agent** (`ai/agents/backend-agent.md`) — must respect module
  boundaries during implementation.
- **Architecture compliance** (`ai/prompts/architecture-compliance.md`) — should
  check for anti-patterns listed here during review.
