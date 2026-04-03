using AiArchitectureToolkit.McpServer.Configuration;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Services;

/// <summary>
/// Discovers and reads project-specific content from the workspace's
/// <c>architecture/</c> and <c>ai/project-context.md</c> files.
/// </summary>
public sealed class ProjectContentService
{
    private readonly string _workspaceRoot;

    public ProjectContentService(IOptions<ServerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _workspaceRoot = Path.GetFullPath(options.Value.WorkspaceRoot);
    }

    /// <summary>
    /// Lists all project artifacts and their existence status.
    /// </summary>
    public Dictionary<string, List<ArtifactInfo>> ListArtifacts()
    {
        var result = new Dictionary<string, List<ArtifactInfo>>();

        // Single files
        result["architecture"] = [CheckFile("architecture/architecture-final.md")];
        result["architecture-blueprint"] = [CheckFile("architecture/architecture-blueprint.md")];
        result["review-report"] = [CheckFile("architecture/review-report.md")];
        result["existing-architecture-review"] = [CheckFile("architecture/existing-architecture-review.md")];
        result["prototype-analysis"] = [CheckFile("architecture/prototype-analysis.md")];
        result["prototype-architecture-alignment"] = [CheckFile("architecture/prototype-architecture-alignment.md")];
        result["delivery-plan"] = [CheckFile("architecture/delivery-plan.md")];
        result["design-system"] = [CheckFile("architecture/design-system.md")];
        result["project-context"] = [CheckFile("ai/project-context.md")];

        // Directories
        result["adrs"] = ListDirectory("architecture/adr");
        result["feature-specs"] = ListDirectory("architecture/feature-specs");
        result["compliance-reports"] = ListDirectory("architecture/compliance-reports");
        result["golden-datasets"] = ListDirectory("architecture/golden-datasets");

        return result;
    }

    /// <summary>
    /// Gets the architecture-final.md content.
    /// </summary>
    public string? GetArchitecture() => ReadSafe("architecture/architecture-final.md");

    /// <summary>
    /// Gets the architecture-blueprint.md content.
    /// </summary>
    public string? GetArchitectureBlueprint() => ReadSafe("architecture/architecture-blueprint.md");

    /// <summary>
    /// Gets the review-report.md content.
    /// </summary>
    public string? GetReviewReport() => ReadSafe("architecture/review-report.md");

    /// <summary>
    /// Gets the existing-architecture-review.md content.
    /// </summary>
    public string? GetExistingArchitectureReview() => ReadSafe("architecture/existing-architecture-review.md");

    /// <summary>
    /// Gets the prototype-analysis.md content.
    /// </summary>
    public string? GetPrototypeAnalysis() => ReadSafe("architecture/prototype-analysis.md");

    /// <summary>
    /// Gets the prototype-architecture-alignment.md content.
    /// </summary>
    public string? GetPrototypeArchitectureAlignment() => ReadSafe("architecture/prototype-architecture-alignment.md");

    /// <summary>
    /// Gets a specific ADR by filename (without extension).
    /// </summary>
    public string? GetAdr(string name) => ReadSafe($"architecture/adr/{EnsureMdExtension(name)}");

    /// <summary>
    /// Lists all ADR file names (without extensions).
    /// </summary>
    public List<string> ListAdrs() => ListMarkdownFileNames("architecture/adr");

    /// <summary>
    /// Gets the delivery plan content.
    /// </summary>
    public string? GetDeliveryPlan() => ReadSafe("architecture/delivery-plan.md");

    /// <summary>
    /// Gets a specific feature spec by name (without extension).
    /// </summary>
    public string? GetFeatureSpec(string name) => ReadSafe($"architecture/feature-specs/{EnsureMdExtension(name)}");

    /// <summary>
    /// Lists all feature spec file names (without extensions).
    /// </summary>
    public List<string> ListFeatureSpecs() => ListMarkdownFileNames("architecture/feature-specs");

    /// <summary>
    /// Gets the project context content.
    /// </summary>
    public string? GetProjectContext() => ReadSafe("ai/project-context.md");

    /// <summary>
    /// Gets the design system content.
    /// </summary>
    public string? GetDesignSystem() => ReadSafe("architecture/design-system.md");

    /// <summary>
    /// Gets a compliance report by slice name.
    /// </summary>
    public string? GetComplianceReport(string name) => ReadSafe($"architecture/compliance-reports/{EnsureMdExtension(name)}");

    /// <summary>
    /// Gets all ADR contents concatenated, for inclusion in prompt context.
    /// </summary>
    public string? GetAllAdrs()
    {
        var adrDir = Path.Combine(_workspaceRoot, "architecture", "adr");
        if (!Directory.Exists(adrDir))
        {
            return null;
        }

        var files = Directory.GetFiles(adrDir, "*.md").Order().ToList();
        if (files.Count == 0)
        {
            return null;
        }

        var parts = new List<string>();
        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            parts.Add($"<!-- {Path.GetFileName(file)} -->\n{content}");
        }

        return string.Join("\n\n---\n\n", parts);
    }

    private string? ReadSafe(string relativePath)
    {
        if (relativePath.Contains("..", StringComparison.Ordinal))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(_workspaceRoot, relativePath));

        // Path sandboxing
        if (!fullPath.StartsWith(_workspaceRoot, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!File.Exists(fullPath))
        {
            return null;
        }

        return File.ReadAllText(fullPath);
    }

    private ArtifactInfo CheckFile(string relativePath)
    {
        var fullPath = Path.Combine(_workspaceRoot, relativePath);
        var name = Path.GetFileNameWithoutExtension(relativePath);
        return new ArtifactInfo
        {
            Name = name,
            Exists = File.Exists(fullPath)
        };
    }

    private List<ArtifactInfo> ListDirectory(string relativeDir)
    {
        var fullDir = Path.Combine(_workspaceRoot, relativeDir);
        if (!Directory.Exists(fullDir))
        {
            return [];
        }

        return Directory.GetFiles(fullDir, "*.md")
            .Where(f => !Path.GetFileName(f).Equals(".gitkeep", StringComparison.OrdinalIgnoreCase))
            .Select(f => new ArtifactInfo
            {
                Name = Path.GetFileNameWithoutExtension(f) ?? f,
                Exists = true
            })
            .OrderBy(a => a.Name)
            .ToList();
    }

    private List<string> ListMarkdownFileNames(string relativeDir)
    {
        var fullDir = Path.Combine(_workspaceRoot, relativeDir);
        if (!Directory.Exists(fullDir))
        {
            return [];
        }

        return Directory.GetFiles(fullDir, "*.md")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n is not null && !n.Equals(".gitkeep", StringComparison.OrdinalIgnoreCase))
            .Cast<string>()
            .Order()
            .ToList();
    }

    private static string EnsureMdExtension(string name)
    {
        if (name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            return name;
        }

        return name + ".md";
    }
}

/// <summary>
/// Describes a project artifact's name and existence status.
/// </summary>
public sealed class ArtifactInfo
{
    public required string Name { get; init; }
    public required bool Exists { get; init; }
}
