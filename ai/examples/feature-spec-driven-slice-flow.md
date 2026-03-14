# Example — Feature Spec Driven Slice Flow

## Example context

You already have:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`

The delivery plan identifies a slice:

- Recommendation Retrieval

## Step 1 — Generate the feature spec

Use:

- `ai/prompts/feature-spec-generator.md`

Write:

- `architecture/feature-specs/recommendation-retrieval.md`

## Step 2 — Run compliance if needed

Use:

- `ai/prompts/architecture-compliance.md`

Inputs:

- final architecture
- ADRs
- delivery plan
- feature spec

## Step 3 — Decompose

Use:

- `skills/plan-decomposer/SKILL.md`

Inputs:

- `architecture/delivery-plan.md`
- `architecture/feature-specs/recommendation-retrieval.md`

## Step 4 — Execute

Use:

- `skills/part-executor-tdd/SKILL.md`

Execute one generated Part at a time.

## Result

The decomposition is now based on:

- the broad implementation order from the delivery plan
- the precise slice constraints from the feature spec
