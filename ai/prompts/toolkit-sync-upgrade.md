# Prompt: Toolkit Sync / Upgrade Runner

Act as a **toolkit maintainer upgrading a project repository's embedded copy
of the AI Architecture Toolkit** to a newer toolkit version. You run inside
the **project repository** (not the toolkit source repo). You upgrade the
whole toolkit — every reusable asset class, not just the skills — while
never touching the project's generated outputs or application code.

## Inputs

- `TOOLKIT_SOURCE` — the toolkit source: a local checkout of
  `ai-architecture-toolkit` (preferred) or its repository URL. Ask for it if
  not provided.
- The project repository (current working directory).
- The project's recorded toolkit version — from the project root `CLAUDE.md`
  ("uses the AI Architecture Toolkit (version …)").
- `VERSION.md` in `TOOLKIT_SOURCE` — the target version and the changelog
  entries between the recorded and target versions.

## What gets replaced vs. what is untouchable

**Replace from `TOOLKIT_SOURCE` (reusable toolkit assets):**

- `ai/agents/`, `ai/guides/`, `ai/prompts/`, `ai/templates/`,
  `ai/workflows/`, `ai/examples/`, `ai/CLAUDE.md`
- `.github/skills/`, `.github/instructions/`, `.github/agents/`,
  `.github/prompts/`, `.github/copilot-instructions.md`
- If the project embeds only a subset (minimal deployment), upgrade the
  subset it embeds **plus** any new files the target version's workflows now
  require (check the changelog for "New:" entries) — a workflow step that
  references a missing file is a broken upgrade.

**Never overwrite (per-project content):**

- `ai/project-context.md`
- everything under `architecture/` and `ai-parts/`
- the project root `CLAUDE.md` (merge — see below), `src/`, `tests/`, infra
  files, `VERSION.md` if the project keeps its own

## Procedure

### 1. Determine versions and read the changelog

Read the recorded version from the project `CLAUDE.md` and the target version
from `TOOLKIT_SOURCE/VERSION.md`. List every changelog entry between them,
and extract:

- all **BREAKING** items with their migration actions
- all new files/steps/rules the project must adopt
- if the recorded version is missing, say so and treat all listed changes as
  potentially applicable — do not guess a version.

### 2. Detect local modifications before overwriting

For every toolkit file about to be replaced, compare the project's copy
against the same file in `TOOLKIT_SOURCE` at the **recorded** version (use
git history of the source repo when available; otherwise diff against the
current source and judge from content). If the project's copy was locally
modified (deliberate project customization, e.g. adapted agent personas or
stack-specific instruction files):

- **stop for that file** — list it with a diff summary and ask whether to
  keep the local version, take the new toolkit version, or merge.
- Never silently discard a local customization.

### 3. Copy the assets

Replace the asset directories/files listed above from `TOOLKIT_SOURCE`.
Delete embedded toolkit files that the target version removed (the changelog
records removals) — but only files that belong to the toolkit asset classes,
never project outputs.

### 4. Merge the project root `CLAUDE.md`

Do not overwrite it. Update in place:

- the recorded toolkit version → the target version
- the working-rules summary, step lists, and directory tree, so they match
  the target version's `ai/templates/project-claude-template.md` — keeping
  all project-specific sections (current state, project specifics, run/test
  commands) intact.

### 5. Apply migration actions

For each **BREAKING** changelog item, apply its stated migration action.
Default stance: completed slices/Parts stay as they are; new conventions
apply from the next slice/Part onward. Record every migration action taken
(or explicitly not needed) in the report.

### 6. Verify

- Every relative path referenced by the copied workflows, skills, prompts,
  and the merged `CLAUDE.md` resolves to an existing file in the project repo
  (grep/glob pass — no dead links).
- The engineering-workflow step list in the project `CLAUDE.md` matches the
  copied `ai/workflows/engineering-workflow.md`.
- No file under `architecture/`, `ai-parts/`, or `ai/project-context.md` was
  modified (verify via `git status`).
- The build/tests were not touched — this upgrade changes process files only.

## Output — upgrade report

Report in the response (and, if the project keeps one, append to its upgrade
log):

1. **Version:** <from> → <to>
2. **Files replaced / added / removed** (grouped by asset class)
3. **Local customizations found** and how each was resolved (kept / replaced
   / merged / awaiting decision)
4. **Breaking changes applied** — each with its migration action and scope
   ("from next slice onward")
5. **Project `CLAUDE.md` changes** made by the merge
6. **Verification results** — pass/fail per check in step 6
7. **Open questions** — anything requiring a human decision before the next
   slice starts

Do not claim the upgrade complete while any step-2 conflict or step-6 check
is unresolved.
