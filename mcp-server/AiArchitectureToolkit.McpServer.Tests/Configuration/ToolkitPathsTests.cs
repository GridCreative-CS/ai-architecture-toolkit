using AiArchitectureToolkit.McpServer.Configuration;

namespace AiArchitectureToolkit.McpServer.Tests.Configuration;

public sealed class ToolkitPathsTests : IDisposable
{
    private readonly string _tempDir;

    public ToolkitPathsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"toolkit-paths-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public void FindToolkitRoot_LocatesTheDirectoryHoldingAiAndGithub()
    {
        var root = Path.Combine(_tempDir, "repo");
        Directory.CreateDirectory(Path.Combine(root, "ai"));
        Directory.CreateDirectory(Path.Combine(root, ".github"));
        var binary = Path.Combine(root, "mcp-server", "Server", "bin", "Debug", "net10.0");
        Directory.CreateDirectory(binary);

        var found = ToolkitPaths.FindToolkitRoot(binary);

        Assert.Equal(root, found);
    }

    [Fact]
    public void FindToolkitRoot_RequiresBothDirectories()
    {
        var root = Path.Combine(_tempDir, "partial");
        Directory.CreateDirectory(Path.Combine(root, "ai"));
        var binary = Path.Combine(root, "mcp-server", "Server", "bin", "Debug", "net10.0");
        Directory.CreateDirectory(binary);

        Assert.Null(ToolkitPaths.FindToolkitRoot(binary));
    }

    [Fact]
    public void FindToolkitRoot_WhenNothingMatches_ReturnsNull()
    {
        var binary = Path.Combine(_tempDir, "orphan", "bin");
        Directory.CreateDirectory(binary);

        Assert.Null(ToolkitPaths.FindToolkitRoot(binary));
    }

    [Fact]
    public void FindToolkitRoot_FromTheTestBinary_FindsThisRepositorysToolkit()
    {
        // Guards the shipped defaults: with no TOOLKIT_ROOT/GITHUB_ROOT set, the
        // server must still find the toolkit it lives next to.
        var found = ToolkitPaths.FindToolkitRoot(AppContext.BaseDirectory);

        Assert.NotNull(found);
        Assert.True(File.Exists(Path.Combine(found, "ai", "guides", "glossary.md")));
        Assert.True(Directory.Exists(Path.Combine(found, ".github", "skills")));
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }
}
