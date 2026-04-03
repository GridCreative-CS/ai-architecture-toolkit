using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

// Route all logs to stderr so they don't interfere with stdio MCP transport
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

// Resolve configuration paths
var serverBinaryDir = AppContext.BaseDirectory;

var toolkitRoot = Environment.GetEnvironmentVariable("TOOLKIT_ROOT")
    ?? Path.GetFullPath(Path.Combine(serverBinaryDir, "..", "..", "..", "..", "ai"));

var workspaceRoot = Environment.GetEnvironmentVariable("WORKSPACE_ROOT")
    ?? Directory.GetCurrentDirectory();

var githubRoot = Environment.GetEnvironmentVariable("GITHUB_ROOT")
    ?? Path.GetFullPath(Path.Combine(serverBinaryDir, "..", "..", "..", "..", ".github"));

builder.Services.Configure<ServerOptions>(options =>
{
    options.ToolkitRoot = toolkitRoot;
    options.WorkspaceRoot = workspaceRoot;
    options.GitHubRoot = githubRoot;
});

// Register services
builder.Services.AddSingleton<ToolkitContentService>();
builder.Services.AddSingleton<ProjectContentService>();
builder.Services.AddSingleton<WorkspaceScanService>();

// Configure MCP server with stdio transport and auto-discovery
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "ai-architecture-toolkit",
            Version = "1.0.0"
        };
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();

await builder.Build().RunAsync();
