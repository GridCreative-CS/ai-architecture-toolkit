# Part Quality Report — <part-id>: <part-title>

> Produced by `part-executor-tdd` at the end of **every** Part execution
> (engineering workflow Step 6) and written to
> `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`. It is the input
> to the Part code review (Step 6a, `ai/prompts/code-quality-reviewer.md`).
> A Part without a completed quality report is **not done**. Every field is
> required — write "none" or "N/A — <reason>" explicitly rather than omitting
> a field. The rules being reported against are defined in
> `ai/guides/code-quality-standard.md`. See the fictional completed example in
> [example-part-quality-report.md](../examples/example-part-quality-report.md).

- **Slice:** <slice-id> — <slice-name>
- **Part:** <part-id> — <part-title> (`ai-parts/<slice-id>/<part-file>.md`)
- **Feature spec:** `architecture/feature-specs/<slice-id>-<slice-name>.md`
- **Date:**
- **Executor:** <agent/model that executed the Part>

## Review Snapshot

- **Base commit (SHA):**
- **HEAD at report time:**
- **Reproducible committed-diff command:** <!-- exact command, e.g. `git diff <base>...<head> -- <files>` -->
- **Uncommitted worktree files in this Part:** <!-- list paths or `none` -->
- **Generated/untracked files belonging to this Part:** <!-- list paths or `none` -->

## 1. Part executed

<One paragraph: what the Part was and what was actually implemented.>

## 2. Files changed

| File | Change (added/modified/deleted) | Purpose |
| --- | --- | --- |
| | | |

## 3. Tests added or updated

| Test file | Test cases | What behavior they lock |
| --- | --- | --- |
| | | |

**TDD evidence:**

- Red observed (command + exact failure):
- Green achieved (command + result):

**Mutation-check evidence** (mandatory for authorization guards, cache
invalidation/refetch, cancellation/supersession, or error-to-message mapping;
record `N/A — <reason>` when no named trigger applies):

- Trigger:
- Mutation (`file:line` and temporary change):
- Observed test failure:
- Restoration and final green result:

## 3b. Requirement Coverage Matrix

Build this matrix from every §6/§9/§11/§11b criterion in the feature spec,
every applicable design-system rule, and every PART_SPEC acceptance criterion.
Use stable criterion IDs (`DR-nn`, `SEC-nn`, `AC-nn`, `UIAC-nn`) when the spec
has them. For an in-flight spec without IDs, use `§<section> "<verbatim
criterion text>"` until the next reconciliation; do not invent a replacement
ID. Use one row per criterion; do not bundle independent acceptance criteria
into one row. The positive-test and negative/edge-test cells must name exact
behavior-proving test cases, not "covered" or a suite name, and verification
evidence must identify whether it is an automated test, mutation check,
browser/E2E evidence, or a named Step 6b deferral. There must be no omitted
rows or blank cells.

Record the Part classification used for the dimension audit. Prefer
`PART_SPEC.part_type`; when it is absent, classify from `file_touch_points` and
record the evidence here. The classification is one of: `backend`, `frontend`,
`shared-contract`, or `infrastructure`.

For a frontend Part, include D7 when the feature spec or Part contains
authorization, role/ABAC, or request-gating behavior, and include D8 when the
feature spec or design system defines error mapping, trace-reference display,
or observable failure presentation. D9 applies to every Part.

- **Part classification:**
- **Classification evidence:**
- **Coverage-map source:** `ai-parts/<slice-id>/OVERVIEW.md` § Requirement Coverage Map

| Requirement | Source | Implementation location | Positive test | Negative/edge test | Verification evidence | Status |
| --- | --- | --- | --- | --- | --- | --- |
| | | | | | | |

Allowed statuses are exactly: `COVERED-THIS-PART`, `COVERED-EARLIER (Pxx)`,
`NOT-YET (owner Pxx)`, `DEFERRED (<workflow step>, owner Pxx)`, and
`N/A — <reason>`. A `COVERED-*` row requires a behavior-proving test or a
named deferral that identifies the workflow step that will prove it; source
inspection alone is not evidence. The final Part of a slice must contain zero
`NOT-YET` rows. Any remaining `DEFERRED` row must name Step 6b.

## 4. Checks run

List every command executed with its result — build, test suites, linters,
E2E. Copy the PART_SPEC `verify` (and `e2e_verify`) commands here with actual
outcomes, not intentions.

| Command | Result |
| --- | --- |
| | |

## 5. Architecture rules verified

- Module/layer boundaries respected (which boundaries this Part touches, and
  how that was verified — e.g., architecture tests run):
- Dependency direction respected: YES/NO
- ADRs applied (cite numbers):
- New boundaries covered by architecture tests: YES/NO/N-A

## 6. Existing patterns followed

- Nearby files read before implementing (list them):
- Patterns followed (error handling, validation, logging, async/cancellation,
  naming, test style — one line each):

## 7. Contract surfaces

State **changed** (with details + spec reference) or **unchanged** for each:

- Public API: CHANGED/UNCHANGED —
- Database/schema: CHANGED/UNCHANGED —
- Events/messages: CHANGED/UNCHANGED —
- UI behavior: CHANGED/UNCHANGED —

## 8. Dependencies

- New libraries/packages added: NONE / <name — justification per
  code-quality standard §3>

## 9. Deviations from existing patterns

For each deviation: what the existing pattern is, what was done instead, and
why. Write "none" if the Part introduces no deviation.

## 10. Remaining risks

Known risks, untested paths, follow-ups needed. Write "none" only if true.

## 10b. Remediation log

Include this section only when re-running after `REJECTED — MUST FIX`. For each
prior finding, record the fix, the test re-run and observed result, explicit
confirmation that no prior assertion was weakened or deleted, and every branch
the remediation touched. Isolate the remediation diff from the original diff.

| Prior finding | Fix | Test re-run + result | Prior assertions preserved | Remediation branches touched | Isolated remediation diff |
| --- | --- | --- | --- | --- | --- |
| | | | | | |

## 11. Prohibited-output check

- No TODO/FIXME/placeholders/stubs in production paths: PASS/FAIL
- No fake implementations: PASS/FAIL
- No dead/unused/commented-out code introduced: PASS/FAIL
- No existing test weakened, deleted, or skipped (or justified above): PASS/FAIL

## 12. Verdict

**Part status: DONE / NOT DONE**

If NOT DONE: what is missing and what happens next. A Part may only be
declared DONE when the snapshot is filled, §3b is complete with no prohibited
status or blank cell, every triggered mutation check is recorded, sections 3,
4, and 11 contain no FAIL, and section 7 has no undeclared change.
