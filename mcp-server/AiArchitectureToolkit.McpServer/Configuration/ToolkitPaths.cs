namespace AiArchitectureToolkit.McpServer.Configuration;

/// <summary>
/// Locates the toolkit tree the server ships next to, for use when
/// <c>TOOLKIT_ROOT</c> and <c>GITHUB_ROOT</c> are not configured explicitly.
/// </summary>
public static class ToolkitPaths
{
    /// <summary>
    /// Walks up from <paramref name="startDirectory"/> to the first directory
    /// containing both <c>ai/</c> and <c>.github/</c> — the toolkit repository
    /// root. Returns <see langword="null"/> when no such directory is found.
    /// </summary>
    /// <remarks>
    /// Walking is used rather than a fixed number of <c>..</c> segments because
    /// the build output depth varies by configuration, target framework, and
    /// publish layout.
    /// </remarks>
    public static string? FindToolkitRoot(string startDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startDirectory);

        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "ai")) &&
                Directory.Exists(Path.Combine(directory.FullName, ".github")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
