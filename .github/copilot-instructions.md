# Copilot Instructions

These are the canonical working rules for repositories using the AI Architecture Toolkit. They apply to all AI agents (Copilot, Claude, Cursor, and others). Root `CLAUDE.md` mirrors these rules — keep the two files in sync.

When working in this repository:

1. Treat `architecture/architecture-final.md` and `architecture/adr/*.md` as authoritative once they exist as real project outputs (scaffold placeholders do not count).
2. For architecture work, follow the workflow for your entry mode (`ai/workflows/architecture-workflow.md` is the mode selector):
   - `ai/workflows/architecture-workflow-prototype-only.md` — Mode A: prototype only
   - `ai/workflows/architecture-workflow-prototype-plus-architecture-doc.md` — Mode B: prototype + architecture doc
   - `ai/workflows/architecture-workflow-architecture-doc-only.md` — Mode C: architecture doc only
   - `ai/workflows/architecture-workflow-legacy-system-replacement.md` — Mode D: legacy system replacement
3. For implementation work, follow:
   - `ai/workflows/engineering-workflow.md` (its step numbers are canonical — cite them exactly)
4. Use `ai/project-context.md` as additional context.
5. Prefer vertical slices. See `ai/guides/vertical-slice-definition.md` for the definition and verticality test.
6. Prefer modular monolith unless another pattern is explicitly justified. See `ai/guides/modular-monolith-definition.md`.
7. Do not introduce new architecture without review. Surface the need as a compliance finding or open question instead of deciding it yourself.
8. Respect TDD and the decomposition/execution skills (`plan-decomposer`, `part-executor-tdd`). Implementation begins only after the selected slice has a feature spec and an `ai-parts/<slice-id>/` handoff (OVERVIEW.md plus Part files).
9. If a feature spec exists for the selected slice, treat it as the primary input for decomposition and implementation.
10. See `ai/guides/glossary.md` for definitions of key terms used throughout the toolkit.
11. Do not make assumptions about the project context beyond what is stated in `ai/project-context.md`. If you need to make assumptions, explicitly state them in your response. But prefer asking for clarification or additional context if something is not clear rather than making assumptions. For every question you ask, provide me with advice.
12. Treat `architecture/design-system.md` as authoritative for UI decisions when it exists.
13. For UI-inclusive projects, follow:
    - `ai/workflows/ui-foundation-workflow.md` — for greenfield projects creating a design system from scratch
    - `ai/workflows/ui-retrofit-workflow.md` — for existing projects adding a design system after implementation
14. For slices with human workflow surfaces, the UI compliance check (engineering workflow Step 4a), Integrated Slice Verification (Step 6b), and the Frontend Agent are **mandatory** — not optional.
15. For projects with UI slices completed under an older toolkit version, use `ai/workflows/ui-remediation-workflow.md` to revalidate before resuming.
16. Never claim a step is done without running the verification that the workflow defines for it (verify commands, checklists, browser verification for UI slices), and report pass/fail per criterion.
