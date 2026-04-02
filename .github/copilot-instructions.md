# Copilot Instructions

When working in this repository:

1. Treat `architecture/architecture-final.md` and `architecture/adr/*.md` as authoritative once they exist.
2. For architecture work, follow:
   - `ai/workflows/architecture-workflow-architecture-doc-only.md`
3. For implementation work, follow:
   - `ai/workflows/engineering-workflow.md`
4. Use `ai/project-context.md` as additional context.
5. Prefer vertical slices.
6. Prefer modular monolith unless another pattern is explicitly justified.
7. Do not introduce new architecture without review.
8. Respect TDD and decomposition / execution skills.
9. If a feature spec exists for the selected slice, treat it as a primary input for decomposition and implementation.
10. See `ai/guides/glossary.md` for definitions of key terms used throughout the toolkit.
11. Do not make assumptions about the project context beyond what is stated in `ai/project-context.md`. If you need to make assumptions, explicitly state them in your response. But prefer asking for clarification or additional context if something is not clear rather than making assumptions. For every question you ask, provide me with advice.
12. Treat `architecture/design-system.md` as authoritative for UI decisions when it exists.
13. For UI-inclusive projects, follow:
    - `ai/workflows/ui-foundation-workflow.md` — for greenfield projects creating a design system from scratch
    - `ai/workflows/ui-retrofit-workflow.md` — for existing projects adding a design system after implementation