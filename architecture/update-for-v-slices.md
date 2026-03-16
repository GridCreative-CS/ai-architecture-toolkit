# Plan: Vertical Slice Discipline — Toolkit Improvements

## TL;DR

Your toolkit has **strong slice-aware infrastructure** — feature specs, decomposer, executor, agents, workflows are all well-designed. The failure is localized: the **delivery planner prompt** is the weak link. It says "produce Vertical Slices" but gives zero guidance on what makes a slice *truly* vertical. Everything downstream faithfully executed a flawed delivery plan. The fix is targeted: define verticality explicitly, enforce it at the point of generation, and add a validation checkpoint.

---

## Diagnosis: Where the Anti-Pattern Entered

I reviewed all 44 files across your 7 toolkit categories. Here's the root cause chain:

| Layer | File | Problem |
|---|---|---|
| **Root cause** | `ai/prompts/delivery-planner.md` | Says "produce Vertical Slices" as section 5, but **no definition, no test, no constraint** on what makes a slice vertical. An LLM can produce pipeline-stage slices + a separate frontend slice and call them "vertical." |
| Missed opportunity | `ai/templates/feature-spec-template.md` | Has "User / System Flows" (good!) but doesn't require "at minimum one human-facing workflow per slice" or test for UI presence. A backend-only feature spec passes silently. |
| Missed opportunity | `ai/guides/definition-of-ready-and-done.md` | Checks architecture compliance, acceptance criteria, testing — but **never asks: "Does this slice include a user-facing workflow?"** |
| Missed opportunity | `ai/prompts/architecture-compliance.md` | Reviews compliance with constraints but **doesn't test for verticality**. A slice can comply with all ADRs while being entirely horizontal. |
| Missed opportunity | `ai/workflows/engineering-workflow.md` | Correctly slice-based workflow but **no checkpoint validates the delivery plan's slice definitions** before proceeding. |
| Missed opportunity | `ai/agents/orchestrator-agent.md` | Lists "Frontend Tasks" per slice (good!) but **doesn't flag when Frontend Tasks is empty** for a slice that has user-facing consequences. |
| Tech mismatch | `ai/agents/frontend-agent.md` | Says "Senior **React** Frontend Engineer" — architecture uses **Blazor WebAssembly** (ADR-016). |

What works well and should **not** change:

- `.github/skills/plan-decomposer/SKILL.md` — feature-spec-aware, scope-bounded, already prevents decomposition drift
- `.github/skills/part-executor-tdd/SKILL.md` — refuses to execute if conflicts with feature spec, escalation rule built in
- `ai/guides/conversation-summary.md` — already says "prefer vertical slices" (but this was aspirational, not enforced)
- `ai/guides/how-feature-specs-are-used.md` — correctly slice-scoped workflow
- Both skills' "Feature Spec Awareness" sections — these are strong and well-designed

---

## Steps

### Phase 1 — Define what "vertical slice" means (new reference doc)

**New file: `ai/guides/vertical-slice-definition.md`**

Create a concise reference that:

- Defines a vertical slice as an **end-to-end capability** that proves a user/operator workflow through all necessary layers (data → backend → API → minimal UI)
- Defines what it is NOT: a pipeline stage, a backend service, a database layer, a frontend layer
- Provides a **verticality test** — 3 questions every slice must answer YES to:
  1. Does this slice deliver a capability a user/operator can exercise or observe?
  2. If the architecture specifies human-in-the-loop for this capability, does the slice include the minimal UI to prove that loop?
  3. Can this slice be called "done" with a user-facing verification, not just an integration test?
- Clarifies that infrastructure bootstrap and production hardening are **phases**, not slices
- Lists anti-patterns: frontend-as-a-slice, API-only slices for human-facing flows, backend-first-then-UI-later

This document becomes the single source of truth that other files reference.

---

### Phase 2 — Fix the delivery planner prompt (root cause fix)

**Modify: `ai/prompts/delivery-planner.md`**

This is where the strongest fix goes. Add:

- Reference to `ai/guides/vertical-slice-definition.md` as a **binding constraint**
- Explicit instruction: *"Each vertical slice MUST include the minimal frontend/human workflow required to prove the capability end-to-end. Do NOT separate frontend into its own slice."*
- A **verticality self-test**: after generating slices, apply the 3-question test to each. If any fails, restructure.
- Instruction to label infrastructure bootstrap and hardening as **"phases"** not slices
- Instruction: *"If the architecture specifies human-in-the-loop controls (approval, override, review, emergency), the slice delivering that capability MUST include the minimal UI surface."*
- Anti-pattern warning: *"Do NOT create a single 'Frontend' slice that bundles all UI surfaces. This converts all other slices into horizontal backend layers."*

---

### Phase 3 — Strengthen feature spec template and generator

**Modify: `ai/templates/feature-spec-template.md`**

Add section **5b. Human Workflow Surfaces** between "User / System Flows" and "Domain Rules":

- Which UI surfaces, operator flows, approval flows, or override flows does this slice include?
- If the slice is purely automated with no human interaction *and the architecture agrees*, state that explicitly with citation
- If the architecture specifies human-in-the-loop for this capability but the spec omits a UI surface, flag as a gap

**Modify: `ai/prompts/feature-spec-generator.md`**

Add instruction: *"Section 5b is mandatory. If the architecture defines human interaction for this capability and the spec omits UI, flag as a gap."*

---

### Phase 4 — Add delivery plan validation to workflow

**Modify: `ai/workflows/engineering-workflow.md`**

Add **Step 1b — Validate Delivery Plan Verticality** between Step 1 and Step 2:

- Before proceeding to slice selection, review slice definitions against `ai/guides/vertical-slice-definition.md`
- Apply the verticality test to each slice
- If any slice is a horizontal layer (all-frontend, all-backend without human workflow), restructure before proceeding

---

### Phase 5 — Strengthen Definition of Ready

**Modify: `ai/guides/definition-of-ready-and-done.md`**

Under "Delivery readiness", add:

- *"The slice passes the verticality test: it includes the minimal human workflow surface if the architecture specifies human interaction for this capability."*

Under Definition of Done → "Implementation completeness", add:

- *"If the slice includes a human workflow surface, a user-facing verification confirms the end-to-end capability, not just API/integration tests."*

---

### Phase 6 — Strengthen orchestrator and compliance agents

**Modify: `ai/agents/orchestrator-agent.md`**

- Add to Responsibilities: *"Verify that Frontend Tasks is not empty for slices where the architecture specifies human-facing workflows."*
- Add to Forbidden Actions: *"do not produce a slice plan where all Frontend Tasks are deferred to a separate slice"*

**Modify: `ai/prompts/architecture-compliance.md`**

Add output section **7. Verticality Assessment**: Does the slice include the human workflow required by the architecture? If the architecture requires human-in-the-loop and the slice omits UI, this is a compliance violation.

---

### Phase 7 — Fix frontend agent tech mismatch

**Modify: `ai/agents/frontend-agent.md`**

Change "Senior React Frontend Engineer" to match the project's technology. Either make framework-agnostic ("Senior Frontend Engineer") or project-specific. The architecture uses Blazor WebAssembly (ADR-016).

---

### Phase 8 — (Optional) Add example

**New file: `ai/examples/vertical-vs-horizontal-slices.md`**

Concrete side-by-side example showing:

- A BAD plan with frontend-as-a-slice
- The SAME plan restructured with true vertical slices
- Comparison of exit criteria (backend-only test vs user-facing verification)

---

## Relevant files

| File | Action | What |
|---|---|---|
| `ai/guides/vertical-slice-definition.md` | **CREATE** | Verticality reference doc with 3-question test and anti-patterns |
| `ai/prompts/delivery-planner.md` | **MODIFY** | Add verticality constraints, self-test, anti-pattern warnings (root cause fix) |
| `ai/templates/feature-spec-template.md` | **MODIFY** | Add "5b. Human Workflow Surfaces" section |
| `ai/prompts/feature-spec-generator.md` | **MODIFY** | Enforce Human Workflow Surfaces section |
| `ai/workflows/engineering-workflow.md` | **MODIFY** | Add Step 1b delivery plan verticality validation |
| `ai/guides/definition-of-ready-and-done.md` | **MODIFY** | Add verticality test to readiness + user-facing verification to done |
| `ai/agents/orchestrator-agent.md` | **MODIFY** | Add verticality gap detection + forbidden deferred-frontend pattern |
| `ai/prompts/architecture-compliance.md` | **MODIFY** | Add Verticality Assessment output section |
| `ai/agents/frontend-agent.md` | **MODIFY** | Fix React → Blazor/framework-agnostic |
| `ai/examples/vertical-vs-horizontal-slices.md` | **CREATE** (optional) | Concrete good/bad example |

---

## Decisions

- The **delivery planner prompt is the root cause** — strongest fix goes there
- Downstream mechanisms (decomposer, executor, agents) are already well-designed — they failed because they received a flawed plan; **fix the inputs, not the downstream tools**
- The `plan-decomposer` and `part-executor-tdd` skills need **no changes** — if feature specs correctly include Human Workflow Surfaces, decomposition will include frontend parts naturally
- `ai/guides/conversation-summary.md` already says "prefer vertical slices" — but this was **aspirational not enforced**; these changes operationalize it

---

## Further Considerations

1. **Should verticality validation be mandatory or optional?** Recommendation: **Mandatory** for the first delivery plan; optional for subsequent slice selections once the plan is validated.
2. **Dedicated delivery plan reviewer prompt?** The architecture-compliance prompt can serve this role with the new Verticality Assessment section. A separate prompt is optional but could be worthwhile for large plans.
