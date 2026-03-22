# AI Toolkit — Claude Context

This directory contains the core AI-assisted architecture and engineering toolkit.

## Before modifying any file here

Read the root `CLAUDE.md` for working rules. All changes to toolkit files must be consistent with the existing structure and cross-references.

## Directory layout

- `agents/` — Specialist agent personas. Each defines a role, responsibilities, and constraints. Read the relevant agent file before adopting that persona.
- `guides/` — Reference documents for key concepts. The glossary (`guides/glossary.md`) is the single source of truth for term definitions. Other guides cover vertical slices, modular monolith, contracts, definition of ready/done, and the operating model.
- `prompts/` — Prompts that drive specific workflow steps (architecture design, delivery planning, feature spec generation, compliance checking, reconciliation). Each prompt specifies its inputs and outputs.
- `templates/` — Templates for generated artifacts (feature specs, ADRs, compliance reports, golden datasets, project context). Prompts reference these templates.
- `workflows/` — End-to-end workflow definitions. The two primary workflows are:
  - `workflows/architecture-workflow.md` (and variants) — for architecture design
  - `workflows/engineering-workflow.md` — for implementation (delivery plan, feature specs, decomposition, TDD execution)
- `examples/` — Concrete good/bad pattern examples (vertical vs horizontal slices, modular monolith patterns, contract patterns, feature-spec-driven flows).
- `project-context.md` — Project-specific context. Fill using `templates/project-context-template.md` before starting project work.

## Cross-references

Many files in this directory reference each other. When editing a file, check whether it is referenced by or references other files. Key cross-reference chains:

- `workflows/engineering-workflow.md` references prompts, templates, and skills
- `prompts/delivery-planner.md` references `guides/vertical-slice-definition.md` and `guides/glossary.md`
- `prompts/feature-spec-generator.md` references `templates/feature-spec-template.md` and `guides/glossary.md`
- `guides/glossary.md` is referenced by most prompts, agents, and skills
- `.github/skills/plan-decomposer/SKILL.md` and `.github/skills/part-executor-tdd/SKILL.md` reference guides and feature spec locations
