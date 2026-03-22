# How Feature Specs Are Used

## Why feature specs exist

A delivery plan is intentionally high-level.
It breaks the system into milestones, subsystems, and slices.

A feature spec exists to make one selected slice precise before decomposition.

It clarifies:

- scope in
- scope out
- flows
- domain rules
- API expectations
- data requirements
- security constraints
- observability requirements
- acceptance criteria
- test implications

## Correct place in the workflow

The correct place for a feature spec is:

```text
Delivery Plan
→ Feature Spec
→ Architecture Compliance (if needed)
→ Feature Spec Reconciliation (if findings)
→ Plan Decomposer
```

Not:

```text
Delivery Plan
→ Plan Decomposer
```

unless no feature spec exists yet.

## Practical rule

If a feature spec exists for a slice, use it as a primary input to decomposition
for that slice.

## Recommended per-slice operating model

1. Generate the delivery plan.
2. Select the next slice.
3. Generate one feature spec for that slice.
4. Optionally run architecture compliance.
5. Reconcile the feature spec if compliance findings require changes.
6. Decompose that slice into Parts.
7. Execute Parts with TDD.
8. Move to the next slice.

## Source priority

For one slice, use these inputs in this practical order:

1. `architecture/feature-specs/<slice-name>.md`
2. `architecture/delivery-plan.md`
3. `architecture/architecture-final.md`
4. `architecture/adr/*.md`

The feature spec does not replace architecture.
It refines one slice inside the approved architecture.
