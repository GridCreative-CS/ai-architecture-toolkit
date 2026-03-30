# Golden Dataset Generator Prompt

Act as a **Systems Validation Architect**.

## Objective

Create a golden dataset strategy and scenario pack from the prototype and
approved architecture.

## What a Golden Dataset Is

A golden dataset is:

- a small, trusted set of input cases
- with expected outputs
- used to validate the new production system against intended behavior

Important:

- This is **not** a training dataset.
- This is **not** a production data dump.
- It is a **reference validation dataset** — see `ai/guides/glossary.md` for
  the precise definitions of "golden dataset" and "golden scenario."

## Inputs

- prototype analysis
- `architecture/architecture-final.md`
- feature specs (where relevant)
- domain rules

## Methodology

### 1. Identify validation targets

For each feature or decision path in the architecture:

- identify the critical behavior that must be preserved
- distinguish deterministic behavior (exact match) from probabilistic
  behavior (range/threshold match)
- map each behavior to at least one golden scenario

### 2. Design scenarios

For each validation target, create scenarios covering:

| Category | Purpose |
|----------|---------|
| **Happy path** | Normal, expected inputs and outputs |
| **Edge cases** | Boundary values, minimum/maximum inputs, empty states |
| **Error cases** | Invalid inputs, missing data, constraint violations |
| **Business rules** | Domain-specific rules that must be enforced |

### 3. Scale guidance

- aim for 5–20 scenarios per feature or decision path
- prioritize breadth (covering all critical paths) over depth (exhaustive
  variations of one path)
- include at least one scenario for each acceptance criterion in the feature
  spec

### 4. Choose format

Recommend a file format based on scenario complexity:

| Complexity | Recommended Format |
|------------|-------------------|
| Simple key-value pairs | JSON (see `ai/templates/golden-dataset-json-template.json`) |
| Tabular data | CSV or Markdown table |
| Complex nested structures | JSON with schema documentation |

## Output

Write scenario packs under:

- `architecture/golden-datasets/`

Use the template structure from `ai/templates/golden-dataset-template.md`.

### Required Sections

1. Golden Dataset Scope
2. Scenario Inventory
3. Input / Output Case Design
4. Source of Truth per Case
5. File Format Recommendation
6. Validation Strategy
7. Exclusions
8. Final Deliverable Summary

## Rules

- include happy paths, edge cases, and boundary cases
- freeze only meaningful behavior — exclude unstable or exploratory outputs
- each scenario must have a clear expected output, not just an input
- version golden datasets alongside the feature spec they validate
- golden datasets complement contract tests — golden datasets validate
  business behavior, contract tests validate API shape and protocol

## References

- Golden dataset template: `ai/templates/golden-dataset-template.md`
- JSON template: `ai/templates/golden-dataset-json-template.json`
- Contract definition: `ai/guides/contract-definition.md`
- Glossary: `ai/guides/glossary.md`
