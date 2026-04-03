# AI Architecture Toolkit — MCP Server

A [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) server that exposes the AI Architecture Toolkit as tools, resources, and prompts for LLM-powered development environments.

## What It Does

This MCP server makes the toolkit **project-aware** — it serves both static toolkit content (guides, prompts, templates, workflows) and dynamically discovered project artifacts (architecture docs, ADRs, feature specs, delivery plans, workspace structure).

### MCP Resources

| URI Pattern | Description |
|---|---|
| `toolkit://guides/{name}` | Toolkit guides (glossary, contract-definition, etc.) |
| `toolkit://prompts/{name}` | Toolkit prompts (delivery-planner, feature-spec-generator, etc.) |
| `toolkit://templates/{name}` | Templates (feature-spec, ADR, compliance-report, etc.) |
| `toolkit://workflows/{name}` | Workflows (engineering, architecture, UI) |
| `toolkit://agents/{name}` | Agent personas (backend, frontend, QA, etc.) |
| `toolkit://examples/{name}` | Example patterns |
| `toolkit://instructions/{name}` | Coding instructions (C#, Docker, etc.) |
| `project://architecture` | Architecture-final.md from the workspace |
| `project://architecture-blueprint` | Architecture blueprint (pre-review draft) |
| `project://review-report` | Architecture review report |
| `project://existing-architecture-review` | Existing architecture review |
| `project://prototype-analysis` | Prototype analysis output |
| `project://prototype-architecture-alignment` | Prototype-architecture alignment analysis |
| `project://adr/{name}` | Specific ADR from the workspace |
| `project://delivery-plan` | Delivery plan from the workspace |
| `project://feature-spec/{name}` | Feature spec for a slice |
| `project://project-context` | Project context |
| `project://design-system` | Design system document |

### MCP Tools

| Tool | Description |
|---|---|
| `list_toolkit_content` | Lists all toolkit files by category with file counts |
| `search_toolkit` | Full-text search across all toolkit markdown files |
| `get_glossary_term` | Looks up a term from the glossary |
| `get_toolkit_file` | Gets full content of a specific toolkit file by category and name |
| `list_project_artifacts` | Lists project artifacts with existence status |
| `get_workspace_structure` | Scans workspace for .NET solutions, projects, and dependencies |
| `get_workflow_context` | **Key tool** — returns everything needed for a workflow step (prompt + template + guide + project artifacts). Supports both engineering and architecture workflow steps |
| `get_slice_context` | Returns all context for a specific slice (feature spec + compliance report + ADRs + delivery plan) |
| `check_verticality` | Provides the verticality test criteria for evaluating a slice |

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
| `existing_architecture_reviewer` | — | Review an existing architecture document (no-prototype path) |
| `architecture_gap_reconciler` | — | Reconcile existing architecture with review findings (no-prototype path) |
| `prototype_architecture_alignment` | — | Compare prototype analysis against architecture document |

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
| `TOOLKIT_ROOT` | `../ai` relative to server binary | Path to the toolkit's `ai/` directory |
| `WORKSPACE_ROOT` | Current working directory | Path to the consumer project's workspace root |
| `GITHUB_ROOT` | `../.github` relative to server binary | Path to `.github/` directory for instructions/agents |

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
│   │   └── ServerOptions.cs                # Path configuration model
│   ├── Services/
│   │   ├── ToolkitContentService.cs        # Reads toolkit files from ai/
│   │   ├── ProjectContentService.cs        # Reads project artifacts from architecture/
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
    ├── Services/                            # Unit tests for services
    └── Tools/                               # Unit tests for tools
```

## Security

All file reads are sandboxed to the configured `TOOLKIT_ROOT`, `WORKSPACE_ROOT`, and `GITHUB_ROOT` paths. Path traversal attempts (e.g., `../../etc/passwd`) are rejected.
