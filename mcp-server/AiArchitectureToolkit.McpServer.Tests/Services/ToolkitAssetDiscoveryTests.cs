using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Services;

/// <summary>
/// Points the content service at the real toolkit tree in this repository and
/// asserts that the assets the workflows depend on are actually discoverable.
/// Listing is directory-driven, so these tests fail when an asset is renamed or
/// removed without the server being updated.
/// </summary>
public sealed class ToolkitAssetDiscoveryTests
{
    private readonly ToolkitContentService _service;

    public ToolkitAssetDiscoveryTests()
    {
        var repoRoot = FindRepositoryRoot();

        _service = new ToolkitContentService(Options.Create(new ServerOptions
        {
            ToolkitRoot = Path.Combine(repoRoot, "ai"),
            GitHubRoot = Path.Combine(repoRoot, ".github"),
            WorkspaceRoot = repoRoot
        }));
    }

    [Theory]
    // Toolkit v4.3.0 assets (alignment items 5 and 7)
    [InlineData("prompts", "slice-preparation-runner")]
    [InlineData("prompts", "golden-dataset-generator")]
    [InlineData("prompts", "legacy-system-analyzer")]
    [InlineData("templates", "golden-dataset-template")]
    [InlineData("templates", "project-claude-template")]
    // Toolkit v4.5.0 assets (alignment item 9)
    [InlineData("prompts", "architecture-final-quality-gate")]
    [InlineData("examples", "example-architecture-final-gate-report")]
    // Toolkit v4.6.0 assets (alignment item 10d)
    [InlineData("prompts", "code-quality-reviewer")]
    [InlineData("templates", "code-quality-checklist-template")]
    [InlineData("guides", "code-quality-standard")]
    [InlineData("examples", "example-part-quality-report")]
    [InlineData("examples", "example-part-review")]
    public void ListAllContent_IncludesToolkitAsset(string category, string name)
    {
        var content = _service.ListAllContent();

        Assert.Contains(name, content[category]);
    }

    [Theory]
    [InlineData("prompts", "slice-preparation-runner")]
    [InlineData("prompts", "architecture-final-quality-gate")]
    [InlineData("prompts", "code-quality-reviewer")]
    [InlineData("templates", "code-quality-checklist-template")]
    [InlineData("templates", "project-claude-template")]
    [InlineData("guides", "code-quality-standard")]
    [InlineData("examples", "example-part-quality-report")]
    [InlineData("examples", "example-part-review")]
    [InlineData("examples", "example-architecture-final-gate-report")]
    public void GetContent_ReadsToolkitAsset(string category, string name)
    {
        var content = _service.GetContent(category, name);

        Assert.False(string.IsNullOrWhiteSpace(content));
    }

    [Fact]
    public void ListAllContent_IncludesJsonExamples()
    {
        var content = _service.ListAllContent();

        Assert.Contains("example-golden-dataset-case.json", content["examples"]);
        Assert.Contains("golden-dataset-json-template.json", content["templates"]);
    }

    [Fact]
    public void GetContent_ReadsListedJsonAssets()
    {
        // Every entry the listing returns must be readable by that same name.
        Assert.False(string.IsNullOrWhiteSpace(_service.GetContent("examples", "example-golden-dataset-case.json")));
        Assert.False(string.IsNullOrWhiteSpace(_service.GetContent("templates", "golden-dataset-json-template.json")));
    }

    [Fact]
    public void ListAllContent_IncludesSkills()
    {
        var content = _service.ListAllContent();

        Assert.Contains("plan-decomposer", content["skills"]);
        Assert.Contains("part-executor-tdd", content["skills"]);
    }

    [Fact]
    public void GetContent_ReadsSkillDefinition()
    {
        var content = _service.GetContent("skills", "plan-decomposer");

        Assert.NotNull(content);
        Assert.Contains("PART_SPEC", content);
    }

    [Fact]
    public void GetContent_SkillPathTraversal_ReturnsNull()
    {
        Assert.Null(_service.GetContent("skills", "../../etc/passwd"));
    }

    /// <summary>
    /// Walks up from the test binary to the repository root — the directory
    /// holding both <c>ai/</c> and <c>.github/</c>.
    /// </summary>
    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "ai")) &&
                Directory.Exists(Path.Combine(directory.FullName, ".github")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"Could not locate the toolkit repository root from '{AppContext.BaseDirectory}'.");
    }
}
