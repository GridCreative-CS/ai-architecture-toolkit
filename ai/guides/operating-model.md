# Operating Model — v2

## Phase 1 — Architecture

Prompts:

- `ai/prompts/prototype-analyzer.md`
- `ai/prompts/architecture-designer.md`
- `ai/prompts/architecture-reviewer.md`
- `ai/prompts/architecture-reconciler.md`
- `ai/prompts/adr-generator.md`

Outputs:

- `architecture/architecture-blueprint.md`
- `architecture/review-report.md`
- `architecture/architecture-final.md`
- `architecture/adr/*.md`

## Phase 2 — Delivery & Specification

Prompts:

- `ai/prompts/delivery-planner.md`
- `ai/prompts/feature-spec-generator.md`
- `ai/prompts/golden-dataset-generator.md`

Outputs:

- `architecture/delivery-plan.md`
- `architecture/feature-specs/*.md`
- `architecture/golden-datasets/*.md`

## Phase 3 — Compliance & Execution

Prompt:

- `ai/prompts/architecture-compliance.md`

Skills:

- `skills/plan-decomposer/SKILL.md`
- `skills/part-executor-tdd/SKILL.md`

Agents:

- `ai/agents/orchestrator-agent.md`
- `ai/agents/backend-agent.md`
- `ai/agents/frontend-agent.md`
- `ai/agents/ai-agent.md`
- `ai/agents/qa-agent.md`
- `ai/agents/ai-testing-agent.md`
- `ai/agents/devops-agent.md`
- `ai/agents/integration-reviewer.md`

## Alternative Input Mode — Prototype + Existing Architecture Document

When both a prototype and an architecture document already exist, use this path.

### Phase 1 — Evidence and Document Review

Prompts:

- `ai/prompts/prototype-analyzer.md`
- `ai/prompts/existing-architecture-reviewer.md`
- `ai/prompts/prototype-architecture-alignment.md`

Outputs:

- `architecture/prototype-analysis.md`
- `architecture/existing-architecture-review.md`
- `architecture/prototype-architecture-alignment.md`

### Phase 2 — Reconciliation

Prompt:

- `ai/prompts/architecture-gap-reconciler.md`

Output:

- `architecture/architecture-final.md`

Then continue with:

- ADR generation
- delivery planning
- feature specs
- compliance
- decomposition
- execution
