---
name: minapilib-setup
description: Create a new ASP.NET Core Minimal API project with Vertical Slice Architecture and MinApiLib, or add MinApiLib to an existing one. Use when bootstrapping an API, or when wiring MinApiLib packages into Program.cs and global usings.
---

# Set up a MinApiLib Vertical Slice API

Follow the conventions in `AGENT.md`.

## 1. Create the project

Skip `dotnet new` when the project already exists.

```bash
dotnet new web -n <Name>
cd <Name>
dotnet add package MinApiLib.Endpoints
dotnet add package MinApiLib.FluentValidation
dotnet add package MinApiLib.HashedIds
dotnet add package MinApiLib.Hypermedia
dotnet add package MinApiLib.OperationCanceled
dotnet add package MinApiLib.DependencyInjection
dotnet add package Swashbuckle.AspNetCore
```

If a package has only prerelease versions, retry with `--prerelease`. Only add the packages the API will use, and add its data access provider too.

Don't use `MinApiLib.Validation`: it's deprecated in favor of `MinApiLib.FluentValidation`.

Set `<Nullable>disable</Nullable>` in the `.csproj`: the slice templates assume it, as the sample does. `dotnet new web` enables it by default.

## 2. `_Imports.cs`

```csharp
global using FluentValidation;

global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;

global using MinApiLib.DependencyInjection;
global using MinApiLib.Endpoints;
global using MinApiLib.FluentValidation;
global using MinApiLib.HashedIds;
global using MinApiLib.Hypermedia;
global using MinApiLib.OperationCanceled;
```

## 3. `Program.cs`

```csharp
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<HashedId>(() => new OpenApiSchema { Type = JsonSchemaType.String });
    c.CustomSchemaIds(x => x.FullName);
});
builder.Services.AddHashedIds(op => op.Passphrase = builder.Configuration["HashedIds:Passphrase"]);
builder.Services.AddHypermedia();
builder.Services.AddAssembly();
builder.Services.AddValidatorsFromAssembly();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.CatchOperationCanceled();
app.MapEndpoints()
   .WithHypermedia();

app.Run();

public partial class Program;
```

- The snippet above is for Swashbuckle 10. On Swashbuckle 6.x, use `using Microsoft.OpenApi.Models;` and `new OpenApiSchema { Type = "string" }`.
- `CustomSchemaIds` is required: every slice has a type named `Response`.
- `AddHypermedia()`, `AddAssembly()`, `MapEndpoints()` and `AddValidatorsFromAssembly()` scan the calling assembly. If slices live in another project, pass `typeof(SomeSliceType).Assembly`.
- `public partial class Program;` lets `WebApplicationFactory<Program>` reach the app from tests.

## 4. Configuration

- Read the passphrase and connection string from configuration, with user secrets or environment variables outside development. `AddHashedIds` throws if the passphrase is null.
- Keep the passphrase stable: changing it changes every public id, and breaks ids clients have stored.
- Seed data in an `IHostedService`, not in `Program.cs`.

## 5. Shared pieces

Create `Features/` and, if the API paginates, `Features/PagedList.cs` (template in `minapilib-add-slice`).

## 6. Verify

Run `dotnet build`, add a first slice with `minapilib-add-slice`, then run the app and confirm `/swagger/v1/swagger.json` returns 200.
