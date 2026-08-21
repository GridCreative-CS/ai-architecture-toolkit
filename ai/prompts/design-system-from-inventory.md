# Design System from Inventory Prompt

Act as a **Senior UI/UX Designer and Design Systems Architect**.

## Objective

Derive a design system from an existing UI inventory. This prompt is used
during the **retrofit** track — when a project already has implemented UI
surfaces but lacks a unified design system.

## Inputs

- `architecture/ui-inventory.md` (produced by `ai/prompts/ui-inventory.md`)
- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `ai/project-context.md`

## Output

Write to:

- `architecture/design-system.md`

Use the template at `ai/templates/design-system-template.md`.

## Methodology

### 1. Analyze the inventory

Read the UI inventory. Identify:

- which color values are used most frequently — these become the token palette
- which font families, sizes, and weights dominate — these become the type
  scale
- which spacing values recur — these become the spacing scale
- which component patterns repeat — these become the component catalog
- which patterns are anomalies — these are migration targets, not design
  system candidates

### 2. Normalize tokens

For each token category:

1. Group near-duplicate values (e.g., `#1A73E8` and `#1B74E9` → one token)
2. Select the most common or intentional value as the canonical token
3. Name the token with a semantic name (e.g., `--color-primary`, not
   `--color-blue-500`)
4. Document which existing values map to each new token

Produce a mapping table:

| Existing Value(s) | Canonical Token | Token Value | Rationale |
|--------------------|----------------|-------------|-----------|
| `#1A73E8`, `#1B74E9` | `--color-primary` | `#1A73E8` | Most common usage across 12 components |

### 3. Formalize component catalog

For each repeated component pattern found in the inventory:

1. Define the canonical component (name, variants, states)
2. Map it to the normalized tokens
3. Note which existing components it replaces

For anomalies (one-off components), decide:

- **Absorb:** if the anomaly is a variation of an existing pattern, map it to
  the closest canonical component
- **Promote:** if the anomaly represents a legitimate new pattern, add it to
  the component catalog
- **Retire:** if the anomaly is unnecessary, flag it for removal during
  retrofit

### 4. Define layout patterns

Extract the most common page structure and layout patterns from the inventory.
Define a canonical page shell, content grid, and responsive behavior.

### 5. Standardize state patterns

Review how existing screens handle loading, success, error, and empty states.
Define a single standard for each, consistent with the frontend agent
methodology (§4: Handle states explicitly).

### 6. Set accessibility baseline

Based on the accessibility audit in the inventory, define the minimum
requirements. Where the inventory reveals gaps, the baseline should close
them.

### 6b. Emit the matrix and compute the contrast table

Two outputs are required regardless of what the inventory contained:

**Variant × state matrix.** For every component in the catalog, one row per
variant and one column per state it can occupy — at minimum `default`,
`hover`, `focus-visible`, `active`, `disabled`, plus `error` and `loading` for
anything that accepts input or displays fetched data. Every cell names the
tokens consumed in that state, or `N/A — <reason>`.

The inventory records what exists; existing code routinely leaves states
unstyled or specified only by browser default. Where the inventory shows no
treatment for a cell, the design system must **decide** one — an inherited
gap is still a gap, and carrying it forward silently is how it survives the
retrofit.

**Computed contrast table.** Compute every foreground/background pair with a
script or tool and record it in §2f. Retrofit makes this sharper than
greenfield: a token that keeps its value but moves onto a new surface has a
new contrast ratio. Recompute every pair against the derived surfaces rather
than carrying forward ratios verified under the old ones.

Any pair below its floor is fixed before the design system is written out.

### 7. Flag conflicts

Where the inventory contains genuinely conflicting patterns (e.g., two
different button styles used intentionally for different contexts), document
the conflict and propose a resolution:

| Conflict | Pattern A | Pattern B | Proposed Resolution |
|----------|-----------|-----------|---------------------|
| | | | |

## Rules

- derive from evidence — every design system decision must trace back to a
  specific inventory finding
- do not invent patterns not found in the inventory or required by the
  architecture
- preserve the dominant patterns — the design system should feel like a
  cleaned-up version of what already exists, not a foreign system
- flag all conflicts explicitly — do not silently choose one pattern over
  another
- produce concrete values (hex codes, px) — not abstract descriptions
- every variant × state cell is specified or marked `N/A — <reason>`; an
  unstyled state in the existing code is a decision to make, not a cell to
  leave blank
- every contrast ratio is computed against the derived surfaces and stated as
  a number; ratios verified under the previous palette do not carry over
- every token referenced in §3–§7 must be defined in §2 — dangling references
  and always-firing hardcoded fallbacks are inventory defects that must not
  survive into the design system

## After derivation

The design system is **not authoritative until it passes the completeness
gate**. Run `ai/prompts/design-system-completeness-gate.md` in a fresh session
(UI retrofit workflow Step 2b) before migration planning.

## References

- UI inventory template: `ai/templates/ui-inventory-template.md`
- Design system template: `ai/templates/design-system-template.md`
- Design system completeness gate: `ai/prompts/design-system-completeness-gate.md`
- Retrofit spec template: `ai/templates/retrofit-spec-template.md`
- Glossary: `ai/guides/glossary.md`
