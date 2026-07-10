---
description: 'Guidelines for building C# applications'
applyTo: '**/*.cs'
---

# C# Development

## C# Instructions
- Always use the latest LTS version C#, currently C# 14 features.
- Write clear and concise comments for each function.

## General Instructions
- Make only high confidence suggestions when reviewing code changes.
- Write code with good maintainability practices, including comments on why certain design decisions were made.
- Handle edge cases and write clear exception handling.
- For libraries or external dependencies, mention their usage and purpose in comments.

## Naming Conventions

- Follow PascalCase for component names, method names, and public members.
- Use camelCase for private fields and local variables.
- Prefix interface names with "I" (e.g., IUserService).

## Formatting

- Apply code-formatting style defined in `.editorconfig`.
- Prefer file-scoped namespace declarations and single-line using directives.
- Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
- Ensure that the final return statement of a method is on its own line.
- Use pattern matching and switch expressions wherever possible.
- Use `nameof` instead of string literals when referring to member names.
- Ensure that XML doc comments are created for any public APIs. When applicable, include `<example>` and `<code>` documentation in the comments.
- **Traceability in doc comments (hard rule):** every public type and member carries an XML doc comment, and whenever the documented behavior implements a rule from the architecture, an ADR, or a feature spec, the comment cites the owning source specifically — slice ID, spec section (e.g. "spec §6 rule 4"), ADR number (e.g. "ADR-08"), and the stable error code where one applies (e.g. `422 CRISIS_ESCALATION_REQUIRED`). The same applies to test-class doc comments: state which contract or spec rules the tests lock. Generic phrases like "per the architecture" or "as specified" without a traceable citation are not acceptable.

## Project Setup and Structure

- Guide users through creating a new .NET project with the appropriate templates.
- Explain the purpose of each generated file and folder to build understanding of the project structure.
- Demonstrate how to organize code using feature folders or domain-driven design principles.
- Show proper separation of concerns with models, services, and data access layers.
- Explain the Program.cs and configuration system in ASP.NET Core 10 including environment-specific settings.
- Use NuGet Central Package Management (`Directory.Packages.props`) to keep all package versions aligned in one place. Individual csproj files must not specify `Version` on `PackageReference`.
- **Always use `.slnx` format** (not `.sln`) for any new solution targeting .NET 9 or later. Create the solution with `dotnet new sln --format slnx`. If an existing `.sln` is found in a .NET 10 project, flag it as a deviation and offer to migrate it to `.slnx`.
- **Test ↔ Src project pairing (CI-enforced):** Every solution must include a `SolutionStructureTests` class (typically in an existing architectural or integration test project) that asserts the following invariant: for every `*.Tests.csproj` discovered under `tests/`, a `ProjectReference` to the matching `src/<Module>/<Module>.csproj` exists inside that test project file. This turns a manual review comment into a failing CI build. If no architectural test project exists yet, create one (e.g., `tests/Architecture.Tests`) and add the assertion there.

## Nullable Reference Types

- Declare variables non-nullable, and check for `null` at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.
- Use `ArgumentNullException.ThrowIfNull(param)` as the first guard in public methods accepting reference types. Throw `ArgumentNullException` for null; reserve `ArgumentException` for non-null invalid values (empty, whitespace, out-of-range). Never combine both cases into a single `ArgumentException`.
- When a guard method (e.g. `Guard.Against.Null`) returns a validated non-null value, always capture and use that return value. Never discard it and then use `!` on the original. This keeps nullable flow analysis accurate and eliminates suppression operators.
- Every public method parameter gets a null guard at the boundary — including delegates (`Func<>`, `Action<>`), collections, and other reference types. Delegate parameters are no different from data parameters.

## Data Access Patterns

- Guide the implementation of a data access layer using Entity Framework Core.
- Explain different options (SQL Server, SQLite, In-Memory) for development and production.
- Demonstrate repository pattern implementation and when it's beneficial.
- Show how to implement database migrations and data seeding.
- Explain efficient query patterns to avoid common performance issues.

## Authentication and Authorization

- Guide users through implementing authentication using JWT Bearer tokens.
- Explain OAuth 2.0 and OpenID Connect concepts as they relate to ASP.NET Core.
- Show how to implement role-based and policy-based authorization.
- Demonstrate integration with Microsoft Entra ID (formerly Azure AD).
- Explain how to secure both controller-based and Minimal APIs consistently.

## Validation and Error Handling

- Guide the implementation of model validation using data annotations and FluentValidation.
- Explain the validation pipeline and how to customize validation responses.
- Demonstrate a global exception handling strategy using middleware.
- Show how to create consistent error responses across the API.
- Explain problem details (RFC 7807) implementation for standardized error responses.

## API Versioning and Documentation

- Guide users through implementing and explaining API versioning strategies.
- Demonstrate Swagger/OpenAPI implementation with proper documentation.
- Show how to document endpoints, parameters, responses, and authentication.
- Explain versioning in both controller-based and Minimal APIs.
- Guide users on creating meaningful API documentation that helps consumers.

## Logging and Monitoring

- Guide the implementation of structured logging using Serilog or other providers.
- Use message templates with named properties (`"SessionId={SessionId}"`), never string interpolation, in log calls.
- Log identifiers, never sensitive payloads: no PII, secrets, tokens, or free-text user content unless the project has an explicit redaction mechanism and the log site uses it.
- Explain the logging levels and when to use each.
- Demonstrate integration with Application Insights for telemetry collection.
- Show how to implement custom telemetry and correlation IDs for request tracking.
- Explain how to monitor API performance, errors, and usage patterns.
- For Serilog redaction, use enrichers that match on **property names** (not CLR type names). `Destructure.ByTransformingWhere` operates on the type's `Name` property (e.g. `"String"`), not on the log-event property name — it will not redact plain `string` values like connection strings.

## Testing

- Always include test cases for critical paths of the application.
- Guide users through creating unit tests.
- Do not emit "Act", "Arrange" or "Assert" comments.
- Use the format `ClassUnderTest_Scenario_ExpectedBehavior` for test method names, where `ClassUnderTest` is the name of the production class or entity being tested — not the test class itself (e.g., `Symbol_WithEmptyString_ThrowsArgumentException`, `DateRange_WithEndBeforeStart_ThrowsArgumentException`). The class-under-test prefix ensures unambiguous test output in filtered and aggregated reports.
- Copy existing style in nearby files for test method names and capitalization.
- Ensure test method names accurately describe the expected behavior and exception type (e.g. use `ThrowsArgumentNullException`, not `ThrowsArgumentException`, when asserting `ArgumentNullException`).
- Test observable behavior, not implementation details. Avoid assertions that depend on which internal code path is exercised (e.g. fallback vs. configured value) — assert the contract instead.
- Explain integration testing approaches for API endpoints.
- Demonstrate how to mock dependencies for effective testing.
- Show how to test authentication and authorization logic.
- Explain test-driven development principles as applied to API development.

## Async and Cancellation

- Every async public method accepts a `CancellationToken` and passes it to every awaited call down to the I/O layer. Do not accept a token and drop it, and do not add `CancellationToken.None` where a real token is in scope.
- Use `ConfigureAwait(false)` on awaits in library/application-layer code (non-UI, non-ASP.NET-request-context code); follow the convention visible in nearby files.
- No `async void` (except event handlers), no sync-over-async (`.Result`, `.Wait()`, `.GetAwaiter().GetResult()`), no fire-and-forget tasks outside an established background-work mechanism (hosted service, queue).
- Timeouts, retries, and resilience use the project's existing mechanism (e.g., `Microsoft.Extensions.Http.Resilience`/Polly) — do not hand-roll retry loops.

## Error Handling and Results

- Follow the project's established error flow: if handlers/services return a result type (e.g., `Result<T>` with an error kind), new code uses it for expected failures — reserve exceptions for unexpected faults.
- Give every externally visible error a stable error identifier (e.g., an `UPPER_SNAKE` code prefix in the error message or a problem-details `type`), formatted exactly like existing errors, so contracts and tests can rely on it.
- Map infrastructure exceptions to the contract's documented error responses at the boundary where the project does so; never swallow exceptions or fall back silently.

## Performance Optimization

- Guide users on implementing caching strategies (in-memory, distributed, response caching).
- Explain asynchronous programming patterns and why they matter for API performance.
- Demonstrate pagination, filtering, and sorting for large data sets.
- Show how to implement compression and other performance optimizations.
- Explain how to measure and benchmark API performance.

## Deployment and DevOps

- Guide users through containerizing their API using .NET's built-in container support (`dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer`).
- Explain the differences between manual Dockerfile creation and .NET's container publishing features.
- Explain CI/CD pipelines for NET applications.
- Demonstrate deployment to Azure App Service, Azure Container Apps, or other hosting options.
- Show how to implement health checks and readiness probes.
- Explain environment-specific configurations for different deployment stages.

## Immutability and Defensive Copying

- Value objects and types documented as immutable must be truly immutable. Never store mutable references (e.g. `byte[]`, `List<T>`) directly from constructor parameters — always defensively copy on input.
- Expose mutable-backing data as read-only views (`ReadOnlyMemory<byte>`, `IReadOnlyList<T>`, `ReadOnlyCollection<T>`) rather than the raw mutable type.
- For `params` array parameters: validate non-null and non-empty, then clone before storing (e.g. `Array.AsReadOnly((T[])arr.Clone())`).
- If a property wraps or transforms a backing field and the wrapper is stable (same backing data), compute the wrapper once and cache it. Properties should be cheap — callers expect field-read cost, not allocation on every access. Example: call `_items.AsReadOnly()` once in the constructor and store the result, then expose it via the property.

## Value Object Type Choice

- Use `sealed record` for domain value objects when `default(T)` would violate an invariant — this includes any value object wrapping a reference type (`string`, collections) or where zero-initialized fields are semantically invalid.
- Use `readonly record struct` only when `default(T)` is a semantically valid state (e.g., a `decimal` wrapper where `0m` is acceptable).
- When in doubt, prefer `sealed record` — correctness of invariants outweighs allocation micro-optimization for domain objects.

## Discriminated State Types

- In types with mutually exclusive states (success/failure, Some/None), properties that belong to only one variant must throw `InvalidOperationException` when accessed on the wrong variant.
- Never rely on an enum's implicit `default(0)` value to represent "not applicable". Use a nullable backing field and throw on wrong-variant access so misuse fails loudly instead of returning plausible-looking garbage.
- Consider adding an explicit `None = 0` member to error-classification enums so the default value is visibly meaningless rather than accidentally valid.

## Entity Identity

- Entity constructors must reject `default(TId)` in addition to `null`. For value-type IDs (e.g. `Guid`), `Guid.Empty` passes a null guard but is semantically invalid as an identifier. Use `EqualityComparer<TId>.Default.Equals(id, default!)` to detect the zero/empty default.
- If this pattern recurs across multiple entity or value types, extract a `Guard.Against.Default<T>()` method.

## Configuration and Secrets

- Never commit credentials, passwords, API keys, or connection strings with real values in `appsettings*.json`. Use empty/placeholder values and document how to supply secrets via `dotnet user-secrets`, environment variables, or `.env` files.
- Fail fast on missing required configuration at startup: use `?? throw new InvalidOperationException("...")` or a guard clause when reading required config values such as connection strings.
- Keep `.env` gitignored and provide a `.env.example` template with empty values and setup instructions.

## New Type Checklist

Before completing any new class, record, or struct, verify:

- **Property allocation:** Does any property allocate or compute on every access? Cache the wrapper.
- **Guard return values:** Am I discarding a guard's return value and then using `!`? Capture and use it.
- **Enum defaults:** Can an enum property be accessed in a state where it's meaningless? Make the wrong-variant accessor throw.
- **Value-type identity:** Does this type accept a generic `TId`? Guard against `default` as well as `null`.
- **Delegate parameters:** Does any public method take a `Func<>` or `Action<>`? Null-guard it at the boundary.
