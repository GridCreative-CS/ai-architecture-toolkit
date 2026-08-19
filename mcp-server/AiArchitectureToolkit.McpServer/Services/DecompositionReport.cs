using System.Text;

namespace AiArchitectureToolkit.McpServer.Services;

/// <summary>
/// Renders a <see cref="SliceDecomposition"/> as markdown — the Part status
/// table, the Step 6 / Step 6a review state, and the warnings that decide
/// whether the next Part may start.
/// </summary>
internal static class DecompositionReport
{
    /// <summary>
    /// Appends the Part table and any warnings under the supplied heading.
    /// </summary>
    public static void Append(StringBuilder sb, SliceDecomposition decomposition, string heading)
    {
        ArgumentNullException.ThrowIfNull(sb);
        ArgumentNullException.ThrowIfNull(decomposition);

        sb.AppendLine(heading);
        sb.AppendLine();
        sb.AppendLine($"- Requirement Coverage Map present: {(decomposition.HasRequirementCoverageMap ? "yes" : "no")}");
        sb.AppendLine();

        if (decomposition.Parts.Count == 0)
        {
            sb.AppendLine("(no Part files found in this slice folder)");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("| Part | Type | Status | Quality report | Review verdict | Criteria covered |");
            sb.AppendLine("| --- | --- | --- | --- | --- | --- |");

            foreach (var part in decomposition.Parts)
            {
                sb.AppendLine(
                    $"| {part.PartId} | {part.PartType ?? "(unclassified)"} | {part.Status} | " +
                    $"{(part.HasQualityReport ? "yes" : "no")} | {part.ReviewVerdict ?? "(none)"} | " +
                    $"{FormatCriteria(part)} |");
            }

            sb.AppendLine();
        }

        if (decomposition.Warnings.Count == 0)
        {
            return;
        }

        sb.AppendLine("### Warnings");
        sb.AppendLine();
        foreach (var warning in decomposition.Warnings)
        {
            sb.AppendLine($"- {warning}");
        }

        sb.AppendLine();
    }

    private static string FormatCriteria(PartInfo part)
    {
        if (!part.DeclaresCriteriaCovered)
        {
            return "(field absent)";
        }

        return part.CriteriaCovered.Count > 0
            ? string.Join(", ", part.CriteriaCovered)
            : "(none declared)";
    }
}
