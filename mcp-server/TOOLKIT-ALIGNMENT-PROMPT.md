# MCP Server — Toolkit v4.3.0+ Alignment Prompt

<!-- One-time implementation prompt. Paste into your AI agent when you are    -->
<!-- ready to align the MCP server with the current toolkit version. Delete   -->
<!-- this file after the work is done (per CLAUDE.md: no executed plan        -->
<!-- documents in the repo).                                                   -->

Act as a **Senior .NET Backend Engineer** working on
`mcp-server/AiArchitectureToolkit.McpServer`.

## Objective

Align the MCP server with the current toolkit version (see `VERSION.md`;
items 1–8 below cover v4.3.0, item 9 covers v4.5.0). The toolkit changed
several output paths and added new assets; the server still serves the
pre-4.3.0 layout. Follow strict TDD (red-green-refactor) per
`.github/skills/part-executor-tdd/SKILL.md` quality gates: tests first for
every behavioral change, run `dotnet build` and `dotnet test` on
`mcp-server/AiArchitectureToolkit.McpServer.slnx`, and follow
`.github/instructions/csharp.instructions.md`.

## Required changes

Work through these in order. For each, write the failing test first.

1. **Per-slice `ai-parts/<slice-id>/` awareness.** The server has no concept
   of decomposition output at all. Add to `ProjectContentService` (and expose
   via `list_project_artifacts` in `ProjectTools` and a
   `project://ai-parts/{slice-id}` resource in `ProjectResources`):
   - list slice folders under `ai-parts/`
   - read `ai-parts/<slice-id>/OVERVIEW.md` and Part files
   - surface each Part's `Status:` line (TODO | IN_PROGRESS | DONE | BLOCKED)

2. **Legacy system analysis (Mode D).** Add a
   `project://legacy-system-analysis` resource reading
   `architecture/legacy-system-analysis.md`, and a `legacy-system-analysis`
   step in `GetWorkflowContext` (`ProjectTools.cs`) that bundles
   `ai/prompts/legacy-system-analyzer.md` with the project context. Mirror
   how `prototype-analysis` is handled.

3. **Slice verification evidence.** Add
   `architecture/slice-verification/` to `ProjectContentService` artifact
   listing and a `project://slice-verification/{name}` resource. The existing
   `slice-verification` workflow step should also report whether evidence
   exists for the requested slice.

4. **UI compliance report naming.** Compliance reports now come in pairs:
   `<slice-id>-<slice-name>.md` and `<slice-id>-<slice-name>-ui.md`. Make
   `GetComplianceReport`/listing distinguish the two, and include both in
   `get_slice_context`.

5. **New workflow steps in `GetWorkflowContext`.** Add:
   - `golden-dataset` (engineering workflow Step 3b) — bundles
     `ai/prompts/golden-dataset-generator.md` +
     `ai/templates/golden-dataset-template.md`
   - `slice-preparation` — bundles `ai/prompts/slice-preparation-runner.md`
   Update the step list in the `[Description]` attribute and the error
   message listing valid steps.

6. **Moved/removed toolkit files.** `ai/guides/conversation-summary.md` moved
   to `docs/design-history.md` (outside the served `ai/` tree — decide:
   either stop serving it or add a `docs` category). The
   `.github/prompts/plan-*.prompt.md` files and
   `architecture/update-for-v-slices.md` were deleted. Verify
   `ToolkitContentService` category listing and any tests do not reference
   them.

7. **New toolkit assets are discoverable.** Confirm `list_toolkit_content`
   and `toolkit://` resources pick up `ai/prompts/slice-preparation-runner.md`
   and `ai/templates/project-claude-template.md` (they should, if listing is
   directory-driven — add a test proving it).

8. **README sync.** Update `mcp-server/README.md` resource/tool tables for
   everything above.

9. **Architecture-final quality gate (toolkit v4.5.0).** Add an MCP prompt in
   `ToolkitPrompts.cs` that bundles
   `ai/prompts/architecture-final-quality-gate.md` with the project's
   `architecture/architecture-final.md` and `ai/project-context.md` (mirror
   `architecture_reconciler`); add `architecture/architecture-final-gate.md`
   to `ProjectContentService` artifact listing and a
   `project://architecture-final-gate` resource; insert the gate between the
   reconcile and ADR steps in the workflow-step sequences hardcoded in
   `ProjectTools.cs` (`architecture-reconciler` → gate → `adr-generator`).

## Definition of done

- All new behavior has tests written first (red observed, then green).
- `dotnet build` and `dotnet test` pass for the full solution.
- `mcp-server/README.md` matches the implemented surface.
- No references remain to deleted toolkit files.
- Delete this prompt file in the same PR.
