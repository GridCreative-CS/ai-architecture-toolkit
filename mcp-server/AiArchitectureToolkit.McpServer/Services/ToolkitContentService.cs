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

    /// <summary>
    /// Extensions served from the toolkit categories. <c>.json</c> is included
    /// because <c>ai/examples/example-golden-dataset-case.json</c> and
    /// <c>ai/templates/golden-dataset-json-template.json</c> are load-bearing
    /// for the <c>golden-dataset</c> workflow step.
    /// </summary>
    private static readonly string[] ContentExtensions = [".md", ".json"];

    /// <summary>
    /// The execution skills live at <c>.github/skills/&lt;name&gt;/SKILL.md</c> —
    /// a nested layout, unlike every other category.
    /// </summary>
    private const string SkillsCategory = "skills";

    private const string SkillFileName = "SKILL.md";

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
            result[category] = ListContentFiles(dir);
        }

        var instructionsDir = Path.Combine(_githubRoot, "instructions");
        result["instructions"] = ListContentFiles(instructionsDir);

        var agentsDir = Path.Combine(_githubRoot, "agents");
        result["github-agents"] = ListContentFiles(agentsDir);

        result[SkillsCategory] = ListSkills();

        return result;
    }

    /// <summary>
    /// Lists the execution skills — the subdirectories of <c>.github/skills/</c>
    /// that contain a <c>SKILL.md</c>.
    /// </summary>
    private List<string> ListSkills()
    {
        var skillsDir = Path.Combine(_githubRoot, SkillsCategory);
        if (!Directory.Exists(skillsDir))
        {
            return [];
        }

        return Directory.GetDirectories(skillsDir)
            .Where(d => File.Exists(Path.Combine(d, SkillFileName)))
            .Select(Path.GetFileName)
            .Where(n => !string.IsNullOrEmpty(n))
            .Cast<string>()
            .Order(StringComparer.Ordinal)
            .ToList();
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
        SearchSkills(query, results);

        return results;
    }

    private void SearchSkills(string query, List<SearchResult> results)
    {
        foreach (var skill in ListSkills())
        {
            var content = GetContent(SkillsCategory, skill);
            if (content is null)
            {
                continue;
            }

            var snippet = BuildSnippet(content, query);
            if (snippet is not null)
            {
                results.Add(new SearchResult
                {
                    Category = SkillsCategory,
                    FileName = skill,
                    Snippet = snippet
                });
            }
        }
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

        if (category == SkillsCategory)
        {
            return ResolveWithinBase(
                Path.Combine(_githubRoot, SkillsCategory),
                Path.Combine(sanitizedName, SkillFileName));
        }

        // Names are listed without their extension for markdown, and with it for
        // every other served extension — so a listed name always resolves back.
        if (!ContentExtensions.Any(e => sanitizedName.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
        {
            sanitizedName += ".md";
        }

        // The category is caller-supplied, so it is matched against the closed
        // set of served categories rather than combined into a path directly:
        // a rooted or traversing category would otherwise become the base
        // directory itself and defeat the containment check below.
        string? baseDir = category switch
        {
            "instructions" => Path.Combine(_githubRoot, "instructions"),
            "github-agents" => Path.Combine(_githubRoot, "agents"),
            _ when ToolkitCategories.Contains(category, StringComparer.Ordinal)
                => Path.Combine(_toolkitRoot, category),
            _ => null
        };

        return baseDir is null ? null : ResolveWithinBase(baseDir, sanitizedName);
    }

    /// <summary>
    /// Combines a base directory with a relative path and rejects the result if
    /// it escapes that base.
    /// </summary>
    private static string? ResolveWithinBase(string baseDir, string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(baseDir, relativePath));
        var fullBase = Path.GetFullPath(baseDir);
        if (!fullBase.EndsWith(Path.DirectorySeparatorChar))
        {
            fullBase += Path.DirectorySeparatorChar;
        }

        // The trailing separator keeps a sibling directory whose name merely
        // starts with the base directory's name from passing as "inside" it.
        // Ordinal, not OrdinalIgnoreCase: both operands derive from the same
        // baseDir string, so an exact prefix is the strictest correct test and
        // never depends on guessing whether the filesystem folds case.
        if (!fullPath.StartsWith(fullBase, StringComparison.Ordinal))
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

        // Every served name is a single segment: the files are flat within
        // their category directory and a skill is a directory name.
        if (name.Contains("..", StringComparison.Ordinal) ||
            name.Contains('/', StringComparison.Ordinal) ||
            name.Contains('\\', StringComparison.Ordinal) ||
            Path.IsPathRooted(name))
        {
            return null;
        }

        return name;
    }

    /// <summary>
    /// Lists the served files in a directory. Markdown files are listed without
    /// their extension (the long-standing convention); other served extensions
    /// keep theirs, so every listed name resolves back to its file.
    /// </summary>
    private static List<string> ListContentFiles(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return [];
        }

        return EnumerateContentFiles(directory)
            .Select(f => f.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
                ? Path.GetFileNameWithoutExtension(f)
                : Path.GetFileName(f))
            .Where(n => !string.IsNullOrEmpty(n))
            .Cast<string>()
            .Order()
            .ToList();
    }

    private static IEnumerable<string> EnumerateContentFiles(string directory) =>
        ContentExtensions.SelectMany(e => Directory.GetFiles(directory, $"*{e}"));

    private static void SearchDirectory(string directory, string category, string query, List<SearchResult> results)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var file in EnumerateContentFiles(directory))
        {
            var snippet = BuildSnippet(File.ReadAllText(file), query);
            if (snippet is null)
            {
                continue;
            }

            results.Add(new SearchResult
            {
                Category = category,
                FileName = file.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
                    ? Path.GetFileNameWithoutExtension(file) ?? file
                    : Path.GetFileName(file),
                Snippet = snippet
            });
        }
    }

    /// <summary>
    /// Returns a context snippet around the first match, or
    /// <see langword="null"/> when the content does not match.
    /// </summary>
    private static string? BuildSnippet(string content, string query)
    {
        var index = content.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return null;
        }

        var snippetStart = Math.Max(0, index - 100);
        var snippetEnd = Math.Min(content.Length, index + query.Length + 100);
        return content[snippetStart..snippetEnd].ReplaceLineEndings(" ");
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
