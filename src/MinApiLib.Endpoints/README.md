[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints)](https://www.nuget.org/packages/MinApiLib.Endpoints)

# MinApiLib.Endpoints

Define Minimal API endpoints as records, one per endpoint, and map them by scanning an assembly.

- [Installation](#installation)
- [Usage](#usage)
- [Choosing a base record](#choosing-a-base-record)
- [Handlers](#handlers)
- [Naming convention endpoints](#naming-convention-endpoints)
- [Mapping](#mapping)
- [The QUERY verb](#the-query-verb)

## Installation

```bash
dotnet add package MinApiLib.Endpoints
```

## Usage

Map every endpoint in the assembly:

```csharp
global using MinApiLib.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();
app.Run();
```

Then create endpoints:

```csharp
public record Hello() : GetHandler("/hello")
{
    protected override IResult Handle()
        => Results.Ok("Hello World!");
}
```

```bash
$ curl "http://localhost:5000/hello"
"Hello World!"
```

## Choosing a base record

There is one base record per verb: `Get`, `Post`, `Put`, `Delete`, `Patch` and `Query`.

| Family | Example | Implement |
|---|---|---|
| Asynchronous handler | `GetHandlerAsync`, `GetHandlerAsync<TRequest>`, `GetHandlerAsync<TRequest, TResponse>` | `HandleAsync`, checked by the compiler |
| Synchronous handler | `GetHandler`, `GetHandler<TRequest>`, `GetHandler<TRequest, TResponse>` | `Handle`, checked by the compiler |
| Naming convention | `Get` | A public `Handle` or `HandleAsync`, found by reflection at startup |

Prefer the handler families. `TRequest` groups the inputs in one type. Without `TResponse` the handler returns `IResult`, so it can answer with any status code; with it, the handler can only answer 200.

## Handlers

```csharp
public record struct Request([FromRoute] string Name);
public record struct Response(string Message);

public record Hello() : GetHandlerAsync<Request>("/hello/{name}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces<Response>(StatusCodes.Status200OK)
                .WithName("Hello")
                .WithTags("Greetings");

    protected override async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        return Results.Ok(new Response($"Hello {request.Name}!"));
    }
}
```

- `Configure` is optional. It receives the `RouteHandlerBuilder` to add metadata and filters.
- `TRequest` is bound with `[AsParameters]`. Its members accept the usual attributes: `[FromRoute]`, `[FromQuery]`, `[FromHeader]`, `[FromBody]` and `[FromServices]`.
- `HandleAsync` always receives the request's `CancellationToken`.

With `TResponse`, return the value directly:

```csharp
public record Hello() : GetHandlerAsync<Request, Response>("/hello/{name}")
{
    protected override Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response($"Hello {request.Name}!"));
}
```

To answer several verbs, derive from `EndpointHandler`, `EndpointHandler<TRequest>` or their `Async` versions, and pass an array:

```csharp
public record Hello() : EndpointHandler(new[] { "GET", "POST" }, "/hello")
{
    protected override IResult Handle()
        => Results.Ok("Hello World!");
}
```

## Naming convention endpoints

These records look for a public method named `Handle` or `HandleAsync`. It binds its parameters like a Minimal API delegate, and can return anything a delegate can:

```csharp
public record Hello() : Get("/hello/{name}")
{
    public IResult Handle(string name, [FromHeader(Name = "User-Agent")] string userAgent, CancellationToken cancellationToken)
        => Results.Ok($"Hello {name}! Your user agent is {userAgent}.");
}
```

Declare exactly one such method per record: with none, or with overloads, mapping the endpoint throws an `InvalidOperationException` that names the record. Override `Configure` to add metadata, as in the handlers.

## Mapping

`MapEndpoints()` maps every class implementing `IEndpoint` in the calling assembly, skipping abstract classes and open generics, inside a group for the root path. It returns that `RouteGroupBuilder`, so you can apply filters to all the endpoints:

```csharp
app.MapEndpoints()
   .WithHypermedia();
```

If the endpoints are in another assembly, or `MapEndpoints()` is called from a helper in another project, pass the assembly:

```csharp
app.MapEndpoints(typeof(Hello).Assembly);
```

To map a single endpoint instead, which needs a public parameterless constructor:

```csharp
app.MapEndpoint<Hello>();
```

**Each endpoint is created once, at startup.** `MapEndpoints()` can inject constructor dependencies, but the instance lives for the whole application, so it must not hold scoped services such as a `DbContext`. Receive those per request instead:

```csharp
public record struct Request([FromServices] AppDbContext Database, [FromRoute] int Id);
```

## The QUERY verb

`Query`, `QueryHandler` and `QueryHandlerAsync` map the HTTP `QUERY` method, for safe reads that carry a body. Mark the body explicitly with `[FromBody]`.

Swashbuckle doesn't support this verb: with version 10, a single `QUERY` endpoint makes the whole `swagger.json` fail. Hide those endpoints from the API description:

```csharp
protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
    => builder.ExcludeFromDescription();
```
