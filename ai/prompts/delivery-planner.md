# Delivery Planner Prompt

Act as a **Technical Delivery Architect and Principal Engineer**.

Convert the final architecture and ADRs into a structured delivery plan.

## Inputs

- `architecture/architecture-final.md`
- `architecture/adr/*.md`

## Binding Constraints

All slices must conform to the vertical slice definition in
`ai/guides/vertical-slice-definition.md`.

### Verticality Rules

- Each vertical slice MUST include the minimal frontend or human workflow
  required to prove the capability end-to-end. Do NOT separate frontend into
  its own slice.
- If the architecture specifies human-in-the-loop controls (approval, override,
  review, emergency), the slice delivering that capability MUST include the
  minimal UI surface.
- Infrastructure bootstrap and production hardening are **phases**, not slices.
  Label them accordingly.

### Anti-Pattern Warning

Do NOT create a single "Frontend" slice that bundles all UI surfaces. This
converts all other slices into horizontal backend layers.

### Verticality Self-Test

After generating slices, apply the following test to each slice. If any slice
fails any question, restructure before finalizing the plan.

1. Does this slice deliver a capability a user/operator can exercise or observe?
2. If the architecture specifies human-in-the-loop for this capability, does the
   slice include the minimal UI to prove that loop?
3. Can this slice be called "done" with a user-facing verification, not just an
   integration test?

## Output

Write to `architecture/delivery-plan.md` with:

1. System Overview
2. Architectural Constraints
3. Major Subsystems
4. Milestones
5. Vertical Slices
6. Dependency Map
7. High Risk Areas
8. Implementation Strategy
9. Ready for Decomposition
