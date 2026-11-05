---
name: minapilib-tests
description: Write integration tests for MinApiLib vertical slices with WebApplicationFactory and xUnit. Use when adding tests for an endpoint or slice, when setting up the API test project, or after creating a slice.
---

# Test a slice

Test slices through HTTP, with the real pipeline: binding, validation filters, hypermedia and the database provider. Handlers have no layers to mock, and their `HandleAsync` is `protected`, so unit tests add little.

## Coverage per slice

- The success path, including body and `Location` where relevant.
- **Every status declared in `Configure`.**
- Invalid input returns 400, if the slice uses `.WithValidation()`. This is the only way to catch a missing validator, which only logs a warning.
- A missing or invalid `HashedId` in the body returns 400, and so does a valid hash of an entity that doesn't exist.
- With the hypermedia `Accept` header: 2xx with `links`, if the response has a body.
- Paging limits, if the slice paginates.

## Layout

One test class per slice, mirroring the feature folder: `tests/<Api>.Tests/Features/<Resource>/<SliceName>Tests.cs`.

## Project

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.*" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
  <PackageReference Include="xunit" Version="2.9.*" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.8.*" />
</ItemGroup>
<ItemGroup>
  <Using Include="Xunit" />
  <ProjectReference Include="../<Api>/<Api>.csproj" />
</ItemGroup>
```

Match `Mvc.Testing` to the API's target framework. The API's `Program.cs` needs `public partial class Program;`.

## Factory

Each test class gets its own database, so classes can run in parallel. This uses a SQLite file; adapt the connection string to the project's provider.

```csharp
public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"api-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={_dbPath};Pooling=False");
        builder.UseSetting("HashedIds:Passphrase", "test passphrase");
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        File.Delete(_dbPath);
    }
}
```

## Test class

```csharp
public class CreateThingTests : IClassFixture<ApiFactory>
{
    private const string HypermediaType = "application/vnd.hypermedia+json";
    private readonly HttpClient _client;

    public CreateThingTests(ApiFactory factory)
    {
        factory.ResetDatabase();
        _client = factory.CreateClient();
    }

    private static StringContent Json(string json) => new(json, Encoding.UTF8, "application/json");

    [Fact]
    public async Task Returns_201_with_a_location_that_resolves()
    {
        var response = await _client.PostAsync("/things", Json("""{"name":"abc"}"""));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var get = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task Returns_400_with_field_errors_when_body_is_invalid()
    {
        var response = await _client.PostAsync("/things", Json("""{"name":"a"}"""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Name", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Includes_links_with_the_hypermedia_accept_header()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/things") { Content = Json("""{"name":"abc"}""") };
        request.Headers.Accept.ParseAdd(HypermediaType);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains("\"links\"", await response.Content.ReadAsStringAsync());
    }
}
```

## Notes

- **Take ids from responses**, such as `Location` or the body. Hashes depend on the passphrase, so don't hardcode them.
- `TestServer` rethrows unhandled exceptions, so a handler failure surfaces as an exception in `SendAsync`, not as a 500 status.
- `QUERY` needs `new HttpMethod("QUERY")` and `SendAsync`.
- Add one test that requests `/swagger/v1/swagger.json` and expects success. It catches a `QUERY` endpoint without `.ExcludeFromDescription()`, which breaks the whole document.
- Keep the same passphrase in all factories in a test run: the hasher is a process-wide static.

## Run

Run the test project. If a new test passes on the first try, break its assertion once to confirm it can fail.
