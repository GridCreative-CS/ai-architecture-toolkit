# Part Quality Report — <part-id>: <part-title>

> Produced by `part-executor-tdd` at the end of **every** Part execution
> (engineering workflow Step 6) and written to
> `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`. It is the input
> to the Part code review (Step 6a, `ai/prompts/code-quality-reviewer.md`).
> A Part without a completed quality report is **not done**. Every field is
> required — write "none" or "N/A — <reason>" explicitly rather than omitting
> a field. The rules being reported against are defined in
> `ai/guides/code-quality-standard.md`.

- **Slice:** <slice-id> — <slice-name>
- **Part:** <part-id> — <part-title> (`ai-parts/<slice-id>/<part-file>.md`)
- **Feature spec:** `architecture/feature-specs/<slice-id>-<slice-name>.md`
- **Date:**
- **Executor:** <agent/model that executed the Part>

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

## 11. Prohibited-output check

- No TODO/FIXME/placeholders/stubs in production paths: PASS/FAIL
- No fake implementations: PASS/FAIL
- No dead/unused/commented-out code introduced: PASS/FAIL
- No existing test weakened, deleted, or skipped (or justified above): PASS/FAIL

## 12. Verdict

**Part status: DONE / NOT DONE**

If NOT DONE: what is missing and what happens next. A Part may only be
declared DONE when sections 3, 4, and 11 contain no FAIL and section 7 has no
undeclared change.
