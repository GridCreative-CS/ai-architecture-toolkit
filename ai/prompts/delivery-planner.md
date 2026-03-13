# Delivery Planner Prompt

Act as a **Technical Delivery Architect and Principal Engineer**.

Convert the final architecture and ADRs into a delivery plan optimized for decomposition.

Inputs:

- `architecture/architecture-final.md`
- `architecture/adr/*.md`

Objectives:

1. identify major subsystems
2. identify vertical slices
3. define milestones
4. define dependencies
5. identify high-risk areas
6. define an implementation order
7. produce a plan consumable by the plan-decomposer skill

Output sections:

1. System Overview
2. Architectural Constraints
3. Major Subsystems
4. Milestones
5. Vertical Slices
6. Dependency Map
7. High Risk Areas
8. Implementation Strategy
9. Ready for Decomposition
