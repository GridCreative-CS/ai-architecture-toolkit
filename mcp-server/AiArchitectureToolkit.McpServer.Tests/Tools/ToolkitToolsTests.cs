using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using AiArchitectureToolkit.McpServer.Tools;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Tools;

public sealed class ToolkitToolsTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ToolkitContentService _toolkitService;

    public ToolkitToolsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"tools-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        var aiDir = Path.Combine(_tempDir, "ai");
        CreateFile(aiDir, "guides", "glossary.md", """
            # Glossary

            ## Vertical Slice

            A vertical slice is an end-to-end capability that proves a workflow.

            ## Modular Monolith

            A single deployable unit with module boundaries.

            ## Contract

            The complete agreement between producer and consumer.
            """);
        CreateFile(aiDir, "prompts", "delivery-planner.md", "# Delivery Planner\n\nAct as a delivery architect.");
        CreateFile(aiDir, "templates", "feature-spec-template.md", "# Feature Spec\n\nTemplate.");
        CreateFile(aiDir, "workflows", "engineering-workflow.md", "# Engineering Workflow");
        CreateFile(aiDir, "agents", "backend-agent.md", "# Backend Agent");
        CreateFile(aiDir, "examples", "contract-patterns.md", "# Contract Patterns");

        var githubDir = Path.Combine(_tempDir, ".github");
        CreateFile(githubDir, "instructions", "csharp.instructions.md", "# C# Instructions");
        CreateFile(githubDir, "agents", "expert.agent.md", "# Expert");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = aiDir,
            GitHubRoot = githubDir,
            WorkspaceRoot = _tempDir
        });

        _toolkitService = new ToolkitContentService(options);
    }

    [Fact]
    public void ListToolkitContent_ReturnsJsonWithAllCategories()
    {
        var result = ToolkitTools.ListToolkitContent(_toolkitService);

        Assert.Contains("guides", result);
        Assert.Contains("prompts", result);
        Assert.Contains("templates", result);
        Assert.Contains("count", result);
    }

    [Fact]
    public void SearchToolkit_FindsResults()
    {
        var result = ToolkitTools.SearchToolkit(_toolkitService, "delivery architect");

        Assert.Contains("delivery-planner", result);
        Assert.Contains("prompts", result);
    }

    [Fact]
    public void GetGlossaryTerm_ExactMatch_ReturnsDefinition()
    {
        var result = ToolkitTools.GetGlossaryTerm(_toolkitService, "Vertical Slice");

        Assert.Contains("end-to-end capability", result);
    }

    [Fact]
    public void GetGlossaryTerm_PartialMatch_ReturnsDefinition()
    {
        var result = ToolkitTools.GetGlossaryTerm(_toolkitService, "Monolith");

        Assert.Contains("Modular Monolith", result);
        Assert.Contains("single deployable unit", result);
    }

    [Fact]
    public void GetGlossaryTerm_NoMatch_ListsAvailableTerms()
    {
        var result = ToolkitTools.GetGlossaryTerm(_toolkitService, "nonexistent");

        Assert.Contains("not found", result);
        Assert.Contains("Available terms", result);
    }

    [Fact]
    public void GetToolkitFile_ReturnsContent()
    {
        var result = ToolkitTools.GetToolkitFile(_toolkitService, "guides", "glossary");

        Assert.Contains("Glossary", result);
    }

    [Fact]
    public void GetToolkitFile_MissingFile_ReturnsNotFoundMessage()
    {
        var result = ToolkitTools.GetToolkitFile(_toolkitService, "guides", "nonexistent");

        Assert.Contains("not found", result);
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
}
