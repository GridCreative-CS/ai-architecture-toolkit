using System.ComponentModel;
using System.Text.Json;
using AiArchitectureToolkit.McpServer.Services;
using ModelContextProtocol.Server;

namespace AiArchitectureToolkit.McpServer.Tools;

/// <summary>
/// MCP tools for searching and discovering toolkit content.
/// </summary>
[McpServerToolType]
public sealed class ToolkitTools
{
    /// <summary>
    /// Lists all available toolkit files grouped by category with file counts.
    /// </summary>
    [McpServerTool, Description("Lists all available toolkit files by category (guides, prompts, templates, workflows, agents, examples, instructions, github-agents). Returns category names with their file lists.")]
    public static string ListToolkitContent(ToolkitContentService toolkitService)
    {
        var content = toolkitService.ListAllContent();

        var summary = content.ToDictionary(
            kv => kv.Key,
            kv => new { count = kv.Value.Count, files = kv.Value });

        return JsonSerializer.Serialize(summary, JsonOptions.Default);
    }

    /// <summary>
    /// Full-text search across all toolkit markdown files.
    /// </summary>
    [McpServerTool, Description("Searches all toolkit markdown files for a query string (case-insensitive). Returns matching file names with context snippets showing where the match was found.")]
    public static string SearchToolkit(
        ToolkitContentService toolkitService,
        [Description("The text to search for across all toolkit files")] string query)
    {
        var results = toolkitService.Search(query);
        return JsonSerializer.Serialize(results, JsonOptions.Default);
    }

    /// <summary>
    /// Looks up a specific term from the glossary.
    /// </summary>
    [McpServerTool, Description("Looks up a specific term from the toolkit glossary (ai/guides/glossary.md). Returns the full definition for the requested term, or lists all available terms if no match is found.")]
    public static string GetGlossaryTerm(
        ToolkitContentService toolkitService,
        [Description("The glossary term to look up (e.g., 'vertical slice', 'contract', 'modular monolith')")] string term)
    {
        var glossary = toolkitService.ParseGlossary();

        if (glossary.TryGetValue(term, out var definition))
        {
            return $"## {term}\n\n{definition}";
        }

        // Try partial match
        var matches = glossary.Keys
            .Where(k => k.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count > 0)
        {
            var parts = matches.Select(m => $"## {m}\n\n{glossary[m]}");
            return string.Join("\n\n---\n\n", parts);
        }

        return $"Term '{term}' not found. Available terms:\n" +
               string.Join("\n", glossary.Keys.Order().Select(k => $"- {k}"));
    }

    /// <summary>
    /// Gets the content of a specific toolkit file by category and name.
    /// </summary>
    [McpServerTool, Description("Gets the full content of a specific toolkit file. Category is one of: prompts, templates, guides, workflows, agents, examples, instructions, github-agents. Name is the file name without .md extension.")]
    public static string GetToolkitFile(
        ToolkitContentService toolkitService,
        [Description("Category: prompts, templates, guides, workflows, agents, examples, instructions, or github-agents")] string category,
        [Description("File name without .md extension (e.g., 'glossary', 'delivery-planner', 'feature-spec-template')")] string name)
    {
        var content = toolkitService.GetContent(category, name);
        return content ?? $"File '{name}' not found in category '{category}'.";
    }
}

internal static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
