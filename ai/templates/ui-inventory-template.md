# UI Inventory

<!-- This template captures the current state of all UI surfaces in an existing  -->
<!-- project. Used as input for deriving a design system during retrofit.         -->
<!-- Reference: ai/guides/glossary.md for term definitions.                      -->

## 1. Inventory Summary

<!-- High-level overview: how many screens/pages, what framework/library is      -->
<!-- used, what styling approach (CSS modules, utility classes, inline styles,    -->
<!-- styled-components, etc.), and the general state of UI consistency.           -->

## 2. Screens / Pages / Routes

<!-- Enumerate every screen, page, or route in the application.                  -->
<!-- For each, list: route/path, purpose, key components used.                   -->

| # | Route / Path | Screen Name | Purpose | Key Components |
|---|--------------|-------------|---------|----------------|
| | | | | |

## 3. Component Inventory

<!-- List every reusable or repeated component found in the codebase.            -->
<!-- For each, note: name, where it is used, and current styling approach.       -->

| # | Component Name | File Location | Used In (screens) | Styling Approach |
|---|----------------|---------------|--------------------|-----------------|
| | | | | |

## 4. Token Audit — Colors

<!-- Extract all color values currently used in the codebase.                    -->
<!-- Group similar colors and note where they appear.                            -->

| Color Value | Occurrences | Used For | Suggested Token Name |
|-------------|-------------|----------|---------------------|
| | | | |

## 5. Token Audit — Typography

<!-- Extract all font families, sizes, weights, and line heights in use.         -->

| Font Family | Size | Weight | Line Height | Occurrences | Used For |
|-------------|------|--------|-------------|-------------|----------|
| | | | | | |

## 6. Token Audit — Spacing

<!-- Extract spacing values (margins, padding, gaps) found in the codebase.     -->

| Spacing Value | Occurrences | Used For |
|---------------|-------------|----------|
| | | |

## 7. Patterns Identified

<!-- Describe recurring patterns: common layouts, repeated component shapes,    -->
<!-- shared state handling approaches. These become candidates for the design    -->
<!-- system's component catalog and layout patterns.                             -->

- Pattern 1:
- Pattern 2:

## 8. Anomalies & Inconsistencies

<!-- List one-off styles, conflicting patterns, or inconsistencies found.        -->
<!-- These are the primary targets for retrofit migration.                        -->

| # | Anomaly | Location | Description | Severity (High/Medium/Low) |
|---|---------|----------|-------------|---------------------------|
| | | | | |

## 9. Accessibility Audit

<!-- Note current accessibility state: are labels present? Is keyboard           -->
<!-- navigation supported? Are ARIA attributes used? What gaps exist?            -->

- Labels:
- Keyboard navigation:
- Color contrast:
- ARIA usage:
- Known gaps:

## 10. Completeness Checklist

<!-- Verify the inventory is complete before proceeding to design system          -->
<!-- derivation.                                                                  -->

- [ ] All routes/pages enumerated
- [ ] All shared/reusable components listed
- [ ] All one-off components listed
- [ ] Color values extracted from stylesheets, inline styles, and theme files
- [ ] Typography values extracted
- [ ] Spacing values extracted
- [ ] Patterns and anomalies documented
- [ ] Accessibility gaps noted
