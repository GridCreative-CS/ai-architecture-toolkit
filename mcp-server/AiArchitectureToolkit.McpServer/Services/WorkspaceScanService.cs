using System.Xml.Linq;
using AiArchitectureToolkit.McpServer.Configuration;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Services;

/// <summary>
/// Scans the workspace for .NET solution files, project files, and builds
/// a project dependency graph.
/// </summary>
public sealed class WorkspaceScanService
{
    private readonly string _workspaceRoot;

    public WorkspaceScanService(IOptions<ServerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _workspaceRoot = Path.GetFullPath(options.Value.WorkspaceRoot);
    }

    /// <summary>
    /// Returns a structured summary of the workspace: solutions, projects, and dependency graph.
    /// </summary>
    public WorkspaceSummary Scan()
    {
        var solutions = FindFiles("*.slnx")
            .Concat(FindFiles("*.sln"))
            .ToList();

        var projects = FindFiles("*.csproj");

        var dependencyGraph = new Dictionary<string, List<string>>();

        foreach (var projectPath in projects)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectPath);
            var references = ExtractProjectReferences(projectPath);
            dependencyGraph[projectName] = references;
        }

        return new WorkspaceSummary
        {
            Solutions = solutions.Select(s => Path.GetRelativePath(_workspaceRoot, s)).ToList(),
            Projects = projects.Select(p => Path.GetRelativePath(_workspaceRoot, p)).ToList(),
            DependencyGraph = dependencyGraph
        };
    }

    private List<string> FindFiles(string pattern)
    {
        if (!Directory.Exists(_workspaceRoot))
        {
            return [];
        }

        return Directory.GetFiles(_workspaceRoot, pattern, SearchOption.AllDirectories)
            .Where(f =>
            {
                var relative = Path.GetRelativePath(_workspaceRoot, f);
                // Skip common non-source directories
                return !relative.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                    && !relative.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                    && !relative.StartsWith($"bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                    && !relative.StartsWith($"obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);
            })
            .Order()
            .ToList();
    }

    private static List<string> ExtractProjectReferences(string csprojPath)
    {
        try
        {
            var doc = XDocument.Load(csprojPath);
            return doc.Descendants("ProjectReference")
                .Select(e => e.Attribute("Include")?.Value)
                .Where(v => v is not null)
                .Select(v => Path.GetFileNameWithoutExtension(v!))
                .ToList();
        }
        catch
        {
            return [];
        }
    }
}

/// <summary>
/// Structured summary of the workspace's .NET projects and their dependencies.
/// </summary>
public sealed class WorkspaceSummary
{
    public required List<string> Solutions { get; init; }
    public required List<string> Projects { get; init; }
    public required Dictionary<string, List<string>> DependencyGraph { get; init; }

    /// <summary>
    /// Returns a human-readable text summary of the workspace structure.
    /// </summary>
    public string ToDisplayString()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("## Solutions");
        if (Solutions.Count == 0)
        {
            sb.AppendLine("  (none found)");
        }
        else
        {
            foreach (var sln in Solutions)
            {
                sb.AppendLine($"  - {sln}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("## Projects");
        if (Projects.Count == 0)
        {
            sb.AppendLine("  (none found)");
        }
        else
        {
            foreach (var proj in Projects)
            {
                sb.AppendLine($"  - {proj}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("## Dependency Graph");
        foreach (var (project, refs) in DependencyGraph.OrderBy(kv => kv.Key))
        {
            if (refs.Count == 0)
            {
                sb.AppendLine($"  {project}: (no project references)");
            }
            else
            {
                sb.AppendLine($"  {project} → {string.Join(", ", refs)}");
            }
        }

        return sb.ToString();
    }
}
