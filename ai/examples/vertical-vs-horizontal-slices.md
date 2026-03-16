# Vertical vs. Horizontal Slices — Concrete Example

## Context

A system with three capabilities:

1. **Document Ingestion** — upload and process documents
2. **AI Classification** — classify documents with human review
3. **Dashboard** — show classification results and allow overrides

The architecture specifies human-in-the-loop review for AI classification.

---

## BAD Plan — Frontend as a Separate Slice

| Slice | Contents |
|---|---|
| Slice 1: Document Ingestion Backend | Database schema, file storage, ingestion API |
| Slice 2: AI Classification Backend | Classification engine, ML pipeline, result storage |
| Slice 3: Frontend | Upload UI, review UI, dashboard UI, override UI |

### Why This Fails

- **Slices 1 and 2** cannot be demonstrated to a stakeholder — no user can
  upload a document or review a classification.
- **Slice 3** bundles all UI, creating a massive integration bottleneck where
  every backend assumption is tested for the first time.
- Human-in-the-loop review is deferred entirely to Slice 3, meaning the
  classification engine was built without validating reviewer workflows.
- Exit criteria for Slices 1–2 are integration tests only — no user-facing
  verification is possible.

### Verticality Test Results

| Slice | Q1: User-observable? | Q2: Human-in-the-loop complete? | Q3: User-facing verification? |
|---|---|---|---|
| Slice 1 | NO — no upload UI | N/A | NO — API test only |
| Slice 2 | NO — no review UI | NO — review deferred to Slice 3 | NO — API test only |
| Slice 3 | YES | YES | YES |

**Result:** Slices 1 and 2 fail the verticality test. This is a horizontal
plan.

---

## GOOD Plan — True Vertical Slices

| Slice | Contents |
|---|---|
| Slice 1: Document Upload & View | Upload API + file storage + minimal upload UI + document list view |
| Slice 2: AI Classification with Human Review | Classification engine + result storage + review UI with approve/reject |
| Slice 3: Dashboard & Override | Dashboard API + dashboard UI + override workflow |

### Why This Works

- **Slice 1** can be demonstrated: a user uploads a document and sees it in a
  list.
- **Slice 2** can be demonstrated: a document is classified and a reviewer
  approves or rejects the result.
- **Slice 3** can be demonstrated: a stakeholder views classification metrics
  and can override decisions.
- Human-in-the-loop review is proven in Slice 2, exactly where the capability
  lives.
- Each slice has a user-facing exit criterion.

### Verticality Test Results

| Slice | Q1: User-observable? | Q2: Human-in-the-loop complete? | Q3: User-facing verification? |
|---|---|---|---|
| Slice 1 | YES — upload and view | N/A | YES — user uploads and sees document |
| Slice 2 | YES — review result | YES — approve/reject UI included | YES — reviewer completes review |
| Slice 3 | YES — dashboard view | YES — override UI included | YES — stakeholder views and overrides |

**Result:** All slices pass the verticality test.

---

## Comparison: Exit Criteria

| Aspect | Bad Plan (Horizontal) | Good Plan (Vertical) |
|---|---|---|
| Slice 1 done when… | API returns 200 on POST /documents | User uploads a document and sees it listed |
| Slice 2 done when… | Classification service returns result JSON | Reviewer sees classification and approves/rejects it |
| Integration risk | All discovered in Slice 3 | Distributed across all slices |
| Stakeholder demo | Only possible after Slice 3 | Possible after every slice |
