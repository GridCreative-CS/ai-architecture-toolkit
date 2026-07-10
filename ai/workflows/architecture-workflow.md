# Architecture Workflow — Mode Selector

## Purpose

This is the entry point for the architecture phase. It selects the correct
workflow variant for your inputs and defines the finalization gate that all
variants must reach before delivery planning starts.

## Step 0 — Select the entry mode

Choose by the strongest available input (details:
`ai/guides/how-to-choose-entry-mode.md`):

| Mode | You have | Follow |
| --- | --- | --- |
| **A — Prototype Only** | A prototype, no useful architecture doc | `ai/workflows/architecture-workflow-prototype-only.md` |
| **B — Prototype + Architecture Doc** | Both a prototype and an architecture doc | `ai/workflows/architecture-workflow-prototype-plus-architecture-doc.md` |
| **C — Architecture Doc Only** | An architecture doc, no prototype | `ai/workflows/architecture-workflow-architecture-doc-only.md` |
| **D — Legacy System Replacement** | A legacy system to replace, not repair | `ai/workflows/architecture-workflow-legacy-system-replacement.md` |

If the mode is ambiguous (e.g., the "prototype" is a production legacy system,
or the architecture doc is untrustworthy), ask the user which mode to use and
recommend one: untrusted doc + prototype → Mode B (validate the doc);
legacy system as the only input → Mode D.

## Missing inputs

- If `ai/project-context.md` is missing or still the unfilled stub, create it
  from `ai/templates/project-context-template.md` and ask the user to fill it
  (or fill it from explicit user statements) **before** running any analysis
  prompt.
- If the input the mode requires (prototype, architecture doc, legacy system)
  is not accessible in the workspace, stop and ask where it is. Do not
  substitute assumptions for the missing input.

## Finalization gate (all modes)

A mode is complete only when all of the following exist as real content (not
placeholders):

- `architecture/architecture-final.md` — the authoritative architecture
- `architecture/adr/*.md` — one ADR per major decision

From this point, rule 1 applies: these files are authoritative for all
downstream work.

## After the gate

1. **UI-inclusive projects:** run `ai/workflows/ui-foundation-workflow.md`
   (greenfield) or `ai/workflows/ui-retrofit-workflow.md` (retrofit) to
   produce `architecture/design-system.md` before delivery planning.
2. **All projects:** proceed to `ai/workflows/engineering-workflow.md`,
   starting at Step 1 (delivery planning).
