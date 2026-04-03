using System.ComponentModel;
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

    [McpServerResource(UriTemplate = "project://remediation-audit"), Description("The UI remediation audit results")]
    public static string GetRemediationAudit(ProjectContentService service)
    {
        return service.GetRemediationAudit()
            ?? "remediation-audit.md not found in this workspace.";
    }
}
