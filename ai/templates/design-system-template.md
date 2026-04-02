# Design System

<!-- This template defines the project's design system — the shared visual       -->
<!-- vocabulary that all UI surfaces must follow. One design system per project.  -->
<!-- Reference: ai/guides/glossary.md for term definitions.                      -->

## 1. Overview

<!-- 2–3 sentences: what this design system covers and which project it serves.  -->
<!-- State the technology context (e.g., React, Vue, server-rendered HTML) or    -->
<!-- note that the system is framework-agnostic.                                  -->

## 2. Design Tokens

<!-- Design tokens are the atomic values that define the visual language.         -->
<!-- Each token must be concrete (hex codes, px values, font stacks) — not       -->
<!-- abstract descriptions like "primary color" without a value.                  -->

### 2a. Color Palette

<!-- List every named color with its value.                                       -->
<!-- Example:                                                                     -->
<!-- | Token Name        | Value     | Usage                                   | -->
<!-- |-------------------|-----------|-----------------------------------------| -->
<!-- | --color-primary   | #1A73E8   | Primary actions, links, active states   | -->
<!-- | --color-error     | #D93025   | Error messages, destructive actions      | -->

| Token Name | Value | Usage |
|------------|-------|-------|
| | | |

### 2b. Typography

<!-- Define the type scale: font families, sizes, weights, and line heights.     -->
<!-- Example:                                                                     -->
<!-- | Token Name     | Font Family       | Size  | Weight | Line Height         | -->
<!-- |----------------|-------------------|-------|--------|---------------------| -->
<!-- | --font-heading | Inter, sans-serif | 24px  | 600    | 1.3                 | -->
<!-- | --font-body    | Inter, sans-serif | 16px  | 400    | 1.5                 | -->

| Token Name | Font Family | Size | Weight | Line Height |
|------------|-------------|------|--------|-------------|
| | | | | |

### 2c. Spacing Scale

<!-- Define the spacing scale used for margins, padding, and gaps.               -->
<!-- Example: 4px, 8px, 12px, 16px, 24px, 32px, 48px, 64px                      -->

| Token Name | Value | Usage |
|------------|-------|-------|
| | | |

### 2d. Breakpoints

<!-- Define responsive breakpoints.                                               -->
<!-- Example:                                                                     -->
<!-- | Name    | Min Width | Target                                              | -->
<!-- |---------|-----------|-----------------------------------------------------| -->
<!-- | mobile  | 0px       | Mobile devices                                      | -->
<!-- | tablet  | 768px     | Tablets and small laptops                           | -->
<!-- | desktop | 1024px    | Desktop and large screens                           | -->

| Name | Min Width | Target |
|------|-----------|--------|
| | | |

### 2e. Other Tokens

<!-- Border radii, shadows, z-index scale, animation durations, opacity levels.  -->
<!-- Add rows as needed.                                                          -->

| Token Name | Value | Usage |
|------------|-------|-------|
| | | |

## 3. Component Catalog

<!-- List the core UI components. For each component, describe its purpose,      -->
<!-- variants, and which design tokens it consumes. Components listed here are    -->
<!-- the only approved building blocks for UI surfaces.                           -->

### 3a. Buttons

<!-- Variants (e.g., primary, secondary, destructive, ghost, disabled).          -->
<!-- For each variant: background color token, text color token, border,         -->
<!-- padding, font token.                                                         -->

### 3b. Form Inputs

<!-- Text input, select, checkbox, radio, textarea. Include focus state,         -->
<!-- error state, disabled state, and label association.                          -->

### 3c. Cards

<!-- Content containers. Describe padding, border, shadow, and spacing.          -->

### 3d. Navigation

<!-- Header, sidebar, breadcrumbs, tabs. Describe active/inactive states.        -->

### 3e. Feedback

<!-- Toast notifications, alerts, inline messages, progress indicators.          -->
<!-- Map to loading/success/error/empty states.                                   -->

### 3f. Modals & Dialogs

<!-- Overlay containers. Describe backdrop, sizing, close behavior, focus trap.  -->

### 3g. Additional Components

<!-- Add project-specific components as needed. Each must reference the tokens   -->
<!-- it consumes and the states it handles.                                       -->

## 4. Layout Patterns

<!-- Describe the page structure and layout system.                               -->

### 4a. Page Shell

<!-- Overall page structure: header, main content area, sidebar (if any),        -->
<!-- footer. Describe how these regions are arranged and how they respond to      -->
<!-- breakpoints.                                                                 -->

### 4b. Content Grid

<!-- Grid system: number of columns, gutter widths, max content width.           -->
<!-- Reference spacing tokens for gutters.                                        -->

### 4c. Responsive Behavior

<!-- How layouts adapt across breakpoints. Which components stack, collapse,     -->
<!-- or hide at each breakpoint.                                                  -->

## 5. State Patterns

<!-- Standard patterns for the four UI states that every API-backed              -->
<!-- interaction must handle (consistent with frontend-agent methodology §4).     -->

### 5a. Loading State

<!-- What the user sees while waiting for data. Skeleton screens, spinners,      -->
<!-- shimmer effects. Which components are used.                                  -->

### 5b. Success State

<!-- How results or confirmations are displayed after a successful operation.    -->

### 5c. Error State

<!-- How errors are communicated. Map to RFC 7807 problem details where          -->
<!-- applicable. Inline vs. toast vs. page-level errors.                          -->

### 5d. Empty State

<!-- What the user sees when no data exists. Distinguish between first-use       -->
<!-- empty and filtered-to-empty.                                                 -->

## 6. Accessibility Baseline

<!-- Minimum accessibility requirements that all components must meet.           -->

- **Color contrast:** minimum contrast ratios (WCAG AA: 4.5:1 for normal text,
  3:1 for large text)
- **Focus management:** visible focus indicators, logical tab order, focus
  trapping in modals
- **Keyboard navigation:** all interactive elements reachable and operable via
  keyboard
- **ARIA:** use semantic HTML first; add ARIA attributes only when semantic
  HTML is insufficient
- **Labels:** all form inputs have associated labels; images have meaningful
  alt text
- **Motion:** respect `prefers-reduced-motion` for animations

## 7. Iconography & Imagery

<!-- Icon set (e.g., Material Icons, Heroicons, custom). Sizing conventions.    -->
<!-- Image aspect ratios and placeholder behavior.                                -->

## 8. Open Questions / Evolution Notes

<!-- List any unresolved decisions or areas where the design system is expected  -->
<!-- to evolve. Note what would change if assumptions are wrong.                  -->
<!-- The design system is a living document — update it as slices reveal new     -->
<!-- patterns.                                                                    -->
