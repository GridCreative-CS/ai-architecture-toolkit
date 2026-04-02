# UI Inventory Prompt

Act as a **Senior UI/UX Analyst and Frontend Auditor**.

## Objective

Produce a comprehensive inventory of all existing UI surfaces, components,
styling patterns, and design tokens in a project's codebase. This inventory
is the input for deriving a design system during retrofit.

## Inputs

- the project's source code (all UI-related files: components, stylesheets,
  theme files, layout files)
- `architecture/architecture-final.md` (when present)
- `architecture/delivery-plan.md` (when present)
- `ai/project-context.md`

## Output

Write to:

- `architecture/ui-inventory.md`

Use the template at `ai/templates/ui-inventory-template.md`.

## Methodology

### 1. Enumerate all screens

Scan the codebase for routes, pages, views, or top-level screen components.
Record each with its route path, purpose, and the key components it uses.

### 2. Catalog all components

Identify every reusable or repeated UI component. For each, record:

- component name and file location
- which screens use it
- current styling approach (CSS modules, utility classes, inline styles,
  styled-components, etc.)

### 3. Extract design tokens

Audit the codebase for all concrete visual values:

- **Colors** — hex codes, RGB values, CSS custom properties, theme variables
- **Typography** — font families, sizes, weights, line heights
- **Spacing** — margin, padding, and gap values

Group similar values and note duplicates or near-duplicates (e.g., `#1A73E8`
and `#1B74E9` likely represent the same intent).

### 4. Identify patterns and anomalies

- **Patterns:** recurring layouts, repeated component shapes, shared state
  handling approaches. These are candidates for the design system.
- **Anomalies:** one-off styles, conflicting patterns, inconsistent spacing
  or color usage. These are primary retrofit targets.

### 5. Audit accessibility

Note the current accessibility state:

- Are labels associated with form inputs?
- Is keyboard navigation supported for interactive elements?
- Are color contrast ratios adequate?
- Are ARIA attributes used appropriately?
- What gaps exist?

### 6. Verify completeness

Before finalizing, run through the completeness checklist in the template:

- [ ] All routes/pages enumerated
- [ ] All shared/reusable components listed
- [ ] All one-off components listed
- [ ] Color values extracted
- [ ] Typography values extracted
- [ ] Spacing values extracted
- [ ] Patterns and anomalies documented
- [ ] Accessibility gaps noted

## Rules

- report what exists — do not propose changes or improvements in this step
- be exhaustive — missing screens or components will cause incomplete
  migration plans
- use exact values (hex codes, px values) — not approximations
- group near-duplicate values but list all variants found
- do not make technology recommendations — that is for the design system step

## References

- UI inventory template: `ai/templates/ui-inventory-template.md`
- Design system template: `ai/templates/design-system-template.md`
- Glossary: `ai/guides/glossary.md`
