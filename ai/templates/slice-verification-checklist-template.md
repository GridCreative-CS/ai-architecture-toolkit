# Slice Completion Verification Checklist

<!-- This checklist is used during engineering workflow Step 6b (Integrated     -->
<!-- Slice Verification). Every item must pass before a UI slice can be marked  -->
<!-- done.                                                                      -->
<!-- Reference: ai/guides/definition-of-ready-and-done.md (UI completeness)    -->

## Metadata

- **Slice name:**
- **Feature spec:** `architecture/feature-specs/<slice-id>-<slice-name>.md`
- **Verification date:**
- **Verified by:**

## Application Startup

- [ ] All required services start without errors (API, frontend, database,
      migrations)
- [ ] Application is accessible at the expected URL
- [ ] No critical errors in startup logs

## Primary User Flow (from feature spec §5/§5b)

- [ ] Primary user flow executes successfully end-to-end
- [ ] Each step produces the expected visible outcome
- [ ] Error flow produces correct error feedback
- [ ] Empty state renders correctly (when applicable)
- [ ] Loading state is visible during async operations

## Interactive Behavior

- [ ] All buttons trigger the expected action
- [ ] All links navigate to the correct destination
- [ ] Form inputs accept input and validate correctly
- [ ] Form submission produces the expected result
- [ ] Navigation to and from this slice works

## Layout and Responsiveness

- [ ] Page renders correctly at desktop width (≥1024px)
- [ ] Page renders correctly at mobile width (≤480px)
- [ ] Shared layout elements (header, navigation, sidebar, footer) are intact
- [ ] No overflow, clipping, or broken layout at either viewport size
- [ ] Content is readable and interactive at both viewport sizes

## Cross-Slice Regression

- [ ] Previously completed slices still render correctly
- [ ] Shared navigation still works across all existing slices
- [ ] No browser console errors related to previously working features
- [ ] No visual regressions in other slices (layout, styling, spacing)

## Design System Compliance (when `architecture/design-system.md` exists)

- [ ] UI compliance check executed
- [ ] No critical findings remain unresolved
- [ ] All tokens, components, and patterns match the design system

## Accessibility Baseline

- [ ] Interactive elements reachable via keyboard
- [ ] Form inputs have associated labels
- [ ] Sufficient color contrast for text
- [ ] Focus indicator visible on interactive elements

## Requirement Coverage Rollup

Take the final Part's quality report §3b requirement coverage matrix and close
it out. Nothing may leave this step unproven.

| Criterion | Status in final Part §3b | Verified here | Evidence |
| --- | --- | --- | --- |
| | | | |

- [ ] Every criterion in the feature spec (§6 `DR-nn`, §9 `SEC-nn`, §11
      `AC-nn`, §11b `UIAC-nn`) appears in the rollup
- [ ] Zero criteria remain `NOT-YET`
- [ ] Every row marked `DEFERRED (Step 6b, …)` has been verified here, with
      the observed result recorded
- [ ] Every `N/A` row still has a reason that holds for the slice as built

A criterion arriving here unowned or unproven is a **slice failure**, not a
note: it means no Part ever implemented it and no review ever caught that.

## Evidence

- [ ] Application startup command documented
- [ ] Browser verification steps documented
- [ ] E2E test results documented (if automated tests exist)
- [ ] Pass/fail recorded for each criterion above

## Result

- **Status:** PASS / FAIL
- **Blocking issues:** <!-- list any failures that prevent marking the slice done -->
- **Notes:**
