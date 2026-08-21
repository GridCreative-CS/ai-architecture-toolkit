# Design System Generator Prompt

Act as a **Senior UI/UX Designer and Design Systems Architect**.

## Objective

Generate a design system v1 for a project that is starting fresh (greenfield).
The design system establishes the shared visual vocabulary — tokens, components,
layout patterns, state patterns, and accessibility baseline — that all UI
surfaces in the project must follow.

## Inputs

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `ai/project-context.md`

## Output

Write to:

- `architecture/design-system.md`

Use the template at `ai/templates/design-system-template.md`.

## Methodology

### 1. Understand the project context

Read the architecture and project context. Identify:

- what types of users will interact with the system
- what human-in-the-loop interactions are specified (see glossary)
- what technology constraints exist (framework, platform, device targets)
- what the first 2–3 slices in the delivery plan will need from a UI
  perspective

### 2. Define design tokens

Produce concrete, usable tokens:

- **Color palette** — primary, secondary, neutral, semantic (success, warning,
  error, info). Each with hex value and usage description.
- **Typography scale** — font families, size scale (heading 1 through body
  small), weights, line heights.
- **Spacing scale** — a consistent scale (e.g., 4px base with multipliers).
- **Breakpoints** — mobile, tablet, desktop at minimum.
- **Other tokens** — border radii, shadows, z-index scale, animation
  durations as needed.

Every token must have a concrete value. Do not use abstract descriptions
without values (e.g., "a calming blue" is not acceptable — `#1A73E8` is).

### 3. Define core components

Describe the minimum set of components needed for the first 2–3 slices:

- Buttons (primary, secondary, destructive, ghost, disabled)
- Form inputs (text, select, checkbox, radio, textarea — with states)
- Cards (content containers)
- Navigation (header, sidebar, breadcrumbs, tabs — as needed)
- Feedback (toast, alert, inline message, progress indicator)
- Modals (if any slice requires overlay interactions)

For each component, reference which tokens it consumes.

**Emit the full variant × state matrix.** For every component, produce a table
with one row per variant and one column per state it can occupy — at minimum
`default`, `hover`, `focus-visible`, `active`, `disabled`, plus `error` and
`loading` for any component that accepts input or displays fetched data. Every
cell states the tokens that variant consumes in that state, or `N/A — <reason>`
where the state cannot occur.

The matrix is the deliverable, not a summary of it. A cell that reads "muted"
or "de-emphasised" is not specified; `background: --color-surface-subtle;
color: --color-text-disabled` is. Naming variants without saying what they look
like in each state is the most common way a design system passes review and
then cannot be built from.

### 3b. Compute the contrast table

Do not assert that the palette meets its accessibility baseline — **compute
it**. Enumerate every foreground/background pair the design system specifies,
including pairs that arise only inside a component state (a disabled label on a
subtle surface, a focus ring against a card), and calculate each WCAG contrast
ratio with a script or tool.

Floors: 4.5:1 normal text, 3:1 large text, 3:1 non-text UI (borders, focus
rings, icons, control boundaries). Where a token is applied at reduced opacity,
compute against the composited colour, not the base token.

Any pair below its floor is fixed **before** the design system is written out —
by changing a token value, not by lowering the floor or by moving the pair into
§8 as an open question. Record the resulting table in §2f.

### 4. Define layout patterns

- Page shell (header, content area, sidebar, footer arrangement)
- Content grid (columns, gutters, max width)
- Responsive behavior across breakpoints

### 5. Define state patterns

Standardize the four states every API-backed interaction must handle:

- **Loading** — skeleton, spinner, or shimmer
- **Success** — result display or confirmation
- **Error** — error message display (map to RFC 9457 where applicable)
- **Empty** — first-use empty vs. filtered-to-empty

### 6. Define accessibility baseline

Set minimum requirements:

- WCAG AA contrast ratios
- Focus management and visible focus indicators
- Keyboard navigation for all interactive elements
- Semantic HTML first, ARIA when semantic HTML is insufficient
- Label associations and alt text
- Respect `prefers-reduced-motion`

## Scope Constraints

- **Produce only what the first 2–3 slices need.** Do not attempt a
  comprehensive component library. The design system is a living document
  that grows with the project.
- **Remain framework-agnostic** in descriptions unless the project context
  specifies a framework. If a framework is specified, note technology-specific
  implementation hints but keep the design system conceptually portable.
- **Do not design screens.** The design system defines the vocabulary; feature
  specs define how that vocabulary is used in each slice.

## Rules

- every token must have a concrete value — no abstract descriptions without
  values
- every component must reference the tokens it consumes
- every variant × state cell is specified or marked `N/A — <reason>`; a bare
  `N/A` is not acceptable
- every contrast ratio is computed and stated as a number; an asserted or
  eyeballed ratio is not evidence
- every token referenced in §3–§7 must be defined in §2 — a reference with a
  hardcoded fallback (`var(--color-focus-ring, #005fcc)`) is a dangling
  reference, not a safe default
- do not invent requirements not present in the architecture
- do not exceed MVP scope — the design system grows iteratively per slice
- do not duplicate architecture decisions — reference them

## After generation

The design system is **not authoritative until it passes the completeness
gate**. Run `ai/prompts/design-system-completeness-gate.md` in a fresh session
(UI foundation workflow Step 1b) before delivery planning. The gate re-derives
the matrix and recomputes every pair independently — writing them here is what
makes that check cheap, not what makes it unnecessary.

## References

- Design system template: `ai/templates/design-system-template.md`
- Feature spec template: `ai/templates/feature-spec-template.md`
- Vertical slice definition: `ai/guides/vertical-slice-definition.md`
- Glossary: `ai/guides/glossary.md`
