# Design System Completeness Gate Prompt

Act as a **Design Systems Auditor**.

## Objective

Verify that `architecture/design-system.md` is **complete enough to build
from** before any slice consumes it. The gate does not judge taste. It
establishes two properties:

1. **Renderability** — every component variant in every state is specified to
   the point where it could be drawn from the document alone, without a
   further decision.
2. **Computed conformance** — every colour pair the document specifies meets
   its stated contrast floor, as a calculated number.

Both are mechanical. Neither requires looking at a rendering.

## When this gate runs

- **Greenfield:** `ai/workflows/ui-foundation-workflow.md` Step 1b, after the
  design system is generated and before delivery planning.
- **Retrofit:** `ai/workflows/ui-retrofit-workflow.md` Step 2b, after the
  design system is derived from the inventory and before retrofit planning.
- **On evolution:** whenever a component, variant, state, or colour token is
  added or changed (UI foundation workflow Step 5). Re-run the gate for the
  affected components; a full re-run is required when a token in §2a changes
  value, because that changes the contrast context of every pair drawn on or
  against it.

Run it in a **fresh session**. The gate must not inherit the reasoning that
produced the document.

## Inputs

- `architecture/design-system.md` (the document under gate)
- `ai/project-context.md`
- `architecture/architecture-final.md` (for the human-in-the-loop surfaces the
  design system must serve)
- `architecture/ui-inventory.md` (retrofit path only, when present)

## Checks (all required)

### C1 — Variant × state matrix completeness

Every component in §3 must enumerate **every variant against every state it
can occupy**. Build the matrix yourself from the document and mark each cell
`SPECIFIED` / `MISSING` / `N/A — <reason>`.

**Scope the state axis to what the component can actually do.** For
**interactive** components (anything focusable, clickable, or accepting input)
the axis is at minimum `default`, `hover`, `focus-visible`, `active`,
`disabled`, plus `error` and `loading` where the component accepts input or
displays fetched data. For **static** components (a card that is only a
container, a layout region, a non-interactive badge) the axis is `default` plus
any state the document itself claims the component has.

Do not force a static component through the interactive axis. A design system
scoped to the first 2–3 slices should not have to write thirty `N/A` cells to
pass this check — if it does, the axis was applied too broadly, which is a
defect in the gate run, not in the document.

A cell is `MISSING` when the document names the variant but never says what
that variant looks like in that state. `N/A` requires a stated reason (a card
that is only a container has no `active`); a bare `N/A` is a FAIL.

FAIL if any cell is `MISSING`.

### C2 — Renderability of every specified cell

For each `SPECIFIED` cell, confirm it resolves to **concrete token references
or concrete values** — not prose intent. "Muted appearance" is not renderable;
`background: --color-surface-subtle; color: --color-text-disabled` is.

FAIL for each cell whose specification cannot be executed without inventing a
value.

### C3 — Computed contrast on every specified pair

Enumerate every foreground/background pair the document specifies — including
pairs that arise inside component states, not only the §2a palette. For each,
**compute** the WCAG contrast ratio and record it as a number.

Floors:

| Pair type | Floor | Source |
|-----------|-------|--------|
| Normal-size text | 4.5:1 | WCAG 2.2 AA 1.4.3 |
| Large text (≥18.66px bold or ≥24px) | 3:1 | WCAG 2.2 AA 1.4.3 |
| Non-text UI (borders, focus rings, icons, control boundaries) | 3:1 | WCAG 2.2 AA 1.4.11 |

Compute the ratios with a script or a tool and state which you used. **An
eyeballed or asserted ratio is not evidence** — a pair reported without a
number is a FAIL for that pair.

Where a token is applied at reduced opacity, compute against the **composited**
colour, not the base token.

Where the document already carries a §2f contrast table, recompute it rather
than reading it — the table is the author's claim, not the gate's evidence. A
§2f whose numbers do not reproduce is itself a finding.

FAIL for each pair below its floor.

### C4 — Semantic distinctness

Two tokens that carry different meanings must not resolve to the same value,
and two states of the same component must not render identically.

Report every collision: the tokens or states involved, their shared value, and
whether a user could distinguish them. A collision between tokens that are
deliberately aliased is a PASS only when the document states the alias and its
reason.

FAIL for each undeclared collision.

### C5 — State pattern renderability

§5 must specify `loading`, `success`, `error`, and `empty` concretely enough to
build: which component renders each, which tokens it consumes, and — for
`error` — how a transport or domain error becomes displayed text. Where the
project exposes an HTTP API, the `error` pattern must state the mapping from
RFC 9457 problem details to the displayed message.

`empty` must distinguish first-use empty from filtered-to-empty.

FAIL for each of the four states that cannot be built from the document.

### C6 — Token reference integrity

Every token referenced anywhere in the document (§3 components, §4 layouts,
§5 states, §7 iconography) must be **defined in §2**. Every token defined in
§2 should be referenced at least once, or marked as reserved with a reason.

Resolve each reference literally. A reference to a token that does not exist is
a FAIL even when a plausible fallback is supplied — a fallback that always
fires is a hardcoded value wearing a token's name.

FAIL for each dangling reference.

## Verdict (exactly one)

- **`APPROVED`** — every applicable check passes.
- **`APPROVED WITH NOTES`** — every applicable check passes except gaps the
  document itself already records in §8 with impact and resolution path
  (nothing is silently missing), and no gap blocks delivery planning or slice
  implementation. A contrast pair below its floor may **never** be a note; it
  is always a FAIL, because every downstream consumer treats the design system
  as having cleared its own accessibility baseline. List each note and where it
  is expected to be resolved.
- **`REJECTED — MUST FIX`** — any applicable check fails. The design system is
  **not authoritative**: delivery planning may proceed, but no slice may be
  decomposed or implemented against it until the findings are fixed and this
  gate re-run to `APPROVED` or `APPROVED WITH NOTES`.

Never soften a FAIL to a note because fixing it is inconvenient. Never fail a
check for a structural preference (section order, table formatting) — tie every
finding to a missing cell, an uncomputed pair, a collision, or a dangling
reference.

## Output

Write the gate report to:

- `architecture/design-system-gate.md`

using this structure:

~~~markdown
# Design System Completeness Gate — <project name>

- Document reviewed: architecture/design-system.md (<version/date if stated>)
- Date:
- Reviewer: <agent/model>
- Contrast computed with: <script/tool used>
- Verdict: <APPROVED | APPROVED WITH NOTES | REJECTED — MUST FIX>

## Check results

| # | Check | Result | Evidence |
|---|-------|--------|----------|
| C1 | Variant × state matrix completeness | PASS/FAIL | <n cells, n missing> |
| C2 | Renderability of specified cells | PASS/FAIL | |
| C3 | Computed contrast | PASS/FAIL | <n pairs computed, n below floor> |
| C4 | Semantic distinctness | PASS/FAIL | |
| C5 | State pattern renderability | PASS/FAIL | |
| C6 | Token reference integrity | PASS/FAIL | |

## Variant × state matrix

<The full matrix. One row per component variant, one column per state.>

## Contrast table

| Foreground | Background | Context | Ratio | Floor | Result |
|------------|------------|---------|-------|-------|--------|

(Every pair, with its computed number. Not a sample.)

## Findings

| # | Check | Finding | Required fix |
|---|-------|---------|--------------|

(Write "No findings." when clean. Every finding names a concrete required fix —
a value, a cell, a token — not "add more detail".)

## Notes (APPROVED WITH NOTES only)

<Each note: what is open, where §8 records it, when it is expected to resolve.>

## Verdict justification

<2–4 sentences.>
~~~

## Rules

- do not edit `architecture/design-system.md` — report; the design system
  author fixes and re-runs
- sweep, do not sample: every component, every state, every pair
- compute contrast; never assert it. State the tool or script used
- a claim that a pair "was verified under an earlier revision" is not evidence
  when any surface token has changed value since — the pair sits on a different
  ground and must be recomputed
- `N/A` requires a stated reason; a bare `N/A` is a FAIL
- do not judge aesthetics, brand fit, or token naming style

## References

- Design system template: `ai/templates/design-system-template.md`
- Design system generator: `ai/prompts/design-system-generator.md`
- Design system from inventory: `ai/prompts/design-system-from-inventory.md`
- UI foundation workflow: `ai/workflows/ui-foundation-workflow.md`
- UI retrofit workflow: `ai/workflows/ui-retrofit-workflow.md`
- UI compliance check: `ai/prompts/ui-compliance-check.md`
- Glossary: `ai/guides/glossary.md`
