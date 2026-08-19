namespace AiArchitectureToolkit.McpServer.Services;

/// <summary>
/// The decomposition output for one slice — the contents of
/// <c>ai-parts/&lt;slice-id&gt;/</c> produced by the <c>plan-decomposer</c>
/// skill (engineering workflow Step 5).
/// </summary>
public sealed class SliceDecomposition
{
    /// <summary>The slice folder name under <c>ai-parts/</c>.</summary>
    public required string SliceId { get; init; }

    /// <summary>Whether <c>OVERVIEW.md</c> exists for this slice.</summary>
    public required bool OverviewExists { get; init; }

    /// <summary>
    /// Whether <c>OVERVIEW.md</c> contains the <c>## Requirement Coverage Map</c>
    /// section required from toolkit v4.6.0. Absence means a pre-v4.6
    /// decomposition whose map the executor must derive from the feature spec.
    /// </summary>
    public required bool HasRequirementCoverageMap { get; init; }

    /// <summary>The slice's Parts, in file-name order.</summary>
    public required IReadOnlyList<PartInfo> Parts { get; init; }

    /// <summary>
    /// Conditions a client should act on before starting the next Part —
    /// a missing coverage map, a DONE Part with no Step 6a review, a rejected
    /// review, or a Part that declares no <c>criteria_covered</c>.
    /// </summary>
    public required IReadOnlyList<string> Warnings { get; init; }
}

/// <summary>
/// One Part file under <c>ai-parts/&lt;slice-id&gt;/</c>, with its Status line,
/// its optional PART_SPEC traceability fields, and its Step 6 / Step 6a review
/// artifacts.
/// </summary>
public sealed class PartInfo
{
    /// <summary>
    /// The Part identifier (e.g. <c>P01</c>, <c>P09b</c>), taken from the
    /// PART_SPEC <c>part_id</c> field when present, otherwise from the file name.
    /// </summary>
    public required string PartId { get; init; }

    /// <summary>The Part file name without its <c>.md</c> extension.</summary>
    public required string FileName { get; init; }

    /// <summary>
    /// The Part's <c>Status:</c> line — <c>TODO</c>, <c>IN_PROGRESS</c>,
    /// <c>DONE</c>, or <c>BLOCKED</c>; <c>UNKNOWN</c> when the line is missing
    /// or carries an unrecognised value.
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// The optional PART_SPEC <c>part_type</c> field. <see langword="null"/>
    /// when absent — the reviewer then classifies from <c>file_touch_points</c>.
    /// </summary>
    public string? PartType { get; init; }

    /// <summary>
    /// The criterion IDs the optional PART_SPEC <c>criteria_covered</c> field
    /// declares. Empty when the field is absent.
    /// </summary>
    public required IReadOnlyList<string> CriteriaCovered { get; init; }

    /// <summary>
    /// Whether the PART_SPEC declares <c>criteria_covered</c> at all.
    /// Distinguishes an absent field from a declared-but-empty one.
    /// </summary>
    public required bool DeclaresCriteriaCovered { get; init; }

    /// <summary>
    /// Whether <c>reviews/&lt;part-id&gt;-quality-report.md</c> exists
    /// (the Part Quality Report, Step 6).
    /// </summary>
    public required bool HasQualityReport { get; init; }

    /// <summary>
    /// Whether <c>reviews/&lt;part-id&gt;-review.md</c> exists
    /// (the Part Code Review, Step 6a).
    /// </summary>
    public required bool HasReview { get; init; }

    /// <summary>
    /// The review verdict — <c>APPROVED</c>, <c>APPROVED WITH NOTES</c>, or
    /// <c>REJECTED — MUST FIX</c>. <see langword="null"/> when there is no
    /// review file, or none of the three verdicts appears in it.
    /// </summary>
    public string? ReviewVerdict { get; init; }
}
