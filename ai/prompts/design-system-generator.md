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
- do not invent requirements not present in the architecture
- do not exceed MVP scope — the design system grows iteratively per slice
- do not duplicate architecture decisions — reference them

## References

- Design system template: `ai/templates/design-system-template.md`
- Feature spec template: `ai/templates/feature-spec-template.md`
- Vertical slice definition: `ai/guides/vertical-slice-definition.md`
- Glossary: `ai/guides/glossary.md`
