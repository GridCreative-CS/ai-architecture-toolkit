# Feature Spec Reconciler — Quick Version

Act as a **Solution Architect and Feature Specification Reconciler**.

Use this prompt for lightweight reconciliation when the compliance report has
few findings (typically 1–3 items). For complex reconciliation with many
findings or scope changes, use the full version:
`ai/prompts/feature-spec-reconciler.md`.

## Inputs

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- compliance report for this slice
- current feature specification

## Source-of-truth priority

When sources conflict, resolve in this order:

1. final architecture
2. ADRs
3. delivery plan
4. compliance report
5. current feature spec

## Rules

- fix only what the compliance report requires — do not rewrite unrelated
  sections
- preserve valid existing content
- **never renumber criterion IDs** (`DR-nn` §6, `SEC-nn` §9, `AC-nn` §11,
  `UIAC-nn` §11b) — they are append-only keys for the Part Quality Report
  coverage matrix; rewording keeps the ID, a new criterion takes the next
  free number, and a dropped criterion stays marked `WITHDRAWN — <reason>`
- if the spec predates criterion IDs, assign them in document order and note
  it in the corrections summary
- narrow scope if the compliance report shows the slice is too broad
- add missing constraints where the compliance report identifies gaps:
  API/contract expectations, security/authorization, observability,
  acceptance criteria, test implications
- do not change the architecture or ADRs
- keep the feature spec decomposition-ready (see `ai/guides/glossary.md`
  for what "decomposition-ready" means)

## Output

Write the corrected feature specification back to its existing file
(`architecture/feature-specs/<slice-id>-<slice-name>.md`), keeping the same
section structure as the original.

At the end, add:

### Compliance Corrections Applied

| Finding | Change Made | Status |
|---------|-------------|--------|
| (finding from report) | (what changed in the spec) | Fixed / Deferred with rationale |

### Open Questions

List any ambiguities that need resolution before decomposition.

## References

- Full reconciler: `ai/prompts/feature-spec-reconciler.md`
- Glossary: `ai/guides/glossary.md`
