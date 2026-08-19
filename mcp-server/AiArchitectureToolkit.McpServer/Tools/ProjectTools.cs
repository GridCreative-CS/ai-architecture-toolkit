using System.ComponentModel;
using System.Text;
using System.Text.Json;
using AiArchitectureToolkit.McpServer.Services;
using ModelContextProtocol.Server;

namespace AiArchitectureToolkit.McpServer.Tools;

/// <summary>
/// MCP tools for project-aware operations.
/// </summary>
[McpServerToolType]
public sealed class ProjectTools
{
    /// <summary>
    /// The workflow steps <see cref="GetWorkflowContext"/> understands, in
    /// workflow order. Shared by the tool description and the unknown-step
    /// error so the two can never drift apart.
    /// </summary>
    private const string ValidSteps =
        "delivery-planning, feature-spec, golden-dataset, compliance-check, ui-compliance, " +
        "feature-spec-reconciliation, decomposition, part-code-review, slice-verification, " +
        "slice-preparation, ui-foundation, ui-inventory, design-system-from-inventory, " +
        "ui-remediation, architecture-design, architecture-blueprint-review, " +
        "architecture-reconciliation, architecture-final-gate, adr-generation, architecture-review, " +
        "existing-architecture-review, architecture-gap-reconciliation, prototype-analysis, " +
        "prototype-architecture-alignment, legacy-system-analysis";

    /// <summary>
    /// Lists all project-specific artifacts with their existence status.
    /// </summary>
    [McpServerTool, Description("Lists all project-specific artifacts (architecture, architecture-final gate report, ADRs, delivery plan, feature specs, architecture and UI compliance reports, golden datasets, slice verification evidence, ai-parts decomposition slices, design system, UI inventory, project context) and whether each exists in the current workspace.")]
    public static string ListProjectArtifacts(ProjectContentService projectService)
    {
        var artifacts = projectService.ListArtifacts();
        return JsonSerializer.Serialize(artifacts, JsonOptions.Default);
    }

    /// <summary>
    /// Returns the workspace structure: solutions, projects, and dependency graph.
    /// </summary>
    [McpServerTool, Description("Scans the workspace for .NET solutions, projects, and their dependency graph. Returns a structured overview of the codebase organization.")]
    public static string GetWorkspaceStructure(WorkspaceScanService workspaceService)
    {
        var summary = workspaceService.Scan();
        return summary.ToDisplayString();
    }

    /// <summary>
    /// Given a workflow step, returns the relevant prompt, template, guide,
    /// skill, and any existing project artifacts for that step.
    /// </summary>
    [McpServerTool, Description("Given a workflow step name, returns all the relevant toolkit files (prompt, template, guide, skill) and existing project artifacts needed for that step. This is the key context-assembly tool — it gives you everything you need for a workflow step in one call. Valid steps: " + ValidSteps)]
    public static string GetWorkflowContext(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Workflow step. Valid steps: " + ValidSteps)] string step,
        [Description("Optional slice name (the feature spec file name without .md). Used by the slice-scoped steps: golden-dataset, part-code-review, slice-verification, compliance-check, ui-compliance.")] string? sliceName = null)
    {
        ArgumentNullException.ThrowIfNull(toolkitService);
        ArgumentNullException.ThrowIfNull(projectService);
        ArgumentNullException.ThrowIfNull(step);

        var sb = new StringBuilder();

        var featureSpec = sliceName is null
            ? null
            : projectService.GetFeatureSpec(sliceName) ?? $"(no feature spec found for '{sliceName}')";

        var (prompt, template, guide, skill, projectArtifacts) = step.ToLowerInvariant() switch
        {
            "delivery-planning" => (
                "delivery-planner",
                (string?)null,
                "vertical-slice-definition",
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Design System", projectService.GetDesignSystem()) }
            ),
            "feature-spec" => (
                "feature-spec-generator",
                (string?)"feature-spec-template",
                "how-feature-specs-are-used",
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Delivery Plan", projectService.GetDeliveryPlan()), ("Design System", projectService.GetDesignSystem()) }
            ),
            // Engineering workflow Step 3b.
            "golden-dataset" => (
                "golden-dataset-generator",
                (string?)"golden-dataset-template",
                (string?)null,
                (string?)null,
                new[] { ("Feature Spec", featureSpec), ("Architecture", projectService.GetArchitecture()) }
            ),
            "compliance-check" => (
                "architecture-compliance",
                (string?)"compliance-report-template",
                "contract-definition",
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Delivery Plan", projectService.GetDeliveryPlan()), ("Feature Spec", featureSpec) }
            ),
            "feature-spec-reconciliation" => (
                "feature-spec-reconciler",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()) }
            ),
            // Engineering workflow Step 5 — the plan-decomposer skill defines the
            // Part handoff contract this step produces.
            "decomposition" => (
                (string?)null,
                (string?)null,
                "glossary",
                (string?)"plan-decomposer",
                new[] { ("Delivery Plan", projectService.GetDeliveryPlan()), ("Feature Spec", featureSpec) }
            ),
            // Engineering workflow Step 6a.
            "part-code-review" => (
                "code-quality-reviewer",
                (string?)"code-quality-checklist-template",
                "code-quality-standard",
                (string?)null,
                new[] { ("Feature Spec", featureSpec), ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()) }
            ),
            "architecture-review" => (
                "existing-architecture-reviewer",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("Project Context", projectService.GetProjectContext()) }
            ),
            "adr-generation" => (
                "adr-generator",
                (string?)"adr-template",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("Architecture-Final Gate Report", projectService.GetArchitectureFinalGate()) }
            ),
            "ui-compliance" => (
                "ui-compliance-check",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Design System", projectService.GetDesignSystem()), ("Feature Spec", featureSpec) }
            ),
            // UI foundation workflow Step 1 (greenfield) — engineering
            // workflow Step 0b. Produces architecture/design-system.md.
            "ui-foundation" => (
                "design-system-generator",
                (string?)"design-system-template",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Project Context", projectService.GetProjectContext()), ("Existing Design System", projectService.GetDesignSystem()) }
            ),
            // UI retrofit workflow Step 1 — produces architecture/ui-inventory.md.
            "ui-inventory" => (
                "ui-inventory",
                (string?)"ui-inventory-template",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("Project Context", projectService.GetProjectContext()), ("Existing UI Inventory", projectService.GetUiInventory()) }
            ),
            // UI retrofit workflow Step 2 — derives the design system from the
            // inventory produced by the ui-inventory step.
            "design-system-from-inventory" => (
                "design-system-from-inventory",
                (string?)"design-system-template",
                (string?)null,
                (string?)null,
                new[] { ("UI Inventory", projectService.GetUiInventory()), ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Project Context", projectService.GetProjectContext()) }
            ),
            "ui-remediation" => (
                "ui-compliance-check",
                (string?)"remediation-spec-template",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("Design System", projectService.GetDesignSystem()), ("Delivery Plan", projectService.GetDeliveryPlan()), ("Remediation Audit", projectService.GetRemediationAudit()) }
            ),
            // Engineering workflow Step 6b — reports whether evidence already
            // exists for the requested slice.
            "slice-verification" => (
                (string?)null,
                (string?)"slice-verification-checklist-template",
                (string?)null,
                (string?)null,
                new[]
                {
                    ("Architecture", projectService.GetArchitecture()),
                    ("Design System", projectService.GetDesignSystem()),
                    ("Delivery Plan", projectService.GetDeliveryPlan()),
                    ("Feature Spec", featureSpec),
                    ("Slice Verification Evidence", SliceVerificationEvidence(projectService, sliceName))
                }
            ),
            // Engineering workflow Steps 2–5 in one agent run.
            "slice-preparation" => (
                "slice-preparation-runner",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Delivery Plan", projectService.GetDeliveryPlan()), ("Design System", projectService.GetDesignSystem()) }
            ),
            "architecture-design" => (
                "architecture-designer",
                (string?)"architecture-blueprint-template",
                "modular-monolith-definition",
                (string?)null,
                new[] { ("Prototype Analysis", projectService.GetPrototypeAnalysis()), ("Project Context", projectService.GetProjectContext()) }
            ),
            "architecture-blueprint-review" => (
                "architecture-reviewer",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Architecture Blueprint", projectService.GetArchitectureBlueprint()) }
            ),
            "architecture-reconciliation" => (
                "architecture-reconciler",
                (string?)"architecture-blueprint-template",
                (string?)null,
                (string?)null,
                new[] { ("Architecture Blueprint", projectService.GetArchitectureBlueprint()), ("Review Report", projectService.GetReviewReport()), ("Prior Gate Report", projectService.GetArchitectureFinalGate()) }
            ),
            // Toolkit v4.5.0 — runs between reconciliation and ADR generation.
            "architecture-final-gate" => (
                "architecture-final-quality-gate",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("Project Context", projectService.GetProjectContext()) }
            ),
            "existing-architecture-review" => (
                "existing-architecture-reviewer",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture() ?? projectService.GetArchitectureBlueprint()) }
            ),
            "architecture-gap-reconciliation" => (
                "architecture-gap-reconciler",
                (string?)"architecture-blueprint-template",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture() ?? projectService.GetArchitectureBlueprint()), ("Review", projectService.GetExistingArchitectureReview() ?? projectService.GetReviewReport()) }
            ),
            "prototype-analysis" => (
                "prototype-analyzer",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Project Context", projectService.GetProjectContext()) }
            ),
            "prototype-architecture-alignment" => (
                "prototype-architecture-alignment",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Prototype Analysis", projectService.GetPrototypeAnalysis()), ("Architecture", projectService.GetArchitecture() ?? projectService.GetArchitectureBlueprint()) }
            ),
            // Architecture Mode D.
            "legacy-system-analysis" => (
                "legacy-system-analyzer",
                (string?)null,
                (string?)null,
                (string?)null,
                new[] { ("Project Context", projectService.GetProjectContext()), ("Legacy System Analysis", projectService.GetLegacySystemAnalysis()) }
            ),
            _ => (
                (string?)null,
                (string?)null,
                (string?)null,
                (string?)null,
                Array.Empty<(string, string?)>()
            )
        };

        if (prompt is null && template is null && guide is null && skill is null)
        {
            return $"Unknown workflow step: '{step}'. Valid steps: {ValidSteps}";
        }

        if (prompt is not null)
        {
            AppendSection(sb, "# Prompt", toolkitService.GetContent("prompts", prompt) ?? $"(prompt '{prompt}' not found)", separator: false);
        }

        if (template is not null)
        {
            AppendSection(sb, "# Template", toolkitService.GetContent("templates", template) ?? $"(template '{template}' not found)");
        }

        if (guide is not null)
        {
            AppendSection(sb, "# Guide", toolkitService.GetContent("guides", guide) ?? $"(guide '{guide}' not found)");
        }

        if (skill is not null)
        {
            AppendSection(sb, "# Skill", toolkitService.GetContent("skills", skill) ?? $"(skill '{skill}' not found)");
        }

        foreach (var (label, content) in projectArtifacts)
        {
            if (content is not null)
            {
                AppendSection(sb, $"# Project: {label}", content);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Given a slice name, returns all context for that slice.
    /// </summary>
    [McpServerTool, Description("Given a slice name, returns the feature spec and its criterion IDs, both compliance reports (architecture and UI), the decomposition status from ai-parts (Part statuses and Step 6a review verdicts), slice verification evidence, relevant ADRs, and the delivery plan. Provides complete context for working on a specific slice.")]
    public static string GetSliceContext(
        ProjectContentService projectService,
        [Description("The slice name (matches the feature spec filename without .md extension)")] string sliceName)
    {
        ArgumentNullException.ThrowIfNull(projectService);
        ArgumentNullException.ThrowIfNull(sliceName);

        var sb = new StringBuilder();
        sb.AppendLine($"# Slice Context: {sliceName}");
        sb.AppendLine();

        var featureSpec = projectService.GetFeatureSpec(sliceName);
        if (featureSpec is not null)
        {
            sb.AppendLine("## Feature Spec");
            sb.AppendLine();
            sb.AppendLine(featureSpec);
            sb.AppendLine();

            AppendCriterionIds(sb, featureSpec);
        }
        else
        {
            sb.AppendLine("## Feature Spec");
            sb.AppendLine();
            sb.AppendLine($"(no feature spec found for '{sliceName}')");
            sb.AppendLine();
        }

        var complianceReport = projectService.GetComplianceReport(sliceName);
        if (complianceReport is not null)
        {
            AppendSection(sb, "## Compliance Report (Step 4 — architecture)", complianceReport);
        }

        var uiComplianceReport = projectService.GetUiComplianceReport(sliceName);
        if (uiComplianceReport is not null)
        {
            AppendSection(sb, "## Compliance Report (Step 4a — UI)", uiComplianceReport);
        }

        AppendDecomposition(sb, projectService, sliceName);

        var verification = projectService.GetSliceVerification(sliceName);
        if (verification is not null)
        {
            AppendSection(sb, "## Slice Verification Evidence (Step 6b)", verification);
        }

        var deliveryPlan = projectService.GetDeliveryPlan();
        if (deliveryPlan is not null)
        {
            AppendSection(sb, "## Delivery Plan", deliveryPlan);
        }

        var adrs = projectService.GetAllAdrs();
        if (adrs is not null)
        {
            AppendSection(sb, "## ADRs", adrs);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Lists the criterion IDs the feature spec defines, so a client can
    /// cross-check them against the decomposition's Requirement Coverage Map.
    /// </summary>
    private static void AppendCriterionIds(StringBuilder sb, string featureSpec)
    {
        var criterionIds = ProjectContentService.ExtractCriterionIds(featureSpec);

        sb.AppendLine("---");
        sb.AppendLine("## Feature Spec Criterion IDs");
        sb.AppendLine();

        if (criterionIds.Count == 0)
        {
            sb.AppendLine(
                "(no DR-nn / SEC-nn / AC-nn / UIAC-nn criterion IDs found — this spec predates toolkit " +
                "v4.6.0; IDs are assigned at its next reconciliation.)");
        }
        else
        {
            sb.AppendLine("Cross-check these against the Requirement Coverage Map in `ai-parts/<slice-id>/OVERVIEW.md`:");
            sb.AppendLine();
            foreach (var id in criterionIds)
            {
                sb.AppendLine($"- {id}");
            }
        }

        sb.AppendLine();
    }

    /// <summary>
    /// Summarises the slice's decomposition: Part statuses, review verdicts,
    /// and the warnings that decide whether the next Part may start.
    /// </summary>
    private static void AppendDecomposition(StringBuilder sb, ProjectContentService projectService, string sliceName)
    {
        var decomposition = projectService.GetDecomposition(sliceName);
        if (decomposition is null)
        {
            return;
        }

        sb.AppendLine("---");
        DecompositionReport.Append(sb, decomposition, "## Decomposition (ai-parts)");
    }

    private static string? SliceVerificationEvidence(ProjectContentService projectService, string? sliceName)
    {
        if (sliceName is null)
        {
            return null;
        }

        return projectService.GetSliceVerification(sliceName)
            ?? $"(no slice verification evidence found for '{sliceName}' at " +
               $"architecture/slice-verification/{sliceName}.md — Step 6b has not been recorded)";
    }

    private static void AppendSection(StringBuilder sb, string heading, string content, bool separator = true)
    {
        if (separator)
        {
            sb.AppendLine("---");
        }

        sb.AppendLine(heading);
        sb.AppendLine();
        sb.AppendLine(content);
        sb.AppendLine();
    }
}
