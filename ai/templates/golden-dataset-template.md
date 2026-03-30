# Golden Dataset

<!-- A golden dataset is a small, trusted set of (input, expected output)      -->
<!-- pairs used to validate that the production system preserves intended      -->
<!-- behavior. It is NOT a training dataset and NOT a production data dump.    -->
<!-- Reference: ai/guides/glossary.md for definitions.                         -->

## 1. Golden Dataset Scope

<!-- Which features, slices, or decision paths does this dataset cover?       -->
<!-- What behavior is being validated?                                         -->

## 2. Scenario Inventory

<!-- List all scenarios by ID. For each: name, category (happy path / edge    -->
<!-- case / error / business rule), and the feature or acceptance criterion    -->
<!-- it validates. Aim for 5–20 scenarios per feature or decision path.        -->

| Scenario ID | Name | Category | Validates |
|-------------|------|----------|-----------|
| GD-001 | (name) | Happy path | (acceptance criterion or rule) |
| GD-002 | (name) | Edge case | (acceptance criterion or rule) |

## 3. Input / Output Case Design

<!-- For each scenario, define the input and expected output. Distinguish:     -->
<!-- - Deterministic: exact match expected                                     -->
<!-- - Probabilistic: confidence bounds or threshold match expected            -->
<!-- Use the JSON template (ai/templates/golden-dataset-json-template.json)   -->
<!-- for structured cases.                                                     -->

## 4. Source of Truth

<!-- For each scenario, document where the expected output comes from:         -->
<!-- prototype run, domain expert, approved business rule, or architecture.    -->

## 5. File Format Recommendation

<!-- Recommend a format based on complexity:                                   -->
<!-- - Simple key-value: JSON                                                  -->
<!-- - Tabular: CSV or Markdown table                                          -->
<!-- - Complex nested: JSON with schema documentation                          -->

## 6. Validation Strategy

<!-- How will these scenarios be executed? (CI test, manual review, both)      -->
<!-- How will failures be triaged? (auto-fail, threshold, human review)        -->

## 7. Exclusions

<!-- What is explicitly NOT validated by this dataset? Why?                    -->
<!-- Examples: unstable prototype behavior, exploratory outputs, non-          -->
<!-- deterministic model drift.                                                -->

## 8. Final Deliverable Summary

<!-- List the files produced, their locations under                            -->
<!-- architecture/golden-datasets/, and the total scenario count.              -->
