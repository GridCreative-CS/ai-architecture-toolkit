using AiArchitectureToolkit.McpServer.Configuration;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Services;

/// <summary>
/// Resolves and reads toolkit content from the <c>ai/</c> and <c>.github/</c> directories.
/// </summary>
public sealed class ToolkitContentService
{
    private static readonly string[] ToolkitCategories =
        ["prompts", "templates", "guides", "workflows", "agents", "examples"];

    private readonly string _toolkitRoot;
    private readonly string _githubRoot;

    public ToolkitContentService(IOptions<ServerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _toolkitRoot = Path.GetFullPath(options.Value.ToolkitRoot);
        _githubRoot = Path.GetFullPath(options.Value.GitHubRoot);
    }

    /// <summary>
    /// Lists all available toolkit files grouped by category.
    /// </summary>
    public Dictionary<string, List<string>> ListAllContent()
    {
        var result = new Dictionary<string, List<string>>();

        foreach (var category in ToolkitCategories)
        {
            var dir = Path.Combine(_toolkitRoot, category);
            result[category] = ListMarkdownFiles(dir);
        }

        var instructionsDir = Path.Combine(_githubRoot, "instructions");
        result["instructions"] = ListMarkdownFiles(instructionsDir);

        var agentsDir = Path.Combine(_githubRoot, "agents");
        result["github-agents"] = ListMarkdownFiles(agentsDir);

        return result;
    }

    /// <summary>
    /// Gets the content of a toolkit file by category and name.
    /// Returns <see langword="null"/> if the file does not exist.
    /// </summary>
    public string? GetContent(string category, string name)
    {
        var filePath = ResolveFilePath(category, name);
        if (filePath is null || !File.Exists(filePath))
        {
            return null;
        }

        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Searches all toolkit markdown files for a query string (case-insensitive).
    /// Returns matching file names with context snippets.
    /// </summary>
    public List<SearchResult> Search(string query)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        var results = new List<SearchResult>();

        foreach (var category in ToolkitCategories)
        {
            var dir = Path.Combine(_toolkitRoot, category);
            SearchDirectory(dir, category, query, results);
        }

        SearchDirectory(Path.Combine(_githubRoot, "instructions"), "instructions", query, results);
        SearchDirectory(Path.Combine(_githubRoot, "agents"), "github-agents", query, results);

        return results;
    }

    /// <summary>
    /// Parses the glossary file and returns definitions keyed by term name.
    /// </summary>
    public Dictionary<string, string> ParseGlossary()
    {
        var content = GetContent("guides", "glossary");
        if (content is null)
        {
            return new Dictionary<string, string>();
        }

        return ParseByHeadings(content);
    }

    private string? ResolveFilePath(string category, string name)
    {
        var sanitizedName = SanitizeFileName(name);
        if (sanitizedName is null)
        {
            return null;
        }

        if (!sanitizedName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            sanitizedName += ".md";
        }

        string baseDir = category switch
        {
            "instructions" => Path.Combine(_githubRoot, "instructions"),
            "github-agents" => Path.Combine(_githubRoot, "agents"),
            _ => Path.Combine(_toolkitRoot, category)
        };

        var fullPath = Path.GetFullPath(Path.Combine(baseDir, sanitizedName));

        // Path sandboxing: ensure the resolved path is within the expected directory
        if (!fullPath.StartsWith(Path.GetFullPath(baseDir), StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return fullPath;
    }

    private static string? SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        // Reject path traversal attempts
        if (name.Contains("..", StringComparison.Ordinal) ||
            Path.IsPathRooted(name))
        {
            return null;
        }

        return name;
    }

    private static List<string> ListMarkdownFiles(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        return Directory.GetFiles(directory, "*.md")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(n => n is not null)
            .Cast<string>()
            .Order()
            .ToList();
    }

    private static void SearchDirectory(string directory, string category, string query, List<SearchResult> results)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var file in Directory.GetFiles(directory, "*.md"))
        {
            var content = File.ReadAllText(file);
            var index = content.IndexOf(query, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                continue;
            }

            var snippetStart = Math.Max(0, index - 100);
            var snippetEnd = Math.Min(content.Length, index + query.Length + 100);
            var snippet = content[snippetStart..snippetEnd].ReplaceLineEndings(" ");

            results.Add(new SearchResult
            {
                Category = category,
                FileName = Path.GetFileNameWithoutExtension(file) ?? file,
                Snippet = snippet
            });
        }
    }

    private static Dictionary<string, string> ParseByHeadings(string content)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lines = content.Split('\n');
        string? currentTerm = null;
        var currentBody = new List<string>();

        foreach (var line in lines)
        {
            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                if (currentTerm is not null)
                {
                    result[currentTerm] = string.Join('\n', currentBody).Trim();
                }

                currentTerm = line[3..].Trim();
                currentBody.Clear();
            }
            else if (currentTerm is not null)
            {
                currentBody.Add(line);
            }
        }

        if (currentTerm is not null)
        {
            result[currentTerm] = string.Join('\n', currentBody).Trim();
        }

        return result;
    }
}

/// <summary>
/// Represents a search result with category, file name, and a context snippet.
/// </summary>
public sealed class SearchResult
{
    public required string Category { get; init; }
    public required string FileName { get; init; }
    public required string Snippet { get; init; }
}
