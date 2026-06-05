# Delivery Planner Prompt

Act as a **Technical Delivery Architect and Principal Engineer**.

Convert the final architecture and ADRs into a structured delivery plan.

## Inputs

- `architecture/architecture-final.md`
- `architecture/adr/*.md`
- `architecture/design-system.md` (when present — slices with UI surfaces
  should reference design system tokens and components in their scope)

Consult `ai/guides/glossary.md` for precise definitions of key terms,
especially: slice, milestone, phase, decomposition-ready.

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

### Browser Verification Approach

For each slice with human workflow surfaces, the delivery plan should indicate
the expected browser verification approach:

- **Automated E2E tests** (Playwright, Cypress) — preferred for critical flows
- **Documented browser walkthrough** — acceptable when automation is not yet
  available
- **Both** — recommended for high-risk or high-visibility slices

This information feeds into the feature spec (§12b Browser Verification Steps)
and the Integrated Slice Verification step (engineering workflow Step 6b).

## Milestone Sizing

- aim for 2–5 slices per milestone
- each milestone should represent a demonstrable progress point (stakeholder
  checkpoint or release boundary)
- if a milestone has more than 5 slices, consider splitting it
- if a milestone has only 1 slice, consider merging it with an adjacent
  milestone unless the slice is large enough to justify its own checkpoint

## Risk Prioritization

For each risk in the High Risk Areas section:

| Impact | Action |
|--------|--------|
| **Blocks deployment** | Must be resolved in the milestone that introduces the dependency |
| **Degrades quality** | Should be resolved before the milestone is marked complete |
| **Future concern** | Document and track; no immediate action required |

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
9. Ready for Decomposition — list which slices are decomposition-ready (see
   `ai/guides/glossary.md`: scope bounded, acceptance criteria binary,
   target files known, verification strategy defined)

## References

- Vertical slice definition: `ai/guides/vertical-slice-definition.md`
- Glossary: `ai/guides/glossary.md`
