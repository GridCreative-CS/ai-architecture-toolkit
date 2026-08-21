using System.Text.Json;
using System.Text.RegularExpressions;
using AiArchitectureToolkit.McpServer.Configuration;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Services;

/// <summary>
/// Discovers and reads project-specific content from the workspace's
/// <c>architecture/</c> and <c>ai/project-context.md</c> files.
/// </summary>
public sealed partial class ProjectContentService
{
    private static readonly string[] PartStatusValues = ["TODO", "IN_PROGRESS", "DONE", "BLOCKED"];

    /// <summary>
    /// The three Step 6a verdicts, longest first — <c>APPROVED</c> is a prefix
    /// of <c>APPROVED WITH NOTES</c>, so order decides correctness here.
    /// </summary>
    private static readonly string[] ReviewVerdicts = ["REJECTED — MUST FIX", "APPROVED WITH NOTES", "APPROVED"];

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
        result["design-system-gate"] = [CheckFile("architecture/design-system-gate.md")];
        result["ui-inventory"] = [CheckFile("architecture/ui-inventory.md")];
        result["project-context"] = [CheckFile("ai/project-context.md")];
        result["remediation-audit"] = [CheckFile("architecture/remediation-audit.md")];
        result["legacy-system-analysis"] = [CheckFile("architecture/legacy-system-analysis.md")];
        result["architecture-final-gate"] = [CheckFile("architecture/architecture-final-gate.md")];

        // Directories
        result["adrs"] = ListDirectory("architecture/adr");
        result["feature-specs"] = ListDirectory("architecture/feature-specs");

        // Compliance reports come in pairs from toolkit v4.3.0: the Step 4
        // architecture report `<slice-id>-<slice-name>.md` and the Step 4a UI
        // report `<slice-id>-<slice-name>-ui.md`. They are listed separately so
        // a client can tell which check has actually run.
        var complianceReports = ListDirectory("architecture/compliance-reports");
        result["compliance-reports"] = complianceReports.Where(a => !IsUiComplianceReport(a.Name)).ToList();
        result["ui-compliance-reports"] = complianceReports.Where(a => IsUiComplianceReport(a.Name)).ToList();

        result["golden-datasets"] = ListDirectory("architecture/golden-datasets");
        result["slice-verification"] = ListDirectory("architecture/slice-verification");

        result["ai-parts"] = ListDecompositionSlices()
            .Select(s => new ArtifactInfo { Name = s, Exists = true })
            .ToList();

        return result;
    }

    private static bool IsUiComplianceReport(string name) =>
        name.EndsWith("-ui", StringComparison.OrdinalIgnoreCase);

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
    /// Gets the design system completeness gate report (UI foundation workflow
    /// Step 1b / UI retrofit workflow Step 2b). The design system is
    /// authoritative only when this report records <c>APPROVED</c> or
    /// <c>APPROVED WITH NOTES</c>.
    /// </summary>
    public string? GetDesignSystemGate() => ReadSafe("architecture/design-system-gate.md");

    /// <summary>
    /// Gets the UI inventory content (UI retrofit workflow Step 1) — the input
    /// the design system is derived from on the retrofit path.
    /// </summary>
    public string? GetUiInventory() => ReadSafe("architecture/ui-inventory.md");

    /// <summary>
    /// Gets the remediation audit content.
    /// </summary>
    public string? GetRemediationAudit() => ReadSafe("architecture/remediation-audit.md");

    /// <summary>
    /// Gets the architecture compliance report (engineering workflow Step 4)
    /// for a slice: <c>architecture/compliance-reports/&lt;name&gt;.md</c>.
    /// </summary>
    public string? GetComplianceReport(string name) => ReadSafe($"architecture/compliance-reports/{EnsureMdExtension(name)}");

    /// <summary>
    /// Gets the UI compliance report (engineering workflow Step 4a) for a
    /// slice: <c>architecture/compliance-reports/&lt;name&gt;-ui.md</c>.
    /// </summary>
    public string? GetUiComplianceReport(string name)
    {
        // The pairing is positional: for a slice, the architecture report is
        // '<slice>.md' and the UI report is '<slice>-ui.md'. Always append the
        // suffix — a slice legitimately named '…-ui' must not resolve to its own
        // architecture report.
        var bareName = name.EndsWith(".md", StringComparison.OrdinalIgnoreCase)
            ? name[..^3]
            : name;

        return ReadSafe($"architecture/compliance-reports/{bareName}-ui.md");
    }

    /// <summary>
    /// Gets the legacy system analysis (architecture Mode D).
    /// </summary>
    public string? GetLegacySystemAnalysis() => ReadSafe("architecture/legacy-system-analysis.md");

    /// <summary>
    /// Gets the architecture-final quality gate report (toolkit v4.5.0).
    /// </summary>
    public string? GetArchitectureFinalGate() => ReadSafe("architecture/architecture-final-gate.md");

    /// <summary>
    /// Gets the Integrated Slice Verification evidence (engineering workflow
    /// Step 6b) for a slice.
    /// </summary>
    public string? GetSliceVerification(string name) => ReadSafe($"architecture/slice-verification/{EnsureMdExtension(name)}");

    /// <summary>
    /// Lists all slice verification evidence file names (without extensions).
    /// </summary>
    public List<string> ListSliceVerifications() => ListMarkdownFileNames("architecture/slice-verification");

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

    /// <summary>
    /// Lists the slice folders under <c>ai-parts/</c> — one per decomposed slice.
    /// </summary>
    public List<string> ListDecompositionSlices()
    {
        var partsDir = Path.Combine(_workspaceRoot, "ai-parts");
        if (!Directory.Exists(partsDir))
        {
            return [];
        }

        return Directory.GetDirectories(partsDir)
            .Select(d => Path.GetFileName(d))
            .Where(n => !string.IsNullOrEmpty(n))
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Gets <c>ai-parts/&lt;slice-id&gt;/OVERVIEW.md</c>.
    /// </summary>
    public string? GetPartsOverview(string sliceId)
    {
        var segment = ResolveSliceFolder(sliceId);
        return segment is null ? null : ReadSafe($"ai-parts/{segment}/OVERVIEW.md");
    }

    /// <summary>
    /// Resolves a slice folder under <c>ai-parts/</c> to its actual directory
    /// name. Feature specs, compliance reports, and verification evidence are
    /// named <c>&lt;slice-id&gt;-&lt;slice-name&gt;</c>, but the decomposition
    /// folder is <c>&lt;slice-id&gt;</c> alone, so a full slice name must fall
    /// back to its slice-id prefix. Returns the real directory name — never the
    /// caller's casing — so composed paths work on case-sensitive filesystems.
    /// </summary>
    private string? ResolveSliceFolder(string sliceId)
    {
        var segment = SafeSegment(sliceId);
        if (segment is null)
        {
            return null;
        }

        var folders = ListDecompositionSlices();

        var exact = folders.FirstOrDefault(f => f.Equals(segment, StringComparison.OrdinalIgnoreCase));
        if (exact is not null)
        {
            return exact;
        }

        // '<slice-id>-<slice-name>' → the '<slice-id>' folder. Longest wins, so a
        // more specific folder is preferred over a shorter shared prefix.
        return folders
            .Where(f => segment.StartsWith(f + "-", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(f => f.Length)
            .FirstOrDefault();
    }

    /// <summary>
    /// Lists the Part file names (without extension) for a slice, excluding
    /// <c>OVERVIEW.md</c> and the <c>reviews/</c> subfolder.
    /// </summary>
    public List<string> ListParts(string sliceId)
    {
        var segment = ResolveSliceFolder(sliceId);
        if (segment is null)
        {
            return [];
        }

        return ListMarkdownFileNames($"ai-parts/{segment}")
            .Where(n => !n.Equals("OVERVIEW", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    /// <summary>
    /// Gets a single Part file by its file name (without extension).
    /// </summary>
    public string? GetPart(string sliceId, string partFileName)
    {
        var slice = ResolveSliceFolder(sliceId);
        var part = SafeSegment(partFileName);
        return slice is null || part is null
            ? null
            : ReadSafe($"ai-parts/{slice}/{EnsureMdExtension(part)}");
    }

    /// <summary>
    /// Gets a Part Quality Report (engineering workflow Step 6):
    /// <c>ai-parts/&lt;slice-id&gt;/reviews/&lt;part-id&gt;-quality-report.md</c>.
    /// </summary>
    public string? GetPartQualityReport(string sliceId, string partId)
    {
        var slice = ResolveSliceFolder(sliceId);
        var part = SafeSegment(partId);
        return slice is null || part is null
            ? null
            : ReadSafe($"ai-parts/{slice}/reviews/{part}-quality-report.md");
    }

    /// <summary>
    /// Gets a Part Code Review (engineering workflow Step 6a):
    /// <c>ai-parts/&lt;slice-id&gt;/reviews/&lt;part-id&gt;-review.md</c>.
    /// </summary>
    public string? GetPartReview(string sliceId, string partId)
    {
        var slice = ResolveSliceFolder(sliceId);
        var part = SafeSegment(partId);
        return slice is null || part is null
            ? null
            : ReadSafe($"ai-parts/{slice}/reviews/{part}-review.md");
    }

    /// <summary>
    /// Reads a slice's whole decomposition: each Part's Status line, its
    /// optional PART_SPEC traceability fields, its Step 6 / Step 6a review
    /// artifacts, and the warnings a client needs to decide whether the next
    /// Part may start. Returns <see langword="null"/> when the slice folder
    /// does not exist.
    /// </summary>
    public SliceDecomposition? GetDecomposition(string sliceId)
    {
        var segment = ResolveSliceFolder(sliceId);
        if (segment is null)
        {
            return null;
        }

        var overview = GetPartsOverview(segment);
        var hasCoverageMap = overview is not null && HasCoverageMapHeading(overview);

        var warnings = new List<string>();
        if (!hasCoverageMap)
        {
            warnings.Add(
                "OVERVIEW.md has no '## Requirement Coverage Map' section — this is a pre-v4.6.0 " +
                "decomposition. The executor must derive the map from the feature spec at its next Part " +
                "and write it into the existing OVERVIEW.md.");
        }

        var parts = new List<PartInfo>();
        foreach (var fileName in ListParts(segment))
        {
            var content = GetPart(segment, fileName);
            if (content is null)
            {
                continue;
            }

            var spec = ParsePartSpec(content);
            var partId = spec?.PartId ?? PartIdFromFileName(fileName);
            var reviewContent = GetPartReview(segment, partId);
            var status = ParseStatusLine(content);
            var verdict = reviewContent is null ? null : ParseReviewVerdict(reviewContent);

            parts.Add(new PartInfo
            {
                PartId = partId,
                FileName = fileName,
                Status = status,
                PartType = spec?.PartType,
                CriteriaCovered = spec?.CriteriaCovered ?? [],
                DeclaresCriteriaCovered = spec?.DeclaresCriteriaCovered ?? false,
                HasQualityReport = GetPartQualityReport(segment, partId) is not null,
                HasReview = reviewContent is not null,
                ReviewVerdict = verdict
            });

            if (status == "DONE" && reviewContent is null)
            {
                warnings.Add(
                    $"{partId} is DONE but has no Part Code Review (Step 6a) at " +
                    $"ai-parts/{segment}/reviews/{partId}-review.md — the next Part may not start.");
            }

            if (verdict == "REJECTED — MUST FIX")
            {
                warnings.Add(
                    $"{partId} review verdict is REJECTED — MUST FIX. The Part may not be marked DONE " +
                    "and the next Part may not start until a re-review approves it.");
            }

            // part_type has a defined fallback (classify from file_touch_points);
            // criteria_covered has none, so its absence is an unowned-criteria risk.
            if (spec?.DeclaresCriteriaCovered != true)
            {
                warnings.Add(
                    $"{partId} declares no PART_SPEC 'criteria_covered' — the criteria it owns cannot be " +
                    "cross-checked against the Requirement Coverage Map (unowned-criteria risk).");
            }
        }

        return new SliceDecomposition
        {
            SliceId = segment,
            OverviewExists = overview is not null,
            HasRequirementCoverageMap = hasCoverageMap,
            Parts = parts,
            Warnings = warnings
        };
    }

    /// <summary>
    /// Extracts the distinct feature spec criterion IDs — <c>DR-nn</c> (§6),
    /// <c>SEC-nn</c> (§9), <c>AC-nn</c> (§11), <c>UIAC-nn</c> (§11b) — present
    /// in a document, ordered.
    /// </summary>
    public static List<string> ExtractCriterionIds(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return CriterionIdRegex().Matches(content)
            .Select(m => m.Value)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    // The \b before the prefix keeps AC-nn from matching inside UIAC-nn.
    [GeneratedRegex(@"\b(?:UIAC|SEC|DR|AC)-\d+\b")]
    private static partial Regex CriterionIdRegex();

    private static bool HasCoverageMapHeading(string overview) =>
        overview.Split('\n')
            .Any(line => line.TrimStart().StartsWith("## Requirement Coverage Map", StringComparison.OrdinalIgnoreCase));

    private static string ParseStatusLine(string partContent)
    {
        foreach (var line in partContent.Split('\n'))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith("Status:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var value = trimmed["Status:".Length..].Trim().Trim('`', '*').Trim();
            var match = PartStatusValues.FirstOrDefault(s => s.Equals(value, StringComparison.OrdinalIgnoreCase));
            return match ?? "UNKNOWN";
        }

        return "UNKNOWN";
    }

    private static string? ParseReviewVerdict(string reviewContent)
    {
        var scope = ExtractSection(reviewContent, "## Verdict") ?? reviewContent;
        return ReviewVerdicts.FirstOrDefault(v => scope.Contains(v, StringComparison.Ordinal));
    }

    private static string? ExtractSection(string content, string heading)
    {
        var lines = content.Split('\n');
        var start = Array.FindIndex(lines, l => l.Trim().StartsWith(heading, StringComparison.OrdinalIgnoreCase));
        if (start < 0)
        {
            return null;
        }

        var body = new List<string>();
        for (var i = start + 1; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("## ", StringComparison.Ordinal))
            {
                break;
            }

            body.Add(lines[i]);
        }

        return string.Join('\n', body);
    }

    private static string PartIdFromFileName(string fileName)
    {
        var dash = fileName.IndexOf('-', StringComparison.Ordinal);
        return dash > 0 ? fileName[..dash] : fileName;
    }

    /// <summary>
    /// Reads the PART_SPEC JSON block from a Part file. Returns
    /// <see langword="null"/> when the section is absent or the JSON does not
    /// parse — a malformed or missing PART_SPEC must never fail the listing.
    /// </summary>
    private static PartSpec? ParsePartSpec(string partContent)
    {
        var section = ExtractSection(partContent, "## PART_SPEC");
        if (section is null)
        {
            return null;
        }

        var json = ExtractFirstJsonObject(section);
        if (json is null)
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            string? partId = root.TryGetProperty("part_id", out var idElement) && idElement.ValueKind == JsonValueKind.String
                ? idElement.GetString()
                : null;

            string? partType = root.TryGetProperty("part_type", out var typeElement) && typeElement.ValueKind == JsonValueKind.String
                ? typeElement.GetString()
                : null;

            var declaresCriteria = root.TryGetProperty("criteria_covered", out var criteriaElement)
                && criteriaElement.ValueKind == JsonValueKind.Array;

            var criteria = declaresCriteria
                ? criteriaElement.EnumerateArray()
                    .Where(e => e.ValueKind == JsonValueKind.String)
                    .Select(e => e.GetString()!)
                    .ToList()
                : [];

            return new PartSpec(partId, partType, criteria, declaresCriteria);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the first balanced <c>{ … }</c> object in the text, ignoring
    /// braces inside JSON strings. Handles the fenced code block the
    /// plan-decomposer skill writes PART_SPEC into.
    /// </summary>
    private static string? ExtractFirstJsonObject(string text)
    {
        var start = text.IndexOf('{', StringComparison.Ordinal);
        if (start < 0)
        {
            return null;
        }

        var depth = 0;
        var inString = false;
        var escaped = false;

        for (var i = start; i < text.Length; i++)
        {
            var c = text[i];

            if (inString)
            {
                if (escaped)
                {
                    escaped = false;
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else if (c == '"')
                {
                    inString = false;
                }

                continue;
            }

            switch (c)
            {
                case '"':
                    inString = true;
                    break;
                case '{':
                    depth++;
                    break;
                case '}':
                    depth--;
                    if (depth == 0)
                    {
                        return text[start..(i + 1)];
                    }

                    break;
            }
        }

        return null;
    }

    /// <summary>
    /// Validates a single path segment supplied by a caller (slice id, Part id,
    /// file name). Rejects traversal, rooted paths, and separators.
    /// </summary>
    private static string? SafeSegment(string? segment)
    {
        if (string.IsNullOrWhiteSpace(segment) ||
            segment.Contains("..", StringComparison.Ordinal) ||
            segment.Contains('/', StringComparison.Ordinal) ||
            segment.Contains('\\', StringComparison.Ordinal) ||
            Path.IsPathRooted(segment))
        {
            return null;
        }

        return segment;
    }

    private sealed record PartSpec(
        string? PartId,
        string? PartType,
        IReadOnlyList<string> CriteriaCovered,
        bool DeclaresCriteriaCovered);

    private string? ReadSafe(string relativePath)
    {
        if (relativePath.Contains("..", StringComparison.Ordinal))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(_workspaceRoot, relativePath));

        // Path sandboxing. The trailing separator keeps a sibling directory
        // whose name merely starts with the workspace root's name from passing
        // as "inside" it; Ordinal because both operands derive from the same
        // _workspaceRoot string, so an exact prefix is the strictest correct
        // test. Matches ToolkitContentService.ResolveWithinBase.
        var rootPrefix = _workspaceRoot.EndsWith(Path.DirectorySeparatorChar)
            ? _workspaceRoot
            : _workspaceRoot + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(rootPrefix, StringComparison.Ordinal))
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
