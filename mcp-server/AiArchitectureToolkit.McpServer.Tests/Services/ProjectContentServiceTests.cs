using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Services;

public sealed class ProjectContentServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ProjectContentService _service;

    public ProjectContentServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"project-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        // Set up project structure
        CreateFile("architecture", "architecture-final.md", "# Final Architecture\n\nThis is authoritative.");
        CreateFile("architecture", "architecture-blueprint.md", "# Architecture Blueprint\n\nDraft design.");
        CreateFile("architecture", "review-report.md", "# Review Report\n\nFindings here.");
        CreateFile("architecture", "existing-architecture-review.md", "# Existing Review\n\nExisting review content.");
        CreateFile("architecture", "prototype-analysis.md", "# Prototype Analysis\n\nBehavior extracted.");
        CreateFile("architecture", "prototype-architecture-alignment.md", "# Alignment\n\nGaps identified.");
        CreateFile("architecture", "delivery-plan.md", "# Delivery Plan\n\nSlice 1: User Registration.");
        CreateFile("architecture/adr", "ADR-001-modular-monolith.md", "# ADR-001\n\nUse modular monolith.");
        CreateFile("architecture/adr", "ADR-002-vertical-slices.md", "# ADR-002\n\nUse vertical slices.");
        CreateFile("architecture/feature-specs", "user-registration.md", "# User Registration\n\nFeature spec content.");
        CreateFile("ai", "project-context.md", "# Project Context\n\nThis is a project.");

        // Don't create design-system.md (to test missing file behavior)

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = Path.Combine(_tempDir, "ai"),
            GitHubRoot = Path.Combine(_tempDir, ".github"),
            WorkspaceRoot = _tempDir
        });

        _service = new ProjectContentService(options);
    }

    [Fact]
    public void GetArchitecture_ReturnsContent()
    {
        var content = _service.GetArchitecture();

        Assert.NotNull(content);
        Assert.Contains("Final Architecture", content);
    }

    [Fact]
    public void GetDeliveryPlan_ReturnsContent()
    {
        var content = _service.GetDeliveryPlan();

        Assert.NotNull(content);
        Assert.Contains("User Registration", content);
    }

    [Fact]
    public void GetAdr_ByName_ReturnsContent()
    {
        var content = _service.GetAdr("ADR-001-modular-monolith");

        Assert.NotNull(content);
        Assert.Contains("modular monolith", content);
    }

    [Fact]
    public void ListAdrs_ReturnsAllAdrNames()
    {
        var adrs = _service.ListAdrs();

        Assert.Equal(2, adrs.Count);
        Assert.Contains("ADR-001-modular-monolith", adrs);
        Assert.Contains("ADR-002-vertical-slices", adrs);
    }

    [Fact]
    public void GetFeatureSpec_ReturnsContent()
    {
        var content = _service.GetFeatureSpec("user-registration");

        Assert.NotNull(content);
        Assert.Contains("Feature spec content", content);
    }

    [Fact]
    public void GetFeatureSpec_MissingSlice_ReturnsNull()
    {
        var content = _service.GetFeatureSpec("nonexistent-slice");

        Assert.Null(content);
    }

    [Fact]
    public void ListFeatureSpecs_ReturnsNames()
    {
        var specs = _service.ListFeatureSpecs();

        Assert.Single(specs);
        Assert.Equal("user-registration", specs[0]);
    }

    [Fact]
    public void GetProjectContext_ReturnsContent()
    {
        var content = _service.GetProjectContext();

        Assert.NotNull(content);
        Assert.Contains("This is a project", content);
    }

    [Fact]
    public void GetDesignSystem_WhenMissing_ReturnsNull()
    {
        var content = _service.GetDesignSystem();

        Assert.Null(content);
    }

    [Fact]
    public void GetAllAdrs_ReturnsConcatenated()
    {
        var content = _service.GetAllAdrs();

        Assert.NotNull(content);
        Assert.Contains("ADR-001", content);
        Assert.Contains("ADR-002", content);
        Assert.Contains("---", content);
    }

    [Fact]
    public void ListArtifacts_ShowsExistenceStatus()
    {
        var artifacts = _service.ListArtifacts();

        Assert.True(artifacts["architecture"][0].Exists);
        Assert.True(artifacts["architecture-blueprint"][0].Exists);
        Assert.True(artifacts["review-report"][0].Exists);
        Assert.True(artifacts["existing-architecture-review"][0].Exists);
        Assert.True(artifacts["prototype-analysis"][0].Exists);
        Assert.True(artifacts["prototype-architecture-alignment"][0].Exists);
        Assert.True(artifacts["delivery-plan"][0].Exists);
        Assert.False(artifacts["design-system"][0].Exists);
        Assert.Equal(2, artifacts["adrs"].Count);
        Assert.Single(artifacts["feature-specs"]);
    }

    [Fact]
    public void GetArchitectureBlueprint_ReturnsContent()
    {
        var content = _service.GetArchitectureBlueprint();

        Assert.NotNull(content);
        Assert.Contains("Draft design", content);
    }

    [Fact]
    public void GetReviewReport_ReturnsContent()
    {
        var content = _service.GetReviewReport();

        Assert.NotNull(content);
        Assert.Contains("Findings here", content);
    }

    [Fact]
    public void GetExistingArchitectureReview_ReturnsContent()
    {
        var content = _service.GetExistingArchitectureReview();

        Assert.NotNull(content);
        Assert.Contains("Existing review content", content);
    }

    [Fact]
    public void GetPrototypeAnalysis_ReturnsContent()
    {
        var content = _service.GetPrototypeAnalysis();

        Assert.NotNull(content);
        Assert.Contains("Behavior extracted", content);
    }

    [Fact]
    public void GetPrototypeArchitectureAlignment_ReturnsContent()
    {
        var content = _service.GetPrototypeArchitectureAlignment();

        Assert.NotNull(content);
        Assert.Contains("Gaps identified", content);
    }

    [Fact]
    public void GetAdr_PathTraversal_ReturnsNull()
    {
        var content = _service.GetAdr("../../etc/passwd");

        Assert.Null(content);
    }

    [Fact]
    public void GetRemediationAudit_WhenMissing_ReturnsNull()
    {
        var content = _service.GetRemediationAudit();

        Assert.Null(content);
    }

    [Fact]
    public void ListArtifacts_IncludesRemediationAudit()
    {
        var artifacts = _service.ListArtifacts();

        Assert.Contains("remediation-audit", artifacts.Keys);
        Assert.False(artifacts["remediation-audit"][0].Exists);
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
