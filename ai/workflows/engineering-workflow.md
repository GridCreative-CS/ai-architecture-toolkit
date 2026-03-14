# Engineering Workflow — Feature Spec Aware

## Purpose

This workflow makes feature specifications a concrete input to decomposition and
implementation.

## Step 1 — Delivery Planning

Use:

- `ai/prompts/delivery-planner.md`

Write:

- `architecture/delivery-plan.md`

## Step 2 — Select the Next Slice

Choose the next implementation slice from the delivery plan.

The selected slice should be:

- meaningful
- bounded
- implementation-ready
- aligned with current priorities and dependencies

## Step 3 — Generate the Feature Spec for That Slice

Use:

- `ai/prompts/feature-spec-generator.md`
- `ai/templates/feature-spec-template.md`

Write:

- `architecture/feature-specs/<slice-name>.md`

## Step 4 — Run Architecture Compliance Check

If the slice is sensitive, cross-cutting, or high-risk, use:

- `ai/prompts/architecture-compliance.md`
- `ai/templates/compliance-report-template.md`

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`
- `architecture/feature-specs/<slice-name>.md`

Write:

- `architecture/compliance-reports/<slice-name>.md`

## Step 5 — Decompose the Slice

Use:

- `skills/plan-decomposer/SKILL.md`

Inputs:

- `architecture/delivery-plan.md`
- `architecture/feature-specs/<slice-name>.md`

If both exist, the feature spec should guide the decomposition for that slice
more precisely than the high-level delivery plan.

Write:

- `ai-parts/OVERVIEW.md`
- `ai-parts/PXX-*.md`

## Step 6 — Execute One Part at a Time

Use:

- `skills/part-executor-tdd/SKILL.md`

Input:

- one Part from `ai-parts/`

Execute exactly one Part at a time using strict TDD.

## Step 7 — Use Specialist Agents Where Helpful

Use specialist agents only after the slice is defined and decomposed.

Possible agents:

- backend
- frontend
- AI
- QA
- AI testing
- DevOps
- integration reviewer

## Step 8 — Repeat Per Slice

Repeat the sequence per slice:

```text
Delivery Plan
→ Select Slice
→ Feature Spec
→ Compliance Check
→ Decomposition
→ TDD Execution
→ Next Slice
```
