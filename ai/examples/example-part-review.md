# Example — Part Code Review (Step 6a)

> **Illustrative example only.** Same fictional project and Part as
> `ai/examples/example-part-quality-report.md`, reviewed per
> `ai/prompts/code-quality-reviewer.md`. It shows a completed dimension audit
> (including a `FAIL` and a justified `N/A`) and a coverage audit that catches
> a criterion the report claimed but did not prove. Copy the rigour, not the
> content.
>
> Note what this example demonstrates: the report's own verdict was DONE and
> its §3b looked complete. The review still finds two Blockers — one from the
> dimension sweep, one from opening a cited test. That is the point of checks
> 11 and 12.

---

# Part Code Review — P07: Inspection history panel

- Slice: S3.2
- Date: 2026-03-11
- Reviewer: code-reviewer-agent (fresh session)
- Review round: 1

## Review snapshot

- Base commit: `4c1e9a2`
- Committed diff reviewed: none — all Part work is uncommitted at `4c1e9a2`
- Uncommitted worktree diff reviewed: `git diff 4c1e9a2 -- web/src/features/inspections/`
  (4 files)
- Generated/untracked Part files reviewed:
  `web/src/api/generated/inspections.ts`
- Matches quality report snapshot: YES

## Findings

| # | Severity | Check | File:line | Finding | Required fix |
|---|---|---|---|---|---|
| 1 | Blocker | 11 (D3) | `useInspectionHistory.ts:41` | The clear/reset branch is not implemented. Resetting the panel leaves the in-flight request running, and its response repopulates the list after the reset. The report's D3 evidence cites the supersession test, which passes for a *different* branch. | Cancel the in-flight request on reset and clear displayed results; add a test that resets mid-flight and asserts the list stays empty after the response arrives. |
| 2 | Blocker | 12 | `HistoryPanel.test.tsx:104` | §3b marks UIAC-03 `COVERED-THIS-PART`, but the `nl-NL` test asserts `catalogue['outcome.passed']` rather than the rendered text — it passes with the Dutch catalogue value equal to the domain code. Removing the display mapping does not fail it. | Assert the rendered string (`'Goedgekeurd'`) in the `nl-NL` case, as the `en-GB` case does. |
| 3 | Major | 11 (D6) | `HistoryPanel.tsx:34` | The loading state renders a bare `<div>` spinner without an accessible name; design-system §5.1 requires `role="status"` with a visible or screen-reader label. UIAC-02 cites §5.1, so this is inside the criterion the report marks covered. | Use the design system's `Spinner` with its label, and assert the accessible name in the loading test. |
| 4 | Minor | 4 | `useInspectionHistory.ts:18` | Cache key is `['inspection-history', id]`; the comparable permits hook uses `['permits','history',id]`. Inconsistent with the nearby pattern. | Align the key shape with `usePermitHistory`. |

## Dimension audit (check 11)

Part classification used: **frontend** (from PART_SPEC `part_type`)

| # | Dimension | Result | Evidence |
|---|---|---|---|
| D1 | Role and authorization behavior | PASS | `useInspectionHistory.test.ts: issues no request when the user lacks the Inspector role` asserts zero fetch calls; re-ran the report's mutation check (removed the `enabled` guard) and the test failed as recorded |
| D2 | Loading, success, empty, error states | PASS | All four rendered in `HistoryPanel.test.tsx`; the panel's states are driven by its own query, independent of the outcome form's mutation state (`HistoryPanel.tsx:28–74`) |
| D3 | Async lifecycle | **FAIL** | Initial, supersession, unmount, and failure are each tested. Clear/reset is not — see finding 1 |
| D4 | Error mapping and diagnostics | PASS | `traceId` from problem details is carried through `useInspectionHistory.ts:57` into the rendered error state; asserted in `HistoryPanel.test.tsx: renders the error state with the trace reference` |
| D5 | Presentation of domain values | **FAIL** | `en-GB` asserts rendered text; `nl-NL` asserts a catalogue lookup — see finding 2. The outcome code itself never reaches the DOM in the `en-GB` path |
| D6 | Accessibility and design system | **FAIL** | Spinner lacks an accessible name — see finding 3. Keyboard reachability is deferred to Step 6b per UIAC-06, which is acceptable |
| D7 | Cache and state invalidation | PASS | `HistoryPanel.test.tsx: refetches history after an outcome is recorded` asserts two calls; re-ran the report's mutation check (removed `invalidateQueries`) and the test failed as recorded |
| D8 | Server-derived vs client-calculated | PASS | Ordering and the 50-entry cap come from the API response; nothing is re-sorted or recomputed client-side (`HistoryPanel.tsx:52`) |
| D9 | Shared-component / public-contract changes | N/A — this Part adds a feature-local component and hook; no shared component, hook, or public contract is touched. Verified by diffing `web/src/components/` and `web/src/api/` (only the generated client changed, and its shape is unchanged from P05) | Diff of `4c1e9a2` across `web/src/components/**` — empty |

## Requirement coverage audit (check 12)

- Criteria in spec: 12 — rows in §3b: 12 — missing: none
- `COVERED-*` rows whose cited test does not prove the behavior: **UIAC-03**
  (finding 2) — the `nl-NL` assertion passes with the display mapping removed
- Status inconsistencies against the coverage map / earlier Parts: none.
  `AC-05 NOT-YET (owner P09)` agrees with the map, and P09 is still `TODO`
- `DEFERRED` rows without a named step and owner: none — UIAC-06 names
  Step 6b and owner P10
- Final Part of slice: NO — one `NOT-YET` is legitimate at this point

## Remediation closure (re-reviews only)

N/A — round 1.

## Checks with no findings

1 (architecture alignment), 2 (feature spec alignment), 3 (Part scope),
5 (test quality — other than the two findings above), 6 (integration risks),
7 (overengineering), 8 (shortcut implementations), 9 (hidden contract
changes — §7 declarations match the diff, including the regenerated client),
10 (missing verification — all four `verify` commands re-run and reproduced).

## Verdict

**`REJECTED — MUST FIX`** — two Blockers: an untested and unimplemented
clear/reset branch (finding 1) and a localization criterion marked covered by
a test that passes with the display mapping removed (finding 2); finding 3
must be fixed with them, as it falls inside a criterion already claimed as
covered.
