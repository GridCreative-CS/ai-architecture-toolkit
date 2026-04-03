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
    /// Lists all project-specific artifacts with their existence status.
    /// </summary>
    [McpServerTool, Description("Lists all project-specific artifacts (architecture, ADRs, delivery plan, feature specs, compliance reports, design system, project context) and whether each exists in the current workspace.")]
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
    /// and any existing project artifacts for that step.
    /// </summary>
    [McpServerTool, Description("Given a workflow step name, returns all the relevant toolkit files (prompt, template, guide) and existing project artifacts needed for that step. This is the key context-assembly tool — it gives you everything you need for a workflow step in one call.")]
    public static string GetWorkflowContext(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Workflow step: 'delivery-planning', 'feature-spec', 'compliance-check', 'feature-spec-reconciliation', 'decomposition', 'architecture-review', 'adr-generation', 'ui-compliance', 'architecture-design', 'architecture-blueprint-review', 'architecture-reconciliation', 'existing-architecture-review', 'architecture-gap-reconciliation', 'prototype-analysis', 'prototype-architecture-alignment'")] string step)
    {
        var sb = new StringBuilder();

        var (prompt, template, guide, projectArtifacts) = step.ToLowerInvariant() switch
        {
            "delivery-planning" => (
                "delivery-planner",
                (string?)null,
                "vertical-slice-definition",
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Design System", projectService.GetDesignSystem()) }
            ),
            "feature-spec" => (
                "feature-spec-generator",
                (string?)"feature-spec-template",
                "how-feature-specs-are-used",
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Delivery Plan", projectService.GetDeliveryPlan()), ("Design System", projectService.GetDesignSystem()) }
            ),
            "compliance-check" => (
                "architecture-compliance",
                (string?)"compliance-report-template",
                "contract-definition",
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()), ("Delivery Plan", projectService.GetDeliveryPlan()) }
            ),
            "feature-spec-reconciliation" => (
                "feature-spec-reconciler",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("ADRs", projectService.GetAllAdrs()) }
            ),
            "decomposition" => (
                (string?)null,
                (string?)null,
                "glossary",
                new[] { ("Delivery Plan", projectService.GetDeliveryPlan()) }
            ),
            "architecture-review" => (
                "existing-architecture-reviewer",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()), ("Project Context", projectService.GetProjectContext()) }
            ),
            "adr-generation" => (
                "adr-generator",
                (string?)"adr-template",
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture()) }
            ),
            "ui-compliance" => (
                "ui-compliance-check",
                (string?)null,
                (string?)null,
                new[] { ("Design System", projectService.GetDesignSystem()) }
            ),
            "architecture-design" => (
                "architecture-designer",
                (string?)"architecture-blueprint-template",
                "modular-monolith-definition",
                new[] { ("Prototype Analysis", projectService.GetPrototypeAnalysis()), ("Project Context", projectService.GetProjectContext()) }
            ),
            "architecture-blueprint-review" => (
                "architecture-reviewer",
                (string?)null,
                (string?)null,
                new[] { ("Architecture Blueprint", projectService.GetArchitectureBlueprint()) }
            ),
            "architecture-reconciliation" => (
                "architecture-reconciler",
                (string?)"architecture-blueprint-template",
                (string?)null,
                new[] { ("Architecture Blueprint", projectService.GetArchitectureBlueprint()), ("Review Report", projectService.GetReviewReport()) }
            ),
            "existing-architecture-review" => (
                "existing-architecture-reviewer",
                (string?)null,
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture() ?? projectService.GetArchitectureBlueprint()) }
            ),
            "architecture-gap-reconciliation" => (
                "architecture-gap-reconciler",
                (string?)"architecture-blueprint-template",
                (string?)null,
                new[] { ("Architecture", projectService.GetArchitecture() ?? projectService.GetArchitectureBlueprint()), ("Review", projectService.GetExistingArchitectureReview() ?? projectService.GetReviewReport()) }
            ),
            "prototype-analysis" => (
                "prototype-analyzer",
                (string?)null,
                (string?)null,
                new[] { ("Project Context", projectService.GetProjectContext()) }
            ),
            "prototype-architecture-alignment" => (
                "prototype-architecture-alignment",
                (string?)null,
                (string?)null,
                new[] { ("Prototype Analysis", projectService.GetPrototypeAnalysis()), ("Architecture", projectService.GetArchitecture() ?? projectService.GetArchitectureBlueprint()) }
            ),
            _ => (
                (string?)null,
                (string?)null,
                (string?)null,
                Array.Empty<(string, string?)>()
            )
        };

        if (prompt is null && template is null && guide is null)
        {
            return $"Unknown workflow step: '{step}'. Valid steps: delivery-planning, feature-spec, compliance-check, feature-spec-reconciliation, decomposition, architecture-review, adr-generation, ui-compliance, architecture-design, architecture-blueprint-review, architecture-reconciliation, existing-architecture-review, architecture-gap-reconciliation, prototype-analysis, prototype-architecture-alignment";
        }

        if (prompt is not null)
        {
            sb.AppendLine("# Prompt");
            sb.AppendLine();
            sb.AppendLine(toolkitService.GetContent("prompts", prompt) ?? $"(prompt '{prompt}' not found)");
            sb.AppendLine();
        }

        if (template is not null)
        {
            sb.AppendLine("---");
            sb.AppendLine("# Template");
            sb.AppendLine();
            sb.AppendLine(toolkitService.GetContent("templates", template) ?? $"(template '{template}' not found)");
            sb.AppendLine();
        }

        if (guide is not null)
        {
            sb.AppendLine("---");
            sb.AppendLine("# Guide");
            sb.AppendLine();
            sb.AppendLine(toolkitService.GetContent("guides", guide) ?? $"(guide '{guide}' not found)");
            sb.AppendLine();
        }

        foreach (var (label, content) in projectArtifacts)
        {
            if (content is not null)
            {
                sb.AppendLine("---");
                sb.AppendLine($"# Project: {label}");
                sb.AppendLine();
                sb.AppendLine(content);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Given a slice name, returns all context for that slice.
    /// </summary>
    [McpServerTool, Description("Given a slice name, returns the feature spec, compliance report, relevant ADRs, and delivery plan entry for that slice. Provides complete context for working on a specific slice.")]
    public static string GetSliceContext(
        ProjectContentService projectService,
        [Description("The slice name (matches the feature spec filename without .md extension)")] string sliceName)
    {
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
            sb.AppendLine("---");
            sb.AppendLine("## Compliance Report");
            sb.AppendLine();
            sb.AppendLine(complianceReport);
            sb.AppendLine();
        }

        var deliveryPlan = projectService.GetDeliveryPlan();
        if (deliveryPlan is not null)
        {
            sb.AppendLine("---");
            sb.AppendLine("## Delivery Plan");
            sb.AppendLine();
            sb.AppendLine(deliveryPlan);
            sb.AppendLine();
        }

        var adrs = projectService.GetAllAdrs();
        if (adrs is not null)
        {
            sb.AppendLine("---");
            sb.AppendLine("## ADRs");
            sb.AppendLine();
            sb.AppendLine(adrs);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
