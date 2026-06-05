using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Services;

public sealed class WorkspaceScanServiceTests : IDisposable
{
    private readonly string _tempDir;
    private readonly WorkspaceScanService _service;

    public WorkspaceScanServiceTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"workspace-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        // Create a solution file
        File.WriteAllText(Path.Combine(_tempDir, "MyProject.slnx"), "<Solution></Solution>");

        // Create project files
        var srcDir = Path.Combine(_tempDir, "src", "MyProject.Api");
        Directory.CreateDirectory(srcDir);
        File.WriteAllText(Path.Combine(srcDir, "MyProject.Api.csproj"), """
            <Project Sdk="Microsoft.NET.Sdk">
              <ItemGroup>
                <ProjectReference Include="..\MyProject.Domain\MyProject.Domain.csproj" />
              </ItemGroup>
            </Project>
            """);

        var domainDir = Path.Combine(_tempDir, "src", "MyProject.Domain");
        Directory.CreateDirectory(domainDir);
        File.WriteAllText(Path.Combine(domainDir, "MyProject.Domain.csproj"), """
            <Project Sdk="Microsoft.NET.Sdk">
            </Project>
            """);

        // Create a bin directory file that should be excluded
        var binDir = Path.Combine(_tempDir, "src", "MyProject.Api", "bin", "Debug");
        Directory.CreateDirectory(binDir);
        File.WriteAllText(Path.Combine(binDir, "MyProject.Api.csproj"), "should be ignored");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = _tempDir,
            GitHubRoot = _tempDir,
            WorkspaceRoot = _tempDir
        });

        _service = new WorkspaceScanService(options);
    }

    [Fact]
    public void Scan_FindsSolutions()
    {
        var summary = _service.Scan();

        Assert.Single(summary.Solutions);
        Assert.Contains("MyProject.slnx", summary.Solutions[0]);
    }

    [Fact]
    public void Scan_FindsProjects_ExcludingBin()
    {
        var summary = _service.Scan();

        Assert.Equal(2, summary.Projects.Count);
        Assert.Contains(summary.Projects, p => p.Contains("MyProject.Api.csproj"));
        Assert.Contains(summary.Projects, p => p.Contains("MyProject.Domain.csproj"));
        Assert.DoesNotContain(summary.Projects, p => p.Contains("bin"));
    }

    [Fact]
    public void Scan_BuildsDependencyGraph()
    {
        var summary = _service.Scan();

        Assert.Contains("MyProject.Api", summary.DependencyGraph.Keys);
        Assert.Contains("MyProject.Domain", summary.DependencyGraph["MyProject.Api"]);
        Assert.Empty(summary.DependencyGraph["MyProject.Domain"]);
    }

    [Fact]
    public void ToDisplayString_ProducesReadableOutput()
    {
        var summary = _service.Scan();
        var display = summary.ToDisplayString();

        Assert.Contains("## Solutions", display);
        Assert.Contains("## Projects", display);
        Assert.Contains("## Dependency Graph", display);
        Assert.Contains("MyProject.Api", display);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }
}
