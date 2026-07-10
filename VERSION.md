# Version

AI Architecture Toolkit — **v4.4.0**

Projects adopting the toolkit should record this version in their project `CLAUDE.md`. Breaking workflow changes (renumbered steps, renamed output paths, changed handoff contracts) are marked **BREAKING** below, with the migration action a project must take.

## Changelog

### v4.4.0 — 2026-07-10

Code-quality release. Second audit pass against the reference project, focused on making cheaper models produce implementation code at the reference project's quality level. All changes are additive — no existing step was renumbered and no existing output path changed.

- New guide: `ai/guides/code-quality-standard.md` — the implementation-quality rules enforced per Part: read-before-write protocol, source precedence, dependency and abstraction discipline, boundaries/dependency direction, error handling with stable error identifiers, validation split, logging/observability, async + cancellation, test quality (no mock-only/fake tests, truth-table coverage for rule matrices, contract tests), prohibited outputs, the four contract surfaces, and the ambiguity rule (stop and list, don't invent a style).
- New template: `ai/templates/code-quality-checklist-template.md` — the **Part Quality Report** every Part execution must produce (`ai-parts/<slice-id>/reviews/<part-id>-quality-report.md`): files changed, tests + TDD evidence, checks run, architecture rules verified, patterns followed, contract surfaces declared changed/unchanged, dependencies, deviations, risks, explicit DONE/NOT DONE.
- New prompt + agent: `ai/prompts/code-quality-reviewer.md` and `ai/agents/code-reviewer-agent.md` — per-Part code review with ten required checks and a single verdict (`APPROVED` / `APPROVED WITH NOTES` / `REJECTED — MUST FIX`) → `ai-parts/<slice-id>/reviews/<part-id>-review.md`.
- **Engineering workflow: new mandatory Step 6a — Part Code Review** (between Step 6 and Step 6b; per Part, also applies to phases). A rejected Part returns to `IN_PROGRESS`; the next Part starts only after approval. Migration: none for completed Parts; apply from the next Part onward.
- `part-executor-tdd` v1.3.0: new required "Read before write" protocol step; non-negotiable rules extended (no new libraries without justification, no unneeded abstractions, no silent contract changes, stop on unclear patterns); quality gates extended (prohibited outputs, behavior-not-mocks tests, no weakened tests, contract-surface declaration); completion report replaced by the Part Quality Report; DONE now additionally gated on the Step 6a review verdict.
- `plan-decomposer` v2.3.0: preflight now builds a **Pattern Inventory** (concrete existing files per artifact kind, written to OVERVIEW.md); new optional PART_SPEC fields `existing_patterns` and `contracts_touched` (additive — existing Part files remain valid).
- Agents: backend, frontend, QA, and integration reviewer upgraded with read-before-write, pattern-adherence, error/validation/logging/cancellation checklist items, test-quality rules (QA), and hidden-contract-change detection across the four contract surfaces (integration reviewer).
- `ai/guides/definition-of-ready-and-done.md`: Quality completeness now requires code-quality-standard adherence, no prohibited outputs, behavior-proving tests, and declared contract surfaces; Review completeness now requires the Part Quality Report and an approving Step 6a review verdict per Part.
- `.github/instructions/csharp.instructions.md`: new "Async and Cancellation" and "Error Handling and Results" sections; structured-logging property/PII rules.
- Rule 17 added to `.github/copilot-instructions.md` (mirrored in root `CLAUDE.md`): code follows the code quality standard; every Part ends with a quality report and passes Step 6a review.
- Synced: `README.md`, root `CLAUDE.md`, `ai/CLAUDE.md` (step list now 0b–8 including 6a), `ai/guides/operating-model.md`, `ai/guides/quick-start.md`, `ai/guides/how-feature-specs-are-used.md`, `ai/guides/toolkit-map.md`.

### v4.3.0 — 2026-07-10

Consistency and cheap-model-reliability release. Audited against a real reference project.

- Root `CLAUDE.md` rewritten: distinguishes maintaining the toolkit (this repo) from using it (project repos); adds asset classes, agent behavior rules, synchronization map, and a Definition of Done for toolkit changes.
- `README.md` synced with actual structure; documents the instruction-file hierarchy (root `CLAUDE.md`, `.github/copilot-instructions.md`, `ai/CLAUDE.md`), the MCP server, and all guides/examples/agents.
- `.github/copilot-instructions.md`: rule 2 now lists all four entry-mode workflows (was Mode C only); added rule 16 (verification before claiming done).
- **BREAKING** — Decomposition output moved from flat `ai-parts/` to per-slice `ai-parts/<slice-id>/` (flat layout collides once a second slice starts). Migration: leave completed slices where they are; use per-slice folders from the next decomposition onward.
- **BREAKING** — Engineering workflow Step 4 (architecture compliance check) is now mandatory per slice (was conditional on vague "sensitive/cross-cutting/high-risk" criteria). A documented **lightweight mode** applies when all six binary trigger questions in Step 4 answer "no" (boundaries, verticality, and touched contracts only). Migration: none for completed slices.
- Engineering workflow: defined output locations for the UI compliance report (`architecture/compliance-reports/<slice-id>-<slice-name>-ui.md`) and Integrated Slice Verification evidence (`architecture/slice-verification/<slice-id>-<slice-name>.md`); added Step 3b (golden dataset, conditional); added missing-input handling and phase-execution rules; fixed skill paths (`.github/skills/...`).
- `part-executor-tdd`: input contract now matches what `plan-decomposer` actually writes (`# Part PNN — <title>`, one to three `#` accepted); fixed a malformed quality-gates list.
- `plan-decomposer`: per-slice output directory, slice-ID naming convention, inserted-Part convention (e.g., `P09b`), stack-specific checklist labeled as .NET default.
- Architecture workflows: `architecture-workflow.md` rewritten as the mode selector + finalization gate; Modes A/B/D now write analysis outputs to concrete file paths (`architecture/prototype-analysis.md`, `architecture/legacy-system-analysis.md`); all four modes end at the same gate and hand off identically.
- Prompts: `prototype-analyzer` and `legacy-system-analyzer` gained output paths; `architecture-designer` is mode-aware (prototype or legacy analysis input); `architecture-gap-reconciler` no longer claims "there is no prototype in this mode" (Mode B aware); `architecture-compliance` and `ui-compliance-check` gained output paths; feature-spec reconcilers write back to the spec file; `adr-generator` naming aligned to `ADR-001-<topic>.md`.
- New: `ai/prompts/slice-preparation-runner.md` — runs engineering workflow Steps 2–5 for one slice in a single agent session, stopping before execution (codifies the start-prompt pattern proven in real projects).
- New: `ai/templates/project-claude-template.md` — the `CLAUDE.md` to place in project repos.
- Guides: `quick-start.md` includes Mode D; `toolkit-map.md` and `operating-model.md` synced (legacy analyzer, Mode D, remediation workflow, missing templates, correct skill paths); `ui-foundation-workflow.md` no longer calls the UI compliance check optional.
- Removed stale, already-executed plan documents: `.github/prompts/plan-verticalSliceDiscipline.prompt.md`, `.github/prompts/plan-glossaryDefinitions.prompt.md`, `architecture/update-for-v-slices.md` (git history preserves them).
- Scaffold `architecture/adr/` is now empty by design: the pre-filled sample ADRs moved to `ai/examples/example-adr-modular-monolith.md` and `ai/examples/example-adr-prototype-interpretation.md` (they were at risk of being treated as authoritative project decisions under working rule 1).
- `ai/guides/conversation-summary.md` moved to `docs/design-history.md` — it is toolkit design rationale, not an operational guide, and is not copied into projects.
- Parts folder casing: `ai-parts/<slice-id>/` matches the delivery plan's slice ID casing exactly (e.g., `ai-parts/S2.6/`).
- Removed the root `ai-architecture-toolkit.sln` (duplicated `mcp-server/AiArchitectureToolkit.McpServer.slnx` and violated the `.slnx` convention).
- The MCP server still serves the pre-4.3.0 layout; `mcp-server/TOOLKIT-ALIGNMENT-PROMPT.md` is the pending one-time prompt to align it (delete after execution).

### v4.2.0 and earlier

Pre-changelog versions (unified toolkit; UI design system tracks; legacy system replacement mode; MCP server; UI remediation workflow). See git history.
