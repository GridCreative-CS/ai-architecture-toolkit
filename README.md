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
