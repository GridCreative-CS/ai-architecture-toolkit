# AI Architecture Toolkit — v2

This toolkit supports a **prototype → architecture → delivery → decomposition → TDD execution** workflow.

It is designed for a way of working where you:

1. Build a prototype (often Spark or other exploratory technology)
2. Extract the validated behavior and business intent
3. Design the production architecture
4. Review and reconcile the architecture
5. Generate ADRs
6. Create a delivery plan
7. Break the work into executable parts
8. Implement using TDD
9. Use specialist AI agents where helpful

---

## End-to-end workflow

```text
Spark Prototype
      ↓
Prototype Analyzer
      ↓
Architecture Designer
      ↓
Architecture Reviewer
      ↓
Architecture Reconciler
      ↓
ADR Generator
      ↓
Delivery Planner
      ↓
Feature Spec Generator
      ↓
Architecture Compliance Check
      ↓
Plan-Decomposer
      ↓
Part-Executor (TDD)
      ↓
Specialist Agents
      ↓
AI Testing Agent
      ↓
Integration Review
```

---

## What is included

### Prompts

Located under:

- `ai/prompts/`

These are used for:

- prototype analysis
- architecture design
- architecture review
- architecture reconciliation
- ADR generation
- delivery planning
- feature specification generation
- architecture compliance
- golden dataset generation

### Agents

Located under:

- `ai/agents/`

These are used for:

- orchestration
- backend work
- frontend work
- AI/model work
- QA work
- AI-focused testing
- DevOps work
- integration review

### Templates

Located under:

- `ai/templates/`

These help structure:

- architecture blueprints
- ADRs
- feature specifications
- golden dataset definitions

### Skills

Located under:

- `skills/plan-decomposer/SKILL.md`
- `skills/part-executor-tdd/SKILL.md`

These are your execution-layer skills and are the bridge between planning and code implementation.

### Existing expert engineering agent

Located under:

- `agents/expert-dotnet-software-engineer.agent.md`

This provides expert .NET engineering guidance and is already integrated into your plan decomposition / implementation model.

---

# How to use this with Claude

There are two practical ways to use this toolkit with Claude.

## Option A — Use it directly from inside the project repo

This is the simplest setup.

Place this toolkit inside your project repository and then use Claude with the repo open.

Recommended project structure:

```text
repo
│
├ architecture
├ ai
├ skills
├ src
├ tests
└ .github
```

### Architecture phase with Claude

#### Step 1 — Prototype analysis

Prompt Claude with something like:

```text
Use ai/prompts/prototype-analyzer.md.

Analyze this repository as a prototype.
Treat it as reference behavior, not reference architecture.
Write the result in architecture/prototype-analysis.md.
```

#### Step 2 — Architecture blueprint

Then:

```text
Use ai/prompts/architecture-designer.md
and ai/templates/architecture-blueprint-template.md.

Generate architecture/architecture-blueprint.md.
```

#### Step 3 — Architecture review

Then:

```text
Use ai/prompts/architecture-reviewer.md.

Review architecture/architecture-blueprint.md
and write the result to architecture/review-report.md.
```

#### Step 4 — Architecture reconciliation

Then:

```text
Use ai/prompts/architecture-reconciler.md.

Use architecture/architecture-blueprint.md
and architecture/review-report.md
to generate architecture/architecture-final.md.
```

#### Step 5 — Generate ADRs

Then:

```text
Use ai/prompts/adr-generator.md
and ai/templates/adr-template.md.

Generate ADR files under architecture/adr/.
```

### Delivery phase with Claude

#### Step 6 — Delivery plan

Then:

```text
Use ai/prompts/delivery-planner.md.

Use architecture/architecture-final.md and architecture/adr/*.md
to create architecture/delivery-plan.md.
```

#### Step 7 — Feature specs

Then:

```text
Use ai/prompts/feature-spec-generator.md
and ai/templates/feature-spec-template.md.

Generate one feature spec at a time in architecture/feature-specs/.
```

#### Step 8 — Golden dataset definitions

Then:

```text
Use ai/prompts/golden-dataset-generator.md
and ai/templates/golden-dataset-template.md.

Generate scenario packs in architecture/golden-datasets/.
```

### Execution phase with Claude

#### Step 9 — Plan decomposition

Use your existing skill:

```text
Use skills/plan-decomposer/SKILL.md.

Input: architecture/delivery-plan.md
Output: ai-parts/OVERVIEW.md and ai-parts/PXX-*.md
```

#### Step 10 — TDD part execution

Then use:

```text
Use skills/part-executor-tdd/SKILL.md.

Execute exactly one Part from ai-parts/.
Follow strict TDD.
```

#### Step 11 — Specialist agent support

When needed, ask Claude to adopt one of the specialist agent roles, for example:

```text
Use ai/agents/backend-agent.md.
Implement the backend tasks for the Recommendation slice.
```

Or:

```text
Use ai/agents/ai-testing-agent.md.
Review the golden scenario coverage for this slice.
```

---

## Option B — Use a central toolkit repo and a separate project repo

This is usually the best model if you work across many repositories.

### Recommended setup

Create one central repo, for example:

```text
ai-architecture-toolkit
```

Store the toolkit there.

Then, in each project repo, keep only:

```text
/architecture
/ai/project-context.md
/.github/copilot-instructions.md
```

In that case, you use the central toolkit as your source of prompts and templates, and store only the generated outputs in the project repo.

### Why this is often better

Benefits:

- one master version of prompts and workflows
- easier maintenance
- less copy/paste between repos
- consistent architecture and delivery process across projects

### When local copies are still useful

Keep local overrides in a project repo when:

- the project needs domain-specific prompt variations
- a project has special compliance requirements
- you want version traceability of exactly which prompt version was used

---

# How to use this with GitHub / Copilot

For GitHub and Copilot, the best model is slightly different from Claude.

Copilot works best when:

- instructions live inside the repo
- architecture files are present locally
- workflows are explicit
- prompts or instructions can be referenced from repo files

## Recommended minimal local setup for GitHub repos

Even if you keep the toolkit centrally, keep these in each project repo:

```text
architecture/
.github/copilot-instructions.md
ai/project-context.md
```

Optionally also keep:

```text
ai/workflows/
ai/prompts/
```

if you want everything self-contained.

---

## Copilot usage pattern

### 1. Architecture must be present in the repo

Copilot should always be guided by:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`

These should be treated as authoritative.

### 2. Copilot instructions

Use `.github/copilot-instructions.md` to tell Copilot to follow:

- architecture
- ADRs
- vertical slices
- modular monolith defaults
- TDD expectations
- workflows in `ai/workflows/`

### 3. Prompting Copilot

Examples:

```text
Follow ai/workflows/architecture-workflow.md
and generate architecture/architecture-blueprint.md.
```

Or:

```text
Follow ai/workflows/engineering-workflow.md.
Use architecture/delivery-plan.md as input.
```

Or:

```text
Use ai/agents/backend-agent.md
and implement the backend tasks for the current slice.
```

---

# Recommended repository strategy

## Best practical strategy for your situation

Use a **hybrid model**:

### Central toolkit repo

Keep the master toolkit centrally:

```text
ai-architecture-toolkit
```

### Per project repo

Keep only:

- generated outputs
- project-specific context
- local overrides if needed
- Copilot instructions

That means:

### Central

- prompts
- templates
- workflows
- base agents
- shared documentation

### Per project

- architecture outputs
- ADR outputs
- delivery plan
- feature specs
- golden datasets
- ai-parts generated from decomposition
- project-specific overrides

This gives you:

- consistency
- maintainability
- flexibility

---

# Suggested way of working

## For new projects

1. start from a prototype
2. run the architecture workflow
3. approve architecture
4. generate ADRs
5. create the delivery plan
6. generate feature specs
7. define golden scenarios where valuable
8. decompose into parts
9. execute parts with TDD
10. use specialist agents only under orchestration

## For ongoing projects

1. update architecture if needed
2. add or update ADRs when decisions change
3. regenerate delivery plan when scope changes
4. use architecture compliance checks before major implementation changes

---

# Important principle

This toolkit is strongest when you treat it as:

**Architecture governance + AI engineering execution**

not just as a collection of prompts.

That means:

- architecture first
- planning second
- implementation third
- review throughout

---

# Included implementation-layer skills

This toolkit already contains your integrated skills:

- `skills/plan-decomposer/SKILL.md`
- `skills/part-executor-tdd/SKILL.md`
- `agents/expert-dotnet-software-engineer.agent.md`

These make the execution phase much stronger because they already enforce:

- decomposition discipline
- file-based handoff
- TDD
- expert .NET engineering guidance

---

# Suggested next step

Use this toolkit as your **central master repo**.

Then for each new product/project:

- create or reuse a project repo
- copy in only the local files you need
- keep the generated outputs in the project repo
- keep the master prompts/templates/workflows centrally maintained


---

## v3 Extension — Support for Prototype + Existing Architecture Document

The earlier versions primarily assumed:

```text
Prototype
   ↓
Architecture generation
```

v3 also supports a second input mode:

```text
Prototype + existing architecture document
   ↓
Existing Architecture Review
   ↓
Prototype vs Architecture Alignment
   ↓
Architecture Gap Reconciliation
   ↓
Final Architecture
```

### Why this matters

In real projects you may already have:
- a prototype
- an architecture draft
- stakeholder notes
- partial design decisions

In that case the toolkit should not always regenerate architecture from scratch.

Instead it should:
- assess the quality of the existing architecture
- compare it with prototype reality
- identify gaps and inconsistencies
- reconcile them into a stronger final architecture

### New prompts in v3

- `ai/prompts/existing-architecture-reviewer.md`
- `ai/prompts/prototype-architecture-alignment.md`
- `ai/prompts/architecture-gap-reconciler.md`

### Additional output files in v3

- `architecture/existing-architecture-review.md`
- `architecture/prototype-architecture-alignment.md`

### Recommended modes

#### Mode A — Prototype only

Use:
1. prototype analyzer
2. architecture designer
3. architecture reviewer
4. architecture reconciler
5. ADR generator

#### Mode B — Prototype + existing architecture document

Use:
1. prototype analyzer
2. existing architecture reviewer
3. prototype vs architecture alignment
4. architecture gap reconciler
5. ADR generator

### Example Claude usage for Mode B

```text
Use ai/prompts/prototype-analyzer.md.

Analyze the repository as a prototype and write the result to architecture/prototype-analysis.md.
```

```text
Use ai/prompts/existing-architecture-reviewer.md.

Review the provided architecture document and write the result to architecture/existing-architecture-review.md.
```

```text
Use ai/prompts/prototype-architecture-alignment.md.

Compare architecture/prototype-analysis.md with the existing architecture document and write the alignment report to architecture/prototype-architecture-alignment.md.
```

```text
Use ai/prompts/architecture-gap-reconciler.md.

Use the existing architecture document, architecture/existing-architecture-review.md, and architecture/prototype-architecture-alignment.md to generate architecture/architecture-final.md.
```

---

# v3.1 Improvement Pack Additions

This toolkit also includes the v3.1 improvement pack.

## Added in v3.1

- `ai/templates/project-context-template.md`
- `ai/templates/compliance-report-template.md`
- `ai/templates/golden-dataset-json-template.json`
- sharpened agent contracts
- `ai/guides/definition-of-ready-and-done.md`
- split workflows:
  - `ai/workflows/architecture-workflow-prototype-only.md`
  - `ai/workflows/architecture-workflow-prototype-plus-architecture-doc.md`
- examples:
  - `ai/examples/example-compliance-report.md`
  - `ai/examples/example-feature-spec-outline.md`
  - `ai/examples/example-golden-dataset-case.json`

## Why these were added

These additions make the toolkit more operational and less ambiguous by improving:

- project context capture
- compliance reporting
- golden dataset structure
- agent input/output contracts
- readiness and done criteria
- explicit workflow selection for the two architecture modes

## Notes

This merged v3.1 toolkit preserves the broader toolkit README and appends the
improvement-pack additions instead of replacing the original documentation.

---

# README Operational Additions

This section makes the toolkit easier to use in practice.

## Quick Start in 5 Minutes

If you want the shortest path from prototype to implementation, use this sequence.

### Quick Start

1. Put the prototype repository in scope.
2. Run `prototype-analyzer`.
3. Run `architecture-designer`.
4. Run `architecture-reviewer`.
5. Run `architecture-reconciler`.
6. Run `adr-generator`.
7. Run `delivery-planner`.
8. Run `plan-decomposer`.
9. Run `part-executor-tdd`.

### Minimal quick-start prompts

Use prompts like these.

#### Step 1 — Prototype analysis

```text
Use ai/prompts/prototype-analyzer.md.

Analyze this repository as a prototype.
Treat it as reference behavior, not reference architecture.
Write the result to architecture/prototype-analysis.md.
```

#### Step 2 — Architecture blueprint

```text
Use ai/prompts/architecture-designer.md
and ai/templates/architecture-blueprint-template.md.

Generate architecture/architecture-blueprint.md.
```

#### Step 3 — Architecture review

```text
Use ai/prompts/architecture-reviewer.md.

Review architecture/architecture-blueprint.md
and write the result to architecture/review-report.md.
```

#### Step 4 — Architecture reconciliation

```text
Use ai/prompts/architecture-reconciler.md.

Use architecture/architecture-blueprint.md
and architecture/review-report.md
to generate architecture/architecture-final.md.
```

#### Step 5 — ADR generation

```text
Use ai/prompts/adr-generator.md
and ai/templates/adr-template.md.

Generate ADR files under architecture/adr/.
```

#### Step 6 — Delivery planning

```text
Use ai/prompts/delivery-planner.md.

Use architecture/architecture-final.md and architecture/adr/*.md
to create architecture/delivery-plan.md.
```

#### Step 7 — Decomposition

```text
Use skills/plan-decomposer/SKILL.md.

Input: architecture/delivery-plan.md
Output: ai-parts/OVERVIEW.md and ai-parts/PXX-*.md
```

#### Step 8 — TDD implementation

```text
Use skills/part-executor-tdd/SKILL.md.

Execute exactly one Part from ai-parts/.
Follow strict TDD.
```

## First 3 Commands or Prompts to Run

If you are starting a new project, these are the first three prompts to run.

### If you only have a prototype

1. `prototype-analyzer`
2. `architecture-designer`
3. `architecture-reviewer`

### If you have a prototype and an existing architecture document

1. `prototype-analyzer`
2. `existing-architecture-reviewer`
3. `prototype-architecture-alignment`

This gets you quickly into the right mode.

## Which Mode Should You Choose

The toolkit supports two architecture entry modes.

### Mode A — Prototype Only

Use this mode when:

- you have a prototype
- you do not yet have a useful architecture document
- you want the toolkit to generate the architecture from the prototype

Use this workflow:

- `ai/workflows/architecture-workflow-prototype-only.md`

Typical sequence:

```text
Prototype
  ↓
Prototype Analyzer
  ↓
Architecture Designer
  ↓
Architecture Reviewer
  ↓
Architecture Reconciler
  ↓
ADR Generator
```

### Mode B — Prototype Plus Existing Architecture Document

Use this mode when:

- you have a prototype
- you also have an existing architecture document
- you want to validate and reconcile the document against prototype reality

Use this workflow:

- `ai/workflows/architecture-workflow-prototype-plus-architecture-doc.md`

Typical sequence:

```text
Prototype
+ Existing Architecture Document
  ↓
Prototype Analyzer
  ↓
Existing Architecture Reviewer
  ↓
Prototype-Architecture Alignment
  ↓
Architecture Gap Reconciler
  ↓
ADR Generator
```

## Central Toolkit Repo vs Project Repo

The toolkit works best with a hybrid model.

### Central toolkit repo

Use a central repo when you want:

- one master version of prompts
- one master version of templates
- one master version of workflows
- one place to improve the toolkit over time

Recommended central repo contents:

```text
ai-architecture-toolkit
├ ai
├ skills
├ agents
├ architecture
└ README.md
```

### Project repo

Use the project repo for:

- generated architecture outputs
- ADR outputs
- delivery plans
- feature specs
- golden datasets
- decomposition output in `ai-parts`
- project-specific context
- local overrides if needed

Recommended minimal project repo contents:

```text
project-repo
├ architecture
├ ai
│  └ project-context.md
├ .github
│  └ copilot-instructions.md
├ src
└ tests
```

### Practical recommendation

For your setup, the best model is:

- keep the master toolkit centrally
- keep project outputs in the project repo
- copy local overrides only when a project really needs them

## Exact Example Project Flow

This is an example of how to run one real project from start to implementation.

### Example scenario

You have:

- a Spark prototype
- no final production architecture yet
- a target stack of .NET and React

### Example flow

#### 1. Create project context

Use:

- `ai/templates/project-context-template.md`

Write:

- `ai/project-context.md`

#### 2. Analyze the prototype

Use:

- `ai/prompts/prototype-analyzer.md`

Write:

- `architecture/prototype-analysis.md`

#### 3. Generate the blueprint

Use:

- `ai/prompts/architecture-designer.md`
- `ai/templates/architecture-blueprint-template.md`

Write:

- `architecture/architecture-blueprint.md`

#### 4. Review the blueprint

Use:

- `ai/prompts/architecture-reviewer.md`

Write:

- `architecture/review-report.md`

#### 5. Reconcile into the final architecture

Use:

- `ai/prompts/architecture-reconciler.md`

Write:

- `architecture/architecture-final.md`

#### 6. Generate ADRs

Use:

- `ai/prompts/adr-generator.md`
- `ai/templates/adr-template.md`

Write:

- `architecture/adr/*.md`

#### 7. Generate the delivery plan

Use:

- `ai/prompts/delivery-planner.md`

Write:

- `architecture/delivery-plan.md`

#### 8. Generate feature specs if needed

Use:

- `ai/prompts/feature-spec-generator.md`
- `ai/templates/feature-spec-template.md`

Write:

- `architecture/feature-specs/*.md`

#### 9. Generate golden scenarios where useful

Use:

- `ai/prompts/golden-dataset-generator.md`
- `ai/templates/golden-dataset-template.md`

Write:

- `architecture/golden-datasets/*.md`

#### 10. Decompose implementation

Use:

- `skills/plan-decomposer/SKILL.md`

Write:

- `ai-parts/OVERVIEW.md`
- `ai-parts/PXX-*.md`

#### 11. Execute parts with TDD

Use:

- `skills/part-executor-tdd/SKILL.md`

#### 12. Use specialist agents only when needed

Examples:

- `ai/agents/backend-agent.md`
- `ai/agents/frontend-agent.md`
- `ai/agents/ai-agent.md`
- `ai/agents/qa-agent.md`
- `ai/agents/ai-testing-agent.md`
- `ai/agents/devops-agent.md`

## Practical Rule of Thumb

If you are unsure where to start, use this rule:

- only prototype available → use **Prototype Only** mode
- prototype + architecture doc available → use **Prototype Plus Existing Architecture Document** mode

If you are unsure whether the architecture document is trustworthy, still use the second mode and validate it rather than assuming it is correct.

---

# v4 Baseline Notes

This v4 baseline is the normalized merged package that combines:

- the full prototype-to-product architecture toolkit
- the v3.1 improvement pack
- the operational README additions

## What this v4 baseline is for

Use this as the stable starting point for real projects.

It is intended to be the version you:

- keep as the central master toolkit repo
- reuse across multiple project repositories
- improve over time based on actual project usage

## Included improvements

This baseline includes:

- project context template
- compliance report template
- golden dataset JSON template
- sharpened agent contracts
- definition of ready / done
- split workflows for both architecture entry modes
- operational quick start guidance
- example compliance report
- example feature spec outline
- example golden dataset case

## Recommended usage model

### Central toolkit repo

Keep the full toolkit centrally and maintain it there.

### Project repo

Keep generated outputs and project-specific overrides in the project repo, including:

- architecture outputs
- ADR outputs
- delivery plan
- feature specs
- golden datasets
- decomposition output
- project context
- local Copilot instructions

## Suggested next evolution after v4

Use this baseline on at least one real project before making a v4.1 or v5.

The best next improvements should come from real friction observed during usage, not from theoretical expansion alone.
