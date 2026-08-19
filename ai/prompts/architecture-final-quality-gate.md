# Prompt: Architecture-Final Quality Gate

Act as a **Principal Architect performing the quality gate review of
`architecture/architecture-final.md`** — the last check before the document
becomes authoritative and ADR generation starts.

You review; you do not fix. Findings go back to the reconciliation step.

**This review must run in a fresh agent session/context** (a new session or a
subagent) — never in the session that wrote the final architecture document.
Base every judgment on the document text itself, not on what the writing
session intended.

## When this gate runs

After `architecture/architecture-final.md` is written (the reconciliation step
of any entry mode) and **before** ADR generation. A mode's finalization gate
(`ai/workflows/architecture-workflow.md`) is not passed until this gate's
verdict is `APPROVED` or `APPROVED WITH NOTES`.

## Inputs

- `architecture/architecture-final.md` — the document under review
- `ai/project-context.md`
- The mode's analysis and review artifacts, as they exist:
  `architecture/prototype-analysis.md`, `architecture/legacy-system-analysis.md`,
  `architecture/review-report.md`, `architecture/existing-architecture-review.md`,
  `architecture/prototype-architecture-alignment.md`, and the original
  existing architecture document (Modes B/C)
- `ai/templates/architecture-blueprint-template.md` — the content bar,
  including its Writing rules
- Companion documents that `architecture/architecture-final.md` cites by path
  (common in Modes B/C, e.g. supplementary test plans or go-live checklists
  carried over from the prior spec) — read them where a check's content is
  claimed to live there

If `architecture/architecture-final.md` is missing or still a placeholder,
stop — there is nothing to gate.

The checks below are **content-based, not structure-based**: a document that
keeps a different section layout (common in Modes B/C) passes if the content
is present, findable, and unambiguous. Judge coverage by content, never by
section numbers. Content may live in a companion document only if the final
document cites that document by path for that purpose — an uncited companion
does not count, however good it is.

## Checks (all required)

For each check, record PASS, FAIL, or N/A (with the reason the check does not
apply). Only check 9 (no human-facing UI) and check 10 (no AI components) may
be N/A as a whole; the personal-data condition inside check 6, the eventing
condition inside check 7, and the golden-dataset condition inside check 13
may be N/A individually. A bare "N/A" without a reason is a FAIL.

1. **Traceability and document control** — The document states its inputs
   (which analysis docs, project context, review reports it was produced
   from) and its status. Every Critical/Major finding from the review
   report(s) is mapped to a decision taken in the document (change log or
   equivalent) or explicitly deferred as an open question with rationale.
   No finding is silently dropped.
2. **Business context** — Business goals with measurable success criteria;
   users/stakeholders with their primary concerns; core workflows described
   concretely in domain language; regulatory/compliance context stated or
   explicitly ruled out. A reader can tell what business outcome the system
   exists to produce.
3. **Evidence and assumptions** — Material claims trace to an input (analysis
   document, project context, existing doc, user statement) or appear in an
   assumptions register with IDs. Constraints (budget, team, timeline,
   technology mandates) are listed with their source — a document-level
   citation suffices for a register wholly sourced from one named input;
   per-row citations are not required in that case. Exclusions — what the
   system deliberately does not do — are stated. Nothing project-shaping is
   invented.
4. **System boundary** — Inside vs outside is explicit; every external actor
   and system is named with the direction and purpose of the interaction.
5. **Modules and dependency rules** — Modules/bounded contexts with
   responsibilities and owned data; every capability described anywhere in
   the document is assigned to exactly one module (scan for orphans — a
   capability without an owner is a FAIL); allowed dependencies AND forbidden
   dependencies are explicit; a concrete enforcement mechanism is named
   (architecture tests in CI, project references, lint rules).
6. **Data architecture** — Data ownership per module; persistence
   technologies named; key entities per module at least named; schema
   evolution/migration governance defined. Where personal or sensitive data
   exists: protection, retention, and deletion/erasure are specified.
7. **APIs, events, integrations** — API style and versioning policy including
   what triggers a new version; error wire contract; every external
   integration has purpose, direction, and defined behavior when it is down;
   cross-module/eventing contracts named with producer and consumer (or
   eventing explicitly N/A).
8. **Security** — Authentication mechanism, authorization model
   (roles/policies named), secrets management, and data protection are
   concrete. "Industry-standard security" and equivalents are a FAIL.
9. **Frontend/backend boundary** — N/A only if the system has no human-facing
   UI. Otherwise: frontend stack; how the frontend consumes backend contracts;
   state/session/token handling; what logic is forbidden client-side — stated
   explicitly, or established by structural guarantees (e.g., a server-side
   gateway boundary, network egress controls) that the document identifies as
   enforcing the prohibition; the mere absence of client-side logic from the
   design is not enough.
10. **AI decision architecture** — N/A only if the system has no AI
    components. Otherwise: where AI decisions occur; deterministic and
    probabilistic responsibilities separated; confidence thresholds and how
    they are calibrated; fallback behavior on model failure or low
    confidence; human override points; model/prompt versioning;
    explainability (what explanation is produced per decision and where it
    is stored); governance (bias monitoring, review and rollback authority).
11. **Error handling and resilience** — An application-level error handling
    approach, and per-failure-mode behavior for every external dependency and
    critical component: retry policy, fallback, user-visible effect. "Handle
    errors gracefully" without the table of what actually happens is a FAIL.
12. **Observability and deployment** — Logging/metrics/tracing approach with
    tools and key signals; telemetry data classification where sensitive data
    exists; deployment target and topology; environments; backup/recovery
    with a stated recovery target where stateful.
13. **Testing strategy** — Test layers and what each proves;
    architecture-boundary enforcement tests matching check 5's rules; golden
    datasets where AI decision paths exist (or AI explicitly N/A); validation
    gates with binary pass criteria.
14. **Decisions, risks, open questions** — Every major decision has rejected
    alternatives and rationale (ADR-ready: context, alternatives, and
    consequences are extractable — or, where authoritative ADR files already
    exist (Modes B/C), every major decision references its ADR); risk
    register with impact, likelihood, and mitigation; open questions with
    impact and resolution path. Trade-offs name their cost, not only their
    benefit.
15. **Specificity and language** — Scan the document for banned vague terms:
    "scalable", "robust", "maintainable", "flexible", "secure", "performant",
    "production-ready", "highly available", "enterprise-grade",
    "best-practice", and similar quality adjectives. Each occurrence passes
    only if the same passage states the mechanism that achieves the quality
    and how it is verified. Scale/load context is quantified (users,
    concurrency, data volume, latency targets) or covered by an identified
    assumption. No TODOs, placeholders, or template comments remain.
16. **Downstream sufficiency** — The document is specific enough that:
    (a) the ADR generator can extract each major decision with context,
    alternatives, and consequences; (b) the delivery planner can derive
    vertical slices from the capability map and module boundaries;
    (c) a feature spec can cite concrete contracts, owned data, and error
    identifiers from it; (d) an implementer who contradicts the document
    would be detectably wrong, not arguably compliant.

## Verdict (exactly one)

- **`APPROVED`** — every applicable check passes.
- **`APPROVED WITH NOTES`** — every applicable check passes except gaps that
  the document itself already records as open questions with impact and
  resolution path (nothing is silently missing), and no gap blocks ADR
  generation or delivery planning. List each note and where it is expected to
  be resolved.
- **`REJECTED — MUST FIX`** — any applicable check fails. The document is
  **not authoritative** and ADR generation **must not start**. Return the gate
  report to the reconciliation step (`ai/prompts/architecture-reconciler.md`
  or `ai/prompts/architecture-gap-reconciler.md`, per the mode) as an
  additional input, then re-run this gate on the revised document until the
  verdict is `APPROVED` or `APPROVED WITH NOTES`.

Never soften a FAIL to a note because fixing it is inconvenient. Never fail a
check for a structural preference (section order, heading names) — tie every
finding to missing, vague, unevidenced, or contradictory content.

## Output

Write the gate report to:

- `architecture/architecture-final-gate.md`

using this structure:

```markdown
# Architecture-Final Quality Gate — <project name>

- Document reviewed: architecture/architecture-final.md (<version/date if stated>)
- Date:
- Reviewer: <agent/model>
- Verdict: <APPROVED | APPROVED WITH NOTES | REJECTED — MUST FIX>

## Check results

| # | Check | Result | Evidence / location in document |
|---|-------|--------|--------------------------------|
| 1 | Traceability and document control | PASS/FAIL/N/A | |
| … | … | | |

## Findings

| # | Check | Finding | Required fix |
|---|-------|---------|--------------|

(Write "No findings." when clean. Every finding must name a concrete required
fix — not "add more detail".)

## Notes (APPROVED WITH NOTES only)

<Each note: what is open, where the document records it, where/when it is
expected to be resolved.>

## Verdict justification

<2–4 sentences.>
```

## Rules

- do not edit `architecture/architecture-final.md` — report; the
  reconciliation step fixes
- verify claims against the inputs where possible: if the document asserts a
  constraint or domain fact, confirm the analysis, project context, or an
  assumption entry supports it
- quote or cite document locations as evidence for FAIL results
- apply "Not applicable — [reason]" sections as PASS for their check when the
  reason is sound; a bare "N/A" without a reason is a FAIL

## References

- Content bar and writing rules: `ai/templates/architecture-blueprint-template.md`
- Sample gate report (expected shape and depth): `ai/examples/example-architecture-final-gate-report.md`
- Mode selector and finalization gate: `ai/workflows/architecture-workflow.md`
- Glossary: `ai/guides/glossary.md`
