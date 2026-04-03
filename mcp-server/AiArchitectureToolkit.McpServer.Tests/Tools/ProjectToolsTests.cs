using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using AiArchitectureToolkit.McpServer.Tools;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Tools;

public sealed class ProjectToolsTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ToolkitContentService _toolkitService;
    private readonly ProjectContentService _projectService;
    private readonly WorkspaceScanService _workspaceService;

    public ProjectToolsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"project-tools-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        // Toolkit content
        var aiDir = Path.Combine(_tempDir, "ai");
        CreateFile(aiDir, "prompts", "delivery-planner.md", "# Delivery Planner\n\nAct as a delivery architect.");
        CreateFile(aiDir, "prompts", "feature-spec-generator.md", "# Feature Spec Generator\n\nGenerate a feature spec.");
        CreateFile(aiDir, "prompts", "architecture-compliance.md", "# Architecture Compliance\n\nCheck compliance.");
        CreateFile(aiDir, "prompts", "architecture-designer.md", "# Architecture Designer\n\nDesign production architecture.");
        CreateFile(aiDir, "prompts", "architecture-reviewer.md", "# Architecture Reviewer\n\nReview the blueprint.");
        CreateFile(aiDir, "prompts", "architecture-reconciler.md", "# Architecture Reconciler\n\nReconcile findings.");
        CreateFile(aiDir, "prompts", "existing-architecture-reviewer.md", "# Existing Architecture Reviewer\n\nReview existing doc.");
        CreateFile(aiDir, "prompts", "architecture-gap-reconciler.md", "# Gap Reconciler\n\nFill gaps.");
        CreateFile(aiDir, "prompts", "prototype-analyzer.md", "# Prototype Analyzer\n\nExtract behavior.");
        CreateFile(aiDir, "prompts", "prototype-architecture-alignment.md", "# Alignment Prompt\n\nCompare prototype with architecture.");
        CreateFile(aiDir, "prompts", "ui-compliance-check.md", "# UI Compliance Check\n\nVerify UI compliance.");
        CreateFile(aiDir, "templates", "feature-spec-template.md", "# Feature Spec Template");
        CreateFile(aiDir, "templates", "compliance-report-template.md", "# Compliance Report Template");
        CreateFile(aiDir, "templates", "architecture-blueprint-template.md", "# Blueprint Template");
        CreateFile(aiDir, "templates", "remediation-spec-template.md", "# Remediation Spec Template");
        CreateFile(aiDir, "templates", "slice-verification-checklist-template.md", "# Slice Verification Checklist");
        CreateFile(aiDir, "guides", "vertical-slice-definition.md", "# Vertical Slice Definition\n\nThe verticality test.");
        CreateFile(aiDir, "guides", "how-feature-specs-are-used.md", "# How Feature Specs Are Used");
        CreateFile(aiDir, "guides", "contract-definition.md", "# Contract Definition");
        CreateFile(aiDir, "guides", "modular-monolith-definition.md", "# Modular Monolith Definition");
        CreateFile(aiDir, "guides", "glossary.md", "# Glossary\n\n## Term\n\nDefinition.");
        CreateFile(aiDir, "workflows", "engineering-workflow.md", "# Engineering Workflow");
        CreateFile(aiDir, "agents", "backend-agent.md", "# Backend Agent");
        CreateFile(aiDir, "examples", "contract-patterns.md", "# Contract Patterns");

        var githubDir = Path.Combine(_tempDir, ".github");
        Directory.CreateDirectory(Path.Combine(githubDir, "instructions"));
        Directory.CreateDirectory(Path.Combine(githubDir, "agents"));

        // Project content
        CreateFile(_tempDir, "architecture", "architecture-final.md", "# Architecture\n\nModular monolith.");
        CreateFile(_tempDir, "architecture", "architecture-blueprint.md", "# Blueprint\n\nDraft design.");
        CreateFile(_tempDir, "architecture", "review-report.md", "# Review Report\n\nFindings.");
        CreateFile(_tempDir, "architecture", "existing-architecture-review.md", "# Existing Review\n\nExisting findings.");
        CreateFile(_tempDir, "architecture", "prototype-analysis.md", "# Prototype Analysis\n\nBehavior.");
        CreateFile(_tempDir, "architecture", "prototype-architecture-alignment.md", "# Alignment\n\nGaps.");
        CreateFile(_tempDir, "architecture", "delivery-plan.md", "# Delivery Plan\n\nSlice: user-registration.");
        CreateFile(_tempDir, "architecture/adr", "ADR-001.md", "# ADR-001\n\nDecision.");
        CreateFile(_tempDir, "architecture/feature-specs", "user-registration.md", "# User Registration Spec");
        CreateFile(_tempDir, "architecture", "design-system.md", "# Design System\n\nToken definitions.");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = aiDir,
            GitHubRoot = githubDir,
            WorkspaceRoot = _tempDir
        });

        _toolkitService = new ToolkitContentService(options);
        _projectService = new ProjectContentService(options);
        _workspaceService = new WorkspaceScanService(options);
    }

    [Fact]
    public void ListProjectArtifacts_ReturnsJson()
    {
        var result = ProjectTools.ListProjectArtifacts(_projectService);

        Assert.Contains("architecture", result);
        Assert.Contains("adrs", result);
        Assert.Contains("feature-specs", result);
    }

    [Fact]
    public void GetWorkspaceStructure_ReturnsReadableOutput()
    {
        var result = ProjectTools.GetWorkspaceStructure(_workspaceService);

        Assert.Contains("Solutions", result);
        Assert.Contains("Projects", result);
    }

    [Fact]
    public void GetWorkflowContext_DeliveryPlanning_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "delivery-planning");

        Assert.Contains("Delivery Planner", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_FeatureSpec_ReturnsPromptTemplateAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "feature-spec");

        Assert.Contains("Feature Spec Generator", result);
        Assert.Contains("Feature Spec Template", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_UnknownStep_ReturnsError()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "unknown-step");

        Assert.Contains("Unknown workflow step", result);
        Assert.Contains("Valid steps", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureDesign_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-design");

        Assert.Contains("Architecture Designer", result);
        Assert.Contains("Blueprint Template", result);
        Assert.Contains("Behavior", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureBlueprintReview_ReturnsPromptAndBlueprint()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-blueprint-review");

        Assert.Contains("Architecture Reviewer", result);
        Assert.Contains("Draft design", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureReconciliation_ReturnsPromptBlueprintAndReview()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-reconciliation");

        Assert.Contains("Architecture Reconciler", result);
        Assert.Contains("Draft design", result);
        Assert.Contains("Findings", result);
    }

    [Fact]
    public void GetWorkflowContext_ExistingArchitectureReview_ReturnsPromptAndArchitecture()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "existing-architecture-review");

        Assert.Contains("Existing Architecture Reviewer", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureGapReconciliation_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-gap-reconciliation");

        Assert.Contains("Gap Reconciler", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Existing findings", result);
    }

    [Fact]
    public void GetWorkflowContext_PrototypeAnalysis_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "prototype-analysis");

        Assert.Contains("Prototype Analyzer", result);
    }

    [Fact]
    public void GetWorkflowContext_PrototypeArchitectureAlignment_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "prototype-architecture-alignment");

        Assert.Contains("Alignment Prompt", result);
        Assert.Contains("Behavior", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_UiRemediation_ReturnsPromptTemplateAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "ui-remediation");

        Assert.Contains("UI Compliance Check", result);
        Assert.Contains("Remediation Spec Template", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Token definitions", result);
    }

    [Fact]
    public void GetWorkflowContext_SliceVerification_ReturnsChecklistAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "slice-verification");

        Assert.Contains("Slice Verification Checklist", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Token definitions", result);
    }

    [Fact]
    public void GetSliceContext_ReturnsFeatureSpecAndPlan()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "user-registration");

        Assert.Contains("User Registration Spec", result);
        Assert.Contains("Delivery Plan", result);
        Assert.Contains("ADR-001", result);
    }

    [Fact]
    public void GetSliceContext_MissingSlice_ShowsNotFound()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "nonexistent");

        Assert.Contains("no feature spec found", result);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }

    private static void CreateFile(string baseDir, string subDir, string fileName, string content)
    {
        var dir = Path.Combine(baseDir, subDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }

    private void CreateFile(string relativeDir, string fileName, string content)
    {
        var dir = Path.Combine(_tempDir, relativeDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }
}
