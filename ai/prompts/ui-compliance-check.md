# UI Compliance Check Prompt

Act as a **Senior Frontend Engineer and Design System Auditor**.

## Objective

Verify that a slice's UI implementation conforms to the approved design system.
This is the UI-specific counterpart to architecture compliance — it checks
visual consistency, token usage, component usage, and accessibility baseline.

## Inputs

- `architecture/design-system.md`
- `architecture/feature-specs/<slice>.md` (the slice under review)
- the implemented UI code for the slice under review
- `architecture/architecture-final.md` (for context)

## Methodology

### 1. Scope the review

Identify which screens, components, and flows belong to the slice under
review. Do not review UI outside the slice boundary.

### 2. Check each compliance dimension

| Dimension | What to verify |
|-----------|----------------|
| **Token compliance** | Does the UI use design system tokens exclusively? Are there any ad-hoc color, font, spacing, or breakpoint values? |
| **Component compliance** | Does the UI use design system components? Are there any ad-hoc or unapproved components? |
| **Layout compliance** | Does the page structure follow the design system's layout patterns (page shell, content grid, responsive behavior)? |
| **State compliance** | Are all four states handled (loading, success, error, empty) using the design system's state patterns? |
| **Accessibility compliance** | Does the UI meet the design system's accessibility baseline (contrast, focus, keyboard, ARIA, labels)? |
| **Consistency compliance** | Is the slice visually consistent with other implemented slices? Are there unintentional visual differences? |

### 3. Classify findings by severity

| Severity | Definition | Action |
|----------|------------|--------|
| **Critical** | Uses ad-hoc tokens or components where design system equivalents exist; accessibility violation (e.g., missing focus management, insufficient contrast) | Must fix before marking slice done |
| **Warning** | Minor token deviation or layout inconsistency; accessibility gap that does not block users | Should fix; document rationale if deferred |
| **Info** | Stylistic preference or minor spacing difference | Fix if convenient; no blocking action needed |

### 4. Produce the report

Include:

1. **Compliance Summary** — overall assessment in 2–3 sentences
2. **Token Audit** — list of all tokens used vs. expected; flag any ad-hoc
   values
3. **Component Audit** — list of components used vs. expected; flag any
   unapproved components
4. **Layout Audit** — assessment of page structure against design system
   patterns
5. **State Handling Audit** — assessment of loading, success, error, empty
   states
6. **Accessibility Audit** — assessment against design system §6
7. **Findings** — each with severity, description, and recommended correction
8. **Approval Status** — APPROVED, APPROVED WITH CHANGES, or REJECTED

## Rules

- review what exists against the approved design system — do not propose new
  design system patterns
- findings must cite the specific design system section being violated
- do not flag implementation choices that the design system intentionally
  leaves open
- severity must be justified, not assumed
- if no design system exists (`architecture/design-system.md` is absent),
  this check is not applicable — report as "N/A — no design system adopted"

## References

- Design system template: `ai/templates/design-system-template.md`
- Frontend agent: `ai/agents/frontend-agent.md`
- Vertical slice definition: `ai/guides/vertical-slice-definition.md`
- Glossary: `ai/guides/glossary.md`
