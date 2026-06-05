using System.ComponentModel;
using AiArchitectureToolkit.McpServer.Services;
using ModelContextProtocol.Server;

namespace AiArchitectureToolkit.McpServer.Tools;

/// <summary>
/// MCP tools for lightweight validation checks.
/// </summary>
[McpServerToolType]
public sealed class ValidationTools
{
    /// <summary>
    /// Evaluates a slice description against the verticality test.
    /// Returns the test criteria and the slice description for the LLM to evaluate.
    /// </summary>
    [McpServerTool, Description("Provides the 3-question verticality test from the toolkit for evaluating whether a slice qualifies as a vertical slice. Returns the test criteria alongside the slice description for structured evaluation.")]
    public static string CheckVerticality(
        ToolkitContentService toolkitService,
        [Description("Description of the slice to evaluate for verticality")] string sliceDescription)
    {
        var verticalSliceGuide = toolkitService.GetContent("guides", "vertical-slice-definition");

        return $"""
            # Verticality Check

            ## Slice Under Evaluation

            {sliceDescription}

            ## Verticality Test (from vertical-slice-definition.md)

            Apply each question to the slice above and determine pass/fail:

            1. **User-observable capability** — Does this slice deliver a capability that a user, operator, or stakeholder can exercise or observe?
            2. **Human-in-the-loop completeness** — If the architecture specifies human interaction for this capability (approval, override, review, emergency control), does the slice include the minimal UI to prove that loop?
            3. **User-facing verification** — Can this slice be called "done" with a user-facing verification, not just an API or integration test?

            If any answer is NO, the slice must be restructured before proceeding.

            ## Exceptions

            Infrastructure bootstrap and production hardening are legitimate **phases**, not slices. Label them accordingly in the delivery plan.

            ## Full Reference

            {verticalSliceGuide ?? "(vertical-slice-definition.md not found — apply the 3 questions above manually)"}
            """;
    }
}
