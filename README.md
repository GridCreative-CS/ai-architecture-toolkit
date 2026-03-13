# AI Architecture Toolkit — v2

This is the stronger v2 toolkit for a **prototype → architecture → delivery → decomposition → TDD execution** workflow.

It extends the complete toolkit with:

- `delivery-planner`
- `architecture-compliance`
- `feature-spec-generator`
- `golden-dataset-generator`
- `ai-testing-agent`

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

## What is new in v2

### Delivery Planner
Bridges architecture and decomposition by turning the final architecture and ADRs into a delivery plan.

### Architecture Compliance
Checks whether implementation plans, parts, or code changes still respect the approved architecture and ADRs.

### Feature Spec Generator
Creates implementation-ready feature specifications with acceptance criteria, constraints, API expectations, and test implications.

### Golden Dataset Generator
Defines small, trusted reference scenarios with fixed inputs and expected outputs for validating the new production system against the intended prototype behavior.

### AI Testing Agent
Acts as a testing-focused specialist agent for validation strategy, regression protection, golden scenarios, and cross-slice verification.

## Existing implementation skills included

- `skills/plan-decomposer/SKILL.md`
- `skills/part-executor-tdd/SKILL.md`
- `agents/expert-dotnet-software-engineer.agent.md`
