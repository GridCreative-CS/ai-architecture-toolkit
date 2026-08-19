# Example — Part Quality Report

> **Illustrative example only.** Fictional project ("Fieldbook", a modular
> monolith with a React frontend). It shows the shape and the evidence level
> expected by `ai/templates/code-quality-checklist-template.md` — in
> particular §3b, the mutation checks, and the review snapshot. Copy the
> rigour, not the content. The matching review is
> `ai/examples/example-part-review.md`.

---

# Part Quality Report — P07: Inspection history panel

- **Slice:** S3.2 — Record inspection outcome
- **Part:** P07 — Inspection history panel (`ai-parts/S3.2/P07-history-panel.md`)
- **Part type:** frontend (from PART_SPEC `part_type`)
- **Feature spec:** `architecture/feature-specs/S3.2-record-inspection-outcome.md`
- **Date:** 2026-03-11
- **Executor:** part-executor-tdd

## Review snapshot

- **Base commit:** `4c1e9a2`
- **HEAD at report time:** `4c1e9a2` — all work is uncommitted
- **Diff command:** `git diff 4c1e9a2 -- web/src/features/inspections/`
- **Uncommitted worktree files in this Part:**
  `web/src/features/inspections/HistoryPanel.tsx`,
  `web/src/features/inspections/useInspectionHistory.ts`,
  `web/src/features/inspections/__tests__/HistoryPanel.test.tsx`,
  `web/src/features/inspections/__tests__/useInspectionHistory.test.ts`
- **Generated / untracked files belonging to this Part:**
  `web/src/api/generated/inspections.ts` (regenerated from the OpenAPI
  document by `npm run api:generate` — included in the reviewed target)

## 1. Part executed

Adds the inspection history panel to the inspection detail screen: a
role-gated list of past outcomes with its own loading, empty, and error
states, refreshing after a new outcome is recorded.

## 2. Files changed

| File | Change (added/modified/deleted) | Purpose |
| --- | --- | --- |
| `web/src/features/inspections/HistoryPanel.tsx` | added | Renders the history list and its four states |
| `web/src/features/inspections/useInspectionHistory.ts` | added | Query hook; gated on the Inspector role |
| `web/src/features/inspections/InspectionDetail.tsx` | modified | Mounts the panel; invalidates history after a recorded outcome |
| `web/src/api/generated/inspections.ts` | regenerated | `GET /inspections/{id}/history` client |
| `web/src/features/inspections/__tests__/HistoryPanel.test.tsx` | added | Panel behavior |
| `web/src/features/inspections/__tests__/useInspectionHistory.test.ts` | added | Hook behavior incl. role gating and lifecycle |

## 3. Tests added or updated

| Test file | Test cases | What behavior they lock |
| --- | --- | --- |
| `__tests__/useInspectionHistory.test.ts` | `issues no request when the user lacks the Inspector role`; `supersedes an in-flight request when the inspection id changes`; `cancels and clears results when the panel is reset`; `performs no state update after unmount`; `surfaces the problem-details traceId on failure` | Role gating as absence of a request; each lifecycle branch separately |
| `__tests__/HistoryPanel.test.tsx` | `renders its own loading state while history is pending`; `renders the empty state when history is empty`; `renders the error state with the trace reference`; `renders the localized outcome name in en-GB`; `renders the localized outcome name in nl-NL`; `refetches history after an outcome is recorded` | Per-source states; display mapping per locale; observable refresh |

**TDD evidence:**

- Red observed (command + exact failure):
  `npm test -- useInspectionHistory` →
  `● issues no request when the user lacks the Inspector role — expected fetch not to have been called, received 1 call to /inspections/a91/history`
- Green achieved (command + result):
  `npm test -- inspections` → `Tests: 11 passed, 11 total`

**Mutation checks**

| Behavior | Mutation applied (file:line + what was changed) | Test that failed | Observed failure | Restored + suite green |
| --- | --- | --- | --- | --- |
| Authorization guard | `useInspectionHistory.ts:24` — removed the `enabled: hasRole('Inspector')` guard | `issues no request when the user lacks the Inspector role` | `expected fetch not to have been called, received 1 call` | Restored; `npm test -- inspections` → 11 passed |
| Cache invalidation | `InspectionDetail.tsx:88` — removed `queryClient.invalidateQueries(['inspection-history', id])` | `refetches history after an outcome is recorded` | `expected 2 calls to /history, received 1` | Restored; 11 passed |
| Cancellation / supersession | `useInspectionHistory.ts:41` — dropped the `signal` passed to `fetch` | `supersedes an in-flight request when the inspection id changes` | `expected the stale response to be ignored, received stale rows in state` | Restored; 11 passed |

Mutations were never committed.

## 3b. Requirement coverage matrix

| Requirement | Source | Implementation location | Positive test | Negative/edge test | Verification evidence | Status |
| --- | --- | --- | --- | --- | --- | --- |
| SEC-01 | spec §9 — only Inspectors read history | `useInspectionHistory.ts:24` | `useInspectionHistory.test.ts: fetches history for an Inspector` | `useInspectionHistory.test.ts: issues no request when the user lacks the Inspector role` | `npm test -- useInspectionHistory` → 5 passed; mutation check above | COVERED-THIS-PART |
| SEC-02 | spec §9 — server rejects non-Inspectors | `src/Inspections/Api/HistoryEndpoint.cs:31` | `HistoryEndpointTests: returns 200 for Inspector` | `HistoryEndpointTests: returns 403 for Viewer` | `dotnet test --filter HistoryEndpoint` → 6 passed | COVERED-EARLIER (P05) |
| AC-03 | spec §11 — history lists outcomes newest first | `HistoryPanel.tsx:52` | `HistoryPanel.test.tsx: orders entries newest first` | `HistoryPanel.test.tsx: renders the empty state when history is empty` | `npm test -- HistoryPanel` → 6 passed | COVERED-THIS-PART |
| AC-04 | spec §11 — recording an outcome refreshes history | `InspectionDetail.tsx:88` | `HistoryPanel.test.tsx: refetches history after an outcome is recorded` | — (single behavior; failure path covered by UIAC-04) | Mutation check above | COVERED-THIS-PART |
| AC-05 | spec §11 — export includes history | — | — | — | Owned by the export Part | NOT-YET (owner P09) |
| DR-02 | spec §6 — an outcome is immutable once recorded | `src/Inspections/Domain/Outcome.cs:44` | `OutcomeTests: rejects a second recording` | `OutcomeTests: rejects an edit after recording` | `dotnet test --filter Outcome` → 9 passed | COVERED-EARLIER (P03) |
| UIAC-02 | spec §11b — panel has its own loading/empty/error states, per design-system §5.1 | `HistoryPanel.tsx:28–74` | `HistoryPanel.test.tsx: renders its own loading state while history is pending` | `HistoryPanel.test.tsx: renders the error state with the trace reference` | `npm test -- HistoryPanel` → 6 passed | COVERED-THIS-PART |
| UIAC-03 | spec §11b — outcome names render localized, never the domain code | `HistoryPanel.tsx:61` | `HistoryPanel.test.tsx: renders the localized outcome name in en-GB` | `HistoryPanel.test.tsx: renders the localized outcome name in nl-NL` | `npm test -- HistoryPanel` → both assert rendered text, not catalogue keys | COVERED-THIS-PART |
| UIAC-04 | spec §11b — error state shows the support reference | `HistoryPanel.tsx:70` | `HistoryPanel.test.tsx: renders the error state with the trace reference` | `useInspectionHistory.test.ts: surfaces the problem-details traceId on failure` | `npm test -- inspections` → 11 passed | COVERED-THIS-PART |
| UIAC-06 | spec §11b — panel is reachable and operable by keyboard | `HistoryPanel.tsx:28` | — | — | Browser verification of the full slice | DEFERRED (Step 6b, owner P10) |
| UIAC-07 | spec §11b — print layout for history | — | — | — | Withdrawn from the slice at Step 4b reconciliation; spec row marked WITHDRAWN | N/A — removed from slice scope at Step 4b |
| PART_SPEC AC 1 | P07 — panel mounts on the detail screen without shifting existing layout | `InspectionDetail.tsx:120` | `InspectionDetail.test.tsx: renders the history panel below the outcome form` | `InspectionDetail.test.tsx: keeps the outcome form usable while history is pending` | `npm test -- InspectionDetail` → 4 passed | COVERED-THIS-PART |

Owners taken from the Requirement Coverage Map in `ai-parts/S3.2/OVERVIEW.md`.

## 4. Checks run

| Command | Result |
| --- | --- |
| `npm test -- inspections` | 11 passed, 0 failed |
| `npm run lint` | 0 errors, 0 warnings |
| `npm run typecheck` | 0 errors |
| `npm run build` | succeeded |

## 5. Architecture rules verified

- Module/layer boundaries respected: the panel consumes the generated
  Inspections API client only; no direct access to another feature's store.
  Verified by `npm run lint:boundaries`.
- Dependency direction respected: YES
- ADRs applied: ADR-0011 (server-owned enumerations are rendered through the
  locale catalogue, never displayed raw)
- New boundaries covered by architecture tests: N-A — no new boundary

## 6. Existing patterns followed

- Nearby files read before implementing: `web/src/features/permits/HistoryPanel.tsx`,
  `web/src/features/permits/usePermitHistory.ts`, and both their test files
- Patterns followed: query-hook shape and cache-key naming from
  `usePermitHistory`; state rendering order (loading → error → empty →
  content) from the permits panel; locale lookup through `useDisplayName`;
  test naming in the project's `it('<behavior>')` style

## 7. Contract surfaces

- Public API: UNCHANGED — consumes `GET /inspections/{id}/history` added in P05
- Database/schema: UNCHANGED
- Events/messages: UNCHANGED
- UI behavior: CHANGED — the inspection detail screen gains a history panel
  (spec §5 flow step 4, §11b UIAC-02); no route or shared-component contract
  changed

## 8. Dependencies

- New libraries/packages added: NONE

## 9. Deviations from existing patterns

The permits panel fetches on mount unconditionally; this panel gates the query
on the Inspector role (spec SEC-01), so the request is not issued at all for
other roles. Deviation is required by the spec and is noted here rather than
silently diverging.

## 10. Remaining risks

The history endpoint is unpaginated; inspections with very long histories will
render every row. Spec §8 caps history at 50 entries server-side, so this is
bounded today — revisit if that cap is raised.

## 10b. Remediation log

N/A — first submission of this Part.

## 11. Prohibited-output check

- No TODO/FIXME/placeholders/stubs in production paths: PASS
- No fake implementations: PASS
- No dead/unused/commented-out code introduced: PASS
- No existing test weakened, deleted, or skipped: PASS

## 12. Verdict

**Part status: DONE**

§3b is complete for the slice (one `NOT-YET` remains, owned by P09; this is
not the slice's final Part), all three triggering behaviors carry mutation
checks, and the snapshot block is filled. Next: Step 6a review in a fresh
session.
