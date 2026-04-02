# CLAUDE.md — AI Architecture Toolkit

## Project overview

This repository uses the AI Architecture Toolkit — a set of reusable prompts, agents, workflows, templates, guides, and skills for AI-assisted architecture design, delivery planning, feature specification, decomposition, and TDD-based implementation. The toolkit is copied into project repositories and adapted to each project's needs.

## Toolkit structure

This toolkit is designed to be copied into other repositories as a foundation for AI-assisted architecture and engineering. The toolkit files live in these directories:

```text
ai/                          # Core toolkit
  agents/                    # Specialist agent personas (backend, frontend, QA, DevOps, etc.)
  guides/                    # Reference guides (glossary, vertical slices, contracts, etc.)
  prompts/                   # Prompts for architecture, delivery, compliance, etc.
  templates/                 # Templates for feature specs, ADRs, compliance reports, etc.
  workflows/                 # End-to-end workflow definitions
  examples/                  # Concrete good/bad pattern examples
  project-context.md         # Project-specific context (fill per project)
.github/
  instructions/              # File-type coding conventions (C#, Docker, Compose)
  skills/                    # Decomposition and execution skills
  agents/                    # .NET engineer agent persona
  prompts/                   # Planning prompts
  copilot-instructions.md    # Cross-tool instructions (also applies to Claude)
```

The `architecture/` directory is an output location created per project (blueprints, ADRs, delivery plans, feature specs, compliance reports). The rest of the project structure depends on the application being built.

## Working rules

These rules apply to ALL work in this repository (sourced from `.github/copilot-instructions.md`):

1. Treat `architecture/architecture-final.md` and `architecture/adr/*.md` as authoritative once they exist.
2. For architecture work, follow `ai/workflows/architecture-workflow.md` (or its variants).
3. For implementation work, follow `ai/workflows/engineering-workflow.md`.
4. Use `ai/project-context.md` as additional context for any project-specific work.
5. Prefer vertical slices. See `ai/guides/vertical-slice-definition.md` for the definition and verticality test.
6. Prefer modular monolith unless another pattern is explicitly justified. See `ai/guides/modular-monolith-definition.md`.
7. Do not introduce new architecture without review.
8. Respect TDD and the decomposition/execution skills (plan-decomposer, part-executor-tdd).
9. If a feature spec exists for the selected slice, treat it as a primary input for decomposition and implementation.
10. See `ai/guides/glossary.md` for definitions of key terms used throughout the toolkit.
11. Do not make assumptions about the project context beyond what is stated in `ai/project-context.md`. Prefer asking for clarification over assuming. For every question you ask, provide advice.
12. Treat `architecture/design-system.md` as authoritative for UI decisions when it exists.
13. For UI-inclusive projects, follow `ai/workflows/ui-foundation-workflow.md` (greenfield) or `ai/workflows/ui-retrofit-workflow.md` (retrofit).

## Workflows

### Architecture workflow

Follow `ai/workflows/architecture-workflow.md`. Variants:
- `ai/workflows/architecture-workflow-architecture-doc-only.md` — when working from an architecture doc without a prototype
- `ai/workflows/architecture-workflow-prototype-only.md` — when working from a prototype
- `ai/workflows/architecture-workflow-prototype-plus-architecture-doc.md` — both available

### Engineering workflow (implementation)

Follow `ai/workflows/engineering-workflow.md`. The sequence is:

1. Delivery planning (`ai/prompts/delivery-planner.md`)
2. Validate delivery plan verticality
3. Select next slice
4. Generate feature spec (`ai/prompts/feature-spec-generator.md` + `ai/templates/feature-spec-template.md`)
5. Architecture compliance check (`ai/prompts/architecture-compliance.md`)
6. Reconcile feature spec if compliance findings exist (`ai/prompts/feature-spec-reconciler.md`)
7. Decompose the slice (use `/plan-decomposer` skill)
8. Execute parts with TDD (use `/part-executor-tdd` skill)
9. Repeat per slice

### UI workflows

For projects with human-facing UI:

- **Greenfield:** Follow `ai/workflows/ui-foundation-workflow.md` to create a design system after architecture finalization, before delivery planning.
- **Retrofit:** Follow `ai/workflows/ui-retrofit-workflow.md` to inventory existing UI, derive a design system, and migrate slices.
- Treat `architecture/design-system.md` as authoritative for UI decisions once it exists.

## Specialist agents

When working on implementation, adopt the relevant persona from `ai/agents/`:

| Agent | File | Use when |
| --- | --- | --- |
| Orchestrator | `ai/agents/orchestrator-agent.md` | Coordinating multi-agent slice work |
| Backend | `ai/agents/backend-agent.md` | Backend/domain/API implementation |
| Frontend | `ai/agents/frontend-agent.md` | Frontend implementation |
| AI Agent | `ai/agents/ai-agent.md` | AI/ML feature implementation |
| QA | `ai/agents/qa-agent.md` | Testing strategy and execution |
| AI Testing | `ai/agents/ai-testing-agent.md` | AI-specific testing (golden datasets) |
| DevOps | `ai/agents/devops-agent.md` | CI/CD, containers, infrastructure |
| Integration Reviewer | `ai/agents/integration-reviewer.md` | Cross-slice contract verification |

Also see `.github/agents/expert-dotnet-software-engineer.agent.md` for the .NET expert engineering persona (SOLID, TDD, clean code).

## Code conventions

Before writing any code, read the full guidance in `.github/instructions/`. Key rules:

### C# (`.github/instructions/csharp.instructions.md`)

- **C# version:** Always use C# 14 features (latest LTS).
- **Solution format:** Use `.slnx` (not `.sln`) for .NET 9+. Create with `dotnet new sln --format slnx`.
- **Central Package Management:** All NuGet versions go in `Directory.Packages.props`. Individual csproj files must NOT specify `Version` on `<PackageReference>`.
- **Shared build props:** `Directory.Build.props` sets `TargetFramework`, `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`. Do not duplicate these in individual csproj files.
- **Nullable:** Use `is null` / `is not null`, never `== null` / `!= null`. Trust null annotations — don't add null checks when the type system says a value cannot be null.
- **Formatting:** File-scoped namespaces, newline before opening brace, pattern matching and switch expressions preferred. Use `nameof` instead of string literals. Follow `.editorconfig` when present.
- **Naming:** PascalCase for public members and methods. camelCase for private fields and locals. Prefix interfaces with `I`.
- **XML docs:** Required for public APIs. Include `<example>` and `<code>` where applicable.
- **Immutability:** Value objects must defensively copy mutable inputs (`byte[]`, `List<T>`) and expose read-only views (`ReadOnlyMemory<byte>`, `IReadOnlyList<T>`). For `params` arrays: validate, clone, then store as read-only.
- **Testing:** No "Arrange/Act/Assert" comments. Test observable behaviour, not implementation details. Copy nearby test naming style. Ensure test method names accurately describe expected behaviour and exception types.
- **Test project pairing:** Every solution must include a `SolutionStructureTests` class asserting that each `*.Tests.csproj` under `tests/` has a `ProjectReference` to the matching `src/` project.
- **Secrets:** Never commit real credentials. Use placeholders and `dotnet user-secrets` or `.env`. Fail fast on missing required configuration at startup.
- **Serilog redaction:** Use enrichers matching on property names, not CLR type names.
- **Data access:** EF Core with repository pattern. Explain different DB options for dev/prod. Proper migrations and seeding.
- **Validation:** Data annotations or FluentValidation. RFC 7807 problem details for error responses.

### Docker (`.github/instructions/dockerfile.instructions.md`)

- Multi-stage builds with separate build and runtime stages.
- Non-root user in production. Pin image versions (no `latest`). Prefer alpine variants.
- Copy project/package files before source for layer caching.
- EF migration containers: explicit build step before `--no-build`, configuration must match (Release/Debug).
- Health checks with HEALTHCHECK instruction. `.dockerignore` to exclude unnecessary files.

### Docker Compose (`.github/instructions/docker-compose.instructions.md`)

- Health checks with `depends_on` conditions (`service_healthy`, `service_completed_successfully`).
- Network isolation: separate internal and Traefik networks.
- Required env vars use `${VAR:?error}` syntax. Provide `.env.example`.
- Migration containers use `restart: "no"` and `CMD` (not `ENTRYPOINT`).
- Named volumes for data persistence.

## Key reference documents

Before making decisions, consult:

- **Glossary:** `ai/guides/glossary.md` — definitions of all load-bearing terms
- **Vertical slices:** `ai/guides/vertical-slice-definition.md` — verticality test and anti-patterns
- **Modular monolith:** `ai/guides/modular-monolith-definition.md` — module boundaries and extraction criteria
- **Contracts:** `ai/guides/contract-definition.md` — three contract layers, testing, versioning
- **Definition of Ready/Done:** `ai/guides/definition-of-ready-and-done.md`
- **How feature specs are used:** `ai/guides/how-feature-specs-are-used.md`
- **Operating model:** `ai/guides/operating-model.md`
- **Design system template:** `ai/templates/design-system-template.md`
- **UI foundation workflow:** `ai/workflows/ui-foundation-workflow.md`
- **UI retrofit workflow:** `ai/workflows/ui-retrofit-workflow.md`
