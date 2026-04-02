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

If unsure, see `ai/guides/how-to-choose-entry-mode.md`.

## Step 3 — Run the architecture workflow

Prompt your AI agent with each step in the workflow for your entry mode. The typical Mode A path is:

1. **Analyze the prototype** — prompt with `ai/prompts/prototype-analyzer.md` → write output to `architecture/prototype-analysis.md`
2. **Design architecture** — prompt with `ai/prompts/architecture-designer.md` → `architecture/architecture-blueprint.md`
3. **Review architecture** — prompt with `ai/prompts/architecture-reviewer.md` → `architecture/review-report.md`
4. **Reconcile feedback** — prompt with `ai/prompts/architecture-reconciler.md` → `architecture/architecture-final.md`
5. **Generate ADRs** — prompt with `ai/prompts/adr-generator.md` → `architecture/adr/*.md`

After this step, `architecture/architecture-final.md` is your source of truth.

## Step 3b — Create a design system (optional — UI projects)

If your project includes human-facing UI, create a design system before
delivery planning:

- **New project:** prompt with `ai/prompts/design-system-generator.md` → `architecture/design-system.md`
- **Existing project with UI:** follow `ai/workflows/ui-retrofit-workflow.md` to inventory existing UI and derive a design system

If your project has no UI, skip this step.

## Step 4 — Create a delivery plan

Prompt with `ai/prompts/delivery-planner.md` → `architecture/delivery-plan.md`

The delivery plan organizes work into milestones and vertical slices. Each slice proves an end-to-end user workflow.

## Step 5 — Pick your first slice and write a feature spec

Select the first slice from the delivery plan, then prompt with `ai/prompts/feature-spec-generator.md` → `architecture/feature-specs/<slice-name>.md`

The feature spec makes one slice precise enough to decompose and implement.

## Step 6 — Decompose and execute with TDD

1. **Decompose** — use `.github/skills/plan-decomposer` to break the slice into independently verifiable Parts → `ai-parts/OVERVIEW.md` and `ai-parts/PXX-*.md`
2. **Execute** — use `.github/skills/part-executor-tdd` to implement one Part at a time with strict red-green-refactor TDD

Repeat steps 5–6 for each slice in the delivery plan.

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
Run `ai/prompts/architecture-compliance.md` before decomposition when the slice touches security, contracts, or cross-cutting concerns — or whenever you want confidence that the feature spec aligns with the approved architecture.

**When do I use specialist agents?**
During execution (Step 6). Ask your AI agent to adopt a specialist role: `ai/agents/backend-agent.md` for backend work, `ai/agents/frontend-agent.md` for UI, `ai/agents/qa-agent.md` for testing strategy, etc. See `ai/guides/toolkit-map.md` for the full list.

**What if I only have a prototype?**
Use Mode A. The toolkit is designed for exactly this — extracting validated architecture from working prototypes.

**What if my architecture doc is outdated?**
Use Mode B. The prototype-architecture alignment step will surface gaps and contradictions, and the gap reconciler will produce a corrected final architecture.

**What if I have an outdated architecture doc but no prototype?**
Use Mode C. The existing-architecture-reviewer prompt will identify gaps and risks, and the gap reconciler will produce a corrected final architecture.

## Next steps

- Read `ai/guides/toolkit-map.md` for a visual map of all toolkit components
- Read `ai/guides/operating-model.md` for the full phase-by-phase operating model
- Read `ai/examples/feature-spec-driven-slice-flow.md` for a worked example of the spec-to-implementation flow
