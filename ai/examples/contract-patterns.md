# Contract Patterns: Good vs Bad

Side-by-side examples illustrating correct and incorrect contract practices. See
[`ai/guides/contract-definition.md`](../guides/contract-definition.md) for the
full definition.

---

## Contract Testing

### BAD — Contract test only checks HTTP 200

The test verifies the endpoint responds, but not what it responds with or how it
handles errors.

```csharp
[Fact]
public async Task CreateJob_Returns200()
{
    // WRONG — Only checks status code
    var response = await _client.PostAsJsonAsync("/api/jobs", new { Title = "Engineer" });
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
}
```

### GOOD — Contract test checks schema, error codes, and idempotency

```csharp
[Fact]
public async Task CreateJob_ReturnsCorrectSchema()
{
    var response = await _client.PostAsJsonAsync("/api/jobs", new { Title = "Engineer" });

    response.EnsureSuccessStatusCode();

    var job = await response.Content.ReadFromJsonAsync<JobResponse>();
    Assert.NotEqual(Guid.Empty, job.Id);
    Assert.Equal("Engineer", job.Title);
    Assert.Equal("Draft", job.Status);
}

[Fact]
public async Task CreateJob_DuplicateTitle_Returns409WithProblemDetails()
{
    await _client.PostAsJsonAsync("/api/jobs", new { Title = "Engineer" });

    var duplicate = await _client.PostAsJsonAsync("/api/jobs", new { Title = "Engineer" });

    Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    var problem = await duplicate.Content.ReadFromJsonAsync<ProblemDetails>();
    Assert.Equal("https://tools.ietf.org/html/rfc7807", problem.Type);
}

[Fact]
public async Task CreateJob_IdempotentRetry_ReturnsSameResource()
{
    var request = new HttpRequestMessage(HttpMethod.Post, "/api/jobs");
    request.Headers.Add("Idempotency-Key", "key-123");
    request.Content = JsonContent.Create(new { Title = "Engineer" });

    var first = await _client.SendAsync(request);
    var firstJob = await first.Content.ReadFromJsonAsync<JobResponse>();

    request = new HttpRequestMessage(HttpMethod.Post, "/api/jobs");
    request.Headers.Add("Idempotency-Key", "key-123");
    request.Content = JsonContent.Create(new { Title = "Engineer" });

    var retry = await _client.SendAsync(request);
    var retryJob = await retry.Content.ReadFromJsonAsync<JobResponse>();

    Assert.Equal(firstJob.Id, retryJob.Id);
}
```

---

## Contract Versioning

### BAD — Breaking change deployed without version bump

A required field is added to the request, breaking all existing consumers.

```csharp
// v1 contract (original)
public record CreateJobRequest(string Title);

// WRONG — Field added as required without a version bump
// All consumers sending { Title: "..." } now get 400 Bad Request
public record CreateJobRequest(string Title, string Department);
```

### GOOD — New version alongside old version with migration timeline

```csharp
// v1 contract (unchanged, still works)
// POST /api/v1/jobs
public record CreateJobRequestV1(string Title);

// v2 contract (new, additive)
// POST /api/v2/jobs
public record CreateJobRequestV2(string Title, string Department);

// v1 → v2 migration documented:
// - v1 deprecated: 2026-04-01
// - v1 removed: 2026-07-01
// - Consumers notified via changelog and API deprecation header
```

---

## Contract Completeness

### BAD — "Contract" is just a C# interface with no behavioral specification

The interface declares method signatures but nothing about expected behavior,
error handling, or side effects.

```csharp
// WRONG — No behavioral contract
public interface ICrmAdapter
{
    Task<Contact> GetContactAsync(string email);
    Task CreateContactAsync(Contact contact);
    Task UpdateContactAsync(Contact contact);
}

// No contract tests exist
// No documentation of error behavior
// No specification of what happens on duplicate, not-found, timeout
```

### GOOD — Interface with contract test suite and documented behavior

```csharp
// Interface declaration
public interface ICrmAdapter
{
    Task<Contact?> GetContactAsync(string email);
    Task<Contact> CreateContactAsync(CreateContactRequest request);
    Task<Contact> UpdateContactAsync(Guid id, UpdateContactRequest request);
}

// Shared contract test suite — all implementations must pass these
public abstract class CrmAdapterContractTests
{
    protected abstract ICrmAdapter CreateAdapter();

    [Fact]
    public async Task GetContact_NotFound_ReturnsNull()
    {
        var adapter = CreateAdapter();
        var result = await adapter.GetContactAsync("nonexistent@example.com");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateContact_DuplicateEmail_ThrowsConflictException()
    {
        var adapter = CreateAdapter();
        var request = new CreateContactRequest("test@example.com", "Test User");

        await adapter.CreateContactAsync(request);

        await Assert.ThrowsAsync<ConflictException>(
            () => adapter.CreateContactAsync(request));
    }

    [Fact]
    public async Task UpdateContact_NonExistent_ThrowsNotFoundException()
    {
        var adapter = CreateAdapter();

        await Assert.ThrowsAsync<NotFoundException>(
            () => adapter.UpdateContactAsync(
                Guid.NewGuid(),
                new UpdateContactRequest("Updated Name")));
    }
}

// Feature spec §7 documents:
// - GetContactAsync: returns null when not found (no exception)
// - CreateContactAsync: throws ConflictException on duplicate email
// - UpdateContactAsync: throws NotFoundException for unknown ID
// - All methods: CRM rate limit exceeded → throws RateLimitException
```
