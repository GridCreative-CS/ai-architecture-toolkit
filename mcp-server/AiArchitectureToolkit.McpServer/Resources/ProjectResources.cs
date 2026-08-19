using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Server;

namespace AiArchitectureToolkit.McpServer.Resources;

using Services;

/// <summary>
/// MCP resources exposing project-specific content from the workspace.
/// </summary>
[McpServerResourceType]
public sealed class ProjectResources
{
    [McpServerResource(UriTemplate = "project://architecture"), Description("The authoritative architecture document (architecture-final.md)")]
    public static string GetArchitecture(ProjectContentService service)
    {
        return service.GetArchitecture()
            ?? "architecture-final.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://architecture-blueprint"), Description("The architecture blueprint (pre-review draft)")]
    public static string GetArchitectureBlueprint(ProjectContentService service)
    {
        return service.GetArchitectureBlueprint()
            ?? "architecture-blueprint.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://review-report"), Description("The architecture review report")]
    public static string GetReviewReport(ProjectContentService service)
    {
        return service.GetReviewReport()
            ?? "review-report.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://existing-architecture-review"), Description("The existing architecture review")]
    public static string GetExistingArchitectureReview(ProjectContentService service)
    {
        return service.GetExistingArchitectureReview()
            ?? "existing-architecture-review.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://prototype-analysis"), Description("The prototype analysis output")]
    public static string GetPrototypeAnalysis(ProjectContentService service)
    {
        return service.GetPrototypeAnalysis()
            ?? "prototype-analysis.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://prototype-architecture-alignment"), Description("The prototype-architecture alignment analysis")]
    public static string GetPrototypeArchitectureAlignment(ProjectContentService service)
    {
        return service.GetPrototypeArchitectureAlignment()
            ?? "prototype-architecture-alignment.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://adr/{name}"), Description("A specific Architecture Decision Record")]
    public static string GetAdr(ProjectContentService service, string name)
    {
        return service.GetAdr(name)
            ?? $"ADR '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "project://delivery-plan"), Description("The delivery plan")]
    public static string GetDeliveryPlan(ProjectContentService service)
    {
        return service.GetDeliveryPlan()
            ?? "delivery-plan.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://feature-spec/{name}"), Description("A feature specification for a specific slice")]
    public static string GetFeatureSpec(ProjectContentService service, string name)
    {
        return service.GetFeatureSpec(name)
            ?? $"Feature spec '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "project://project-context"), Description("The project context file")]
    public static string GetProjectContext(ProjectContentService service)
    {
        return service.GetProjectContext()
            ?? "project-context.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://design-system"), Description("The design system document")]
    public static string GetDesignSystem(ProjectContentService service)
    {
        return service.GetDesignSystem()
            ?? "design-system.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://ui-inventory"), Description("The UI inventory (UI retrofit workflow Step 1) — the input the design system is derived from on the retrofit path")]
    public static string GetUiInventory(ProjectContentService service)
    {
        return service.GetUiInventory()
            ?? "ui-inventory.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://remediation-audit"), Description("The UI remediation audit results")]
    public static string GetRemediationAudit(ProjectContentService service)
    {
        return service.GetRemediationAudit()
            ?? "remediation-audit.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://legacy-system-analysis"), Description("The legacy system analysis (architecture Mode D)")]
    public static string GetLegacySystemAnalysis(ProjectContentService service)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.GetLegacySystemAnalysis()
            ?? "legacy-system-analysis.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://architecture-final-gate"), Description("The architecture-final quality gate report — architecture-final.md is authoritative only once this records APPROVED or APPROVED WITH NOTES")]
    public static string GetArchitectureFinalGate(ProjectContentService service)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.GetArchitectureFinalGate()
            ?? "architecture-final-gate.md not found in this workspace.";
    }

    [McpServerResource(UriTemplate = "project://slice-verification/{name}"), Description("Integrated Slice Verification evidence (engineering workflow Step 6b) for a slice")]
    public static string GetSliceVerification(ProjectContentService service, string name)
    {
        ArgumentNullException.ThrowIfNull(service);

        return service.GetSliceVerification(name)
            ?? $"Slice verification evidence for '{name}' not found.";
    }

    /// <summary>
    /// Renders a slice's decomposition output: <c>OVERVIEW.md</c>, each Part's
    /// Status line, its Step 6 / Step 6a review state, and the warnings that
    /// decide whether the next Part may start.
    /// </summary>
    [McpServerResource(UriTemplate = "project://ai-parts/{sliceId}"), Description("The decomposition output for a slice (ai-parts/<slice-id>/): OVERVIEW.md, each Part's Status, its Part Quality Report and Part Code Review verdict, and warnings blocking the next Part")]
    public static string GetAiParts(ProjectContentService service, string sliceId)
    {
        ArgumentNullException.ThrowIfNull(service);

        var decomposition = service.GetDecomposition(sliceId);
        if (decomposition is null)
        {
            return $"Decomposition for slice '{sliceId}' not found under ai-parts/.";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"# Decomposition: {decomposition.SliceId}");
        sb.AppendLine();

        DecompositionReport.Append(sb, decomposition, "## Parts");

        var overview = service.GetPartsOverview(decomposition.SliceId);
        sb.AppendLine("---");
        sb.AppendLine("## OVERVIEW.md");
        sb.AppendLine();
        sb.AppendLine(overview ?? "(no OVERVIEW.md in this slice folder)");
        sb.AppendLine();

        foreach (var part in decomposition.Parts)
        {
            var content = service.GetPart(decomposition.SliceId, part.FileName);
            if (content is null)
            {
                continue;
            }

            sb.AppendLine("---");
            sb.AppendLine($"## {part.FileName}.md");
            sb.AppendLine();
            sb.AppendLine(content);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
