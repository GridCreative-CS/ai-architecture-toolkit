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
→ Architecture Compliance (every slice — Step 4)
→ UI Compliance (UI slices — Step 4a)
→ Feature Spec Reconciliation (if findings — Step 4b)
→ Plan Decomposer (Step 5)
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

(Engineering workflow step numbers in parentheses — those are canonical.)

1. Generate the delivery plan (Step 1) and validate verticality (Step 1b).
2. Select the next slice (Step 2).
3. Generate one feature spec for that slice (Step 3).
4. Run architecture compliance — every slice (Step 4); UI compliance for UI
   slices (Step 4a).
5. Reconcile the feature spec if compliance findings require changes (Step 4b).
6. Decompose that slice into Parts (Step 5).
7. Execute Parts with TDD (Step 6), each ending in a Part Quality Report;
   review each Part before the next one starts (Step 6a); Integrated Slice
   Verification for UI slices (Step 6b).
8. Move to the next slice (Step 8).

## Source priority

For one slice, use these inputs in this practical order:

1. `architecture/feature-specs/<slice-id>-<slice-name>.md`
2. `architecture/delivery-plan.md`
3. `architecture/architecture-final.md`
4. `architecture/adr/*.md`

The feature spec does not replace architecture.
It refines one slice inside the approved architecture.
