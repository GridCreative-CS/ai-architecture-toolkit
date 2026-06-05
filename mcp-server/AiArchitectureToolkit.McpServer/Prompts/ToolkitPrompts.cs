using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AiArchitectureToolkit.McpServer.Prompts;

using Services;

/// <summary>
/// MCP prompts that assemble complete, context-rich message chains
/// for key toolkit workflows.
/// </summary>
[McpServerPromptType]
public sealed class ToolkitPrompts
{
    /// <summary>
    /// Assembles a complete feature spec generation prompt with all required context.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for creating a feature specification for a slice. Includes the feature-spec-generator prompt, template, architecture, delivery plan, and ADRs.")]
    public static string FeatureSpec(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Name of the slice to generate a feature spec for")] string sliceName)
    {
        var prompt = toolkitService.GetContent("prompts", "feature-spec-generator") ?? "";
        var template = toolkitService.GetContent("templates", "feature-spec-template") ?? "";
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";
        var deliveryPlan = projectService.GetDeliveryPlan() ?? "(no delivery-plan.md found)";
        var adrs = projectService.GetAllAdrs() ?? "(no ADRs found)";
        var designSystem = projectService.GetDesignSystem();

        var designSystemSection = designSystem is not null
            ? $"\n\n## Design System\n\n{designSystem}"
            : "";

        return $"""
            {prompt}

            ---

            ## Template

            {template}

            ---

            ## Context: Architecture

            {architecture}

            ---

            ## Context: Delivery Plan

            {deliveryPlan}

            ---

            ## Context: ADRs

            {adrs}
            {designSystemSection}

            ---

            ## Task

            Generate a feature specification for the slice: **{sliceName}**
            """;
    }

    /// <summary>
    /// Assembles a complete delivery plan generation prompt with all required context.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for creating a delivery plan. Includes the delivery-planner prompt, architecture, ADRs, and design system if present.")]
    public static string DeliveryPlan(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "delivery-planner") ?? "";
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";
        var adrs = projectService.GetAllAdrs() ?? "(no ADRs found)";
        var designSystem = projectService.GetDesignSystem();

        var designSystemSection = designSystem is not null
            ? $"\n\n## Design System\n\n{designSystem}"
            : "";

        return $"""
            {prompt}

            ---

            ## Context: Architecture

            {architecture}

            ---

            ## Context: ADRs

            {adrs}
            {designSystemSection}

            ---

            ## Task

            Generate a delivery plan based on the architecture and ADRs above.
            """;
    }

    /// <summary>
    /// Assembles a complete architecture compliance check prompt with all required context.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for running an architecture compliance check on a slice. Includes the compliance prompt, architecture, ADRs, and the feature spec for the slice.")]
    public static string ArchitectureCompliance(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Name of the slice to check for compliance")] string sliceName)
    {
        var prompt = toolkitService.GetContent("prompts", "architecture-compliance") ?? "";
        var template = toolkitService.GetContent("templates", "compliance-report-template") ?? "";
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";
        var adrs = projectService.GetAllAdrs() ?? "(no ADRs found)";
        var featureSpec = projectService.GetFeatureSpec(sliceName) ?? $"(no feature spec found for '{sliceName}')";

        return $"""
            {prompt}

            ---

            ## Report Template

            {template}

            ---

            ## Context: Architecture

            {architecture}

            ---

            ## Context: ADRs

            {adrs}

            ---

            ## Artifact Under Review: Feature Spec for {sliceName}

            {featureSpec}

            ---

            ## Task

            Run an architecture compliance check on the feature spec for **{sliceName}**.
            """;
    }

    /// <summary>
    /// Assembles a complete ADR generation prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for creating an Architecture Decision Record. Includes the ADR generator prompt, template, and current architecture context.")]
    public static string AdrGenerator(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Title of the architecture decision")] string decisionTitle)
    {
        var prompt = toolkitService.GetContent("prompts", "adr-generator") ?? "";
        var template = toolkitService.GetContent("templates", "adr-template") ?? "";
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";

        return $"""
            {prompt}

            ---

            ## Template

            {template}

            ---

            ## Context: Architecture

            {architecture}

            ---

            ## Task

            Generate an ADR for the decision: **{decisionTitle}**
            """;
    }

    /// <summary>
    /// Assembles a complete feature spec reconciliation prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for reconciling a feature spec against compliance findings. Includes the reconciler prompt, compliance report, feature spec, architecture, and ADRs.")]
    public static string FeatureSpecReconciler(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Name of the slice whose feature spec needs reconciliation")] string sliceName)
    {
        var prompt = toolkitService.GetContent("prompts", "feature-spec-reconciler") ?? "";
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";
        var adrs = projectService.GetAllAdrs() ?? "(no ADRs found)";
        var featureSpec = projectService.GetFeatureSpec(sliceName) ?? $"(no feature spec found for '{sliceName}')";
        var complianceReport = projectService.GetComplianceReport(sliceName) ?? $"(no compliance report found for '{sliceName}')";

        return $"""
            {prompt}

            ---

            ## Context: Architecture

            {architecture}

            ---

            ## Context: ADRs

            {adrs}

            ---

            ## Feature Spec: {sliceName}

            {featureSpec}

            ---

            ## Compliance Report: {sliceName}

            {complianceReport}

            ---

            ## Task

            Reconcile the feature spec for **{sliceName}** against the compliance findings above.
            """;
    }

    /// <summary>
    /// Assembles a complete architecture design prompt from a prototype analysis.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for designing a production architecture from a prototype analysis. Includes the architecture-designer prompt, blueprint template, project context, prototype analysis, and relevant guides.")]
    public static string ArchitectureDesigner(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "architecture-designer") ?? "";
        var template = toolkitService.GetContent("templates", "architecture-blueprint-template") ?? "";
        var projectContext = projectService.GetProjectContext() ?? "(no project-context.md found)";
        var prototypeAnalysis = projectService.GetPrototypeAnalysis() ?? "(no prototype-analysis.md found)";
        var modularMonolith = toolkitService.GetContent("guides", "modular-monolith-definition") ?? "";
        var contractDef = toolkitService.GetContent("guides", "contract-definition") ?? "";

        return $"""
            {prompt}

            ---

            ## Template

            {template}

            ---

            ## Context: Project

            {projectContext}

            ---

            ## Context: Prototype Analysis

            {prototypeAnalysis}

            ---

            ## Guide: Modular Monolith

            {modularMonolith}

            ---

            ## Guide: Contracts

            {contractDef}

            ---

            ## Task

            Design a production-grade architecture based on the prototype analysis and project context above. Output using the blueprint template.
            """;
    }

    /// <summary>
    /// Assembles a complete architecture review prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for reviewing an architecture blueprint. Includes the architecture-reviewer prompt and the current blueprint.")]
    public static string ArchitectureReviewer(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "architecture-reviewer") ?? "";
        var blueprint = projectService.GetArchitectureBlueprint() ?? "(no architecture-blueprint.md found)";

        return $"""
            {prompt}

            ---

            ## Artifact Under Review: Architecture Blueprint

            {blueprint}

            ---

            ## Task

            Review the architecture blueprint above. Identify issues, risks, and improvement opportunities.
            """;
    }

    /// <summary>
    /// Assembles a complete architecture reconciliation prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for reconciling an architecture blueprint with review findings to produce architecture-final.md. Includes the reconciler prompt, blueprint template, blueprint, and review report.")]
    public static string ArchitectureReconciler(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "architecture-reconciler") ?? "";
        var template = toolkitService.GetContent("templates", "architecture-blueprint-template") ?? "";
        var blueprint = projectService.GetArchitectureBlueprint() ?? "(no architecture-blueprint.md found)";
        var reviewReport = projectService.GetReviewReport() ?? "(no review-report.md found)";

        return $"""
            {prompt}

            ---

            ## Template

            {template}

            ---

            ## Context: Architecture Blueprint

            {blueprint}

            ---

            ## Context: Review Report

            {reviewReport}

            ---

            ## Task

            Reconcile the architecture blueprint with the review findings above. Produce architecture-final.md using the template.
            """;
    }

    /// <summary>
    /// Assembles a complete existing architecture review prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for reviewing an existing architecture document (no prototype). Includes the existing-architecture-reviewer prompt and the architecture document.")]
    public static string ExistingArchitectureReviewer(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "existing-architecture-reviewer") ?? "";
        var architecture = projectService.GetArchitecture()
            ?? projectService.GetArchitectureBlueprint()
            ?? "(no architecture document found)";

        return $"""
            {prompt}

            ---

            ## Artifact Under Review: Architecture Document

            {architecture}

            ---

            ## Task

            Review the existing architecture document above critically. Identify gaps, risks, and improvement opportunities.
            """;
    }

    /// <summary>
    /// Assembles a complete architecture gap reconciliation prompt (no-prototype path).
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for reconciling an existing architecture document with review findings to produce architecture-final.md (no-prototype path). Includes the gap-reconciler prompt, blueprint template, architecture document, and review.")]
    public static string ArchitectureGapReconciler(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "architecture-gap-reconciler") ?? "";
        var template = toolkitService.GetContent("templates", "architecture-blueprint-template") ?? "";
        var architecture = projectService.GetArchitecture()
            ?? projectService.GetArchitectureBlueprint()
            ?? "(no architecture document found)";
        var review = projectService.GetExistingArchitectureReview()
            ?? projectService.GetReviewReport()
            ?? "(no review found)";

        return $"""
            {prompt}

            ---

            ## Template

            {template}

            ---

            ## Context: Architecture Document

            {architecture}

            ---

            ## Context: Review

            {review}

            ---

            ## Task

            Reconcile the architecture document with the review findings above. Fill gaps, resolve inconsistencies, and produce architecture-final.md using the template.
            """;
    }

    /// <summary>
    /// Assembles a complete prototype-architecture alignment prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for comparing a prototype analysis against an existing architecture document to identify alignment, gaps, and inconsistencies.")]
    public static string PrototypeArchitectureAlignment(
        ToolkitContentService toolkitService,
        ProjectContentService projectService)
    {
        var prompt = toolkitService.GetContent("prompts", "prototype-architecture-alignment") ?? "";
        var prototypeAnalysis = projectService.GetPrototypeAnalysis() ?? "(no prototype-analysis.md found)";
        var architecture = projectService.GetArchitecture()
            ?? projectService.GetArchitectureBlueprint()
            ?? "(no architecture document found)";

        return $"""
            {prompt}

            ---

            ## Context: Prototype Analysis

            {prototypeAnalysis}

            ---

            ## Context: Architecture Document

            {architecture}

            ---

            ## Task

            Compare the prototype analysis against the architecture document. Identify alignment, gaps, and inconsistencies.
            """;
    }

    /// <summary>
    /// Assembles a complete UI compliance check prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for running a UI compliance check on a slice. Includes the UI compliance check prompt, design system, feature spec, and architecture context.")]
    public static string UiComplianceCheck(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Name of the slice to check for UI compliance")] string sliceName)
    {
        var prompt = toolkitService.GetContent("prompts", "ui-compliance-check") ?? "";
        var designSystem = projectService.GetDesignSystem() ?? "(no design-system.md found)";
        var featureSpec = projectService.GetFeatureSpec(sliceName) ?? $"(no feature spec found for '{sliceName}')";
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";

        return $"""
            {prompt}

            ---

            ## Context: Design System

            {designSystem}

            ---

            ## Context: Architecture

            {architecture}

            ---

            ## Artifact Under Review: Feature Spec for {sliceName}

            {featureSpec}

            ---

            ## Task

            Run a UI compliance check on the slice **{sliceName}** against the design system above.
            """;
    }

    /// <summary>
    /// Assembles a complete slice verification checklist prompt.
    /// </summary>
    [McpServerPrompt, Description("Generates a complete prompt for running the integrated slice verification checklist (Step 6b). Includes the checklist template, feature spec, design system, and architecture context.")]
    public static string SliceVerification(
        ToolkitContentService toolkitService,
        ProjectContentService projectService,
        [Description("Name of the slice to verify")] string sliceName)
    {
        var checklist = toolkitService.GetContent("templates", "slice-verification-checklist-template") ?? "";
        var featureSpec = projectService.GetFeatureSpec(sliceName) ?? $"(no feature spec found for '{sliceName}')";
        var designSystem = projectService.GetDesignSystem();
        var architecture = projectService.GetArchitecture() ?? "(no architecture-final.md found)";

        var designSystemSection = designSystem is not null
            ? $"\n\n## Context: Design System\n\n{designSystem}"
            : "";

        return $"""
            # Slice Verification — {sliceName}

            Use the checklist below to verify this slice in the running application.

            ---

            ## Checklist

            {checklist}

            ---

            ## Context: Feature Spec

            {featureSpec}

            ---

            ## Context: Architecture

            {architecture}
            {designSystemSection}

            ---

            ## Task

            Walk through the checklist for slice **{sliceName}**. Verify each item in the running application and report pass/fail status.
            """;
    }
}
