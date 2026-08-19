using System.ComponentModel;
using ModelContextProtocol.Server;

namespace AiArchitectureToolkit.McpServer.Resources;

using Services;

/// <summary>
/// MCP resources exposing static toolkit content (guides, prompts, templates, etc.).
/// </summary>
[McpServerResourceType]
public sealed class ToolkitResources
{
    [McpServerResource(UriTemplate = "toolkit://guides/{name}"), Description("A toolkit guide document")]
    public static string GetGuide(ToolkitContentService service, string name)
    {
        return service.GetContent("guides", name)
            ?? $"Guide '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://prompts/{name}"), Description("A toolkit prompt document")]
    public static string GetPrompt(ToolkitContentService service, string name)
    {
        return service.GetContent("prompts", name)
            ?? $"Prompt '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://templates/{name}"), Description("A toolkit template document")]
    public static string GetTemplate(ToolkitContentService service, string name)
    {
        return service.GetContent("templates", name)
            ?? $"Template '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://workflows/{name}"), Description("A toolkit workflow document")]
    public static string GetWorkflow(ToolkitContentService service, string name)
    {
        return service.GetContent("workflows", name)
            ?? $"Workflow '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://agents/{name}"), Description("A toolkit agent persona document")]
    public static string GetAgent(ToolkitContentService service, string name)
    {
        return service.GetContent("agents", name)
            ?? $"Agent '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://examples/{name}"), Description("A toolkit example document")]
    public static string GetExample(ToolkitContentService service, string name)
    {
        return service.GetContent("examples", name)
            ?? $"Example '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://skills/{name}"), Description("An execution skill definition from .github/skills/<name>/SKILL.md (plan-decomposer, part-executor-tdd) — these define the Part handoff contract")]
    public static string GetSkill(ToolkitContentService service, string name)
    {
        return service.GetContent("skills", name)
            ?? $"Skill '{name}' not found.";
    }

    [McpServerResource(UriTemplate = "toolkit://instructions/{name}"), Description("A coding instruction document from .github/instructions/")]
    public static string GetInstruction(ToolkitContentService service, string name)
    {
        return service.GetContent("instructions", name)
            ?? $"Instruction '{name}' not found.";
    }
}
