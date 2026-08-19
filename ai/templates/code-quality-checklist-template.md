# Part Quality Report — <part-id>: <part-title>

> Produced by `part-executor-tdd` at the end of **every** Part execution
> (engineering workflow Step 6) and written to
> `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`. It is the input
> to the Part code review (Step 6a, `ai/prompts/code-quality-reviewer.md`).
> A Part without a completed quality report is **not done**. Every field is
> required — write "none" or "N/A — <reason>" explicitly rather than omitting
> a field. The rules being reported against are defined in
> `ai/guides/code-quality-standard.md`. A filled example is
> `ai/examples/example-part-quality-report.md`.

- **Slice:** <slice-id> — <slice-name>
- **Part:** <part-id> — <part-title> (`ai-parts/<slice-id>/<part-file>.md`)
- **Part type:** backend / frontend / shared-contract / infrastructure
  (from PART_SPEC `part_type`; if the field is absent, state the
  classification you derived from `file_touch_points` and say so)
- **Feature spec:** `architecture/feature-specs/<slice-id>-<slice-name>.md`
- **Date:**
- **Executor:** <agent/model that executed the Part>

## Review snapshot

The reviewed target, frozen. The Step 6a review must describe this same
snapshot; if any production file changes after the review begins, the review
restarts against a new snapshot.

- **Base commit:** <SHA the Part started from>
- **HEAD at report time:** <SHA, or "same as base — work is uncommitted">
- **Diff command:** <the exact command that reproduces the reviewed change,
  e.g. `git diff <base>..HEAD -- <paths>`>
- **Uncommitted worktree files in this Part:** <list, or "none">
- **Generated / untracked files belonging to this Part:** <list, or "none">
  (generated files are part of the reviewed target — name them, do not let
  them sit outside the diff)

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

**Mutation checks** (mandatory when this Part implements an authorization
guard, cache invalidation/refetch, cancellation/supersession, or
error→message mapping — `ai/guides/code-quality-standard.md` §10). Write
"N/A — this Part implements none of the four triggering behaviors" when none
applies.

| Behavior | Mutation applied (file:line + what was changed) | Test that failed | Observed failure | Restored + suite green |
| --- | --- | --- | --- | --- |
| | | | | |

The mutation is never committed. Record it in enough detail that the reviewer
can re-run it.

## 3b. Requirement coverage matrix

Every acceptance criterion of the **whole slice** appears here — not only the
ones this Part implements. A criterion that no Part owns, or that stalls at
`NOT-YET` while its owning Part is `DONE`, is visible here at the next Part's
review instead of surfacing at Step 6b.

Include one row for each of:

- every §6 domain rule (`DR-nn`), §9 security constraint (`SEC-nn`), §11
  acceptance criterion (`AC-nn`), and §11b UI/UX criterion (`UIAC-nn`) in the
  slice's feature spec
- every acceptance criterion in this Part's PART_SPEC

Design-system rules get no row of their own — they enter through the
`UIAC-nn` criterion that cites them.

If the feature spec predates criterion IDs, key each row as
`§<section> "<verbatim criterion text>"` instead.

| Requirement | Source | Implementation location | Positive test | Negative/edge test | Verification evidence | Status |
| --- | --- | --- | --- | --- | --- | --- |
| | | | | | | |

**Status vocabulary** (exactly one per row):

| Status | Meaning |
| --- | --- |
| `COVERED-THIS-PART` | Implemented and proven in this Part |
| `COVERED-EARLIER (Pxx)` | Proven by an earlier Part — cite it |
| `NOT-YET (owner Pxx)` | Assigned to a later Part — cite the owner |
| `DEFERRED (<workflow step>, owner Pxx)` | Verification happens at a named later step (e.g. `DEFERRED (Step 6b, owner P14)`) |
| `N/A — <reason>` | Does not apply to this slice as built — the reason is required |

**Rules:**

- No omitted rows and no blank cells. Write `—` where a column genuinely does
  not apply and the Status explains why.
- A `COVERED-*` row requires a test that **proves the behavior** — one that
  fails if the implementation is removed (`ai/guides/code-quality-standard.md`
  §10). Implementation inspection, a catalogue/key parity check, or the mere
  existence of a component, query, or key never earns `COVERED`. If the
  behavior genuinely cannot be tested at this level, use `DEFERRED` and name
  the step that will prove it.
- The owner in `NOT-YET` / `DEFERRED` comes from the Requirement Coverage Map
  in `ai-parts/<slice-id>/OVERVIEW.md` — it is not guessed. If the slice was
  decomposed before that map existed, derive it once from the feature spec,
  write it into the existing `OVERVIEW.md`, and fill the column from it.
- The **final Part of a slice must show zero `NOT-YET`**, and every remaining
  `DEFERRED` must name Step 6b.

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

Required **only** when this report is a re-run after a `REJECTED — MUST FIX`
review. Write "N/A — first submission of this Part" otherwise.

- **Review being answered:** `ai-parts/<slice-id>/reviews/<part-id>-review.md`
  (round <n>)
- **Remediation diff:** <command isolating the remediation from the original
  Part diff, e.g. `git diff <post-review-SHA>..HEAD`>

| Finding # | Severity | Fix applied (file:line) | Test re-run + result | Branches the fix touches |
| --- | --- | --- | --- | --- |
| | | | | |

Confirm explicitly:

- Every previous finding's test was re-run: PASS/FAIL per finding above
- No previously passing assertion was weakened, loosened, or deleted to
  accommodate the fix: YES/NO (if NO, name each and justify)
- All branches affected by the remediation were exercised: YES/NO
- §3b was updated to reflect the remediation: YES/NO
- Shared API, design-system, or shared-hook surfaces touched by the
  remediation: <list, or "none"> — each declared in §7

## 11. Prohibited-output check

- No TODO/FIXME/placeholders/stubs in production paths: PASS/FAIL
- No fake implementations: PASS/FAIL
- No dead/unused/commented-out code introduced: PASS/FAIL
- No existing test weakened, deleted, or skipped (or justified above): PASS/FAIL

## 12. Verdict

**Part status: DONE / NOT DONE**

If NOT DONE: what is missing and what happens next. A Part may only be
declared DONE when:

- sections 3, 4, and 11 contain no FAIL, and section 7 has no undeclared
  change
- §3b is complete under its rules — every slice criterion present, no blank
  cells, no `COVERED-*` without a behavior-proving test, and (final Part of a
  slice) zero `NOT-YET`
- a mutation check is recorded for every triggering behavior this Part
  implements, with the worktree restored and the suite green
- the Review snapshot block is filled
- on a re-run, §10b is complete and reports no weakened assertion

Claims are not evidence. Do not write "localization complete", "cancellation
covered", or "history refresh verified" anywhere in this report unless the
matching §3b row names the test and the observed result that prove it.
