# Example — Feature Spec Driven Slice Flow

## Example context

You already have:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/delivery-plan.md`

The delivery plan identifies a slice:

- S3.1 — Recommendation Retrieval

## Step 1 — Generate the feature spec (engineering workflow Step 3)

Use:

- `ai/prompts/feature-spec-generator.md`

Write:

- `architecture/feature-specs/S3.1-recommendation-retrieval.md`

## Step 2 — Run compliance (engineering workflow Steps 4/4a)

Use:

- `ai/prompts/architecture-compliance.md`

Inputs:

- final architecture
- ADRs
- delivery plan
- feature spec

Write:

- `architecture/compliance-reports/S3.1-recommendation-retrieval.md`
- `architecture/compliance-reports/S3.1-recommendation-retrieval-ui.md`
  (because this slice has a human workflow surface — Step 4a is mandatory)

## Step 3 — Decompose (engineering workflow Step 5)

Use:

- `.github/skills/plan-decomposer/SKILL.md`

Inputs:

- `architecture/feature-specs/S3.1-recommendation-retrieval.md`
- `architecture/delivery-plan.md`

Write:

- `ai-parts/S3.1/OVERVIEW.md`
- `ai-parts/S3.1/P01-*.md` … `PXX-*.md`

## Step 4 — Execute (engineering workflow Step 6)

Use:

- `.github/skills/part-executor-tdd/SKILL.md`

Execute one generated Part at a time. Because the slice has a human workflow
surface, the final Part is a Terminal Verification Part, and Integrated Slice
Verification (Step 6b) writes its evidence to
`architecture/slice-verification/S3.1-recommendation-retrieval.md`.

## Result

The decomposition is now based on:

- the broad implementation order from the delivery plan
- the precise slice constraints from the feature spec
