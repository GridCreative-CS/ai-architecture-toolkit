using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Prompts;
using AiArchitectureToolkit.McpServer.Resources;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Resources;

/// <summary>
/// Covers the project resources added for toolkit v4.3.0–v4.6.0: decomposition
/// output, Mode D legacy analysis, slice verification evidence, and the
/// architecture-final quality gate.
/// </summary>
public sealed class ProjectResourcesTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ProjectContentService _projectService;
    private readonly ToolkitContentService _toolkitService;

    public ProjectResourcesTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"project-resources-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        CreateFile("architecture", "architecture-final.md", "# Architecture\n\nModular monolith.");
        CreateFile("architecture", "legacy-system-analysis.md", "# Legacy System Analysis\n\nLegacy constraints.");
        CreateFile("architecture", "architecture-final-gate.md", "# Gate Report\n\nVerdict: APPROVED.");
        CreateFile("architecture/slice-verification", "user-registration.md", "# Slice Verification\n\nBrowser evidence recorded.");
        CreateFile("ai", "project-context.md", "# Project Context\n\nA project.");
        CreateFile("architecture", "ui-inventory.md", "# UI Inventory\n\nCatalogued screens and components.");
        CreateFile("ai-parts/user-registration", "OVERVIEW.md",
            "# AI Parts Overview\n\n## Requirement Coverage Map\n\n| AC-01 | P01 |\n\n## Parts Index");
        CreateFile("ai-parts/user-registration", "P01-domain.md",
            "# Part P01 — Domain\nStatus: DONE\n\n## PART_SPEC\n\n```json\n{ \"part_id\": \"P01\", \"part_type\": \"backend\", \"criteria_covered\": [\"AC-01\"] }\n```");
        CreateFile("ai-parts/user-registration/reviews", "P01-review.md",
            "# Part Code Review — P01\n\n## Verdict\n\n`REJECTED — MUST FIX` — one Blocker.");

        var aiDir = Path.Combine(_tempDir, "ai");
        CreateFile("ai/prompts", "architecture-final-quality-gate.md", "# Architecture Final Quality Gate\n\nRun 16 checks.");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = aiDir,
            GitHubRoot = Path.Combine(_tempDir, ".github"),
            WorkspaceRoot = _tempDir
        });

        _projectService = new ProjectContentService(options);
        _toolkitService = new ToolkitContentService(options);
    }

    [Fact]
    public void GetLegacySystemAnalysis_ReturnsContent()
    {
        Assert.Contains("Legacy constraints", ProjectResources.GetLegacySystemAnalysis(_projectService));
    }

    [Fact]
    public void GetArchitectureFinalGate_ReturnsContent()
    {
        Assert.Contains("Verdict: APPROVED", ProjectResources.GetArchitectureFinalGate(_projectService));
    }

    [Fact]
    public void GetUiInventory_ReturnsContent()
    {
        Assert.Contains("Catalogued screens and components", ProjectResources.GetUiInventory(_projectService));
    }

    [Fact]
    public void GetSliceVerification_ReturnsContent()
    {
        Assert.Contains("Browser evidence recorded", ProjectResources.GetSliceVerification(_projectService, "user-registration"));
    }

    [Fact]
    public void GetSliceVerification_Missing_ReportsNotFound()
    {
        Assert.Contains("not found", ProjectResources.GetSliceVerification(_projectService, "nonexistent"));
    }

    [Fact]
    public void GetAiParts_RendersOverviewPartStatusAndReviewVerdict()
    {
        var result = ProjectResources.GetAiParts(_projectService, "user-registration");

        Assert.Contains("AI Parts Overview", result);
        Assert.Contains("P01", result);
        Assert.Contains("DONE", result);
        Assert.Contains("REJECTED — MUST FIX", result);
        Assert.Contains("backend", result);
        Assert.Contains("AC-01", result);
    }

    [Fact]
    public void GetAiParts_SurfacesWarnings()
    {
        var result = ProjectResources.GetAiParts(_projectService, "user-registration");

        Assert.Contains("Warnings", result);
        Assert.Contains("may not be marked DONE", result);
    }

    [Fact]
    public void GetAiParts_UnknownSlice_ReportsNotFound()
    {
        Assert.Contains("not found", ProjectResources.GetAiParts(_projectService, "nonexistent"));
    }

    [Fact]
    public void ArchitectureFinalQualityGate_BundlesPromptArchitectureAndProjectContext()
    {
        var result = ToolkitPrompts.ArchitectureFinalQualityGate(_toolkitService, _projectService);

        Assert.Contains("Run 16 checks", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("A project", result);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }

    private void CreateFile(string relativeDir, string fileName, string content)
    {
        var dir = Path.Combine(_tempDir, relativeDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }
}
