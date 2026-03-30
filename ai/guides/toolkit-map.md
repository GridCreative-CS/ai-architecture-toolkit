# Toolkit Map

A visual reference showing how all toolkit components connect — prompts, templates, agents, skills, guides, and outputs — organized by phase.

## End-to-end flow

```text
┌─────────────────────────────────────────────────────────────────────────┐
│                        ARCHITECTURE PHASE                              │
│                                                                        │
│  Inputs:          Prompts:                    Outputs:                  │
│  ─────────        ────────                    ────────                  │
│  prototype    →   prototype-analyzer      →   prototype-analysis.md    │
│  existing doc →   existing-arch-reviewer  →   existing-arch-review.md  │
│                   arch-designer           →   architecture-blueprint.md│
│                   arch-reviewer           →   review-report.md         │
│                   arch-reconciler         →   architecture-final.md ★  │
│                   adr-generator           →   adr/*.md                 │
│                                                                        │
│  Templates used: architecture-blueprint-template, adr-template         │
│  Guides: how-to-choose-entry-mode, glossary                           │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                     DELIVERY & SPECIFICATION PHASE                      │
│                                                                        │
│  Prompts:                         Outputs:                             │
│  ────────                         ────────                             │
│  delivery-planner             →   delivery-plan.md                     │
│  feature-spec-generator       →   feature-specs/<slice>.md             │
│  golden-dataset-generator     →   golden-datasets/<slice>.md           │
│                                                                        │
│  Templates used: feature-spec-template, golden-dataset-template        │
│  Guides: vertical-slice-definition, how-feature-specs-are-used         │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                     COMPLIANCE & RECONCILIATION                        │
│                                                                        │
│  Prompts:                         Outputs:                             │
│  ────────                         ────────                             │
│  architecture-compliance      →   compliance-report.md                 │
│  feature-spec-reconciler      →   updated feature-specs/<slice>.md     │
│                                                                        │
│  Templates used: compliance-report-template                            │
│  Guides: definition-of-ready-and-done                                  │
└─────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────┐
│                        EXECUTION PHASE                                 │
│                                                                        │
│  Skills:                          Outputs:                             │
│  ───────                          ────────                             │
│  plan-decomposer              →   ai-parts/OVERVIEW.md                 │
│                                   ai-parts/PXX-*.md                    │
│  part-executor-tdd            →   implemented code + tests             │
│                                                                        │
│  Agents (adopt during execution):                                      │
│  ─────────────────────────────                                         │
│  orchestrator-agent   — coordinates the overall workflow               │
│  backend-agent        — backend / domain / API implementation          │
│  frontend-agent       — frontend / UI implementation                   │
│  ai-agent             — AI/ML model integration                        │
│  qa-agent             — quality assurance and testing strategy          │
│  ai-testing-agent     — AI-specific testing and golden datasets        │
│  devops-agent         — CI/CD, infrastructure, deployment              │
│  integration-reviewer — cross-boundary integration review              │
│                                                                        │
│  Guides: contract-definition, modular-monolith-definition              │
└─────────────────────────────────────────────────────────────────────────┘

★ = architecture-final.md is the authoritative source of truth
```

## Component index

### Prompts — `ai/prompts/`

| Prompt | Phase | Purpose |
|--------|-------|---------|
| `prototype-analyzer` | Architecture | Extract behavior and intent from a prototype |
| `architecture-designer` | Architecture | Design production architecture from analysis |
| `architecture-reviewer` | Architecture | Review architecture for risks, gaps, and quality |
| `architecture-reconciler` | Architecture | Reconcile reviewer feedback into final architecture |
| `architecture-gap-reconciler` | Architecture | Reconcile gaps when starting from an existing doc |
| `existing-architecture-reviewer` | Architecture | Review a pre-existing architecture document |
| `prototype-architecture-alignment` | Architecture | Align prototype behavior with architecture doc |
| `adr-generator` | Architecture | Generate Architecture Decision Records |
| `delivery-planner` | Delivery | Create milestone-based delivery plan with vertical slices |
| `feature-spec-generator` | Delivery | Generate detailed spec for one slice |
| `golden-dataset-generator` | Delivery | Generate test datasets for AI/business logic validation |
| `architecture-compliance` | Compliance | Verify feature spec aligns with approved architecture |
| `feature-spec-reconciler` | Compliance | Reconcile feature spec after compliance findings |
| `feature-spec-reconciler-quickversion` | Compliance | Lightweight reconciliation for smaller changes |

### Templates — `ai/templates/`

| Template | Used by |
|----------|---------|
| `architecture-blueprint-template` | `architecture-designer` |
| `adr-template` | `adr-generator` |
| `feature-spec-template` | `feature-spec-generator` |
| `compliance-report-template` | `architecture-compliance` |
| `golden-dataset-template` | `golden-dataset-generator` |
| `golden-dataset-json-template.json` | `golden-dataset-generator` |
| `project-context-template` | Filled in manually as the first step |

### Agents — `ai/agents/`

| Agent | Specialty |
|-------|-----------|
| `orchestrator-agent` | Coordinates multi-agent slice work and task sequencing |
| `backend-agent` | Backend, domain logic, API, persistence |
| `frontend-agent` | Frontend, UI components, user flows |
| `ai-agent` | AI/ML model integration, explainability, governance |
| `qa-agent` | Testing strategy, coverage, regression |
| `ai-testing-agent` | AI-specific testing, golden datasets, probabilistic validation |
| `devops-agent` | CI/CD, containers, infrastructure, deployment |
| `integration-reviewer` | Cross-slice contract verification and drift detection |

### Skills — `.github/skills/`

| Skill | Purpose |
|-------|---------|
| `plan-decomposer` | Decomposes a slice into independently verifiable Parts |
| `part-executor-tdd` | Executes one Part at a time using strict red-green-refactor TDD |

### Guides — `ai/guides/`

| Guide | What it covers |
|-------|----------------|
| `quick-start` | First-time walkthrough — zero to first slice in 15 minutes |
| `toolkit-map` | This file — visual map of all components |
| `glossary` | Definitions of all load-bearing terms |
| `vertical-slice-definition` | Verticality test, definition, and anti-patterns |
| `modular-monolith-definition` | Modular monolith pattern and extraction criteria |
| `contract-definition` | API contract structure and testing guidance |
| `how-to-choose-entry-mode` | Decision guide for which workflow to use |
| `how-feature-specs-are-used` | How feature specs bridge planning and implementation |
| `definition-of-ready-and-done` | Readiness and completion criteria |
| `operating-model` | Full operating model across all phases |
| `conversation-summary` | Guidance for summarizing conversations |

### Examples — `ai/examples/`

| Example | What it demonstrates |
|---------|----------------------|
| `vertical-vs-horizontal-slices` | Good vs. bad slice decomposition |
| `modular-monolith-patterns` | Module boundary patterns |
| `contract-patterns` | API contract examples |
| `feature-spec-driven-slice-flow` | Walkthrough from spec to implementation |
| `example-compliance-report` | Sample compliance report |
| `example-feature-spec-outline` | Sample feature spec |
| `example-golden-dataset-case.json` | Sample golden dataset case |

## Where to start

- **First time?** → `ai/guides/quick-start.md`
- **Choosing a workflow?** → `ai/guides/how-to-choose-entry-mode.md`
- **Understanding terms?** → `ai/guides/glossary.md`
- **Full operating model?** → `ai/guides/operating-model.md`
- **Writing a feature spec?** → `ai/guides/how-feature-specs-are-used.md`
