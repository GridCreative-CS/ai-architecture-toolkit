# Plan: Toolkit Glossary & Definition Guides

## TL;DR

The Atlas AI toolkit uses ~20 load-bearing terms as decision-making anchors across prompts, agents, skills, workflows, and guides — but never defines them explicitly. An LLM (or human) can reasonably interpret them differently, causing the same class of structural failure as the "vertical slice" misapplication. This plan adds: (1) a central glossary covering all terms, (2) a dedicated modular monolith guide, (3) a dedicated contract definition guide, and (4) good/bad pattern examples for the highest-risk concepts.

## Scope

- **In scope:** New reference documents and examples for under-defined toolkit terms
- **Out of scope:** Vertical slice fixes (covered by separate plan), delivery plan corrections, code changes, architecture changes

---

## Steps

### Phase 1 — Create the central glossary

**Create: `ai/guides/glossary.md`**

A single reference document defining all load-bearing terms. Organized by category. Each term gets:
- A concise 2–5 line definition
- The key distinction or boundary that prevents misinterpretation
- Cross-reference to the dedicated guide if one exists

**Terms to define (grouped):**

**Architecture & structure:**
- **modular monolith** — 2-line definition + reference to dedicated guide
- **bounded context / domain boundary** — what constitutes a boundary (domain responsibility + data ownership + communication pattern), what does NOT (layer, file location, framework concern)
- **cross-cutting concern** — enumerated list (security, observability, error handling, validation, caching, configuration) + handling guidance (shared infrastructure vs per-slice implementation)
- **production-grade** — minimum qualities (scalability, security, observability, resilience, documentation, monitoring)

**Delivery & decomposition:**
- **milestone** — container for multiple slices; marks a release or review boundary
- **slice** (vertical slice) — 1-line definition + reference to `ai/guides/vertical-slice-definition.md`
- **feature spec** — detailed specification of ONE slice; the bridge between delivery plan and decomposition
- **part** — smallest independently verifiable unit of work within a slice; the TDD execution target
- **phase** — precondition work (infrastructure, hardening) that enables slices but doesn't directly serve users
- **decomposition-ready** — criteria: scope bounded, acceptance criteria binary, target files known, estimated 1–3 sessions per Part, no architectural unknowns, verification strategy defined
- **independently verifiable** — can be verified once prior dependencies are met; does not depend on future Parts; tests can run without manual setup; no implicit state assumptions
- **scope creep** — additions not in original PART_SPEC or feature spec; distinguished from: bug fixes discovered during implementation (not creep — fix and document), edge cases already implied by acceptance criteria (not creep), new requirements (creep — escalate, do not implement)

**Contracts & integration:**
- **contract / API contract** — 2-line definition + reference to dedicated guide
- **contract test** — test that validates an implementation against its declared contract; runs in CI; covers schema + behavior
- **architecture compliance** — explicit verification that work conforms to approved architecture and ADRs
- **architecture drift** — unintended, undocumented movement away from approved architecture; a compliance failure is a detected violation; drift is the gradual, undetected accumulation

**Human interaction:**
- **human-in-the-loop** — human decision required before system proceeds; three tiers:
  - **Mandatory (same-slice UI required):** approval/override decisions, emergency controls, compliance actions
  - **Context-dependent:** monitoring dashboards, alert triage, review queues
  - **Not in-the-loop:** read-only reporting, async email notifications, batch summary consumption
- **end-to-end** — for slices with UI: user interaction through to persistence and back; for automated slices: external trigger through to observable outcome; always includes error paths

**Testing & quality:**
- **golden dataset** — curated collection of (input, expected output) pairs for validating AI or business logic behavior; version-controlled; CI-enforced
- **golden scenario** — a single case within a golden dataset; one input/output pair with acceptance criteria
- **TDD (test-driven development)** — red-green-refactor cycle; applies to all behavioral changes; "behavioral change" = any code that changes observable output, return values, side effects, or error behavior; NOT: formatting, renaming, import reordering, configuration changes; "not feasible" exceptions: UI layout, third-party SDK behavior, infrastructure config — must be documented and replaced with alternative verification
- **acceptable risk** — risk mitigated to documented level + owned by named stakeholder + recorded in risk register or ADR; **unresolved critical risk** = could cause production failure, data loss, or compliance violation and has no documented mitigation

**AI-specific:**
- **explainability** — minimum viable explanation of an AI decision: inputs used, rules applied, confidence score, model version, reasoning trace; scope varies by domain — regulated domains require full audit trail; operational domains require sufficient trace for debugging
- **deterministic vs probabilistic** — deterministic: same input always produces same output (rule engines, calculations, threshold checks); probabilistic: output varies (ML models, LLM generation); testing strategy differs: deterministic = exact assertion; probabilistic = confidence bounds, regression thresholds, statistical tests
- **reference behavior** — what a prototype DOES (workflows, data flows, business rules, algorithms); extracted as evidence, not as architecture
- **reference architecture** — how a system IS ORGANIZED (components, boundaries, communication patterns, technology choices); designed, not extracted from prototype code

---

### Phase 2 — Create dedicated modular monolith guide

**Create: `ai/guides/modular-monolith-definition.md`**

Covers:
- **Definition:** A single deployable unit with explicitly defined internal module boundaries that enforce encapsulation. Modules communicate through well-defined interfaces. Data ownership is per-module. The system is deployed as one unit but organized as if modules could be extracted.
- **Module boundary rules:**
  - Each module owns its data (tables, schemas). No cross-module direct DB access.
  - Modules expose public interfaces (services, events). Internal types are not shared.
  - Module-to-module communication: in-process via interface calls or in-process events. No HTTP between modules within the same deployment.
  - Shared kernel (common types, value objects, base classes) is minimal and explicit.
- **What prevents ball-of-mud:** Module visibility rules (C#: internal by default, public only for contracts), dependency rules (no circular module references), data ownership (no shared tables).
- **What prevents over-engineering into microservices:** Single deployment, single database (with schema-per-module), in-process communication. Don't add network boundaries, separate databases, or independent deployments until scaling evidence demands it.
- **When to extract a module to an independent service:** When it needs independent scaling, independent deployment cadence, or a different technology stack. Document the decision in an ADR.
- **Relationship to domain clusters:** In Atlas, the architecture uses Domain Service Clusters (4 deployment units). This is NOT a modular monolith — it's a bounded-service architecture. The modular monolith guidance applies WITHIN each cluster: services within a cluster are modules in a monolith, not independent microservices.
- **Anti-patterns:** Distributed monolith (multiple deployments but tightly coupled), monolithic module (one module doing everything), shared database without ownership, circular dependencies between modules.

---

### Phase 3 — Create dedicated contract definition guide

**Create: `ai/guides/contract-definition.md`**

Covers:
- **Definition:** A contract is the complete, testable agreement between a producer and a consumer. It includes schema, behavior, and versioning.
- **Three layers of a contract:**
  1. **Schema contract:** Request/response shapes, field types, required vs optional fields, enum values. Declared via: OpenAPI spec, typed interfaces (C#), JSON schema, protobuf.
  2. **Behavioral contract:** Expected outcomes for valid/invalid inputs, error codes and shapes (RFC 7807), idempotency guarantees, side effects (events published, records created), ordering guarantees. Declared via: contract test assertions, documented in feature spec §7.
  3. **Non-functional contract:** Latency expectations (P95 target), availability, rate limits, timeout behavior. Declared via: SLO definitions, API Gateway configuration.
- **How contracts are declared:**
  - Interface contracts: C# interfaces (`ICrmAdapter`, `ILinkedInService`) with typed inputs/outputs
  - API contracts: OpenAPI specification, versioned
  - Event contracts: message schema + topic/queue name + ordering guarantee
  - Webhook contracts: payload schema + HMAC signature scheme + retry policy
- **Contract tests:**
  - A contract test validates that an implementation conforms to its declared contract
  - Runs in CI on every PR
  - Tests BOTH schema (correct types, required fields) AND behavior (correct responses to valid/invalid input, correct error codes, idempotency)
  - For adapter interfaces: shared test suite that all implementations must pass (e.g., all `ICrmAdapter` implementations run the same contract test suite)
- **Contract versioning:**
  - Additive changes (new optional fields, new endpoints): backward-compatible, no version bump required
  - Breaking changes (removed fields, changed types, changed behavior): require version bump, migration strategy, and Architecture Board review
  - Contract version tracked in API path or header, not in payload
- **What constitutes a violation:**
  - Adding a required field without default value
  - Changing response type or shape
  - Changing error codes for existing scenarios
  - Changing idempotency guarantee
  - Exceeding declared latency target without documented justification
- **Relationship to toolkit:**
  - Feature spec §7 (API / Contract Expectations) = where contracts are first declared
  - Contract tests = where contracts are enforced
  - Architecture compliance = where contract drift is detected
  - Integration reviewer = who verifies cross-slice contract compatibility

---

### Phase 4 — Create good/bad pattern examples

**Create: `ai/examples/modular-monolith-patterns.md`**

Side-by-side examples:
- **BAD:** Module A directly queries Module B's database tables
- **GOOD:** Module A calls Module B's public interface; Module B returns a DTO
- **BAD:** Shared `DbContext` across all modules with 50 `DbSet` properties
- **GOOD:** Per-module `DbContext` with only that module's entities; shared kernel for value objects only
- **BAD:** All types are `public`; any module can instantiate any other module's internal class
- **GOOD:** Internal by default; only contracts (interfaces, DTOs, events) are public

**Create: `ai/examples/contract-patterns.md`**

Side-by-side examples:
- **BAD:** Contract test only checks HTTP 200 response status
- **GOOD:** Contract test checks response schema, error codes for invalid input, idempotency on retry
- **BAD:** Breaking change deployed without version bump; consumers fail silently
- **GOOD:** New version added alongside old version; consumers migrated; old version deprecated with timeline
- **BAD:** "Contract" is just a C# interface with no behavioral specification
- **GOOD:** Interface + contract test suite + documented behavior in feature spec §7

---

### Phase 5 — Wire glossary into existing toolkit files

**Modify (add cross-references only, no structural changes):**

| File | Change |
|---|---|
| `.github/copilot-instructions.md` | Add: "See `ai/guides/glossary.md` for definitions of key terms used throughout the toolkit." |
| `ai/guides/conversation-summary.md` | Add reference to glossary for "modular monolith" and "vertical slices" |
| `ai/guides/definition-of-ready-and-done.md` | Add reference to glossary for "decomposition-ready", "independently verifiable", "scope creep", "architecture drift" |
| `ai/prompts/delivery-planner.md` | Add: "Consult `ai/guides/glossary.md` for precise definitions of key terms." |
| `ai/prompts/feature-spec-generator.md` | Add: "Consult `ai/guides/glossary.md` for term definitions, especially: contract, human-in-the-loop, end-to-end." |
| `ai/agents/backend-agent.md` | Add reference to glossary for "bounded context", "contract" |
| `ai/agents/orchestrator-agent.md` | Add reference to glossary for "slice", "milestone", "human-in-the-loop" |
| `.github/skills/plan-decomposer/SKILL.md` | Add reference to glossary for "independently verifiable", "scope creep", "decomposition-ready" |
| `.github/skills/part-executor-tdd/SKILL.md` | Add reference to glossary for "TDD", "scope creep", "contract" |

---

## Relevant files

| File | Action | What |
|---|---|---|
| `ai/guides/glossary.md` | **CREATE** | Central glossary of all load-bearing terms |
| `ai/guides/modular-monolith-definition.md` | **CREATE** | Dedicated guide: module boundaries, anti-patterns, extraction criteria |
| `ai/guides/contract-definition.md` | **CREATE** | Dedicated guide: three contract layers, testing, versioning, violations |
| `ai/examples/modular-monolith-patterns.md` | **CREATE** | Good/bad module boundary examples |
| `ai/examples/contract-patterns.md` | **CREATE** | Good/bad contract examples |
| `.github/copilot-instructions.md` | **MODIFY** | Add glossary reference |
| `ai/guides/conversation-summary.md` | **MODIFY** | Add glossary cross-reference |
| `ai/guides/definition-of-ready-and-done.md` | **MODIFY** | Add glossary cross-references |
| `ai/prompts/delivery-planner.md` | **MODIFY** | Add glossary reference |
| `ai/prompts/feature-spec-generator.md` | **MODIFY** | Add glossary reference |
| `ai/agents/backend-agent.md` | **MODIFY** | Add glossary cross-reference |
| `ai/agents/orchestrator-agent.md` | **MODIFY** | Add glossary cross-reference |
| `.github/skills/plan-decomposer/SKILL.md` | **MODIFY** | Add glossary cross-reference |
| `.github/skills/part-executor-tdd/SKILL.md` | **MODIFY** | Add glossary cross-reference |

## Verification

1. Read glossary.md and verify every HIGH-severity term has a definition with clear boundary/distinction
2. Read modular-monolith-definition.md and verify it answers: "when is it NOT a modular monolith?" and "when should I extract?"
3. Read contract-definition.md and verify it answers: "what is a violation?" and "how do I test it?"
4. Grep toolkit files for "glossary" reference and verify all key files link to it
5. Run the delivery planner prompt with updated references against the Atlas architecture — verify it uses glossary definitions consistently

## Decisions

- One glossary file, not 20 separate definition files — keeps cross-referencing simple
- Two dedicated guides (modular monolith, contract) because these terms are too complex for 4-line glossary entries
- Examples are separate files in `ai/examples/` to keep guides focused on definitions
- Cross-references are lightweight (one line per file) — no structural changes to existing toolkit files
- Terms that are already well-defined elsewhere (vertical slice, phase vs slice) get glossary entries that reference the existing definition document

## Execution order

Phases 1–4 can be executed in parallel (independent document creation). Phase 5 depends on Phase 1 completion (needs the glossary path to reference).
