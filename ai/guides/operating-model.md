# Operating Model

The full phase-by-phase model. File paths are the canonical outputs; the
engineering workflow (`ai/workflows/engineering-workflow.md`) owns the
canonical step numbering.

## Phase 1 — Architecture

Entry depends on the mode (see `ai/workflows/architecture-workflow.md` and
`ai/guides/how-to-choose-entry-mode.md`):

| Mode | Prompts (in order) | Mode-specific outputs |
| --- | --- | --- |
| **A — Prototype Only** | `prototype-analyzer` → `architecture-designer` → `architecture-reviewer` → `architecture-reconciler` → `architecture-final-quality-gate` → `adr-generator` | `architecture/prototype-analysis.md`, `architecture/architecture-blueprint.md`, `architecture/review-report.md` |
| **B — Prototype + Doc** | `prototype-analyzer` → `existing-architecture-reviewer` → `prototype-architecture-alignment` → `architecture-gap-reconciler` → `architecture-final-quality-gate` → `adr-generator` | `architecture/prototype-analysis.md`, `architecture/existing-architecture-review.md`, `architecture/prototype-architecture-alignment.md` |
| **C — Doc Only** | `existing-architecture-reviewer` → `architecture-gap-reconciler` → `architecture-final-quality-gate` → `adr-generator` | `architecture/existing-architecture-review.md` |
| **D — Legacy Replacement** | `legacy-system-analyzer` → `architecture-designer` → `architecture-reviewer` → `architecture-reconciler` → `architecture-final-quality-gate` → `adr-generator` | `architecture/legacy-system-analysis.md`, `architecture/architecture-blueprint.md`, `architecture/review-report.md` |

All prompts live in `ai/prompts/`. All modes converge on the finalization gate:

- `architecture/architecture-final.md`
- `architecture/architecture-final-gate.md` — quality gate report with verdict
  `APPROVED` or `APPROVED WITH NOTES` (`architecture-final-quality-gate`, run
  in a fresh session; `REJECTED — MUST FIX` returns the document to the
  reconciliation step before ADRs may be generated)
- `architecture/adr/*.md`

## Phase 1b — UI Foundation (Conditional)

Mandatory when the project includes human-facing UI and no design system
exists. Skip entirely if the project has no UI.

**Greenfield** (new project, no existing UI):

- Prompt: `ai/prompts/design-system-generator.md`
- Output: `architecture/design-system.md`
- Gate: `ai/prompts/design-system-completeness-gate.md` → `architecture/design-system-gate.md` (Step 1b, mandatory, fresh session)
- Workflow: `ai/workflows/ui-foundation-workflow.md`

**Retrofit** (existing project, UI already implemented):

- Prompts: `ai/prompts/ui-inventory.md`, then `ai/prompts/design-system-from-inventory.md`
- Outputs: `architecture/ui-inventory.md`, then `architecture/design-system.md`
- Gate: `ai/prompts/design-system-completeness-gate.md` → `architecture/design-system-gate.md` (Step 2b, mandatory, fresh session)
- Workflow: `ai/workflows/ui-retrofit-workflow.md`

## Phase 2 — Delivery & Specification

Prompts:

- `ai/prompts/delivery-planner.md` → `architecture/delivery-plan.md`
  (validate verticality — engineering workflow Step 1b)
- `ai/prompts/feature-spec-generator.md` →
  `architecture/feature-specs/<slice-id>-<slice-name>.md` (one per slice)
- `ai/prompts/golden-dataset-generator.md` → `architecture/golden-datasets/`
  (mandatory for slices with AI decision paths or critical business rules —
  Step 3b)

## Phase 3 — Compliance & Execution

Per slice (engineering workflow Steps 4–6b):

Prompts:

- `ai/prompts/architecture-compliance.md` →
  `architecture/compliance-reports/<slice-id>-<slice-name>.md` (every slice —
  Step 4; full or lightweight per the six trigger questions)
- `ai/prompts/ui-compliance-check.md` →
  `architecture/compliance-reports/<slice-id>-<slice-name>-ui.md`
  (mandatory for slices with human workflow surfaces — Step 4a)
- `ai/prompts/feature-spec-reconciler.md` (or the quick version) — updates the
  feature spec in place when findings exist (Step 4b)
- `ai/prompts/slice-preparation-runner.md` — optional single-session runner
  for Steps 2–5

Skills:

- `.github/skills/plan-decomposer/SKILL.md` →
  `ai-parts/<slice-id>/OVERVIEW.md` (incl. the Requirement Coverage Map:
  every feature spec criterion → owning Part) + `ai-parts/<slice-id>/PXX-*.md`
  (Step 5)
- `.github/skills/part-executor-tdd/SKILL.md` — one Part at a time, strict TDD
  (Step 6); every Part ends with a Part Quality Report →
  `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`
  (`ai/templates/code-quality-checklist-template.md`)

Verification:

- Part Code Review → `ai-parts/<slice-id>/reviews/<part-id>-review.md`
  (mandatory per Part — Step 6a, using `ai/prompts/code-quality-reviewer.md`;
  twelve checks against a frozen snapshot, including the nine-dimension audit
  and the requirement coverage audit; the next Part starts only after
  `APPROVED` / `APPROVED WITH NOTES`)
- Integrated Slice Verification →
  `architecture/slice-verification/<slice-id>-<slice-name>.md`
  (mandatory for UI slices — Step 6b, using
  `ai/templates/slice-verification-checklist-template.md`)

Agents (`ai/agents/`):

- `orchestrator-agent.md`
- `backend-agent.md`
- `frontend-agent.md` (mandatory for slices with human workflow surfaces)
- `ai-agent.md`
- `qa-agent.md`
- `ai-testing-agent.md`
- `devops-agent.md`
- `code-reviewer-agent.md` (mandatory per Part — Step 6a)
- `integration-reviewer.md`

### Mandatory UI gates (for slices with human workflow surfaces)

- UI compliance check: `ai/prompts/ui-compliance-check.md` — Step 4a
- Integrated slice verification: `ai/templates/slice-verification-checklist-template.md` — Step 6b
- Frontend agent: `ai/agents/frontend-agent.md`

### Phases (not slices)

Infrastructure bootstrap and hardening phases use the same machinery: phase
spec (feature spec template, §5b/§11b/§12b marked N/A) → compliance check →
decomposition (`ai-parts/<phase-id>/`) → TDD execution with per-Part quality
reports and code review (Step 6a). UI gates do not apply.
See "Executing Phases" in `ai/workflows/engineering-workflow.md`.

### Remediation (for projects built under older toolkit versions)

- Workflow: `ai/workflows/ui-remediation-workflow.md`
- Template: `ai/templates/remediation-spec-template.md`
- Output: `architecture/remediation-audit.md` +
  `architecture/feature-specs/remediation-<slice-name>.md`
