using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Services;

/// <summary>
/// Covers the per-slice <c>ai-parts/</c> decomposition surface: Part status,
/// Step 6 / Step 6a review artifacts, and the v4.6.0 requirement-traceability
/// fields.
/// </summary>
public sealed class ProjectContentServiceAiPartsTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ProjectContentService _service;

    public ProjectContentServiceAiPartsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ai-parts-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        // Slice s1 — a fully v4.6.0-shaped decomposition.
        CreateFile("ai-parts/s1-user-registration", "OVERVIEW.md", """
            # AI Parts Overview

            ## Preflight

            ## Requirement Coverage Map

            | Criterion | Text (short) | Owning Part(s) | Verified at |
            | --- | --- | --- | --- |
            | AC-01 | Registers a user | P01 | P01 tests |

            ## Parts Index
            """);
        CreateFile("ai-parts/s1-user-registration", "P01-domain-model.md", PartFile(
            partId: "P01",
            title: "Domain model",
            status: "DONE",
            spec: """
                {
                  "part_id": "P01",
                  "title": "Domain model",
                  "part_type": "backend",
                  "criteria_covered": ["AC-01", "SEC-01"]
                }
                """));
        CreateFile("ai-parts/s1-user-registration", "P02-api-endpoint.md", PartFile(
            partId: "P02",
            title: "API endpoint",
            status: "TODO",
            spec: """
                {
                  "part_id": "P02",
                  "title": "API endpoint",
                  "part_type": "shared-contract",
                  "criteria_covered": ["AC-02"]
                }
                """));
        CreateFile("ai-parts/s1-user-registration/reviews", "P01-quality-report.md",
            "# Part Quality Report — P01\n\nPart status: DONE");
        CreateFile("ai-parts/s1-user-registration/reviews", "P01-review.md",
            "# Part Code Review — P01\n\n## Verdict\n\n`APPROVED WITH NOTES` — one Minor note deferred to P03.");

        // Slice s2 — a pre-v4.6.0 decomposition: no coverage map, no optional
        // PART_SPEC fields, a DONE Part with no review, and a rejected Part.
        CreateFile("ai-parts/s2-legacy-slice", "OVERVIEW.md", """
            # AI Parts Overview

            ## Preflight

            ## Parts Index
            """);
        CreateFile("ai-parts/s2-legacy-slice", "P01-groundwork.md", PartFile(
            partId: "P01",
            title: "Groundwork",
            status: "DONE",
            spec: """
                {
                  "part_id": "P01",
                  "title": "Groundwork"
                }
                """));
        CreateFile("ai-parts/s2-legacy-slice", "P02-rejected-work.md", PartFile(
            partId: "P02",
            title: "Rejected work",
            status: "IN_PROGRESS",
            spec: """
                {
                  "part_id": "P02",
                  "title": "Rejected work"
                }
                """));
        CreateFile("ai-parts/s2-legacy-slice/reviews", "P02-review.md",
            "# Part Code Review — P02\n\n## Verdict\n\n`REJECTED — MUST FIX` — one Blocker in check 12.");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = Path.Combine(_tempDir, "ai"),
            GitHubRoot = Path.Combine(_tempDir, ".github"),
            WorkspaceRoot = _tempDir
        });

        _service = new ProjectContentService(options);
    }

    [Fact]
    public void ListDecompositionSlices_ReturnsSliceFolderNames()
    {
        var slices = _service.ListDecompositionSlices();

        Assert.Equal(["s1-user-registration", "s2-legacy-slice"], slices);
    }

    [Fact]
    public void ListDecompositionSlices_WhenNoAiPartsDirectory_ReturnsEmpty()
    {
        var emptyDir = Path.Combine(_tempDir, "empty-workspace");
        Directory.CreateDirectory(emptyDir);
        var service = new ProjectContentService(Options.Create(new ServerOptions
        {
            ToolkitRoot = emptyDir,
            GitHubRoot = emptyDir,
            WorkspaceRoot = emptyDir
        }));

        Assert.Empty(service.ListDecompositionSlices());
    }

    [Fact]
    public void GetPartsOverview_ReturnsContent()
    {
        var content = _service.GetPartsOverview("s1-user-registration");

        Assert.NotNull(content);
        Assert.Contains("AI Parts Overview", content);
    }

    [Fact]
    public void ListParts_ExcludesOverviewAndReviews()
    {
        var parts = _service.ListParts("s1-user-registration");

        Assert.Equal(["P01-domain-model", "P02-api-endpoint"], parts);
    }

    [Fact]
    public void GetPart_ReturnsContent()
    {
        var content = _service.GetPart("s1-user-registration", "P01-domain-model");

        Assert.NotNull(content);
        Assert.Contains("Domain model", content);
    }

    [Fact]
    public void GetPart_PathTraversal_ReturnsNull()
    {
        Assert.Null(_service.GetPart("../../etc", "passwd"));
        Assert.Null(_service.GetPart("s1-user-registration", "../../../etc/passwd"));
    }

    [Fact]
    public void GetPartsOverview_PathTraversal_ReturnsNull()
    {
        Assert.Null(_service.GetPartsOverview("../../etc"));
    }

    [Fact]
    public void GetDecomposition_SurfacesPartStatusLines()
    {
        var decomposition = _service.GetDecomposition("s1-user-registration");

        Assert.NotNull(decomposition);
        Assert.Equal("DONE", decomposition.Parts[0].Status);
        Assert.Equal("TODO", decomposition.Parts[1].Status);
        Assert.Equal("P01", decomposition.Parts[0].PartId);
    }

    [Fact]
    public void GetDecomposition_UnknownSlice_ReturnsNull()
    {
        Assert.Null(_service.GetDecomposition("no-such-slice"));
    }

    [Fact]
    public void GetDecomposition_SurfacesReviewVerdictAlongsideStatus()
    {
        var part = _service.GetDecomposition("s1-user-registration")!.Parts[0];

        Assert.True(part.HasQualityReport);
        Assert.True(part.HasReview);
        Assert.Equal("APPROVED WITH NOTES", part.ReviewVerdict);
    }

    [Fact]
    public void GetDecomposition_ApprovedWithNotesIsNotReportedAsPlainApproved()
    {
        // 'APPROVED' is a prefix of 'APPROVED WITH NOTES' — the longer verdict wins.
        var part = _service.GetDecomposition("s1-user-registration")!.Parts[0];

        Assert.NotEqual("APPROVED", part.ReviewVerdict);
    }

    [Fact]
    public void GetDecomposition_DonePartWithoutReview_IsWarned()
    {
        var decomposition = _service.GetDecomposition("s2-legacy-slice");

        var part = decomposition!.Parts.Single(p => p.PartId == "P01");
        Assert.Equal("DONE", part.Status);
        Assert.False(part.HasReview);
        Assert.Null(part.ReviewVerdict);
        Assert.Contains(decomposition.Warnings, w => w.Contains("P01") && w.Contains("no Part Code Review"));
    }

    [Fact]
    public void GetDecomposition_RejectedReview_IsWarned()
    {
        var decomposition = _service.GetDecomposition("s2-legacy-slice");

        var part = decomposition!.Parts.Single(p => p.PartId == "P02");
        Assert.Equal("REJECTED — MUST FIX", part.ReviewVerdict);
        Assert.Contains(decomposition.Warnings, w => w.Contains("P02") && w.Contains("REJECTED"));
    }

    [Fact]
    public void GetDecomposition_MissingRequirementCoverageMap_IsWarned()
    {
        var withMap = _service.GetDecomposition("s1-user-registration");
        var withoutMap = _service.GetDecomposition("s2-legacy-slice");

        Assert.True(withMap!.HasRequirementCoverageMap);
        Assert.False(withoutMap!.HasRequirementCoverageMap);
        Assert.Contains(withoutMap.Warnings, w => w.Contains("Requirement Coverage Map"));
        Assert.DoesNotContain(withMap.Warnings, w => w.Contains("Requirement Coverage Map"));
    }

    [Fact]
    public void GetDecomposition_SurfacesOptionalPartSpecFields()
    {
        var parts = _service.GetDecomposition("s1-user-registration")!.Parts;

        Assert.Equal("backend", parts[0].PartType);
        Assert.Equal(["AC-01", "SEC-01"], parts[0].CriteriaCovered);
        Assert.Equal("shared-contract", parts[1].PartType);
    }

    [Fact]
    public void GetDecomposition_PartSpecWithoutOptionalFields_DoesNotFail()
    {
        var decomposition = _service.GetDecomposition("s2-legacy-slice");

        var part = decomposition!.Parts.Single(p => p.PartId == "P01");
        Assert.Null(part.PartType);
        Assert.Empty(part.CriteriaCovered);
        Assert.False(part.DeclaresCriteriaCovered);
    }

    [Fact]
    public void GetDecomposition_AbsentCriteriaCovered_IsWarnedAsUnownedCriteriaRisk()
    {
        var decomposition = _service.GetDecomposition("s2-legacy-slice");

        Assert.Contains(decomposition!.Warnings, w => w.Contains("P01") && w.Contains("criteria_covered"));
    }

    [Fact]
    public void GetDecomposition_AbsentPartType_IsNotWarned()
    {
        // part_type has a defined fallback (classify from file_touch_points); its
        // absence is not a risk.
        var decomposition = _service.GetDecomposition("s2-legacy-slice");

        Assert.DoesNotContain(decomposition!.Warnings, w => w.Contains("part_type"));
    }

    [Fact]
    public void GetDecomposition_UnparseablePartSpec_DoesNotFail()
    {
        CreateFile("ai-parts/s3-broken", "OVERVIEW.md", "# AI Parts Overview");
        CreateFile("ai-parts/s3-broken", "P01-broken.md",
            "# Part P01 — Broken\nStatus: TODO\n\n## PART_SPEC\n\n```json\n{ not json at all,\n```\n");

        var decomposition = _service.GetDecomposition("s3-broken");

        Assert.NotNull(decomposition);
        var part = Assert.Single(decomposition.Parts);
        Assert.Equal("TODO", part.Status);
        Assert.Null(part.PartType);
    }

    [Fact]
    public void GetDecomposition_ResolvesTheSliceIdFolderFromAFullSliceName()
    {
        // Feature specs are named <slice-id>-<slice-name>, but the decomposition
        // folder is <slice-id> alone (VERSION.md v4.3.0).
        CreateFile("ai-parts/S2.6", "OVERVIEW.md", "# AI Parts Overview\n\n## Requirement Coverage Map\n");
        CreateFile("ai-parts/S2.6", "P01-history.md", PartFile("P01", "History", "TODO", "{ \"part_id\": \"P01\" }"));

        var decomposition = _service.GetDecomposition("S2.6-inspection-history");

        Assert.NotNull(decomposition);
        Assert.Equal("S2.6", decomposition.SliceId);
        Assert.Single(decomposition.Parts);
    }

    [Fact]
    public void GetDecomposition_ReportsTheActualFolderNameNotTheCallersCasing()
    {
        CreateFile("ai-parts/S2.6", "OVERVIEW.md", "# AI Parts Overview");

        var decomposition = _service.GetDecomposition("s2.6");

        Assert.NotNull(decomposition);
        Assert.Equal("S2.6", decomposition.SliceId);
    }

    [Fact]
    public void GetDecomposition_PrefersAnExactFolderMatchOverAPrefixMatch()
    {
        CreateFile("ai-parts/S4", "OVERVIEW.md", "# AI Parts Overview");
        CreateFile("ai-parts/S4-billing", "OVERVIEW.md", "# AI Parts Overview");

        Assert.Equal("S4-billing", _service.GetDecomposition("S4-billing")!.SliceId);
        Assert.Equal("S4", _service.GetDecomposition("S4-shipping")!.SliceId);
    }

    [Fact]
    public void GetPartsOverview_ResolvesTheSliceIdFolderFromAFullSliceName()
    {
        CreateFile("ai-parts/S7", "OVERVIEW.md", "# AI Parts Overview\n\nSeven.");

        Assert.Contains("Seven.", _service.GetPartsOverview("S7-reporting")!);
    }

    [Fact]
    public void GetPartQualityReport_AndGetPartReview_ReturnContent()
    {
        Assert.Contains("Part Quality Report", _service.GetPartQualityReport("s1-user-registration", "P01")!);
        Assert.Contains("Part Code Review", _service.GetPartReview("s1-user-registration", "P01")!);
        Assert.Null(_service.GetPartReview("s1-user-registration", "P02"));
    }

    [Theory]
    [InlineData("DR-01")]
    [InlineData("SEC-02")]
    [InlineData("AC-03")]
    [InlineData("UIAC-04")]
    public void ExtractCriterionIds_FindsEachPrefix(string id)
    {
        var ids = ProjectContentService.ExtractCriterionIds($"- {id}: some criterion text.");

        Assert.Equal([id], ids);
    }

    [Fact]
    public void ExtractCriterionIds_DoesNotMatchAcInsideUiac()
    {
        var ids = ProjectContentService.ExtractCriterionIds("- UIAC-04: the panel shows a spinner.");

        Assert.Equal(["UIAC-04"], ids);
    }

    [Fact]
    public void ExtractCriterionIds_DeduplicatesAndOrders()
    {
        var ids = ProjectContentService.ExtractCriterionIds("AC-02 then AC-01 then AC-02 again, plus DR-01.");

        Assert.Equal(["AC-01", "AC-02", "DR-01"], ids);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }

    private static string PartFile(string partId, string title, string status, string spec) => $"""
        # Part {partId} — {title}
        Status: {status}

        ## Summary
        - Goal: do the thing.

        ## PART_SPEC

        ```json
        {spec}
        ```
        """;

    private void CreateFile(string relativeDir, string fileName, string content)
    {
        var dir = Path.Combine(_tempDir, relativeDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }
}
