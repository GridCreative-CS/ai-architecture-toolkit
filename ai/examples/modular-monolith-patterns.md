# Modular Monolith Patterns: Good vs Bad

Side-by-side examples illustrating correct and incorrect module boundary
practices. See [`ai/guides/modular-monolith-definition.md`](../guides/modular-monolith-definition.md)
for the full definition.

---

## Data Access

### BAD — Cross-module direct database access

Module A directly queries Module B's database tables.

```csharp
// In Module A (Jobs module) — WRONG
public class JobService
{
    private readonly AppDbContext _db;

    public async Task<JobWithCompany> GetJobAsync(Guid jobId)
    {
        var job = await _db.Jobs.FindAsync(jobId);

        // Directly querying the Companies table owned by Module B
        var company = await _db.Companies.FindAsync(job.CompanyId);

        return new JobWithCompany(job, company);
    }
}
```

### GOOD — Module A calls Module B's public interface

```csharp
// In Module A (Jobs module) — CORRECT
public class JobService
{
    private readonly AppDbContext _db;
    private readonly ICompanyService _companyService; // Module B's public interface

    public async Task<JobWithCompany> GetJobAsync(Guid jobId)
    {
        var job = await _db.Jobs.FindAsync(jobId);

        // Call Module B's public interface instead of querying its tables
        var company = await _companyService.GetCompanyAsync(job.CompanyId);

        return new JobWithCompany(job, company);
    }
}
```

---

## DbContext Scope

### BAD — Shared DbContext across all modules

A single DbContext with all entities from all modules. Any module can access any
table.

```csharp
// WRONG — One DbContext for the entire application
public class AppDbContext : DbContext
{
    // Jobs module entities
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }

    // Companies module entities
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }

    // AI module entities
    public DbSet<AiRecommendation> AiRecommendations { get; set; }
    public DbSet<ModelVersion> ModelVersions { get; set; }

    // ... 50 more DbSets from other modules
}
```

### GOOD — Per-module DbContext

Each module has its own DbContext containing only its entities. Shared value
objects live in a minimal shared kernel.

```csharp
// CORRECT — Jobs module has its own DbContext
public class JobsDbContext : DbContext
{
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Application> Applications { get; set; }
}

// CORRECT — Companies module has its own DbContext
public class CompaniesDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyProfile> CompanyProfiles { get; set; }
}

// Shared kernel — minimal, explicit
public record Money(decimal Amount, string Currency);
public record DateRange(DateOnly Start, DateOnly End);
```

---

## Type Visibility

### BAD — All types are public

Any module can instantiate any other module's internal class, creating invisible
coupling.

```csharp
// In Module B (Companies module) — WRONG: everything is public
public class CompanyRepository { /* ... */ }
public class CompanyValidator { /* ... */ }
public class CompanyDomainService { /* ... */ }
public class CompanyEmailSender { /* ... */ }
public class CompanyProfile { /* ... */ }
```

```csharp
// In Module A (Jobs module) — WRONG: using Module B's internals
var validator = new CompanyValidator(); // Should not be accessible
var repo = new CompanyRepository();     // Should not be accessible
```

### GOOD — Internal by default; only contracts are public

```csharp
// In Module B (Companies module) — CORRECT

// PUBLIC: Only the contract surface
public interface ICompanyService
{
    Task<CompanyDto> GetCompanyAsync(Guid companyId);
    Task<bool> CompanyExistsAsync(Guid companyId);
}

public record CompanyDto(Guid Id, string Name, string Industry);

// INTERNAL: All implementation details
internal class CompanyRepository { /* ... */ }
internal class CompanyValidator { /* ... */ }
internal class CompanyDomainService { /* ... */ }
internal class CompanyEmailSender { /* ... */ }
```

```csharp
// In Module A (Jobs module) — CORRECT: can only use public contracts
public class JobService
{
    private readonly ICompanyService _companyService; // interface only
}
```
