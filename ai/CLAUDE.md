# AI Toolkit — Claude Context

This directory contains the core AI-assisted architecture and engineering toolkit (reusable assets).

## Before modifying any file here

Read the root `CLAUDE.md` — in particular the **Toolkit maintenance rules** (synchronization map and Definition of Done for toolkit changes). All changes to toolkit files must keep cross-references, step numbers, section numbers (§), and file paths consistent across the toolkit.

## Directory layout

- `agents/` — Specialist agent personas. Each defines a role, responsibilities, and constraints. Read the relevant agent file before adopting that persona.
- `guides/` — Reference documents for key concepts. The glossary (`guides/glossary.md`) is the single source of truth for term definitions. Other guides cover vertical slices, modular monolith, contracts, definition of ready/done, entry-mode selection, the operating model, and the quick start.
- `prompts/` — Prompts that drive specific workflow steps (architecture design, delivery planning, feature spec generation, compliance checking, reconciliation, UI, golden datasets, legacy analysis, slice preparation). Each prompt specifies its inputs and its output file path.
- `templates/` — Templates for generated artifacts (feature specs, ADRs, compliance reports, golden datasets, project context, project CLAUDE.md, design system, UI inventory, retrofit/remediation specs, slice verification checklist). Prompts reference these templates.
- `workflows/` — End-to-end workflow definitions:
  - `workflows/architecture-workflow.md` — entry-mode selector and finalization gate (includes the architecture-final quality gate, `prompts/architecture-final-quality-gate.md`); variants for Modes A–D
  - `workflows/engineering-workflow.md` — implementation loop (canonical step numbering: 0b, 1, 1b, 2, 3, 3b, 4, 4a, 4b, 5, 6, 6a, 6b, 7, 8)
  - `workflows/ui-foundation-workflow.md`, `workflows/ui-retrofit-workflow.md`, `workflows/ui-remediation-workflow.md` — UI tracks
- `examples/` — Concrete good/bad pattern examples (vertical vs horizontal slices, modular monolith patterns, contract patterns, feature-spec-driven flows, explainability patterns) and worked artifact examples (architecture-final gate report, Part Quality Report, Part code review).
- `project-context.md` — Project-specific context. A stub in the toolkit source repo; filled per project using `templates/project-context-template.md`.

## Cross-references

Many files in this directory reference each other. When editing a file, check whether it is referenced by or references other files. Key cross-reference chains:

- `workflows/engineering-workflow.md` references prompts, templates, and `.github/skills/` — its step numbers are cited by root `CLAUDE.md`, `README.md`, `.github/copilot-instructions.md`, `guides/operating-model.md`, `guides/quick-start.md`, and both skills
- `templates/feature-spec-template.md` section numbers (§5b, §11b, §12b) are cited by `prompts/feature-spec-generator.md`, `prompts/ui-compliance-check.md`, both skills, `templates/slice-verification-checklist-template.md`, and `guides/definition-of-ready-and-done.md`
- **criterion ID prefixes** (`DR-nn` §6, `SEC-nn` §9, `AC-nn` §11, `UIAC-nn` §11b) defined in `templates/feature-spec-template.md` are emitted by `prompts/feature-spec-generator.md`, preserved (never renumbered) by both reconcilers, cited by `prompts/ui-compliance-check.md`, mapped to Parts by `plan-decomposer`, keyed into the §3b matrix by `part-executor-tdd`, audited by `prompts/code-quality-reviewer.md` check 12, and rolled up by `templates/slice-verification-checklist-template.md` — changing a prefix requires updating all of them
- `templates/code-quality-checklist-template.md` section numbers are a handoff contract: §7 (contract surfaces) is cited by `prompts/code-quality-reviewer.md` check 9, both skills, and `guides/code-quality-standard.md` §12; §3b (requirement coverage matrix) and §10b (remediation log) are cited by the reviewer prompt, `part-executor-tdd`, `workflows/engineering-workflow.md` Steps 6/6a/6b, and `guides/definition-of-ready-and-done.md` — never renumber §§1–12
- `prompts/code-quality-reviewer.md` defines **twelve** required checks (11 = dimension audit D1–D9, 12 = requirement coverage audit); the count and the dimension IDs are cited by `agents/code-reviewer-agent.md`, `workflows/engineering-workflow.md` Step 6a, and the frontend/backend agents — changing either requires updating all of them
- `prompts/delivery-planner.md` references `guides/vertical-slice-definition.md` and `guides/glossary.md`
- `guides/glossary.md` is referenced by most prompts, agents, and skills — define terms there, not inline
- `.github/skills/plan-decomposer/SKILL.md` and `.github/skills/part-executor-tdd/SKILL.md` share the Part Handoff Contract — changing one requires checking the other
- `guides/code-quality-standard.md` section numbers (§1–§13) are cited by both skills, `prompts/code-quality-reviewer.md`, `templates/code-quality-checklist-template.md`, `agents/code-reviewer-agent.md`, the backend/frontend/qa/integration-reviewer agents, and `guides/definition-of-ready-and-done.md` — renumbering its sections requires updating all of them
- `templates/architecture-blueprint-template.md` (structure + Writing rules) binds `prompts/architecture-designer.md`, both reconcilers, and `prompts/architecture-final-quality-gate.md`; the gate's checks are cited by `prompts/architecture-reviewer.md`, `prompts/existing-architecture-reviewer.md`, both reconcilers, `prompts/adr-generator.md` (precondition), the four mode workflows, and `workflows/architecture-workflow.md` — changing the gate's checks or verdict vocabulary requires updating all of them
