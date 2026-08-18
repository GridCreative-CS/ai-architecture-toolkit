# Example Part Quality Report — P01: Load Saved Draft

> Fictional example showing the minimum evidence expected from a completed Part.
> The names and paths are illustrative and are not project requirements.

- **Slice:** draft-review — review a saved draft
- **Part:** P01 — Load Saved Draft (`ai-parts/draft-review/P01-load-saved-draft.md`)
- **Feature spec:** `architecture/feature-specs/draft-review-review-a-saved-draft.md`
- **Date:** 2026-02-14
- **Executor:** Backend Agent / Example Model

## Review Snapshot

- **Base commit (SHA):** `1111111`
- **HEAD at report time:** `2222222`
- **Reproducible committed-diff command:** `git diff 1111111...2222222 -- src/Drafts/GetDraft.cs tests/Drafts/GetDraftTests.cs`
- **Uncommitted worktree files in this Part:** `none`
- **Generated/untracked files belonging to this Part:** `none`

## 1. Part executed

Implemented the authenticated read path for a saved draft, including ownership
validation, not-found mapping, and cancellation propagation to the repository.

## 2. Files changed

| File | Change (added/modified/deleted) | Purpose |
| --- | --- | --- |
| `src/Drafts/GetDraft.cs` | added | Query handler and response mapping |
| `tests/Drafts/GetDraftTests.cs` | added | Observable success, ownership, not-found, and cancellation behavior |

## 3. Tests added or updated

| Test file | Test cases | What behavior they lock |
| --- | --- | --- |
| `tests/Drafts/GetDraftTests.cs` | `ReturnsDraftForOwner`; `RejectsDraftOwnedByAnotherUser`; `MapsMissingDraftToNotFound`; `PropagatesCancellation` | Authorized retrieval, authorization boundary, missing data contract, and cancellation behavior |

**TDD evidence:**

- Red observed (`dotnet test --filter FullyQualifiedName~GetDraftTests`): `ReturnsDraftForOwner` failed because the handler did not exist.
- Green achieved (`dotnet test --filter FullyQualifiedName~GetDraftTests`): 4 passed, 0 failed.

**Mutation-check evidence** (mandatory for authorization guards, cache
invalidation/refetch, cancellation/supersession, or error-to-message mapping;
record `N/A — <reason>` when no named trigger applies):

- **Trigger:** authorization guard, cancellation, and error-to-message mapping.
- **Mutation** (`src/Drafts/GetDraft.cs:18` and `src/Drafts/GetDraft.cs:24`, temporary change): removed the owner comparison and changed the missing-draft result to `Ok`; separately removed the cancellation token passed to the repository.
- **Observed test failure:** `RejectsDraftOwnedByAnotherUser`, `MapsMissingDraftToNotFound`, and `PropagatesCancellation` each failed with an observable mismatch.
- **Restoration and final green result:** restored all three guards/mappings; focused suite returned 4 passed, 0 failed.

## 3b. Requirement Coverage Matrix

- **Part classification:** `backend`
- **Classification evidence:** `PART_SPEC.part_type=backend`; all implementation touch points are the draft query handler and its backend tests.
- **Coverage-map source:** `ai-parts/draft-review/OVERVIEW.md` § Requirement Coverage Map

**Dimension-audit handoff:** Step 6a evaluates D1–D9. This backend Part
supplies evidence for D1, D4, D7, D8, and D9; the review records N/A for the
frontend-only dimensions D2, D3, D5, and D6.

| Requirement | Source | Implementation location | Positive test | Negative/edge test | Verification evidence | Status |
| --- | --- | --- | --- | --- | --- | --- |
| `DR-01` | §6 Draft retrieval | `src/Drafts/GetDraft.cs:10-31` | `ReturnsDraftForOwner` | `MapsMissingDraftToNotFound` | Focused test command: 4 passed | `COVERED-THIS-PART` |
| `SEC-01` | §9 Ownership | `src/Drafts/GetDraft.cs:18-22` | `ReturnsDraftForOwner` | `RejectsDraftOwnedByAnotherUser` | Mutation removed owner comparison and test failed | `COVERED-THIS-PART` |
| `AC-01` | §11 Cancellation | `src/Drafts/GetDraft.cs:12` | `ReturnsDraftForOwner` | `PropagatesCancellation` | Mutation removed token and test failed | `COVERED-THIS-PART` |

## 4. Checks run

| Command | Result |
| --- | --- |
| `dotnet test --filter FullyQualifiedName~GetDraftTests` | PASS — 4 passed, 0 failed |
| `dotnet build src/Drafts/Drafts.csproj --no-restore` | PASS |

## 5. Architecture rules verified

- Module/layer boundaries respected: the handler depends on the draft repository abstraction; architecture test `DraftsDoNotReferenceWeb` passed.
- Dependency direction respected: YES
- ADRs applied: `ADR-002` repository access remains behind the module boundary.
- New boundaries covered by architecture tests: YES

## 6. Existing patterns followed

- Nearby files read before implementing: `src/Drafts/ListDrafts.cs`, `src/Drafts/DraftErrors.cs`, `tests/Drafts/ListDraftsTests.cs`.
- Patterns followed: typed result mapping, repository cancellation tokens, and test naming match neighboring draft handlers.

## 7. Contract surfaces

- Public API: UNCHANGED — internal query handler only.
- Database/schema: UNCHANGED — existing repository query used.
- Events/messages: UNCHANGED — no events emitted.
- UI behavior: UNCHANGED — UI consumes the existing not-found contract.

## 8. Dependencies

- New libraries/packages added: NONE

## 9. Deviations from existing patterns

none

## 10. Remaining risks

none

## 11. Prohibited-output check

- No TODO/FIXME/placeholders/stubs in production paths: PASS
- No fake implementations: PASS
- No dead/unused/commented-out code introduced: PASS
- No existing test weakened, deleted, or skipped (or justified above): PASS

## 12. Verdict

**Part status: DONE**

The snapshot is reproducible, all three criteria have behavior evidence, the
triggered mutation checks are recorded, and the focused build/test commands
passed.
