namespace AiArchitectureToolkit.McpServer.Configuration;

/// <summary>
/// Configuration options for the MCP server's path resolution.
/// </summary>
public sealed class ServerOptions
{
    /// <summary>
    /// Path to the toolkit's <c>ai/</c> directory.
    /// Defaults to <c>../ai</c> relative to the server binary.
    /// </summary>
    public string ToolkitRoot { get; set; } = string.Empty;

    /// <summary>
    /// Path to the consumer project's workspace root.
    /// Defaults to the current working directory.
    /// </summary>
    public string WorkspaceRoot { get; set; } = string.Empty;

    /// <summary>
    /// Path to the <c>.github/</c> directory for instructions and agents.
    /// Defaults to <c>../.github</c> relative to the server binary.
    /// </summary>
    public string GitHubRoot { get; set; } = string.Empty;
}
