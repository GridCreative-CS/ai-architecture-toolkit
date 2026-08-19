# MCP Server — Toolkit v4.3.0+ Alignment Prompt

<!-- STATUS: NOT YET EXECUTED — this is tracked, outstanding work, listed as  -->
<!-- pending in VERSION.md. Do NOT delete it as a stale plan document: the    -->
<!-- MCP server still serves the pre-4.3.0 layout. Paste it into your AI      -->
<!-- agent when you are ready to do the alignment, and delete it only once    -->
<!-- the Definition of done below is met (per CLAUDE.md: no *executed* plan   -->
<!-- documents in the repo).                                                  -->

Act as a **Senior .NET Backend Engineer** working on
`mcp-server/AiArchitectureToolkit.McpServer`.

## Objective

Align the MCP server with the current toolkit version (see `VERSION.md`;
items 1–8 below cover v4.3.0, item 9 covers v4.5.0, item 10 covers v4.6.0).
The toolkit changed several output paths and added new assets; the server
still serves the pre-4.3.0 layout. Follow strict TDD (red-green-refactor) per
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

10. **Requirement traceability and Part review surfaces (toolkit v4.6.0).**
   v4.6.0 made per-Part review output and criterion traceability part of the
   workflow; the server has no concept of either. Item 1 is a prerequisite for
   10a and 10c.

   a. **Part review artifacts.** Extend the `ai-parts/<slice-id>/` support from
      item 1 to the `reviews/` subfolder: list and read
      `reviews/<part-id>-quality-report.md` (Part Quality Report, Step 6) and
      `reviews/<part-id>-review.md` (Part Code Review, Step 6a). Surface each
      review's verdict (`APPROVED` / `APPROVED WITH NOTES` /
      `REJECTED — MUST FIX`) alongside the Part's `Status:` line, so a client
      can tell whether the next Part is allowed to start. A Part at `DONE`
      with no review file, or with a `REJECTED` review, is the case that
      matters — make sure a test covers it.

   b. **`part-code-review` workflow step (Step 6a).** Add to
      `GetWorkflowContext`, bundling `ai/prompts/code-quality-reviewer.md`
      (prompt slot), `ai/templates/code-quality-checklist-template.md`
      (template slot), `ai/guides/code-quality-standard.md` (guide slot), and
      the slice's feature spec plus architecture as project artifacts. Add it
      to the `[Description]` step list and the unknown-step error message
      (same two places as item 5).

   c. **Requirement Coverage Map and criterion IDs.** v4.6.0 requires a
      `## Requirement Coverage Map` section in `ai-parts/<slice-id>/OVERVIEW.md`
      and stable criterion IDs (`DR-nn`, `SEC-nn`, `AC-nn`, `UIAC-nn`) in the
      feature spec. Surface both:
      - report whether OVERVIEW.md has a Requirement Coverage Map; its absence
        means a pre-v4.6 decomposition whose map the executor must derive
      - have `get_slice_context` extract the criterion IDs present in the
        feature spec, so a client can cross-check them against the map
      - surface the PART_SPEC fields `part_type` and `criteria_covered`. Both
        sit under OPTIONAL in the plan-decomposer schema, so never fail on a
        Part file that omits them — but the skill's Required behavior section
        obliges a v2.4.0 decomposition to emit both, so report an absent
        `criteria_covered` as an unowned-criteria risk rather than as normal.
        `part_type` has a defined fallback (classify from `file_touch_points`);
        `criteria_covered` has none

   d. **New v4.6.0 assets are discoverable.** Same directory-driven check as
      item 7 — add tests proving `list_toolkit_content` and `toolkit://`
      resources return `ai/prompts/code-quality-reviewer.md`,
      `ai/templates/code-quality-checklist-template.md`,
      `ai/guides/code-quality-standard.md`,
      `ai/examples/example-part-quality-report.md`,
      `ai/examples/example-part-review.md`, and
      `ai/examples/example-architecture-final-gate-report.md`.

   e. **Two pre-existing gaps this release exposes — decide explicitly.**
      Neither is caused by v4.6.0, but both now hide load-bearing assets. Pick
      a resolution and record it in `mcp-server/README.md`:
      - `ToolkitContentService.ListMarkdownFiles` matches `*.md` only, so
        `ai/examples/example-golden-dataset-case.json` is invisible — which
        undercuts the `golden-dataset` step added in item 5. Either widen the
        examples category to `.json` or document the exclusion.
      - `.github/skills/` is not served at any category, yet
        `plan-decomposer` (v2.4.0) and `part-executor-tdd` (v1.4.0) define the
        Part handoff contract the `decomposition` step describes. Either add a
        `skills` category or document why the skills stay unserved.

## Definition of done

- All new behavior has tests written first (red observed, then green).
- Coverage-map absence, missing/rejected Part reviews, and Part files without
  the optional v4.6.0 PART_SPEC fields are each covered by a test.
- `dotnet build` and `dotnet test` pass for the full solution.
- `mcp-server/README.md` matches the implemented surface.
- No references remain to deleted toolkit files.
- Delete this prompt file in the same PR.
