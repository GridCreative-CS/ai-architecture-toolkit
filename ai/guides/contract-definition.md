# Contract Definition

## Purpose

This document defines what a contract is, how it is layered, how it is tested,
how it is versioned, and what constitutes a violation. It is the reference guide
for API design, integration, and contract testing in this toolkit.

---

## Definition

A **contract** is the complete, testable agreement between a producer and a
consumer. It includes schema, behavior, and versioning. A contract is not just a
type signature — it is the full set of guarantees a producer makes to its
consumers.

---

## Three Layers of a Contract

### 1. Schema Contract

Request/response shapes, field types, required vs optional fields, enum values.

**Declared via:** OpenAPI spec, typed interfaces (C#), JSON schema, protobuf.

**Example:** `POST /api/jobs` accepts `{ title: string (required), description: string (optional) }` and returns `{ id: guid, title: string, status: string }`.

### 2. Behavioral Contract

Expected outcomes for valid/invalid inputs, error codes and shapes (RFC 9457,
which obsoletes RFC 7807), idempotency guarantees, side effects (events
published, records created), ordering guarantees.

**Declared via:** contract test assertions, documented in feature spec §7.

**Example:** `POST /api/jobs` with a duplicate title returns `409 Conflict` with
a ProblemDetails body. Retrying the same POST with the same idempotency key
returns the original resource without creating a duplicate.

### 3. Non-Functional Contract

Latency expectations (P95 target), availability, rate limits, timeout behavior.

**Declared via:** SLO definitions, API Gateway configuration.

**Example:** `GET /api/jobs/{id}` responds within 200ms at P95 under normal load.

---

## How Contracts Are Declared

| Contract Type | Declaration Mechanism |
|---|---|
| Interface contract | C# interfaces (`ICrmAdapter`, `ILinkedInService`) with typed inputs/outputs |
| API contract | OpenAPI specification, versioned |
| Event contract | Message schema + topic/queue name + ordering guarantee |
| Webhook contract | Payload schema + HMAC signature scheme + retry policy |

---

## Contract Tests

A **contract test** validates that an implementation conforms to its declared
contract. Contract tests:

- Run in CI on every PR.
- Test BOTH schema (correct types, required fields) AND behavior (correct
  responses to valid/invalid input, correct error codes, idempotency).
- For adapter interfaces: use a shared test suite that all implementations must
  pass. For example, all `ICrmAdapter` implementations run the same contract
  test suite.

---

## Contract Versioning

| Change Type | Compatibility | Action Required |
|---|---|---|
| **Additive** (new optional fields, new endpoints) | Backward-compatible | No version bump required |
| **Breaking** (removed fields, changed types, changed behavior) | NOT backward-compatible | Version bump, migration strategy, Architecture Board review |

- Contract version is tracked in the API path or header, not in the payload.
- Old versions remain available during the migration window.
- Deprecation includes a documented timeline for consumer migration.

---

## What Constitutes a Violation

A contract violation occurs when a producer changes behavior in a way that
breaks consumer expectations without following the versioning process:

| Violation | Why It Breaks Consumers |
|---|---|
| Adding a required field without a default value | Existing consumers cannot construct valid requests |
| Changing response type or shape | Existing consumers cannot parse responses |
| Changing error codes for existing scenarios | Consumer error handling logic breaks |
| Changing idempotency guarantee | Consumer retry logic produces unexpected results |
| Exceeding declared latency target without documented justification | Consumer timeout and resilience logic becomes insufficient |

---

## Relationship to the Toolkit

| Toolkit Component | How Contracts Are Used |
|---|---|
| Feature spec §7 (API / Contract Expectations) | Where contracts are first declared |
| Contract tests | Where contracts are enforced in CI |
| Architecture compliance | Where contract drift is detected during review |
| Integration reviewer | Who verifies cross-slice contract compatibility |

---

## How This Document Is Used

- **Glossary** (`ai/guides/glossary.md`) — references this document for the
  "contract" and "contract test" entries.
- **Feature spec generator** (`ai/prompts/feature-spec-generator.md`) — §7
  should declare contracts following this structure.
- **Backend agent** (`ai/agents/backend-agent.md`) — must implement and test
  contracts as defined.
- **Integration reviewer** (`ai/agents/integration-reviewer.md`) — must verify
  cross-slice contract compatibility against this definition.
