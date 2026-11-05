[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints.Aot)](https://www.nuget.org/packages/MinApiLib.Endpoints.Aot)

# MinApiLib.Endpoints.Aot

Source generator for [MinApiLib.Endpoints](../MinApiLib.Endpoints/README.md). It generates a `MapAotEndpoints()` method that maps each endpoint explicitly, so the application doesn't need to scan the assembly with reflection.

## Installation

The generator doesn't bring `MinApiLib.Endpoints` with it, so install both:

```bash
dotnet add package MinApiLib.Endpoints
dotnet add package MinApiLib.Endpoints.Aot
```

It works with the .NET 8 SDK and later.

## Usage

Replace `MapEndpoints()` with the generated method. It lives in the `MinApiLib.Endpoints` namespace:

```csharp
using MinApiLib.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// app.MapEndpoints();
app.MapAotEndpoints();

app.Run();
```

Like `MapEndpoints()`, it returns a `RouteGroupBuilder` to apply filters to all the endpoints.

## What gets mapped

The classes and records of the project implementing `IEndpoint`, including internal and nested ones. It skips:

- Abstract and generic types.
- Types without a public parameterless constructor. Unlike `MapEndpoints()`, this method can't inject constructor dependencies, so an endpoint that declares them is **not mapped**.
- Types that the generated code can't access, such as private nested types.

## Limitations

The generator only replaces the assembly scan. Endpoints with the naming convention (`Get`, `Post`...) still use reflection, as do `AddHypermedia()`, `AddAssembly()`, `AddValidatorsFromAssembly()` and the validation filters. Publishing a full application with native AOT has not been validated.
