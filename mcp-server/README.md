# AI Architecture Toolkit — MCP Server

A [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) server that exposes the AI Architecture Toolkit as tools, resources, and prompts for LLM-powered development environments.

## What It Does

This MCP server makes the toolkit **project-aware** — it serves both static toolkit content (guides, prompts, templates, workflows, skills) and dynamically discovered project artifacts (architecture docs, the architecture-final gate report, ADRs, feature specs, delivery plans, compliance reports, decomposition output under `ai-parts/`, slice verification evidence, workspace structure).

### MCP Resources

| URI Pattern | Description |
|---|---|
| `toolkit://guides/{name}` | Toolkit guides (glossary, contract-definition, code-quality-standard, etc.) |
| `toolkit://prompts/{name}` | Toolkit prompts (delivery-planner, feature-spec-generator, code-quality-reviewer, etc.) |
| `toolkit://templates/{name}` | Templates (feature-spec, ADR, compliance-report, code-quality-checklist, etc.) |
| `toolkit://workflows/{name}` | Workflows (engineering, architecture, UI) |
| `toolkit://agents/{name}` | Agent personas (backend, frontend, QA, etc.) |
| `toolkit://examples/{name}` | Example patterns and worked examples |
| `toolkit://instructions/{name}` | Coding instructions (C#, Docker, etc.) |
| `toolkit://skills/{name}` | Execution skills from `.github/skills/<name>/SKILL.md` (`plan-decomposer`, `part-executor-tdd`) — these define the Part handoff contract |
| `project://architecture` | `architecture-final.md` from the workspace |
| `project://architecture-blueprint` | Architecture blueprint (pre-review draft) |
| `project://architecture-final-gate` | Architecture-final quality gate report — `architecture-final.md` is authoritative only once this records `APPROVED` or `APPROVED WITH NOTES` |
| `project://review-report` | Architecture review report |
| `project://existing-architecture-review` | Existing architecture review |
| `project://prototype-analysis` | Prototype analysis output |
| `project://prototype-architecture-alignment` | Prototype-architecture alignment analysis |
| `project://legacy-system-analysis` | Legacy system analysis (architecture Mode D) |
| `project://adr/{name}` | Specific ADR from the workspace |
| `project://delivery-plan` | Delivery plan from the workspace |
| `project://feature-spec/{name}` | Feature spec for a slice |
| `project://ai-parts/{sliceId}` | Decomposition output for a slice: `OVERVIEW.md`, each Part's `Status:` line, its Part Quality Report and Part Code Review verdict, each Part file, and warnings blocking the next Part |
| `project://slice-verification/{name}` | Integrated Slice Verification evidence (engineering workflow Step 6b) |
| `project://project-context` | Project context |
| `project://design-system` | Design system document |
| `project://remediation-audit` | UI remediation audit results |

### MCP Tools

| Tool | Description |
|---|---|
| `list_toolkit_content` | Lists all toolkit files by category with file counts |
| `search_toolkit` | Full-text search across all toolkit markdown and JSON files, including the execution skills |
| `get_glossary_term` | Looks up a term from the glossary |
| `get_toolkit_file` | Gets full content of a specific toolkit file by category and name |
| `list_project_artifacts` | Lists project artifacts with existence status, including `ai-parts` slice folders, `slice-verification` evidence, and architecture vs UI compliance reports separately |
| `get_workspace_structure` | Scans workspace for .NET solutions, projects, and dependencies |
| `get_workflow_context` | **Key tool** — returns everything needed for a workflow step (prompt + template + guide + skill + project artifacts). Takes an optional `sliceName` for the slice-scoped steps |
| `get_slice_context` | Returns all context for a specific slice: feature spec and its criterion IDs, both compliance reports, decomposition status from `ai-parts/`, verification evidence, ADRs, delivery plan |
| `check_verticality` | Provides the verticality test criteria for evaluating a slice |

#### `get_workflow_context` steps

| Step | Serves | Workflow |
|---|---|---|
| `delivery-planning` | `delivery-planner` + vertical-slice guide | Engineering Step 1 |
| `feature-spec` | `feature-spec-generator` + template | Engineering Step 3 |
| `golden-dataset` | `golden-dataset-generator` + `golden-dataset-template` | Engineering Step 3b |
| `compliance-check` | `architecture-compliance` + report template | Engineering Step 4 |
| `ui-compliance` | `ui-compliance-check` + design system | Engineering Step 4a |
| `feature-spec-reconciliation` | `feature-spec-reconciler` | Engineering Step 4b |
| `decomposition` | `plan-decomposer` **skill** + glossary | Engineering Step 5 |
| `part-code-review` | `code-quality-reviewer` + `code-quality-checklist-template` + `code-quality-standard` | Engineering Step 6a |
| `slice-verification` | Checklist template + reports whether Step 6b evidence exists for the slice | Engineering Step 6b |
| `slice-preparation` | `slice-preparation-runner` | Engineering Steps 2–5 |
| `ui-remediation` | `ui-compliance-check` + `remediation-spec-template` | UI remediation |
| `architecture-design` | `architecture-designer` + blueprint template | Architecture Modes A/B |
| `architecture-blueprint-review` | `architecture-reviewer` | Architecture Modes A/B |
| `architecture-reconciliation` | `architecture-reconciler` + blueprint template | Architecture Modes A/B/D |
| `architecture-final-gate` | `architecture-final-quality-gate` | Between reconciliation and ADR generation |
| `adr-generation` | `adr-generator` + ADR template + gate report | After an approving gate verdict |
| `architecture-review`, `existing-architecture-review` | `existing-architecture-reviewer` | Architecture Mode C |
| `architecture-gap-reconciliation` | `architecture-gap-reconciler` | Architecture Mode C |
| `prototype-analysis` | `prototype-analyzer` | Architecture Modes A/B |
| `prototype-architecture-alignment` | `prototype-architecture-alignment` | Architecture Mode B |
| `legacy-system-analysis` | `legacy-system-analyzer` | Architecture Mode D |

Steps taking an optional `sliceName`: `golden-dataset`, `compliance-check`, `ui-compliance`, `decomposition`, `part-code-review`, `slice-verification`.

### MCP Prompts

| Prompt | Arguments | Description |
|---|---|---|
| `feature_spec` | `slice_name` | Complete prompt for generating a feature spec with all context |
| `delivery_plan` | — | Complete prompt for delivery plan generation |
| `architecture_compliance` | `slice_name` | Complete compliance check prompt with architecture + feature spec |
| `adr_generator` | `decision_title` | ADR generation prompt with template and architecture context |
| `feature_spec_reconciler` | `slice_name` | Reconciliation prompt with compliance findings and feature spec |
| `architecture_designer` | — | Design production architecture from prototype analysis, with blueprint template and guides |
| `architecture_reviewer` | — | Review an architecture blueprint for issues and risks |
| `architecture_reconciler` | — | Reconcile blueprint with review findings to produce architecture-final.md |
| `architecture_final_quality_gate` | — | Run the 16-check quality gate on architecture-final.md (toolkit v4.5.0), in a fresh session, before ADR generation |
| `existing_architecture_reviewer` | — | Review an existing architecture document (no-prototype path) |
| `architecture_gap_reconciler` | — | Reconcile existing architecture with review findings (no-prototype path) |
| `prototype_architecture_alignment` | — | Compare prototype analysis against architecture document |
| `ui_compliance_check` | `slice_name` | UI compliance check against design system for a slice |
| `slice_verification` | `slice_name` | Integrated slice verification checklist (Step 6b) with full context |

## Decomposition awareness (`ai-parts/`)

The server reads the per-slice decomposition output written by the `plan-decomposer` skill and consumed by `part-executor-tdd`:

- **Part status** — each Part file's `Status:` line (`TODO` / `IN_PROGRESS` / `DONE` / `BLOCKED`; `UNKNOWN` when the line is missing or unrecognised).
- **Review state** — whether `reviews/<part-id>-quality-report.md` (Step 6) and `reviews/<part-id>-review.md` (Step 6a) exist, and the review's verdict (`APPROVED`, `APPROVED WITH NOTES`, `REJECTED — MUST FIX`).
- **Requirement traceability** — whether `OVERVIEW.md` contains the `## Requirement Coverage Map` section required from toolkit v4.6.0, and the optional `PART_SPEC` fields `part_type` and `criteria_covered`.

`PART_SPEC` parsing never fails a listing: a missing or malformed block leaves the optional fields unset. The two optional fields are treated differently, matching the toolkit:

- `part_type` has a defined fallback (the reviewer classifies from `file_touch_points`), so its absence is reported as `(unclassified)` and is **not** a warning.
- `criteria_covered` has no fallback, so its absence **is** reported as an unowned-criteria risk.

Warnings are surfaced for the cases that block progress: a missing Requirement Coverage Map, a Part at `DONE` with no Step 6a review, a `REJECTED — MUST FIX` verdict, and a Part declaring no `criteria_covered`.

### Slice identifiers

Feature specs, compliance reports, and verification evidence are named `<slice-id>-<slice-name>.md`, but the decomposition folder is `ai-parts/<slice-id>/` alone. The server accepts either form: a slice folder is resolved by exact match first, then by the `<slice-id>` prefix of the supplied name (longest match wins). The actual directory name is always what gets reported and used to compose paths, so casing differences do not break lookups on case-sensitive filesystems.

## Content coverage decisions

Two toolkit locations sit outside the server's default reach. Both were resolved deliberately:

- **`.github/skills/` is served** under the `skills` category. `plan-decomposer` and `part-executor-tdd` define the Part handoff contract the `decomposition` step describes, so leaving them unserved made that step incomplete. The nested `<name>/SKILL.md` layout is handled by listing skill folders and resolving a skill name to its `SKILL.md`.
- **`.json` toolkit assets are served** from every category. `ai/examples/example-golden-dataset-case.json` and `ai/templates/golden-dataset-json-template.json` are load-bearing for the `golden-dataset` step and were previously invisible. Markdown files are still listed without their extension; JSON assets keep theirs, so every listed name resolves back to its file.
- **`docs/` is deliberately not served.** `ai/guides/conversation-summary.md` moved to `docs/design-history.md`, which the toolkit's root `CLAUDE.md` defines as toolkit-repo documentation that is *not copied into projects*. Serving it would require a fourth configured root for a file that does not exist in consumer repositories.

One known limitation follows from the compliance-report naming convention itself: `list_project_artifacts` splits the two report kinds on the `-ui` filename suffix, so for a slice whose own name ends in `-ui` its architecture report is listed under `ui-compliance-reports`. Lookups are unaffected — `get_slice_context` resolves each report positionally (`<slice>.md` and `<slice>-ui.md`), so the correct document is always returned.

## Requirements

- [.NET 10 SDK](https://dot.net/download)

## Configuration

### VS Code (`mcp.json`)

Add to your workspace's `.vscode/mcp.json`:

```json
{
  "servers": {
    "ai-architecture-toolkit": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/ai-architecture-toolkit/mcp-server/AiArchitectureToolkit.McpServer/AiArchitectureToolkit.McpServer.csproj"
      ],
      "env": {
        "WORKSPACE_ROOT": "${workspaceFolder}"
      }
    }
  }
}
```

### Environment Variables

| Variable | Default | Description |
|---|---|---|
| `TOOLKIT_ROOT` | `<toolkit repo>/ai` | Path to the toolkit's `ai/` directory |
| `WORKSPACE_ROOT` | Current working directory | Path to the consumer project's workspace root |
| `GITHUB_ROOT` | `<toolkit repo>/.github` | Path to `.github/` directory for instructions, agents, and skills |

`<toolkit repo>` is found by walking up from the server binary to the first directory containing both `ai/` and `.github/`. Set both variables explicitly when the published binary lives outside the toolkit repository.

### Published Binary (faster startup)

For faster startup, publish and reference the binary directly:

```bash
cd mcp-server
dotnet publish AiArchitectureToolkit.McpServer -c Release -o ./publish
```

Then in `mcp.json`:

```json
{
  "servers": {
    "ai-architecture-toolkit": {
      "type": "stdio",
      "command": "/path/to/publish/AiArchitectureToolkit.McpServer",
      "env": {
        "TOOLKIT_ROOT": "/path/to/ai-architecture-toolkit/ai",
        "GITHUB_ROOT": "/path/to/ai-architecture-toolkit/.github",
        "WORKSPACE_ROOT": "${workspaceFolder}"
      }
    }
  }
}
```

## Building

```bash
cd mcp-server
dotnet build AiArchitectureToolkit.McpServer.slnx
```

## Testing

```bash
cd mcp-server
dotnet test AiArchitectureToolkit.McpServer.slnx
```

## Architecture

```
mcp-server/
├── AiArchitectureToolkit.McpServer/
│   ├── Program.cs                          # Entry point, DI, MCP registration
│   ├── Configuration/
│   │   ├── ServerOptions.cs                # Path configuration model
│   │   └── ToolkitPaths.cs                 # Locates the shipped toolkit tree
│   ├── Services/
│   │   ├── ToolkitContentService.cs        # Reads toolkit files from ai/ and .github/
│   │   ├── ProjectContentService.cs        # Reads project artifacts from architecture/ and ai-parts/
│   │   ├── SliceDecomposition.cs           # Decomposition and Part models
│   │   ├── DecompositionReport.cs          # Renders Part status / review state as markdown
│   │   └── WorkspaceScanService.cs         # Scans workspace for .NET projects
│   ├── Resources/
│   │   ├── ToolkitResources.cs             # MCP resources for toolkit content
│   │   └── ProjectResources.cs             # MCP resources for project content
│   ├── Tools/
│   │   ├── ToolkitTools.cs                 # Search, list, glossary lookup
│   │   ├── ProjectTools.cs                 # Workflow context, slice context, workspace scan
│   │   └── ValidationTools.cs              # Verticality check
│   └── Prompts/
│       └── ToolkitPrompts.cs               # Parameterized prompt assembly
└── AiArchitectureToolkit.McpServer.Tests/
    ├── Configuration/                       # Path resolution tests
    ├── Resources/                           # Resource and prompt tests
    ├── Services/                            # Unit tests for services
    └── Tools/                               # Unit tests for tools
```

## Security

All file reads are sandboxed to the configured `TOOLKIT_ROOT`, `WORKSPACE_ROOT`, and `GITHUB_ROOT` paths. Path traversal attempts (e.g., `../../etc/passwd`) are rejected, including in slice IDs, Part IDs, and skill names.
