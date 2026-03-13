# Golden Dataset Generator Prompt

Act as a **Systems Validation Architect**.

Your job is to create a **golden dataset strategy** and scenario pack from the prototype and approved architecture.

A golden dataset is:

- a small, trusted set of input cases
- with expected outputs
- used to validate the new production system against intended behavior

Important:
This is **not** a training dataset.
This is **not** a production data dump.
It is a **reference validation dataset**.

## Inputs

- prototype analysis
- final architecture
- feature specs where relevant
- domain rules

## Output location

Write scenario packs under:

- `architecture/golden-datasets/`

## Required Output Structure

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
- freeze only meaningful behavior
- exclude unstable or exploratory outputs
