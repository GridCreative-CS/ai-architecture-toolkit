# Retrofit Specification

<!-- This template defines a behavior-preserving UI migration for ONE existing   -->
<!-- slice. One retrofit spec = one slice migration.                              -->
<!-- Reference: ai/guides/glossary.md for term definitions.                      -->
<!-- Reference: ai/templates/design-system-template.md for the target system.    -->

## 1. Slice Name

<!-- The name of the existing slice being retrofitted. Must match a slice in     -->
<!-- the delivery plan.                                                           -->

## 2. Purpose

<!-- 2–3 sentences: why this slice needs retrofit and what the migration         -->
<!-- achieves. Focus on consistency, not new features.                            -->

## 3. Current State

<!-- Describe the slice's current UI: screens, components, styling approach,     -->
<!-- tokens in use. Reference the UI inventory for specifics.                     -->
<!-- Include screenshots or route paths where helpful.                            -->

### Screens Affected

| Screen / Route | Current Components | Current Styling |
|----------------|--------------------|-----------------|
| | | |

### Current Tokens in Use

<!-- List the ad-hoc color values, font sizes, spacing values currently used    -->
<!-- by this slice. Reference the UI inventory.                                   -->

## 4. Target State

<!-- Describe what the slice should look like after migration. Reference the     -->
<!-- design system for token names, component names, and patterns.               -->

### Target Components

| Screen / Route | Target Components | Design System Reference |
|----------------|-------------------|------------------------|
| | | |

### Target Tokens

<!-- Map current ad-hoc values to design system tokens.                          -->

| Current Value | Design System Token | Section Reference |
|---------------|--------------------|--------------------|
| | | |

## 5. Behavior Preservation Checklist

<!-- CRITICAL: List every observable behavior that must remain unchanged after   -->
<!-- the migration. This is the contract for green-to-green TDD.                 -->

- [ ] Behavior 1: (describe the user-observable behavior)
- [ ] Behavior 2:
- [ ] Behavior 3:

<!-- Add rows as needed. Every existing test assertion is implicitly included.   -->

## 6. Migration Steps

<!-- Ordered sequence of changes. Each step should be independently verifiable.  -->
<!-- Do NOT bundle behavioral changes with styling changes.                      -->

| # | Step | What Changes | Verification |
|---|------|-------------|--------------|
| 1 | | | |
| 2 | | | |
| 3 | | | |

### Migration Order

<!-- Recommended order: 1) token swaps, 2) component replacements, 3) layout   -->
<!-- adjustments. Justify any deviation.                                          -->

1. **Token swaps** — replace ad-hoc color/font/spacing values with design
   system tokens
2. **Component replacements** — swap ad-hoc components with design system
   components
3. **Layout adjustments** — align page structure with design system layout
   patterns

## 7. Scope Out

<!-- Explicitly list what this retrofit does NOT change. Prevents scope creep.   -->

## 8. Rollback Plan

<!-- How to revert if the migration breaks behavior. Should be possible at       -->
<!-- each migration step, not just at the end.                                    -->

## 9. Acceptance Criteria

<!-- Each criterion must be binary and testable.                                  -->

- [ ] All existing tests pass without modification (green-to-green)
- [ ] UI surfaces use only design system tokens (no ad-hoc values)
- [ ] UI surfaces use only design system components (no ad-hoc components)
- [ ] Visual appearance matches the design system target state
- [ ] Accessibility baseline from design system §6 is met
- [ ] UI compliance check (`ai/prompts/ui-compliance-check.md`) passes

## 10. Test Implications

<!-- What new tests are added? What existing tests must continue passing?        -->

- **Existing tests:** must continue passing unchanged (behavior preservation)
- **New tests:** design system conformance tests (e.g., token usage assertions)

## 11. Open Questions / Assumptions

<!-- List unresolved questions and working assumptions. Each assumption should   -->
<!-- note what would change if the assumption is wrong.                           -->
