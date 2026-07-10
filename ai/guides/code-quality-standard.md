# Code Quality Standard

## Purpose

This guide defines the implementation-quality rules that apply to **all
production code and test code** written under this toolkit. It exists so that
any agent — regardless of model capability — produces code that matches the
project's established quality level instead of generic model-generated code.

It is enforced at three points:

1. **During Part execution** — `part-executor-tdd` applies these rules and
   reports against them in the Part Quality Report
   (`ai/templates/code-quality-checklist-template.md`).
2. **During Part code review** — engineering workflow Step 6a
   (`ai/prompts/code-quality-reviewer.md`) verifies them independently.
3. **In agent personas** — the backend, frontend, QA, and integration reviewer
   agents reference this standard.

The rules here are project-agnostic. Stack-specific rules (language idioms,
framework conventions) live in `.github/instructions/` and in the project's
own code — this guide tells you how to find and follow them.

---

## 1. Read before write (mandatory)

Before writing any code for a Part:

1. **Open the nearest existing implementations.** Find at least two files that
   do the same kind of work as the Part (same layer, same artifact type: a
   comparable handler, endpoint, entity, component, API module).
2. **Open the tests for those files.** Test structure, naming, fixtures, and
   assertion style are part of the pattern.
3. **Record the observed pattern** before implementing. At minimum, note how
   the surrounding code handles:
   - file/folder placement and naming
   - error handling and error identifiers
   - validation (where request-shape checks live vs. business rules)
   - logging, metrics, and tracing
   - async and cancellation propagation
   - documentation comments (density, style, what they cite)
   - test naming, test data setup, and assertion style
4. **Follow that pattern.** The existing project pattern beats any pattern the
   model would generate by default. "I know a better way" is not a reason to
   deviate — a deviation needs a justification recorded in the Part Quality
   Report, and anything architectural needs a compliance finding first.

If no comparable code exists yet (first slice, new layer), derive the pattern
from `architecture/architecture-final.md`, the ADRs, and
`.github/instructions/` — and say explicitly in the quality report that the
Part establishes a new pattern.

### The ambiguity rule

If the existing pattern is unclear, inconsistent (two nearby files do the same
thing differently), or conflicts with an instruction file: **stop and list the
ambiguity as an open question.** Do not invent a third style. Pick nothing
until the conflict is resolved or you can state which source wins under the
precedence order below.

## 2. Source precedence

When sources disagree, this order wins (highest first):

1. `architecture/architecture-final.md` and `architecture/adr/*.md`
2. The slice feature spec and the Part file (PART_SPEC)
3. `architecture/design-system.md` (for UI decisions)
4. Project instruction files (`.github/instructions/`, project `CLAUDE.md`)
5. The dominant pattern in nearby project code
6. Generic best practice

A lower source never silently overrides a higher one. If nearby code violates
an ADR, follow the ADR and report the discrepancy — do not copy the violation
and do not "fix" the old code inside an unrelated Part.

## 3. Dependencies

- **No new libraries, packages, or frameworks without explicit justification.**
  A new dependency must be named in the Part Quality Report with: why existing
  project dependencies cannot do the job, and confirmation it does not
  conflict with the architecture. If the Part file does not already call for
  it, treat it as a scope question and escalate before adding it.
- Never add a dependency to work around an unclear pattern.
- Version management follows the project's existing mechanism (e.g., central
  package management) — never pin versions in a new place.

## 4. Abstractions

- **Do not create an abstraction the current Part does not need.** No
  interfaces with a single implementation "for testability" unless that is the
  project's established pattern, no base classes for one subclass, no generic
  parameters with one instantiation, no configuration options nothing reads,
  no wrapper layers "for the future".
- Extract an abstraction when the second concrete usage appears, not before.
- The inverse also holds: if the project already has an abstraction for the
  job (a result type, a repository interface, a shared component), use it —
  do not hand-roll a parallel mechanism.

## 5. Boundaries and dependency direction

- Respect the module and layer boundaries in
  `architecture/architecture-final.md`. Dependencies point inward: domain code
  depends on nothing outer; application code does not depend on transport or
  persistence details; UI consumes contracts, never internals.
- No cross-module data access; use the owning module's public interface.
- When a Part adds or modifies a boundary, the project's architecture tests
  (dependency/layer tests) must cover the new prohibited directions — all of
  them, not a sample.
- If a boundary cannot be respected as specified, stop the Part and raise a
  compliance finding (engineering workflow Step 6 rule). Never "temporarily"
  cross a boundary.

## 6. Error handling

- Follow the project's established error flow. If the project uses a
  result/error type for expected failures, new code uses it too — do not
  introduce exception-based flow beside it (or vice versa).
- **Every externally visible error is identifiable and documented**: a stable
  error identifier (code or type) that the contract, tests, and consumers can
  rely on. Match the format used by existing errors exactly.
- Map failures to the contract's documented status codes / error responses.
  No new error surface without updating the contract.
- Never swallow exceptions, return defaults on failure, or add silent
  fallbacks. A failure the caller cannot observe is a defect.
- Error messages must not leak information the caller is not authorized to
  see (e.g., existence of another tenant's resource).

## 7. Validation

- Two layers, deliberately split:
  - **Shape validation at the boundary** — required fields, ranges, formats;
    rejected with the contract's validation error response.
  - **Business rules in the domain/application layer** — rules that depend on
    state or computed values; rejected with the contract's business-rule error
    response.
- Keep the split where the project keeps it. Do not duplicate a business rule
  into boundary validation or push shape checks into the domain.
- Where a rule is safety- or compliance-critical, an inner defensive check is
  correct even when an outer validator already gates it — the inner layer must
  not crash if the outer layer is bypassed.
- Bounds and limits come from named constants owned by the domain, not
  magic numbers repeated across layers.

## 8. Logging and observability

- Use structured logging with named properties, in the project's existing
  style. Never string-interpolate values into log messages.
- Log identifiers, never sensitive payloads: no PII, secrets, tokens, or
  free-text user content in logs unless the project has an explicit redaction
  mechanism and the log site uses it.
- If the surrounding code emits metrics or traces for comparable operations,
  the new code does too, following the same naming conventions.
- Every new failure path must be observable: a failure that produces no log,
  metric, or trace signal fails review.

## 9. Async and cancellation

- Propagate the platform's cancellation mechanism (e.g., a cancellation token)
  through **every** async call in the chain — from the entry point down to
  I/O. A new async method that accepts no cancellation input, or accepts one
  and drops it, fails review.
- Follow the project's async conventions exactly as observed in nearby code
  (e.g., context-capture settings, no sync-over-async, no fire-and-forget
  outside the project's established background-work pattern).
- Timeouts and retries follow the project's existing resilience mechanism —
  do not hand-roll new ones.

## 10. Test quality

- **Test-first is real, not claimed.** A behavioral change without a failing
  test observed first is not TDD — the Part Quality Report must show the red
  evidence (command + failure), and the reviewer rejects TDD claims without it.
- **Test names state behavior**: what is exercised, under which condition,
  with which observable outcome — in the project's existing naming style.
  A name like `Test1` or `Works` fails review.
- **Test observable behavior, not implementation.** A test that only verifies
  a mock was called, mirrors the implementation's internal steps, or would
  pass with the production logic deleted is not a test — it is a fake. Assert
  on outputs, state transitions, persisted effects, and contract responses.
- **Rule matrices get truth-table tests.** When the spec defines a decision
  table or scoring rule, cover every row with parameterized tests plus the
  boundary values and the invalid-input behavior (including expected error
  types).
- **Contracts get contract tests.** Endpoints and public interfaces lock their
  full observable surface: status codes (success and every documented error),
  response shapes, and error identifiers.
- Do not weaken, delete, or skip an existing test to make new code pass. A
  legitimately obsolete test is removed with justification in the quality
  report.
- UI components follow the project's component-test pattern (including
  accessibility assertions where the project has them); slice-level UI proof
  is browser-based (engineering workflow Step 6b), never unit tests alone.

## 11. Prohibited outputs

None of the following may exist when a Part is reported complete:

- `TODO` / `FIXME` / `HACK` comments, placeholder text, or stub bodies
  (`throw NotImplemented`, empty handlers, hard-coded sample data) in
  production paths
- fake implementations that satisfy the tests without implementing the
  specified behavior
- commented-out code, dead code, unused parameters, unused imports/usings,
  or code only reachable by no caller
- partial refactors (old and new pattern coexisting without a recorded reason)
- copy-pasted blocks that diverge from their source without justification
- silent changes to any contract surface (see §12)
- suppressed warnings or disabled lint/analyzer rules without a recorded
  justification

## 12. Contract surfaces — declare changed or unchanged

Every Part must explicitly account for all four contract surfaces, in the
Part Quality Report:

| Surface | Examples of a change |
| --- | --- |
| **Public API** | new/renamed/removed endpoint, changed request/response shape, changed status code or error identifier, changed auth requirement |
| **Database/schema** | migration, new/renamed column or table, changed constraint, changed data ownership |
| **Events/messages** | new/changed event schema, changed topic/queue, changed ordering or delivery semantics |
| **UI behavior** | changed user-visible flow, changed route, changed rendered contract of a shared component |

"Changed" requires: the feature spec covers it (or a spec reconciliation is
raised), consumers are accounted for, and versioning/backward-compatibility
follows `ai/guides/contract-definition.md`. **A contract change the Part file
and quality report do not declare is a silent contract change — an automatic
review rejection.**

## 13. Documentation in code

- Match the surrounding code's documentation density and style. Where the
  project's instruction files mandate doc comments on public members (the
  .NET default in `.github/instructions/csharp.instructions.md` does), that
  is a hard rule — a new public member without one fails review.
- **Traceability is a hard rule:** when a doc comment or test description
  states an architecture- or spec-sourced rule, it must cite the owning
  source specifically — slice ID, spec section (e.g. "spec §6 rule 4"), ADR
  number, and the stable error code where one applies. Generic phrases like
  "per the architecture" without a traceable citation fail review.
- Comments state constraints the code cannot show; they do not narrate the
  code or justify the change to a reviewer.

---

## Glossary reference

For definitions of "contract," "architecture drift," "scope creep," "TDD,"
and other load-bearing terms, see `ai/guides/glossary.md`.

## Related documents

- Part Quality Report template: `ai/templates/code-quality-checklist-template.md`
- Part code review (Step 6a): `ai/prompts/code-quality-reviewer.md`
- Code reviewer persona: `ai/agents/code-reviewer-agent.md`
- Definition of Ready/Done: `ai/guides/definition-of-ready-and-done.md`
- Contract definition: `ai/guides/contract-definition.md`
