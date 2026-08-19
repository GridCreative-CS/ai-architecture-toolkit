using AiArchitectureToolkit.McpServer.Configuration;
using AiArchitectureToolkit.McpServer.Services;
using AiArchitectureToolkit.McpServer.Tools;
using Microsoft.Extensions.Options;

namespace AiArchitectureToolkit.McpServer.Tests.Tools;

public sealed class ProjectToolsTests : IDisposable
{
    private readonly string _tempDir;
    private readonly ToolkitContentService _toolkitService;
    private readonly ProjectContentService _projectService;
    private readonly WorkspaceScanService _workspaceService;

    public ProjectToolsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"project-tools-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        // Toolkit content
        var aiDir = Path.Combine(_tempDir, "ai");
        CreateFile(aiDir, "prompts", "delivery-planner.md", "# Delivery Planner\n\nAct as a delivery architect.");
        CreateFile(aiDir, "prompts", "feature-spec-generator.md", "# Feature Spec Generator\n\nGenerate a feature spec.");
        CreateFile(aiDir, "prompts", "architecture-compliance.md", "# Architecture Compliance\n\nCheck compliance.");
        CreateFile(aiDir, "prompts", "architecture-designer.md", "# Architecture Designer\n\nDesign production architecture.");
        CreateFile(aiDir, "prompts", "architecture-reviewer.md", "# Architecture Reviewer\n\nReview the blueprint.");
        CreateFile(aiDir, "prompts", "architecture-reconciler.md", "# Architecture Reconciler\n\nReconcile findings.");
        CreateFile(aiDir, "prompts", "existing-architecture-reviewer.md", "# Existing Architecture Reviewer\n\nReview existing doc.");
        CreateFile(aiDir, "prompts", "architecture-gap-reconciler.md", "# Gap Reconciler\n\nFill gaps.");
        CreateFile(aiDir, "prompts", "prototype-analyzer.md", "# Prototype Analyzer\n\nExtract behavior.");
        CreateFile(aiDir, "prompts", "prototype-architecture-alignment.md", "# Alignment Prompt\n\nCompare prototype with architecture.");
        CreateFile(aiDir, "prompts", "ui-compliance-check.md", "# UI Compliance Check\n\nVerify UI compliance.");
        CreateFile(aiDir, "prompts", "legacy-system-analyzer.md", "# Legacy System Analyzer\n\nAnalyze the legacy system.");
        CreateFile(aiDir, "prompts", "golden-dataset-generator.md", "# Golden Dataset Generator\n\nBuild the golden dataset.");
        CreateFile(aiDir, "prompts", "slice-preparation-runner.md", "# Slice Preparation Runner\n\nRun Steps 2-5.");
        CreateFile(aiDir, "prompts", "architecture-final-quality-gate.md", "# Architecture Final Quality Gate\n\nRun 16 checks.");
        CreateFile(aiDir, "prompts", "code-quality-reviewer.md", "# Code Quality Reviewer\n\nRun twelve checks.");
        CreateFile(aiDir, "prompts", "design-system-generator.md", "# Design System Generator\n\nDerive the design system.");
        CreateFile(aiDir, "prompts", "ui-inventory.md", "# UI Inventory\n\nInventory the existing UI.");
        CreateFile(aiDir, "prompts", "design-system-from-inventory.md", "# Design System From Inventory\n\nDerive from the inventory.");
        CreateFile(aiDir, "templates", "design-system-template.md", "# Design System Template");
        CreateFile(aiDir, "templates", "ui-inventory-template.md", "# UI Inventory Template");
        CreateFile(aiDir, "templates", "golden-dataset-template.md", "# Golden Dataset Template");
        CreateFile(aiDir, "templates", "code-quality-checklist-template.md", "# Code Quality Checklist Template");
        CreateFile(aiDir, "guides", "code-quality-standard.md", "# Code Quality Standard\n\nImplementation quality rules.");
        CreateFile(aiDir, "templates", "feature-spec-template.md", "# Feature Spec Template");
        CreateFile(aiDir, "templates", "compliance-report-template.md", "# Compliance Report Template");
        CreateFile(aiDir, "templates", "architecture-blueprint-template.md", "# Blueprint Template");
        CreateFile(aiDir, "templates", "remediation-spec-template.md", "# Remediation Spec Template");
        CreateFile(aiDir, "templates", "slice-verification-checklist-template.md", "# Slice Verification Checklist");
        CreateFile(aiDir, "guides", "vertical-slice-definition.md", "# Vertical Slice Definition\n\nThe verticality test.");
        CreateFile(aiDir, "guides", "how-feature-specs-are-used.md", "# How Feature Specs Are Used");
        CreateFile(aiDir, "guides", "contract-definition.md", "# Contract Definition");
        CreateFile(aiDir, "guides", "modular-monolith-definition.md", "# Modular Monolith Definition");
        CreateFile(aiDir, "guides", "glossary.md", "# Glossary\n\n## Term\n\nDefinition.");
        CreateFile(aiDir, "workflows", "engineering-workflow.md", "# Engineering Workflow");
        CreateFile(aiDir, "agents", "backend-agent.md", "# Backend Agent");
        CreateFile(aiDir, "examples", "contract-patterns.md", "# Contract Patterns");

        var githubDir = Path.Combine(_tempDir, ".github");
        Directory.CreateDirectory(Path.Combine(githubDir, "instructions"));
        Directory.CreateDirectory(Path.Combine(githubDir, "agents"));

        // Project content
        CreateFile(_tempDir, "architecture", "architecture-final.md", "# Architecture\n\nModular monolith.");
        CreateFile(_tempDir, "architecture", "architecture-blueprint.md", "# Blueprint\n\nDraft design.");
        CreateFile(_tempDir, "architecture", "review-report.md", "# Review Report\n\nFindings.");
        CreateFile(_tempDir, "architecture", "existing-architecture-review.md", "# Existing Review\n\nExisting findings.");
        CreateFile(_tempDir, "architecture", "prototype-analysis.md", "# Prototype Analysis\n\nBehavior.");
        CreateFile(_tempDir, "architecture", "prototype-architecture-alignment.md", "# Alignment\n\nGaps.");
        CreateFile(_tempDir, "architecture", "delivery-plan.md", "# Delivery Plan\n\nSlice: user-registration.");
        CreateFile(_tempDir, "architecture/adr", "ADR-001.md", "# ADR-001\n\nDecision.");
        CreateFile(_tempDir, "architecture/feature-specs", "user-registration.md",
            "# User Registration Spec\n\n- DR-01: A user has one email.\n- SEC-01: Anonymous callers get 401.\n- AC-01: Returns 201 on success.\n- UIAC-01: The form shows a pending spinner.");
        CreateFile(_tempDir, "architecture", "design-system.md", "# Design System\n\nToken definitions.");
        CreateFile(_tempDir, "architecture", "ui-inventory.md", "# UI Inventory\n\nCatalogued screens and components.");
        CreateFile(_tempDir, "architecture", "legacy-system-analysis.md", "# Legacy System Analysis\n\nLegacy constraints.");
        CreateFile(_tempDir, "architecture", "architecture-final-gate.md", "# Gate Report\n\nGate verdict recorded.");
        CreateFile(_tempDir, "architecture/compliance-reports", "user-registration.md", "# Compliance\n\nArchitecture compliance findings.");
        CreateFile(_tempDir, "architecture/compliance-reports", "user-registration-ui.md", "# UI Compliance\n\nUI compliance findings.");
        CreateFile(_tempDir, "architecture/slice-verification", "user-registration.md", "# Slice Verification\n\nBrowser evidence recorded.");
        CreateFile(_tempDir, "ai-parts/user-registration", "OVERVIEW.md",
            "# AI Parts Overview\n\n## Requirement Coverage Map\n\n| AC-01 | P01 |\n\n## Parts Index");
        CreateFile(_tempDir, "ai-parts/user-registration", "P01-domain.md",
            "# Part P01 — Domain\nStatus: DONE\n\n## PART_SPEC\n\n```json\n{ \"part_id\": \"P01\", \"part_type\": \"backend\", \"criteria_covered\": [\"AC-01\"] }\n```");
        CreateFile(_tempDir, "ai-parts/user-registration/reviews", "P01-review.md",
            "# Part Code Review — P01\n\n## Verdict\n\n`APPROVED` — no findings.");

        var options = Options.Create(new ServerOptions
        {
            ToolkitRoot = aiDir,
            GitHubRoot = githubDir,
            WorkspaceRoot = _tempDir
        });

        _toolkitService = new ToolkitContentService(options);
        _projectService = new ProjectContentService(options);
        _workspaceService = new WorkspaceScanService(options);
    }

    [Fact]
    public void ListProjectArtifacts_ReturnsJson()
    {
        var result = ProjectTools.ListProjectArtifacts(_projectService);

        Assert.Contains("architecture", result);
        Assert.Contains("adrs", result);
        Assert.Contains("feature-specs", result);
    }

    [Fact]
    public void GetWorkspaceStructure_ReturnsReadableOutput()
    {
        var result = ProjectTools.GetWorkspaceStructure(_workspaceService);

        Assert.Contains("Solutions", result);
        Assert.Contains("Projects", result);
    }

    [Fact]
    public void GetWorkflowContext_DeliveryPlanning_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "delivery-planning");

        Assert.Contains("Delivery Planner", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_FeatureSpec_ReturnsPromptTemplateAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "feature-spec");

        Assert.Contains("Feature Spec Generator", result);
        Assert.Contains("Feature Spec Template", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_UnknownStep_ReturnsError()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "unknown-step");

        Assert.Contains("Unknown workflow step", result);
        Assert.Contains("Valid steps", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureDesign_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-design");

        Assert.Contains("Architecture Designer", result);
        Assert.Contains("Blueprint Template", result);
        Assert.Contains("Behavior", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureBlueprintReview_ReturnsPromptAndBlueprint()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-blueprint-review");

        Assert.Contains("Architecture Reviewer", result);
        Assert.Contains("Draft design", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureReconciliation_ReturnsPromptBlueprintAndReview()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-reconciliation");

        Assert.Contains("Architecture Reconciler", result);
        Assert.Contains("Draft design", result);
        Assert.Contains("Findings", result);
    }

    [Fact]
    public void GetWorkflowContext_ExistingArchitectureReview_ReturnsPromptAndArchitecture()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "existing-architecture-review");

        Assert.Contains("Existing Architecture Reviewer", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureGapReconciliation_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-gap-reconciliation");

        Assert.Contains("Gap Reconciler", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Existing findings", result);
    }

    [Fact]
    public void GetWorkflowContext_PrototypeAnalysis_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "prototype-analysis");

        Assert.Contains("Prototype Analyzer", result);
    }

    [Fact]
    public void GetWorkflowContext_PrototypeArchitectureAlignment_ReturnsPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "prototype-architecture-alignment");

        Assert.Contains("Alignment Prompt", result);
        Assert.Contains("Behavior", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_UiFoundation_ReturnsDesignSystemGeneratorAndTemplate()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "ui-foundation");

        Assert.Contains("Design System Generator", result);
        Assert.Contains("Design System Template", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("ADR-001", result);
    }

    [Fact]
    public void GetWorkflowContext_UiInventory_ReturnsInventoryPromptAndTemplate()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "ui-inventory");

        Assert.Contains("Inventory the existing UI", result);
        Assert.Contains("UI Inventory Template", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_DesignSystemFromInventory_IncludesTheInventory()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "design-system-from-inventory");

        Assert.Contains("Derive from the inventory", result);
        Assert.Contains("Design System Template", result);
        Assert.Contains("Catalogued screens and components", result);
    }

    [Fact]
    public void GetWorkflowContext_UiRemediation_ReturnsPromptTemplateAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "ui-remediation");

        Assert.Contains("UI Compliance Check", result);
        Assert.Contains("Remediation Spec Template", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Token definitions", result);
    }

    [Fact]
    public void GetWorkflowContext_SliceVerification_ReturnsChecklistAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "slice-verification");

        Assert.Contains("Slice Verification Checklist", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Token definitions", result);
    }

    [Fact]
    public void GetSliceContext_ReturnsFeatureSpecAndPlan()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "user-registration");

        Assert.Contains("User Registration Spec", result);
        Assert.Contains("Delivery Plan", result);
        Assert.Contains("ADR-001", result);
    }

    [Fact]
    public void GetSliceContext_MissingSlice_ShowsNotFound()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "nonexistent");

        Assert.Contains("no feature spec found", result);
    }

    [Fact]
    public void GetWorkflowContext_LegacySystemAnalysis_ReturnsPromptAndProjectContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "legacy-system-analysis");

        Assert.Contains("Legacy System Analyzer", result);
        Assert.Contains("Legacy constraints", result);
    }

    [Fact]
    public void GetWorkflowContext_GoldenDataset_ReturnsPromptAndTemplate()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "golden-dataset");

        Assert.Contains("Golden Dataset Generator", result);
        Assert.Contains("Golden Dataset Template", result);
    }

    [Fact]
    public void GetWorkflowContext_GoldenDataset_WithSliceName_IncludesFeatureSpec()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "golden-dataset", "user-registration");

        Assert.Contains("User Registration Spec", result);
    }

    [Fact]
    public void GetWorkflowContext_SlicePreparation_ReturnsRunnerPromptAndContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "slice-preparation");

        Assert.Contains("Slice Preparation Runner", result);
        Assert.Contains("Modular monolith", result);
        Assert.Contains("Slice: user-registration", result);
    }

    [Fact]
    public void GetWorkflowContext_ArchitectureFinalGate_ReturnsGatePromptAndArchitecture()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "architecture-final-gate");

        Assert.Contains("Architecture Final Quality Gate", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_PartCodeReview_BundlesPromptTemplateGuideAndSliceContext()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "part-code-review", "user-registration");

        Assert.Contains("Code Quality Reviewer", result);
        Assert.Contains("Code Quality Checklist Template", result);
        Assert.Contains("Implementation quality rules", result);
        Assert.Contains("User Registration Spec", result);
        Assert.Contains("Modular monolith", result);
    }

    [Fact]
    public void GetWorkflowContext_Decomposition_ServesThePlanDecomposerSkill()
    {
        CreateSkill("plan-decomposer", "# Plan Decomposer\n\nEmit a PART_SPEC per Part.");

        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "decomposition");

        Assert.Contains("Plan Decomposer", result);
        Assert.Contains("PART_SPEC", result);
    }

    [Fact]
    public void GetWorkflowContext_SliceVerification_ReportsWhetherEvidenceExists()
    {
        var withEvidence = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "slice-verification", "user-registration");
        var withoutEvidence = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "slice-verification", "unverified-slice");

        Assert.Contains("Browser evidence recorded", withEvidence);
        Assert.Contains("no slice verification evidence", withoutEvidence, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetWorkflowContext_UnknownStep_ListsTheNewSteps()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "unknown-step");

        Assert.Contains("legacy-system-analysis", result);
        Assert.Contains("golden-dataset", result);
        Assert.Contains("slice-preparation", result);
        Assert.Contains("architecture-final-gate", result);
        Assert.Contains("part-code-review", result);
    }

    [Fact]
    public void GetWorkflowContext_ValidStepList_PlacesTheGateBetweenReconciliationAndAdrGeneration()
    {
        var result = ProjectTools.GetWorkflowContext(_toolkitService, _projectService, "unknown-step");

        var reconciliation = result.IndexOf("architecture-reconciliation", StringComparison.Ordinal);
        var gate = result.IndexOf("architecture-final-gate", StringComparison.Ordinal);
        var adr = result.IndexOf("adr-generation", StringComparison.Ordinal);

        Assert.True(reconciliation < gate, "the gate must be listed after architecture-reconciliation");
        Assert.True(gate < adr, "the gate must be listed before adr-generation");
    }

    [Fact]
    public void GetSliceContext_IncludesBothComplianceReports()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "user-registration");

        Assert.Contains("Architecture compliance findings", result);
        Assert.Contains("UI compliance findings", result);
    }

    [Fact]
    public void GetSliceContext_ListsFeatureSpecCriterionIds()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "user-registration");

        Assert.Contains("DR-01", result);
        Assert.Contains("SEC-01", result);
        Assert.Contains("AC-01", result);
        Assert.Contains("UIAC-01", result);
    }

    [Fact]
    public void GetSliceContext_IncludesDecompositionStatusAndReviewVerdict()
    {
        var result = ProjectTools.GetSliceContext(_projectService, "user-registration");

        Assert.Contains("P01", result);
        Assert.Contains("DONE", result);
        Assert.Contains("APPROVED", result);
    }

    [Fact]
    public void GetSliceContext_FindsTheDecompositionWhenTheSpecIsNamedSliceIdAndSliceName()
    {
        // The real toolkit shape: the feature spec is '<slice-id>-<slice-name>.md'
        // while the decomposition folder is '<slice-id>' alone.
        CreateFile(_tempDir, "architecture/feature-specs", "S2.6-inspection-history.md", "# Inspection History Spec\n\n- AC-01: Shows history.");
        CreateFile(_tempDir, "ai-parts/S2.6", "OVERVIEW.md", "# AI Parts Overview\n\n## Requirement Coverage Map\n\n| AC-01 | P01 |");
        CreateFile(_tempDir, "ai-parts/S2.6", "P01-history.md",
            "# Part P01 — History\nStatus: IN_PROGRESS\n\n## PART_SPEC\n\n```json\n{ \"part_id\": \"P01\", \"part_type\": \"frontend\", \"criteria_covered\": [\"AC-01\"] }\n```");

        var result = ProjectTools.GetSliceContext(_projectService, "S2.6-inspection-history");

        Assert.Contains("Decomposition (ai-parts)", result);
        Assert.Contains("IN_PROGRESS", result);
        Assert.Contains("frontend", result);
    }

    [Fact]
    public void ListProjectArtifacts_IncludesAiPartsAndSliceVerification()
    {
        var result = ProjectTools.ListProjectArtifacts(_projectService);

        Assert.Contains("ai-parts", result);
        Assert.Contains("slice-verification", result);
        Assert.Contains("ui-compliance-reports", result);
        Assert.Contains("architecture-final-gate", result);
        Assert.Contains("legacy-system-analysis", result);
        Assert.Contains("user-registration", result);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* best effort cleanup */ }
    }

    private void CreateSkill(string skillName, string content)
    {
        var dir = Path.Combine(_tempDir, ".github", "skills", skillName);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, "SKILL.md"), content);
    }

    private static void CreateFile(string baseDir, string subDir, string fileName, string content)
    {
        var dir = Path.Combine(baseDir, subDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }

    private void CreateFile(string relativeDir, string fileName, string content)
    {
        var dir = Path.Combine(_tempDir, relativeDir);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, fileName), content);
    }
}
