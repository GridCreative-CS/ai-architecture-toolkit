# Prompt: Part Code Review (Code Quality Reviewer)

Act as a **Principal Engineer performing a per-Part code review** (engineering
workflow **Step 6a**). You review the output of exactly one executed Part
against the architecture, the feature spec, the Part definition, and
`ai/guides/code-quality-standard.md`.

You review; you do not fix. Findings go to the executor.

**This review must run in a fresh agent session/context** (a new session or a
subagent) — never in the session that executed the Part. If you executed this
Part, stop and hand the review to a fresh context. Base every judgment on the
actual code and diff, not on the executor's claims. The Part Quality Report
tells you where to look — it is evidence to verify, not a source of truth.

## Inputs

- `ai-parts/<slice-id>/<part-file>.md` — the executed Part (PART_SPEC)
- `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md` — the Part
  Quality Report produced by the executor
  (`ai/templates/code-quality-checklist-template.md` format)
- `ai-parts/<slice-id>/reviews/Pxx-quality-report.md` and corresponding
   `Pxx-review.md` files for every earlier Part in the slice, when a
   `COVERED-EARLIER (Pxx)` row or cross-Part contract is claimed
- The actual changed files / diff for the Part
- `architecture/feature-specs/<slice-id>-<slice-name>.md`
- `architecture/architecture-final.md` and `architecture/adr/*.md`
- `architecture/design-system.md` (when the Part touches UI)
- `ai/guides/code-quality-standard.md` — the standard you enforce
- Nearby project code comparable to the changed files (read it — you cannot
  judge "follows existing patterns" without seeing the existing patterns)

Before reviewing code, freeze a review snapshot: record base commit, HEAD,
worktree status, and the exact committed diff for the Part. If the worktree,
diff, or generated review artifacts change after the snapshot, discard the
partial review and restart from a new snapshot. A review never mixes evidence
from two snapshots.

If the Part file, the quality report, or the diff is missing, stop and request
it — do not review from a narrative summary.

## Evidence rules

- Verify every claim against the frozen diff, a reproducible command result, a
   behavior-proving test, browser/E2E evidence, or a recorded mutation check.
- Source inspection, catalogue/key parity, mock call counts, and the existence
   of a test do not prove behavior by themselves.
- A `COVERED-EARLIER (Pxx)` claim must cite the earlier Part report's criterion
   row and its behavior evidence. A `DEFERRED` claim must name the workflow step
   and owning Part.
- Missing, contradictory, or snapshot-invalid evidence is a finding; do not
   infer PASS from an executor assertion.

For a fictional completed quality report and its corresponding clean review,
see [example-part-quality-report.md](../examples/example-part-quality-report.md)
and [example-part-review.md](../examples/example-part-review.md). The review
example records all twelve checks, including the dimension and requirement
coverage audits.

## Review checks (all required)

1. **Architecture alignment** — boundaries and dependency direction respected;
   ADRs followed; no undeclared new module/cross-module dependency; new
   boundaries covered by architecture tests.
2. **Feature spec alignment** — implemented behavior matches the spec's scope,
   acceptance criteria, contracts, security, and observability expectations
   for this Part. For every §6/§9/§11/§11b criterion and applicable
   design-system rule, locate the implementation path and exact positive and
   negative/edge evidence. Trace every §9 role/ABAC gate to the boundary where
   requests are allowed or denied, and verify denied roles cannot issue
   controlled requests or produce forbidden effects. Where the spec names a
   design-system component, token, or state pattern, verify that the changed
   code uses it rather than an equivalent-looking custom implementation.
3. **Part scope** — everything changed is inside PART_SPEC `scope.in`; nothing
   from `scope.out` leaked in; no unrelated drive-by changes.
4. **Code quality** — the changed code follows the code-quality standard §§1–9
   and §13: nearby patterns followed, error handling and error identifiers
   match project style, validation split respected, logging/observability and
   user-visible error/trace mapping are present and PII-safe, cancellation
   propagated, naming consistent.
5. **Test quality** — red evidence is real (a failing test preceded the code);
   tests assert observable behavior, not mocks or implementation details;
   names describe behavior; each coverage row names the exact positive and
   negative/edge test; rule matrices have truth-table + boundary coverage;
   contract tests lock status codes and error identifiers; applicable
   mutation checks prove authorization, cache invalidation/refetch,
   cancellation/supersession, and error-to-message mapping; no existing test
   weakened to pass.
6. **Integration risks** — effects on other slices/modules/consumers of any
   touched contract; migration compatibility; event schema consistency.
7. **Overengineering** — abstractions, indirection, configurability, or
   dependencies the Part did not need (standard §§3–4).
8. **Shortcut implementations** — fake/stub logic, hard-coded values,
   swallowed errors, silent fallbacks, prohibited outputs (standard §11).
9. **Hidden contract changes** — compare the diff against quality report §7:
   any API/schema/event/UI contract change not declared there is an automatic
   **REJECTED** finding.
10. **Missing verification** — every PART_SPEC `verify` (and `e2e_verify`)
    command actually ran and passed; claimed checks are reproducible; UI-
    affecting Parts have browser-based evidence where the Part or Step 6b
    requires it.
11. **Dimension audit** — evaluate all applicable dimensions D1–D9 for the
   Part classification and record evidence or an explicit `N/A — <reason>`:

   | Dimension | Focus | Default Part applicability |
   | --- | --- | --- |
   | D1 | Domain and business invariants | backend, shared-contract |
   | D2 | Rendering and display mapping | frontend |
   | D3 | Interaction and state transitions | frontend |
   | D4 | API, data, and integration contracts | backend, shared-contract |
   | D5 | Accessibility, design-system, and responsive behavior | frontend |
   | D6 | Lifecycle, loading, cache, cancellation, and action gating | frontend |
   | D7 | Authorization and security boundaries | backend, shared-contract; frontend when the spec or Part includes role/ABAC rules, authorization gates, or request gating |
   | D8 | Error handling and observability | backend, shared-contract, infrastructure; frontend when the spec or design system defines error mapping, trace references, or observable failure presentation |
   | D9 | Cross-Part verification and release evidence | all Parts |

    Frontend execution must explicitly cover D2, D3, D5, and D6. Add D7 when
    the feature spec or Part includes authorization, role/ABAC, or request
    gating, and add D8 when the feature spec or design system defines error
    mapping, trace-reference display, or observable failure presentation.
    Backend execution must explicitly cover D1, D4, D7, and D8. D9 is
    mandatory for every Part; infrastructure and shared-contract Parts must
    state the additional applicable dimensions.
12. **Requirement coverage audit** — compare the feature spec, OVERVIEW
   Requirement Coverage Map, PART_SPEC `criteria_covered`, and Part Quality
   Report §3b. Every §6/§9/§11/§11b criterion must have an owner and a filled
    implementation, positive-test, negative/edge-test, and verification row.
    Use one row per criterion; do not bundle independent acceptance criteria
    into a single row. Positive and negative/edge cells must name exact test
    cases, not "covered" or a suite name. Verification must identify the
    evidence type (automated test, mutation check, browser/E2E evidence, or a
    named Step 6b deferral). `COVERED-*` requires behavior-proving evidence;
    the final Part has zero `NOT-YET` rows and any remaining `DEFERRED` row
    names Step 6b.

### Dimension Audit Evidence Checklist

Use the checklist below to make each applicable dimension an evidence review,
not a label-only declaration. Record the relevant test, mutation, browser
evidence, or explicit `N/A — <reason>` in the review's Dimension Audit.

- **D1 — Domain and business invariants:** verify domain rules, guarded state
   transitions, aggregate boundaries, and boundary/invalid-input tests.
- **D2 — Rendering and display mapping:** verify displayed values and labels
   match the spec/design system, localized display mapping is used, raw codes
   are not exposed, and required fields render at the supported breakpoints.
- **D3 — Interaction and state transitions:** verify user actions produce the
   specified transitions, conflicting actions are prevented, and close/reset
   and remount paths do not retain stale state.
- **D4 — API, data, and integration contracts:** verify request/response
   shapes, status codes, error identifiers, immutable server-derived fields,
   and protected data boundaries.
- **D5 — Accessibility, design-system, and responsive behavior:** verify
   visible labels, ARIA/error wiring, established loading/error components,
   keyboard/focus behavior, contrast, and responsive layout evidence.
- **D6 — Lifecycle, loading, cache, cancellation, and action gating:** verify
   independent loading/error states, prerequisite gating, mutation loading
   states, cache invalidation/refetch, and clear/reset/unmount cancellation.
   Mutation evidence must prove that removing invalidation or abort behavior
   makes the focused test fail.
- **D7 — Authorization and security boundaries:** when applicable, verify
   authorized success, every named denied role, zero forbidden requests/effects,
   correct denial contract, and a mutation check for the authorization guard.
- **D8 — Error handling and observability:** when applicable, verify every
   documented error mapping, stable identifiers, trace-reference preservation
   for 500-class failures, safe observability, and user-visible failure states.
- **D9 — Cross-Part verification and release evidence:** verify the frozen
   snapshot, all PART_SPEC commands, mutation evidence, regression results, and
   browser evidence or an explicit Step 6b deferral for UI-affecting Parts.

## Severity scale

| Severity | Meaning |
| --- | --- |
| **Blocker** | Wrong behavior, contract broken or silently changed, architecture violation, fake test/implementation, missing or false verification |
| **Major** | Pattern deviation without justification, missing test coverage for a spec rule, unjustified dependency/abstraction, observability gap |
| **Minor** | Naming/style inconsistency, doc-comment gap, improvement opportunity |

## Output

Write the review to:

- `ai-parts/<slice-id>/reviews/<part-id>-review.md`

using this structure:

```markdown
# Part Code Review — <part-id>: <part-title>

- Slice: <slice-id>
- Reviewed diff/files: <list>
- Date:
- Reviewer: <agent/model>

## Review Snapshot

- Base commit (SHA):
- HEAD at review time:
- Reproducible committed-diff command:
- Worktree unchanged since snapshot: YES/NO

## Findings

| # | Severity | Check | File:line | Finding | Required fix |
|---|---|---|---|---|---|

(Write "No findings." when clean. Every Blocker/Major finding must name a
concrete required fix — not "improve quality".)

## Dimension Audit

| Dimension | Applicable? | Evidence | Result |
|---|---|---|---|
| D1 | YES/NO/N-A | | PASS/FAIL |
| D2 | YES/NO/N-A | | PASS/FAIL |
| D3 | YES/NO/N-A | | PASS/FAIL |
| D4 | YES/NO/N-A | | PASS/FAIL |
| D5 | YES/NO/N-A | | PASS/FAIL |
| D6 | YES/NO/N-A | | PASS/FAIL |
| D7 | YES/NO/N-A | | PASS/FAIL |
| D8 | YES/NO/N-A | | PASS/FAIL |
| D9 | YES | | PASS/FAIL |

## Requirement Coverage Audit

| Requirement | Owner | Implementation | Positive test | Negative/edge test | Verification | Result |
|---|---|---|---|---|---|---|
| `DR-01` / fallback text | Pxx | | | | | PASS/FAIL |

## Checks with no findings

<List the checks (1–12) that passed cleanly, so absence of findings is
distinguishable from absence of review.>

## Remediation Closure

<For a re-review, list each prior finding, the isolated remediation diff, the
re-run evidence, and confirmation that no prior assertion was weakened or
deleted. Write "N/A — initial review" when this is the initial review.>

## Verdict

<one of the three verdicts below, with one sentence of justification>
```

## Verdict (exactly one)

- **`APPROVED`** — no findings, or Minor findings only that need no action.
- **`APPROVED WITH NOTES`** — Minor findings the executor should address in a
  named later Part or slice; none affect correctness, contracts, or
  verification. List where each note is expected to be resolved.
- **`REJECTED — MUST FIX`** — one or more Blocker or Major findings. The Part
  **may not be marked DONE**. The executor must apply the required fixes
   (within the same Part, keeping TDD discipline), update §3b and §10b,
   capture a fresh review snapshot, regenerate the Part Quality Report, and
   the review must be re-run until the verdict is APPROVED or APPROVED WITH
   NOTES.

Never soften a Blocker to a note because fixing it is inconvenient. Never
reject for style preferences that no project source establishes — tie every
finding to the standard, the architecture, the spec, or observed project
patterns.
