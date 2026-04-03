# Remediation Spec — [Slice Name]

## 1. Slice Name

<!-- The name of the original slice being remediated. -->

## 2. Purpose

<!-- Brief statement: what this remediation fixes and why. -->
<!-- This is NOT a feature spec for new functionality. It fixes identified    -->
<!-- issues in a previously completed slice.                                   -->

## 3. Issues Found

<!-- List every issue discovered during the remediation audit (Step 1).       -->
<!-- For each issue, include:                                                  -->
<!-- - Description                                                             -->
<!-- - Severity (Blocking / Degraded / Cosmetic)                              -->
<!-- - Where it was observed (page, component, viewport, browser)             -->
<!-- - How to reproduce                                                        -->

| # | Description | Severity | Location | Reproduction Steps |
|---|-------------|----------|----------|--------------------|
| | | | | |

## 4. Root Cause Analysis

<!-- For each issue, identify the likely root cause:                           -->
<!-- - Missing state handling?                                                 -->
<!-- - CSS/layout issue?                                                       -->
<!-- - Missing integration between frontend and backend?                      -->
<!-- - Navigation/routing misconfiguration?                                    -->
<!-- - Missing error handling?                                                 -->
<!-- - Cross-slice interference?                                               -->

## 5. Proposed Fixes

<!-- For each issue, describe the minimal fix required.                        -->
<!-- Fixes must be scoped to the identified issue only — no refactoring,      -->
<!-- no redesign, no new features.                                             -->

| # | Issue | Proposed Fix | Files Affected |
|---|-------|-------------|----------------|
| | | | |

## 6. Scope Out

<!-- Explicitly list what this remediation does NOT cover:                     -->
<!-- - Visual/design-system migration (use ui-retrofit-workflow)              -->
<!-- - New features or enhancements                                            -->
<!-- - Backend behavioral changes                                              -->
<!-- - Performance optimization                                                -->

## 7. Acceptance Criteria

<!-- Binary, testable criteria that prove each issue is fixed.                -->
<!-- Each criterion maps to an issue from §3.                                  -->
<!-- Format: Given [context], when [action], then [expected outcome].          -->

## 8. Test Implications

<!-- For each fix:                                                             -->
<!-- - What E2E browser test should be added or updated?                       -->
<!-- - What component test (if any) should be added?                           -->
<!-- - How does this protect against regression?                               -->

## 9. Browser Verification Steps

<!-- Step-by-step browser walkthrough to confirm all issues are fixed.         -->
<!-- This becomes the input for Step 6b (Integrated Slice Verification).      -->
<!--                                                                           -->
<!-- Include:                                                                   -->
<!-- - Application startup commands                                            -->
<!-- - URL to access                                                           -->
<!-- - For each fixed issue: steps to verify the fix                           -->
<!-- - Cross-slice navigation check                                            -->
<!-- - Viewport sizes to check                                                 -->

## 10. Rollback Plan

<!-- If the remediation introduces new issues, how to revert safely.           -->

## 11. Open Questions

<!-- Any unknowns that need clarification before or during remediation.        -->
