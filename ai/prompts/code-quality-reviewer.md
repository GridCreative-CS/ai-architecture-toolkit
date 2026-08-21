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
- The actual changed files / diff for the Part
- `ai-parts/<slice-id>/OVERVIEW.md` — for the Requirement Coverage Map
- The quality reports of **earlier Parts in this slice** (their §3b matrices
  at minimum) — check 12 cannot verify cross-Part status consistency without
  them
- The previous review file(s) for this Part when this is a re-review
- `architecture/feature-specs/<slice-id>-<slice-name>.md`
- `architecture/architecture-final.md` and `architecture/adr/*.md`
- `architecture/design-system.md` (when the Part touches UI)
- `ai/guides/code-quality-standard.md` — the standard you enforce
- Nearby project code comparable to the changed files (read it — you cannot
  judge "follows existing patterns" without seeing the existing patterns)

If the Part file, the quality report, or the diff is missing **in a repository
that has the toolkit artifacts**, stop and request it — do not review from a
narrative summary.

**Reviewing a change in a repository that has none of these artifacts** (an
arbitrary PR, branch, or commit range in a repo that has never run the
toolkit): use the **Portable mode** appendix at the end of this prompt, which
degrades each missing input to a named substitute and inlines the rules the
main body cites by section number. It is a degradation, not an alternative —
when the artifacts exist, this main body applies.

## Review snapshot (freeze the target)

Before reviewing anything, fix the target and state it. The reviewed snapshot
is:

- the **base commit** the Part started from
- the **committed diff** from that base to HEAD
- the **uncommitted worktree diff**
- the **generated or untracked files** belonging to the Part

Restate all four in the review output and confirm they match the quality
report's Review snapshot block. If they disagree, that is a finding — the
report describes a different target than the one you can see.

**If any production file changes after the review begins, the review restarts
against the new snapshot.** A review whose findings span two snapshots is
void: half its line references are stale and its verdict covers code nobody
read. Discard the partial review and start again rather than patching it.

## Review checks (all required)

Twelve checks, all required. Checks 1–10 are defect categories; check 11 is
the dimension audit and check 12 is the requirement coverage audit.

1. **Architecture alignment** — boundaries and dependency direction respected;
   ADRs followed; no undeclared new module/cross-module dependency; new
   boundaries covered by architecture tests.
2. **Feature spec alignment** — implemented behavior matches the spec's scope,
   acceptance criteria, contracts, security, and observability expectations
   for this Part.
3. **Part scope** — everything changed is inside PART_SPEC `scope.in`; nothing
   from `scope.out` leaked in; no unrelated drive-by changes.
4. **Code quality** — the changed code follows the code-quality standard §§1–9
   and §13: nearby patterns followed, error handling and error identifiers
   match project style, validation split respected, logging/observability
   present and PII-safe, cancellation propagated, naming consistent.
5. **Test quality** — red evidence is real (a failing test preceded the code);
   tests assert observable behavior, not mocks or implementation details;
   names describe behavior; rule matrices have truth-table + boundary
   coverage; contract tests lock status codes and error identifiers; no
   existing test weakened to pass.
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
11. **Dimension audit** — sweep every applicable surface below, explicitly.
    See the table.
12. **Requirement coverage audit** — verify quality report §3b independently.
    See below.

### Check 11 — Dimension audit

Checks 1–10 ask "is there a defect of this kind?". This check asks "did
anyone look at this surface at all?" — it is the sweep that stops a criterion
being missed for three rounds because no check happened to point at it.

Report **every** dimension with `PASS` / `FAIL` / `DEFERRED (owner)` /
`N/A — <reason>` plus the evidence you based it on. A bare `N/A` with no
reason, or a `DEFERRED` with no owner, is itself a Major finding — that is
how this table gets hollowed out.

Applicability follows the Part's classification, taken from PART_SPEC
`part_type`. **If `part_type` is absent** (any slice decomposed before the
field existed), classify the Part yourself from its `file_touch_points` and
**state the classification you used** in the audit header, so the `N/A` rows
stay auditable.

| # | Dimension | What must be shown | Applies to |
| --- | --- | --- | --- |
| D1 | Role and authorization behavior | Each permitted role gets the specified access; each denied role is refused **and** no request or side effect is issued on its behalf where the design says none should be | backend, frontend, shared-contract |
| D2 | Loading, success, empty, and error states | All four states exist and are driven **independently per async source** — one source failing or pending does not silently present another source's state | frontend |
| D3 | Async lifecycle | Initial request, supersession by a newer request, clear/reset, unmount/teardown, and failure — each exercised as its own case, not collapsed into one "cancellation works" claim | frontend, backend |
| D4 | Error mapping and diagnostics | Errors map to the project's stable identifiers; the error contract's trace reference (e.g. Problem Details `traceId`) survives every mapping hop and reaches the surface that reports it | backend, shared-contract |
| D5 | Presentation of domain values | Every user-visible domain code, enum, or key is rendered through its display mapping, in every supported locale — never as the raw stable value | frontend |
| D6 | Accessibility and design system | The design system's state patterns, visible labels, focus handling, and accessibility baseline are met for what this Part renders | frontend |
| D7 | Cache and state invalidation | After a mutation, dependent reads observably refresh — proven by observed refresh, not by the presence of an invalidation call | frontend, backend |
| D8 | Server-derived vs client-calculated values | Values the server owns are read from the server rather than recomputed client-side (and the reverse where the design says so) | all |
| D9 | Shared-component and public-contract changes | Every consumer of a touched shared component, hook, or public contract is identified and accounted for | all |

A `FAIL` in any dimension is a finding in the findings table, with a severity
from the scale below — the dimension row is the sweep, not the report.

### Check 12 — Requirement coverage audit

Verify quality report §3b yourself against the feature spec, the OVERVIEW
Requirement Coverage Map, and the earlier Parts' reports:

- **Completeness** — every §6 `DR-nn`, §9 `SEC-nn`, §11 `AC-nn`, and §11b
  `UIAC-nn` in the spec has a row, plus every PART_SPEC acceptance criterion.
  A missing row is a Major finding; a criterion no Part owns is a Blocker
  (nothing will ever implement it).
- **Evidence** — every `COVERED-*` row names a test that fails if the
  implementation is removed. Open the cited tests. A row backed by a test that
  only asserts existence, mirrors the implementation, or checks catalogue
  parity is a `COVERED` claim without proof: Blocker.
- **Consistency** — statuses agree with the coverage map and with earlier
  Parts' matrices. A criterion sitting at `NOT-YET (owner Pxx)` whose owner
  `Pxx` is already `DONE` is a Blocker: it was dropped, and this is the
  review that catches it.
- **Deferrals** — every `DEFERRED` names a real workflow step and an owner.
  "Deferred" with no destination is an omission wearing a label.
- **Final Part** — if this is the slice's last Part, zero `NOT-YET` rows may
  remain.

## Severity scale

| Severity | Meaning |
| --- | --- |
| **Blocker** | Wrong behavior, contract broken or silently changed, architecture violation, fake test/implementation, missing or false verification |
| **Major** | Pattern deviation without justification, missing test coverage for a spec rule, unjustified dependency/abstraction, observability gap |
| **Minor** | Naming/style inconsistency, doc-comment gap, improvement opportunity |

## Evidence rules for claims

A completeness claim in the quality report is a finding unless the report
names the evidence for it. Judge the evidence, never the adjective:

| Claim | Not evidence | Evidence |
| --- | --- | --- |
| "Localization complete" | Catalogue/key parity between locale files | The rendered value asserted in each supported locale |
| "Cancellation covered" | One test named for cancellation; an abort signal being passed | Each lifecycle branch (supersede, clear/reset, unmount) exercised as its own case |
| "History refreshes" | An invalidation call present in the code | The refetch observed after the mutation — and the test fails when invalidation is removed |
| "Role restrictions enforced" | A guard visible in the implementation | The denied role asserted to receive no data **and** to issue no request |
| "States handled" | The state components existing | Each state rendered under its own condition, per async source |

The same rule applies to your own review: do not write that a check passed
without naming what you read or ran.

**When you cannot verify something yourself** — the toolchain is unavailable
in your environment, the application cannot be started, a command will not
run — say so explicitly and mark the affected dimension or coverage row
`DEFERRED` with the reason and the owner. Do not mark it `PASS` on the
strength of the executor's report: an unverifiable claim is not a verified
one, and silently upgrading it to PASS is how a whole class of findings
survives a review. Static evidence you *can* read (the diff, the test bodies,
the assertions) still supports findings — a test whose assertions do not
prove the behavior is a finding whether or not you can execute it.

## Reviewing a mutation check

Where the quality report records a mutation check (mandatory when the Part
implements an authorization guard, cache invalidation/refetch,
cancellation/supersession, or error→message mapping — code-quality standard
§10), verify it is real: the mutation is specific enough to re-run
(`file:line` + what changed), the named test is the one that failed, and the
worktree was restored with the suite green. A missing mutation check for a
triggering behavior is a Major finding; a fabricated one is a Blocker. Re-run
it yourself when the cost is low.

## Re-review after `REJECTED — MUST FIX`

A re-review is not a fresh review of the whole Part, and it is not a check
that the listed fixes were typed in. Remediation is where regressions enter,
and where assertions get quietly loosened to make a fix pass. Required:

1. **Re-run every previous finding's test** — each one, by name, with its
   result. A finding whose test you did not run is not closed.
2. **Verify no assertion was weakened** — diff the test files against the
   pre-remediation snapshot. A loosened matcher, a widened tolerance, a
   removed case, a newly skipped test, or an assertion moved behind a
   condition is a Blocker even when every test is green.
3. **Review the remediation diff separately** from the original Part diff,
   with its own snapshot. Findings in remediation code are new findings, not
   continuations.
4. **Check every branch the remediation touches** — a fix to one path
   frequently leaves its sibling paths (error, empty, cancelled, denied-role)
   unchanged and now inconsistent.
5. **Confirm §3b was updated** and that no row regressed from `COVERED-*`
   back to an unproven state.

Pay particular attention when the remediation touched a **shared API,
design-system component, or shared hook**: re-run check 11 D9 for the
remediation diff on its own, and name every consumer.

## Output

Write the review to:

- `ai-parts/<slice-id>/reviews/<part-id>-review.md`

using this structure:

```markdown
# Part Code Review — <part-id>: <part-title>

- Slice: <slice-id>
- Date:
- Reviewer: <agent/model>
- Review round: <n> (round 1 = first review of this Part)

## Review snapshot

- Base commit:
- Committed diff reviewed: <command + SHA range>
- Uncommitted worktree diff reviewed: <files, or "none">
- Generated/untracked Part files reviewed: <list, or "none">
- Matches quality report snapshot: YES / NO — <difference>

## Findings

| # | Severity | Check | File:line | Finding | Required fix |
|---|---|---|---|---|---|

(Write "No findings." when clean. Every Blocker/Major finding must name a
concrete required fix — not "improve quality".)

## Dimension audit (check 11)

Part classification used: <backend / frontend / shared-contract /
infrastructure> — <from PART_SPEC `part_type`, or derived from
file_touch_points>

| # | Dimension | Result | Evidence |
|---|---|---|---|
| D1 | Role and authorization behavior | PASS / FAIL / DEFERRED (owner) / N/A — reason | |
| D2 | Loading, success, empty, error states | | |
| D3 | Async lifecycle | | |
| D4 | Error mapping and diagnostics | | |
| D5 | Presentation of domain values | | |
| D6 | Accessibility and design system | | |
| D7 | Cache and state invalidation | | |
| D8 | Server-derived vs client-calculated | | |
| D9 | Shared-component / public-contract changes | | |

## Requirement coverage audit (check 12)

- Criteria in spec: <n> — rows in §3b: <n> — missing: <list, or "none">
- `COVERED-*` rows whose cited test does not prove the behavior: <list, or "none">
- Status inconsistencies against the coverage map / earlier Parts: <list, or "none">
- `DEFERRED` rows without a named step and owner: <list, or "none">
- Final Part of slice: YES/NO — if YES, `NOT-YET` rows remaining: <n>

## Remediation closure (re-reviews only)

Write "N/A — round 1" on a first review.

- Previous findings re-run: <n of n> — failures: <list, or "none">
- Assertions weakened, loosened, removed, or skipped since the last review:
  <list, or "none">
- Remediation diff reviewed separately: YES/NO — <command>
- Branches affected by the remediation and checked: <list>
- §3b updated for the remediation: YES/NO
- Shared API / design-system / shared-hook surfaces touched: <list, or "none">

## Checks with no findings

<List the checks (1–12) that passed cleanly, so absence of findings is
distinguishable from absence of review.>

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
  (within the same Part, keeping TDD discipline), regenerate the Part Quality
  Report, and the review must be re-run until the verdict is APPROVED or
  APPROVED WITH NOTES.

Never soften a Blocker to a note because fixing it is inconvenient. Never
reject for style preferences that no project source establishes — tie every
finding to the standard, the architecture, the spec, or observed project
patterns.

A worked example of a completed review — including a dimension audit with a
`FAIL` and a justified `N/A`, and a coverage audit that catches a stale
`NOT-YET` — is `ai/examples/example-part-review.md`. Its matching quality
report is `ai/examples/example-part-quality-report.md`.

## Why this review is heavier than a checklist

Checks 11 and 12 — the dimension sweep and the criterion-by-criterion audit —
cost more per review than the ten defect checks did alone. That is the trade:
reviews that find everything in round 1 instead of rediscovering the same
original defects in rounds 2 and 3. Do not shorten the sweep to save time — a
dimension left unswept is exactly the finding that comes back two rounds
later.

## Appendix — Portable mode (repository without toolkit artifacts)

Everything above assumes the toolkit's artifacts exist: a PART_SPEC file, a
Part Quality Report, a feature spec, `architecture/architecture-final.md`,
ADRs, and `ai/guides/code-quality-standard.md`. **When they exist, use the
main body — portable mode is a degradation, not an alternative.**

Use this appendix when the same review has to run against an arbitrary
change — a PR, a branch, a commit range — in a repository that has never run
the toolkit. The section is written to be self-contained: it inlines the
substance of every rule the main body cites by section number, so a reviewer
working from it alone never follows a dead reference.

### What does not degrade

These are load-bearing and survive unchanged:

- **Fresh session.** The reviewer is not the author. If you wrote the code,
  hand the review to a new session or subagent.
- **You review; you do not fix.**
- **The frozen snapshot** — four parts, restated in the output. In portable
  mode the base commit is the PR base, the merge-base with the default
  branch, or the explicitly named commit. If a production file changes after
  the review begins, the review restarts.
- **The dimension audit (check 11, D1–D9)** exactly as tabulated above.
- **The evidence rules** — judge the evidence, never the adjective.
- **`DEFERRED`, never `PASS`, for anything you could not verify yourself**,
  with the reason and the owner.
- **The severity scale and the three verdicts.**

### The only mandatory input is the diff

The main body's hard stop ("if the Part file, the quality report, or the diff
is missing, stop") becomes: **stop only if you cannot see the diff.**
Everything else degrades to a named substitute, and the substitute is
recorded in the review header so the reader knows what the verdict rests on.

| Missing artifact | Checks affected | Substitute to use |
| --- | --- | --- |
| PART_SPEC / Part file | 3 (scope), `part_type` | Scope = the stated task, PR description, issue, or commit range. Classify the change using check 11's absent-`part_type` fallback, reading the changed files in place of `file_touch_points`, and state the classification used. |
| Part Quality Report | 9, 10, 12, mutation review | 9 → enumerate the contract surfaces the diff changes yourself; an undeclared change is still a finding, measured against the PR description instead of §7. 10 → run the project's own verify commands (test, lint, build, typecheck) and report what you ran. 12 → derive the criteria from the issue/PR/spec-in-lieu; if there is no stated criterion set, mark check 12 `N/A — no stated acceptance criteria` and say so in the verdict. |
| Feature spec | 2 | The intent stated in the PR description, issue, or commit message. If intent is unstated, that is itself a Major finding — nobody can review behavior against an unstated goal. |
| `architecture-final.md`, ADRs | 1 | Infer boundaries from observed structure: project/module references, folder layout, existing dependency rules or architecture tests, import conventions. Judge the change against the architecture the repository actually has, not one you would prefer. |
| `design-system.md` | D6 | The project's existing UI patterns plus a generic accessibility baseline (visible labels, focus handling, keyboard reachability, state announcement). |
| Earlier Parts' reports | 12 consistency | Prior commits in the same branch/PR series; if unavailable, mark the consistency line `DEFERRED — no prior-change record`. |
| `code-quality-standard.md` | 4, 5, 7, 8 | The inlined quality bar below. |

### Inlined quality bar

The bar the changed code must meet. Q1–Q13 correspond to the standard's
sections §§1–13; nothing here requires reading another file.

- **Q1 Read before write.** The reviewer must read the neighbouring
  production code and tests before judging "follows existing patterns". A
  review that never opened a comparable existing file cannot make that call.
- **Q2 Source precedence.** Existing project code and tests outrank written
  docs, which outrank model defaults. Never reject for a style no project
  source establishes.
- **Q3 Dependencies.** A new runtime dependency needs a justification the
  diff or PR states. Unjustified → Major.
- **Q4 Abstractions.** No interface, indirection, generic parameter, or
  configuration knob with exactly one caller and no stated second one.
- **Q5 Boundaries.** Dependency direction respected; no new cross-module
  dependency that the observed structure does not already sanction.
- **Q6 Error handling.** Matches the project's existing error type and
  identifier style. No swallowed exceptions, no bare catch that logs and
  continues, no error path that silently returns a success-shaped value.
- **Q7 Validation.** At the layer the project already validates at, not
  duplicated across layers by accident and not skipped at the boundary.
- **Q8 Logging and observability.** Present where the project logs
  comparable operations, at the project's level conventions, with no PII or
  secrets in the payload.
- **Q9 Async and cancellation.** Cancellation tokens / abort signals
  propagated to every call that accepts one; no fire-and-forget without a
  stated reason; no blocking on async in a request path.
- **Q10 Test quality.** Test-first evidence is real where TDD is claimed.
  Names state behavior. Tests assert observable behavior, not mocks or
  internal steps. **Structural is not behavioral** — a test must fail when
  the implementation is removed:

  | Requirement | Structural (insufficient) | Behavioral (required) |
  | --- | --- | --- |
  | Role restriction | The guard is present | The denied role receives no data **and** issues no request |
  | Display mapping | Locale catalogues have matching keys | The rendered output asserted in each supported locale |
  | Cancellation | An abort signal is passed | Supersede, clear/reset, and unmount each as their own case |
  | Cache refresh | An invalidation call appears in the code | The dependent read observably refetches after the mutation |
  | Gating on required data | The disabled prop is wired up | The action proven unavailable while required data is pending, and again while failed |
  | Error mapping | The mapper unit-tested in isolation | The identifier and its trace reference survive to the reporting surface |

  **Mutation check** — mandatory when the change implements an
  **authorization guard**, **cache invalidation/refetch**,
  **cancellation/supersession**, or **error→message mapping**: break the
  implementation, observe the named test fail, restore, re-run green. In
  portable mode the author rarely recorded one — run it yourself when the
  cost is low, and where you cannot, mark the claim `DEFERRED` rather than
  `PASS`. A behavior in these four categories whose test passes with the
  implementation removed is a **Blocker**.
- **Q11 Prohibited outputs.** TODO markers, placeholder logic, hard-coded
  values standing in for real computation, stubbed returns, silent fallbacks,
  and dead configuration shipped as if complete.
- **Q12 Contract surfaces.** Every API, schema, event, or UI contract the
  diff changes must be declared somewhere the consumer can see (PR body,
  changelog, migration note). An undeclared contract change is a **Blocker**.
- **Q13 Documentation in code.** Public API documentation per the project's
  existing convention — matched to what the neighbouring code does, not to a
  general standard.

### Portable output

Write to `reviews/<change-id>-review.md` (or paste into the PR), using the
main body's structure with these substitutions:

- Header: `# Code Review — <change-id>: <title>`, plus a line
  **`Mode: portable — artifacts substituted: <list>`** naming every row of
  the degradation ladder you used.
- **Review snapshot** — base commit, committed diff (command + SHA range),
  uncommitted worktree diff, untracked/generated files. The "matches quality
  report snapshot" line becomes `N/A — no quality report`.
- **Findings**, **Dimension audit**, **Checks with no findings**, and
  **Verdict** are unchanged.
- **Requirement coverage audit** — rows derived from the stated criteria; if
  none were stated, `N/A — no stated acceptance criteria`, and say so in the
  verdict rather than approving silently.
- **Remediation closure** — unchanged; `N/A — round 1` on a first review.

### Kickoff prompt (copy-paste into a fresh session)

```text
Act as a Principal Engineer performing a code review, following
ai/prompts/code-quality-reviewer.md — main body if this repository has the
toolkit artifacts (ai-parts/, architecture/feature-specs/,
architecture/architecture-final.md), otherwise the "Portable mode" appendix.
If neither is available in this repository, follow the review contract
restated below.

Target: <PR #n | branch <name> | commit range <base>..<head> | working tree>
Stated intent: <link or one paragraph — what this change is supposed to do>
Verify commands: <test / lint / build commands, or "discover them">

Rules:
- You review; you do not fix.
- Freeze and restate the snapshot (base commit, committed diff, uncommitted
  worktree diff, untracked/generated files) before reviewing anything. If a
  production file changes mid-review, restart against the new snapshot.
- Read comparable existing code before judging "follows existing patterns".
- Run the verify commands yourself; report what you ran and what happened.
- Mark anything you could not verify DEFERRED with a reason and an owner —
  never PASS on the author's claim.
- Judge evidence, not adjectives: catalogue parity, the presence of a call,
  and implementation inspection do not prove behavior. A test that passes
  with the implementation removed proves nothing.
- Complete the D1–D9 dimension audit explicitly (roles/authorization; per-
  source loading/success/empty/error states; async lifecycle incl.
  supersession, clear/reset, unmount; error mapping and trace reference;
  localized display of domain values; accessibility and design system;
  observable cache invalidation; server-derived vs client-calculated values;
  shared-component and public-contract consumers). Each row gets
  PASS / FAIL / DEFERRED (owner) / N/A — reason, with evidence; a bare N/A
  or an ownerless DEFERRED is itself a Major finding.
- Severity: Blocker (wrong behavior, broken or undeclared contract,
  architecture violation, fake test or implementation, missing/false
  verification) / Major (unjustified pattern deviation, missing coverage for
  a stated rule, unjustified dependency or abstraction, observability gap) /
  Minor (naming, style, docs).
- End with exactly one verdict: APPROVED / APPROVED WITH NOTES /
  REJECTED — MUST FIX. Never soften a Blocker to a note.

Write the review to <path>.
```
