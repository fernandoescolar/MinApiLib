[![license](https://img.shields.io/badge/License-MIT-purple.svg)](LICENSE)
![This package is compatible with this framework or higher](https://img.shields.io/badge/.Net-8.0-blue)
![This package is compatible with this framework or higher](https://img.shields.io/badge/.Net-9.0-blue)
![This package is compatible with this framework or higher](https://img.shields.io/badge/.Net-10.0-blue)
![release action](https://github.com/fernandoescolar/MinApiLib/actions/workflows/release.yml/badge.svg)
![ci action](https://github.com/fernandoescolar/MinApiLib/actions/workflows/ci.yml/badge.svg)

# MinApiLib

ASP.NET Core Minimal API library with extensions, filters and utilities to build APIs with one record per endpoint.

![MinApiLib](icon.png)

## Packages

Each package can be used on its own, except where noted.

| Package | Purpose |
|---|---|
| [MinApiLib.Endpoints](src/MinApiLib.Endpoints/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints)](https://www.nuget.org/packages/MinApiLib.Endpoints) | Define endpoints as records and map them by scanning an assembly |
| [MinApiLib.Endpoints.Aot](src/MinApiLib.Endpoints.Aot/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints.Aot)](https://www.nuget.org/packages/MinApiLib.Endpoints.Aot) | Source generator that maps endpoints without scanning; requires `MinApiLib.Endpoints` |
| [MinApiLib.FluentValidation](src/MinApiLib.FluentValidation/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.FluentValidation)](https://www.nuget.org/packages/MinApiLib.FluentValidation) | Validate requests with FluentValidation |
| [MinApiLib.HashedIds](src/MinApiLib.HashedIds/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.HashedIds)](https://www.nuget.org/packages/MinApiLib.HashedIds) | Expose integer ids as short hashes |
| [MinApiLib.Hypermedia](src/MinApiLib.Hypermedia/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Hypermedia)](https://www.nuget.org/packages/MinApiLib.Hypermedia) | Add links to responses when the client asks for them |
| [MinApiLib.OperationCanceled](src/MinApiLib.OperationCanceled/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.OperationCanceled)](https://www.nuget.org/packages/MinApiLib.OperationCanceled) | Answer `499` when the client cancels a request |
| [MinApiLib.DependencyInjection](src/MinApiLib.DependencyInjection/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.DependencyInjection)](https://www.nuget.org/packages/MinApiLib.DependencyInjection) | Register services with an attribute |
| [MinApiLib.Validation](src/MinApiLib.Validation/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Validation)](https://www.nuget.org/packages/MinApiLib.Validation) | **Deprecated:** use `MinApiLib.FluentValidation` |

## Quick start

Install the packages you need:

```bash
dotnet add package MinApiLib.Endpoints
dotnet add package MinApiLib.FluentValidation
```

Create an endpoint:

```csharp
public record struct Request([FromBody] RequestBody Body);

public class RequestBody
{
    public string Name { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 100);
    }
}

public record CreateThing() : PostHandlerAsync<Request>("/things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status201Created)
                .WithName("CreateThing")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Results.Created("/things/1", request.Body);
    }
}
```

And map every endpoint of the assembly:

```csharp
global using FluentValidation;
global using Microsoft.AspNetCore.Mvc;
global using MinApiLib.Endpoints;
global using MinApiLib.FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssembly();

var app = builder.Build();
app.MapEndpoints();
app.Run();
```

## Samples

The [samples](samples) folder has two APIs over the same data:

- [Example.VerticalSlice.Api](samples/Example.VerticalSlice.Api): one folder per use case, with its own handler, request and response.
- [Example.Simple.Api](samples/Example.Simple.Api): endpoints grouped by resource, sharing their models.

Open `samples/MinApiLib.Samples.sln`. They target .NET 10.

## Development

`MinApiLib.sln` contains the libraries and their tests:

```bash
dotnet build MinApiLib.sln
dotnet test MinApiLib.sln
dotnet build samples/MinApiLib.Samples.sln
```

The libraries and their test projects target `net8.0`, `net9.0` and `net10.0`, so running every test requires those three runtimes. The CI runs all of them, and also builds the samples, on pushes and pull requests to `main`.

## License

[![license](https://img.shields.io/badge/License-MIT-purple.svg)](LICENSE)

The source code we develop at **MinApiLib** is licensed as **MIT** by default. You can read more about it [here](LICENSE).

The package icon is the <a target="_blank" href="https://icons8.com/icon/1dMXEkMIykkN/constellation">Constellation</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>.
