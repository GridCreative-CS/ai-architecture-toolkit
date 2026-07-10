# CLAUDE.md — <Project Name>

<!-- TEMPLATE: copy this file to the ROOT of your project repository as       -->
<!-- CLAUDE.md when adopting the AI Architecture Toolkit. Replace every       -->
<!-- <angle-bracket> placeholder. Delete these template comments.             -->
<!-- Source: ai/templates/project-claude-template.md (AI Architecture Toolkit)-->

## Project overview

<2–4 sentences: what this application is, who uses it, and its current phase
(architecture / implementation / remediation).>

This repository uses the **AI Architecture Toolkit** (version <toolkit
version from VERSION.md>) — reusable prompts, agents, workflows, templates,
guides, and skills that drive architecture design, delivery planning, feature
specification, decomposition, and TDD-based implementation.

## Where things are

```text
ai/                        # Toolkit assets (read-only process definitions) + project-context.md
architecture/              # Generated project outputs — the source of truth
  architecture-final.md    # Authoritative architecture
  architecture-final-gate.md # Quality gate verdict for architecture-final.md
  adr/                     # Architecture Decision Records (authoritative)
  design-system.md         # Authoritative for UI decisions (when present)
  delivery-plan.md         # Milestones, phases, and vertical slices
  feature-specs/           # One spec per slice: <slice-id>-<slice-name>.md
  compliance-reports/      # Per slice: <slice-id>-<slice-name>.md and -ui.md
  slice-verification/      # Integrated Slice Verification evidence per slice
  golden-datasets/         # Validation datasets
ai-parts/                  # Decomposition output, one folder per slice: <slice-id>/
                           #   <slice-id>/reviews/ — Part Quality Reports + Step 6a review verdicts
.github/
  copilot-instructions.md  # Canonical numbered working rules (all agents)
  skills/                  # plan-decomposer, part-executor-tdd
  instructions/            # File-type coding conventions
src/ · tests/              # Application code and tests
```

## Working rules

Follow the numbered rules in `.github/copilot-instructions.md`. The
load-bearing ones:

- `architecture/architecture-final.md` and `architecture/adr/*.md` are
  **authoritative**. Do not introduce new architecture without review —
  surface the need as a compliance finding or open question.
- For implementation, follow `ai/workflows/engineering-workflow.md`. Its step
  numbers are canonical. Implementation starts only when the selected slice
  has a feature spec and an `ai-parts/<slice-id>/` handoff.
- Strict TDD via the `plan-decomposer` and `part-executor-tdd` skills; execute
  one Part at a time. All implementation code follows
  `ai/guides/code-quality-standard.md`: read nearby code and tests before
  writing, follow existing project patterns, no new libraries or speculative
  abstractions without justification, no silent contract changes.
- Every Part ends with a Part Quality Report and the Part code review
  (engineering workflow Step 6a). **The review must run in a fresh agent
  session/subagent** — never in the session that executed the Part. The next
  Part starts only after an `APPROVED` or `APPROVED WITH NOTES` verdict.
- For slices with human workflow surfaces: UI compliance check (Step 4a),
  Integrated Slice Verification (Step 6b), and the Frontend Agent are
  **mandatory**. `architecture/design-system.md` is authoritative for UI.
- Do not assume project context beyond `ai/project-context.md`; prefer asking
  over assuming, and provide advice with every question.
- Never claim a step done without running its defined verification and
  reporting pass/fail per criterion.
- Do not modify toolkit assets under `ai/` (except `ai/project-context.md`)
  or `.github/skills/` to make a task easier — propose toolkit changes
  separately.

## Current state

<!-- Keep this section updated as the project progresses — it is the first   -->
<!-- thing an agent reads to orient itself.                                   -->

- **Entry mode used:** <A | B | C | D>
- **Architecture:** <finalized YYYY-MM-DD | in progress at step …>
- **Current slice:** <slice-id + name | "none selected">
- **Completed slices:** <list, or reference to delivery plan status>
- **Known deviations from the toolkit process:** <none | list them>

## Project specifics

<!-- Anything an agent must know that the toolkit cannot know: primary        -->
<!-- architecture document if not architecture-final.md, naming scheme        -->
<!-- overrides, how to run the app and tests, domain-critical constraints.    -->

- **Run the application:** `<command, e.g. docker compose up>`
- **Run all tests:** `<command, e.g. dotnet test MySolution.slnx>`
- **Run E2E browser tests:** `<command, e.g. npx playwright test>`
- <other project-specific facts>

## Code conventions

Before writing any code, read `.github/instructions/` in full
(<list the instruction files present, e.g. csharp, dockerfile,
docker-compose>). <Add or reference project-specific conventions here — do
not restate what the instruction files already say.>
