using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Services;

public sealed class ToolkitContentServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ToolkitContentService _service;

    public ToolkitContentServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"toolkit-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        var aiDir = Path.Combine(_tempDir, "ai");
        CreateFile(aiDir, "guides", "glossary.md", """
            # Glossary

            ## Vertical Slice

            A vertical slice is an end-to-end capability.

            ## Contract

            A contract is the complete agreement between producer and consumer.
            """);
        CreateFile(aiDir, "prompts", "delivery-planner.md", "# Delivery Planner Prompt\n\nAct as a delivery architect.");
        CreateFile(aiDir, "templates", "feature-spec-template.md", "# Feature Spec Template\n\nFill in sections.");
        CreateFile(aiDir, "workflows", "engineering-workflow.md", "# Engineering Workflow\n\nStep 1.");
        CreateFile(aiDir, "agents", "backend-agent.md", "# Backend Agent\n\nAct as a senior .NET engineer.");
        CreateFile(aiDir, "examples", "contract-patterns.md", "# Contract Patterns\n\nExample patterns.");

        var githubDir = Path.Combine(_tempDir, ".github");
        CreateFile(githubDir, "instructions", "csharp.instructions.md", "# C# Development\n\nAlways use C# 14.");
        CreateFile(githubDir, "agents", "expert.agent.md", "# Expert Agent\n\nDotnet expert.");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = aiDir,
            GitHubRoot = githubDir,
            WorkspaceRoot = _tempDir
        });

        _service = new ToolkitContentService(options);
    }

    [Fact]
    public void ListAllContent_ReturnsAllCategories()
    {
        var content = _service.ListAllContent();

        Assert.Contains("guides", content.Keys);
        Assert.Contains("prompts", content.Keys);
        Assert.Contains("templates", content.Keys);
        Assert.Contains("workflows", content.Keys);
        Assert.Contains("agents", content.Keys);
        Assert.Contains("examples", content.Keys);
        Assert.Contains("instructions", content.Keys);
        Assert.Contains("github-agents", content.Keys);
    }

    [Fact]
    public void ListAllContent_ReturnsCorrectFilesPerCategory()
    {
        var content = _service.ListAllContent();

        Assert.Contains("glossary", content["guides"]);
        Assert.Contains("delivery-planner", content["prompts"]);
        Assert.Contains("feature-spec-template", content["templates"]);
    }

    [Fact]
    public void GetContent_ReturnsFileContent()
    {
        var content = _service.GetContent("guides", "glossary");

        Assert.NotNull(content);
        Assert.Contains("Glossary", content);
    }

    [Fact]
    public void GetContent_WithMdExtension_ReturnsFileContent()
    {
        var content = _service.GetContent("guides", "glossary.md");

        Assert.NotNull(content);
        Assert.Contains("Glossary", content);
    }

    [Fact]
    public void GetContent_MissingFile_ReturnsNull()
    {
        var content = _service.GetContent("guides", "nonexistent");

        Assert.Null(content);
    }

    [Fact]
    public void GetContent_PathTraversal_ReturnsNull()
    {
        var content = _service.GetContent("guides", "../../etc/passwd");

        Assert.Null(content);
    }

    [Fact]
    public void GetContent_AbsolutePath_ReturnsNull()
    {
        var content = _service.GetContent("guides", "/etc/passwd");

        Assert.Null(content);
    }

    [Theory]
    [InlineData("guides", "glossary")]
    [InlineData("prompts", "delivery-planner")]
    [InlineData("templates", "feature-spec-template")]
    [InlineData("workflows", "engineering-workflow")]
    [InlineData("agents", "backend-agent")]
    [InlineData("examples", "contract-patterns")]
    [InlineData("instructions", "csharp.instructions")]
    [InlineData("github-agents", "expert.agent")]
    public void GetContent_EveryServedCategory_ReturnsFileContent(string category, string name)
    {
        Assert.NotNull(_service.GetContent(category, name));
    }

    [Fact]
    public void GetContent_AbsoluteCategory_ReturnsNull()
    {
        var outsideDir = CreateOutsideFile();

        var content = _service.GetContent(outsideDir, "secret");

        Assert.Null(content);
    }

    [Fact]
    public void GetContent_TraversingCategory_ReturnsNull()
    {
        CreateOutsideFile();

        // Relative to the toolkit root (<temp>/ai), this resolves to <temp>/outside.
        var content = _service.GetContent("../outside", "secret");

        Assert.Null(content);
    }

    [Fact]
    public void GetContent_NameWithDirectorySeparator_ReturnsNull()
    {
        CreateFile(Path.Combine(_tempDir, "ai"), Path.Combine("guides", "nested"), "secret.md", "nested secret");

        var content = _service.GetContent("guides", "nested/secret");

        Assert.Null(content);
    }

    [Fact]
    public void Search_FindsMatchingContent()
    {
        var results = _service.Search("delivery architect");

        Assert.Single(results);
        Assert.Equal("prompts", results[0].Category);
        Assert.Equal("delivery-planner", results[0].FileName);
    }

    [Fact]
    public void Search_CaseInsensitive()
    {
        var results = _service.Search("DELIVERY ARCHITECT");

        Assert.Single(results);
    }

    [Fact]
    public void Search_NoMatches_ReturnsEmpty()
    {
        var results = _service.Search("xyznonexistent");

        Assert.Empty(results);
    }

    [Fact]
    public void ParseGlossary_ReturnsTerms()
    {
        var glossary = _service.ParseGlossary();

        Assert.Contains("Vertical Slice", glossary.Keys);
        Assert.Contains("Contract", glossary.Keys);
        Assert.Contains("end-to-end capability", glossary["Vertical Slice"]);
    }

    [Fact]
    public void GetContent_Instructions_ReturnsContent()
    {
        var content = _service.GetContent("instructions", "csharp.instructions");

        Assert.NotNull(content);
        Assert.Contains("C# Development", content);
    }

    [Fact]
    public void GetContent_GitHubAgents_ReturnsContent()
    {
        var content = _service.GetContent("github-agents", "expert.agent");

        Assert.NotNull(content);
        Assert.Contains("Expert Agent", content);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }

    /// <summary>
    /// Writes a readable file outside both served roots and returns its directory.
    /// </summary>
    private string CreateOutsideFile()
    {
        var outsideDir = Path.Combine(_tempDir, "outside");
        Directory.CreateDirectory(outsideDir);
        File.WriteAllText(Path.Combine(outsideDir, "secret.md"), "outside secret");
        return outsideDir;
    }

    private static void CreateFile(string baseDir, string subDir, string fileName, string content)
    {
        var dir = Path.Combine(baseDir, subDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }
}
