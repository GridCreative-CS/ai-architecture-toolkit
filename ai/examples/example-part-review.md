# Example Part Code Review — P01: Load Saved Draft

> Fictional example showing a clean Step 6a review. The names and paths are
> illustrative and are not project requirements.

- **Slice:** draft-review — review a saved draft
- **Reviewed diff/files:** `src/Drafts/GetDraft.cs`, `tests/Drafts/GetDraftTests.cs`
- **Date:** 2026-02-15
- **Reviewer:** Principal Engineer / Fresh Review Model

## Review Snapshot

- **Base commit (SHA):** `1111111`
- **HEAD at review time:** `2222222`
- **Reproducible committed-diff command:** `git diff 1111111...2222222 -- src/Drafts/GetDraft.cs tests/Drafts/GetDraftTests.cs`
- **Worktree unchanged since snapshot:** YES

## Findings

No findings.

## Dimension Audit

| Dimension | Applicable? | Evidence | Result |
|---|---|---|---|
| D1 | YES | Owner identity and missing-draft invariants are asserted by the focused tests. | PASS |
| D2 | N/A — backend Part; no rendering or display mapping | No UI files or display contract touched. | PASS |
| D3 | N/A — backend Part; no interaction state transitions | No UI interaction state touched. | PASS |
| D4 | YES | Existing draft repository and not-found result contract are preserved; build and tests pass. | PASS |
| D5 | N/A — backend Part; no accessibility, design-system, or responsive surface | No UI files touched. | PASS |
| D6 | N/A — backend Part; no client lifecycle or cache behavior | Server cancellation is covered under AC-01; no client lifecycle touched. | PASS |
| D7 | YES | `SEC-01` ownership guard has a negative test and mutation evidence. | PASS |
| D8 | YES | Missing-draft mapping and cancellation behavior have focused assertions. | PASS |
| D9 | YES | Snapshot is unchanged; focused build/test evidence and Part Quality Report §3b are reproducible. | PASS |

## Requirement Coverage Audit

| Requirement | Owner | Implementation | Positive test | Negative/edge test | Verification | Result |
|---|---|---|---|---|---|---|
| `DR-01` | P01 | `GetDraft.cs:10-31` | `ReturnsDraftForOwner` | `MapsMissingDraftToNotFound` | 4 focused tests passed | PASS |
| `SEC-01` | P01 | `GetDraft.cs:18-22` | `ReturnsDraftForOwner` | `RejectsDraftOwnedByAnotherUser` | Owner-comparison mutation failed as expected | PASS |
| `AC-01` | P01 | `GetDraft.cs:12` | `ReturnsDraftForOwner` | `PropagatesCancellation` | Cancellation-token mutation failed as expected | PASS |

## Checks with no findings

Checks 1–12 all passed cleanly: architecture alignment, feature spec
alignment, Part scope, code quality, test quality, integration risks,
overengineering, shortcut implementations, hidden contract changes, missing
verification, dimension audit, and requirement coverage audit.

## Remediation Closure

N/A — initial review

## Verdict

**APPROVED** — the frozen diff stays within Part scope, preserves contracts,
provides behavior-proving positive and negative evidence for every owned
criterion, and passes all twelve checks.
