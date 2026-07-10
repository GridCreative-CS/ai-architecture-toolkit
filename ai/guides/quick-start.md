# Quick-Start Guide

Get from zero to your first vertical slice in 15 minutes. This guide walks you through the minimum viable path — fill in context, design architecture, plan delivery, and execute with TDD.

## Before you begin

You need:

- An AI coding agent (GitHub Copilot, Claude, Cursor, or any LLM-based assistant)
- A prototype, an architecture document, or both
- 15–30 minutes for the first pass

## Step 1 — Fill in project context

Copy `ai/templates/project-context-template.md` to `ai/project-context.md` and fill it in. This gives every downstream prompt the information it needs about your project.

## Step 2 — Choose your entry mode

Your starting point depends on what you have:

| You have | Entry mode | Workflow file |
|----------|------------|---------------|
| A prototype, no architecture doc | Mode A | `ai/workflows/architecture-workflow-prototype-only.md` |
| A prototype + an architecture doc | Mode B | `ai/workflows/architecture-workflow-prototype-plus-architecture-doc.md` |
| An architecture doc, no prototype | Mode C | `ai/workflows/architecture-workflow-architecture-doc-only.md` |
| A legacy system to replace, not repair | Mode D | `ai/workflows/architecture-workflow-legacy-system-replacement.md` |

If unsure, see `ai/guides/how-to-choose-entry-mode.md`.

## Step 3 — Run the architecture workflow

Prompt your AI agent with each step in the workflow for your entry mode. The typical Mode A path is:

1. **Analyze the prototype** — prompt with `ai/prompts/prototype-analyzer.md` → write output to `architecture/prototype-analysis.md`
2. **Design architecture** — prompt with `ai/prompts/architecture-designer.md` → `architecture/architecture-blueprint.md`
3. **Review architecture** — prompt with `ai/prompts/architecture-reviewer.md` → `architecture/review-report.md`
4. **Reconcile feedback** — prompt with `ai/prompts/architecture-reconciler.md` → `architecture/architecture-final.md`
5. **Generate ADRs** — prompt with `ai/prompts/adr-generator.md` → `architecture/adr/*.md`

After this step, `architecture/architecture-final.md` is your source of truth.

## Step 3b — Create a design system (UI projects — mandatory when the project has human-facing UI)

If your project includes human-facing UI, create a design system before
delivery planning:

- **New project:** prompt with `ai/prompts/design-system-generator.md` → `architecture/design-system.md`
- **Existing project with UI:** follow `ai/workflows/ui-retrofit-workflow.md` to inventory existing UI and derive a design system

If your project has no UI, skip this step.

## Step 4 — Create a delivery plan

Prompt with `ai/prompts/delivery-planner.md` → `architecture/delivery-plan.md`

The delivery plan organizes work into milestones and vertical slices. Each slice proves an end-to-end user workflow.

## Step 5 — Pick your first slice and write a feature spec

Select the first slice from the delivery plan, then prompt with `ai/prompts/feature-spec-generator.md` → `architecture/feature-specs/<slice-id>-<slice-name>.md`

The feature spec makes one slice precise enough to decompose and implement.

## Step 6 — Decompose and execute with TDD

1. **Decompose** — use `.github/skills/plan-decomposer` to break the slice into independently verifiable Parts → `ai-parts/<slice-id>/OVERVIEW.md` and `ai-parts/<slice-id>/PXX-*.md`
2. **Execute** — use `.github/skills/part-executor-tdd` to implement one Part at a time with strict red-green-refactor TDD, following `ai/guides/code-quality-standard.md` (read nearby code first, follow existing patterns). Every Part ends with a Part Quality Report → `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`
3. **Review** — run the Part code review (engineering workflow Step 6a, `ai/prompts/code-quality-reviewer.md`) → `ai-parts/<slice-id>/reviews/<part-id>-review.md`. Start the next Part only after an `APPROVED` or `APPROVED WITH NOTES` verdict

Implementation starts at **Step 6.2** — when you execute a specific Part file.
Approving the delivery plan alone does not start implementation; it only tells
you which slice to prepare next.

Repeat steps 5–6 for each slice in the delivery plan.

## Where to find the plan and execution artifacts

Use these paths as your handoff trail from planning to implementation:

| Stage | File(s) | What you find there |
|------|---------|---------------------|
| Delivery plan | `architecture/delivery-plan.md` | Ordered slices and milestones |
| Selected slice spec | `architecture/feature-specs/<slice-id>-<slice-name>.md` | Scope, acceptance criteria, contracts, test implications |
| Decomposition overview | `ai-parts/<slice-id>/OVERVIEW.md` | Part index, execution order, preflight notes |
| Execution-ready work items | `ai-parts/<slice-id>/PXX-*.md` | The exact Part to implement with TDD |
| Part quality gate | `ai-parts/<slice-id>/reviews/<part-id>-quality-report.md` and `<part-id>-review.md` | The executor's quality report and the reviewer's verdict per Part |

## Five terms you need to know

| Term | One-line definition |
|------|---------------------|
| **Slice** | An end-to-end capability that proves a user workflow through all layers |
| **Feature Spec** | A detailed specification of exactly one slice — the bridge between planning and implementation |
| **Part** | The smallest independently verifiable unit of work within a slice — the TDD execution target |
| **Contract** | The complete testable agreement between a producer and a consumer (schema + behavior + NFRs) |
| **Design System** | The shared visual vocabulary (tokens, components, patterns) for a project's UI — documented in `architecture/design-system.md` |
| **Verticality test** | A slice passes if it proves user value through all required layers end-to-end |

See `ai/guides/glossary.md` for full definitions of all terms.

## Common mistakes to avoid

- **Don't skip the feature spec.** Decomposing directly from the delivery plan loses precision. The feature spec defines scope, acceptance criteria, and contracts for one slice.
- **Don't decompose without architecture.** Parts that contradict the approved architecture create rework. Always have `architecture/architecture-final.md` before execution.
- **Don't make horizontal slices.** A "database layer" or "API layer" slice is not vertical — it does not prove a user workflow. See `ai/guides/vertical-slice-definition.md`.
- **Don't skip TDD.** Parts are designed around the red-green-refactor cycle. Skipping it removes the verification guarantee.
- **Don't treat prototype code as architecture.** A prototype shows *what the system does* (reference behavior), not *how it should be organized* (reference architecture). Extract behavior, then design architecture.

## FAQ

**When do I need a design system?**
If your project has human-facing UI, create a design system after architecture finalization. For new projects, use `ai/prompts/design-system-generator.md`. For existing projects with inconsistent UI, follow `ai/workflows/ui-retrofit-workflow.md`. Projects with no UI can skip this entirely.

**When do I need a compliance check?**
For every slice: run `ai/prompts/architecture-compliance.md` before decomposition (engineering workflow Step 4). Trivial slices that pass all six trigger questions in Step 4 may use the lightweight mode (boundaries, verticality, touched contracts only). For slices with human workflow surfaces, also run `ai/prompts/ui-compliance-check.md` (Step 4a — mandatory).

**When do I use specialist agents?**
During execution (Step 6). Ask your AI agent to adopt a specialist role: `ai/agents/backend-agent.md` for backend work, `ai/agents/frontend-agent.md` for UI, `ai/agents/qa-agent.md` for testing strategy, etc. See `ai/guides/toolkit-map.md` for the full list.

**What if I only have a prototype?**
Use Mode A. The toolkit is designed for exactly this — extracting validated architecture from working prototypes.

**What if my architecture doc is outdated?**
Use Mode B. The prototype-architecture alignment step will surface gaps and contradictions, and the gap reconciler will produce a corrected final architecture.

**What if I have an outdated architecture doc but no prototype?**
Use Mode C. The existing-architecture-reviewer prompt will identify gaps and risks, and the gap reconciler will produce a corrected final architecture.

**What if all I have is a legacy system I want to replace?**
Use Mode D. The legacy-system-analyzer extracts business intent, workflows, and compatibility constraints (not the legacy architecture), and the architecture designer designs the replacement system from that analysis.

**How do I prepare a slice without re-explaining the process each time?**
Use `ai/prompts/slice-preparation-runner.md` — it runs engineering workflow Steps 2–5 for one slice in a single agent session and stops before implementation.

## Next steps

- Read `ai/guides/toolkit-map.md` for a visual map of all toolkit components
- Read `ai/guides/operating-model.md` for the full phase-by-phase operating model
- Read `ai/examples/feature-spec-driven-slice-flow.md` for a worked example of the spec-to-implementation flow
