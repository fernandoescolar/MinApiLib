[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints)](https://www.nuget.org/packages/MinApiLib.Endpoints)

# MinApiLib.Endpoints

This package provides extensions that enable creating endpoints using records and one file per endpoint.

Table of conents:
- [MinApiLib.Endpoints](#minapilibendpoints)
  - [Installation](#installation)
  - [Usage](#usage)
  - [Endpoints](#endpoints)
    - [Naming convention endpoints](#naming-convention-endpoints)
    - [Synchronous handlers](#synchronous-handlers)
    - [Asynchronous handlers](#asynchronous-handlers)


## Installation

You can install this package using either the NuGet package manager or the .NET CLI:

Using the NuGet package manager:

```bash
Install-Package MinApiLib.Endpoints
```

Using the .NET CLI:

```bash
dotnet add package MinApiLib.Endpoints
```

## Usage

To create endpoints, you can use the MapEndpoints extension method to map all endpoints in the assembly.

```csharp
global using MinApiLib.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();
app.Run();
```

By default, the method maps the classes in the current executing assembly, but you can change this behavior by adding the Assembly parameter.:

```csharp
app.MapEndpoints(typeof(SomeObject).Assembly);
```

Then, you can create endpoints:

```csharp
public record Hello() : Get("/hello")
{
    public IResult Handle()
        => Results.Ok("Hello World!");
}
```

And test it with curl:

```bash
$ curl "http://localhost:5000/hello"
"Hello World!"%
```

## Endpoints

### Naming convention endpoints

If you want to use the naming convention to create endpoints, you can use the following records:

- `Get`
- `Post`
- `Put`
- `Delete`
- `Patch`

This endpoints are based on the `Endpoint` class, which provides the following methods:

- `Configure`: this virtual method is called when the endpoint is configured. You can override it to add your custom configuration.

These endpoints will look for a method named `Handle` or `HandleAsync` in the record to use it as the endpoint handler.

```csharp
public record Hello(): Get("/hello")
{
    public IResult Handle()
        => Results.Ok("Hello World!");
}
```

or:

```csharp
public record Hello(): Get("/hello")
{
    public Task<IResult> HandleAsync()
        => Task.FromResult(Results.Ok("Hello World!"));
}
```

You can specify the parameters and bind them in the same way you do in the minimal API mapping methods:

```csharp
public record Hello(): Get("/hello/{name}")
{
    public IResult Handle(string name)
        => Results.Ok($"Hello {name}!");
}
```

This is the same as:

```csharp
public record Hello(): Get("/hello/{name}")
{
    public IResult Handle([FromRoute] string name)
        => Results.Ok($"Hello {name}!");
}
```

You can use all attribute decorators to specify the `Handle` method parameters:

```csharp
public record Hello(): Get("/hello")
{
    public IResult Handle([FromQuery] string name, [FromHeader] string userAgent, [FromServices] CancellationToken cancellationToken)
        => Results.Ok($"Hello {name}! Your user agent is {userAgent}.");
}
```

And you can also specify the response type:

```csharp
public record struct Response(string Message);

public record Hello(): Get("/hello/{name}")
{
    public Response Handle(string name)
        => new Response($"Hello {name}!");
}
```

### Synchronous handlers

For synchronous strong typed handlers, you can use the following records:

- `GetHandler`, `GetHandler<TRequest>` and `GetHandler<TRequest, TResponse>`
- `PostHandler`, `PostHandler<TRequest>` and `PostHandler<TRequest, TResponse>`
- `PutHandler`, `PutHandler<TRequest>` and `PutHandler<TRequest, TResponse>`
- `DeleteHandler`, `DeleteHandler<TRequest>` and `DeleteHandler<TRequest, TResponse>`
- `PatchHandler`, `PatchHandler<TRequest>` and `PatchHandler<TRequest, TResponse>`

These endpoints are based on the `EndpointHandler` class, which provides the following methods:

- `Configure`: this virtual method is called when the endpoint is configured. You can override it to add your custom configuration.
- `Handle`: this abstract method is called when the endpoint is invoked. You must override it to implement your custom logic.

```csharp
public record Hello() : GetHandler("/hello")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    protected override IResult Handle()
        => Results.Ok("Hello World!");
}
```

If you specify the `TRequest` generic type, the endpoint will automatically bind the request to the specified type using `AsParametersAttribute`.

```csharp
public record struct Request(string Name);

public record Hello() : GetHandler<Request>("/hello/{name}")
{
    protected override IResult Handle(Request request)
        => Results.Ok($"Hello {request.Name}!");
}
```

This is the same as:

```csharp
public record struct Request([FromRoute] string Name);

public record Hello() : GetHandler<Request>("/hello/{name}")
{
    protected override IResult Handle(Request request)
        => Results.Ok($"Hello {request.Name}!");
}
```

In the `Request` record you can specify the parameter name using the `FromRouteAttribute`, `FromQueryAttribute`, `FromHeaderAttribute`, `FromBodyAttribute` or `FromServicesAttribute` attributes.

And if you specify the `TResponse` generic type, the endpoint will automatically return the response to the specified type.

```csharp
public record struct Request(string Name);

public record struct Response(string Message);

public record Hello() : GetHandler<Request, Response>("/hello/{name}")
{
    protected override Response Handle(Request request)
        => new Response($"Hello {request.Name}!");
}
```

### Asynchronous handlers

For asynchronous strong typed handlers, you can use the following records:

- `GetHandlerAsync`, `GetHandlerAsync<TRequest>` and `GetHandlerAsync<TRequest, TResponse>`
- `PostHandlerAsync`, `PostHandlerAsync<TRequest>` and `PostHandlerAsync<TRequest, TResponse>`
- `PutHandlerAsync`, `PutHandlerAsync<TRequest>` and `PutHandlerAsync<TRequest, TResponse>`
- `DeleteHandlerAsync`, `DeleteHandlerAsync<TRequest>` and `DeleteHandlerAsync<TRequest, TResponse>`
- `PatchHandlerAsync`, `PatchHandlerAsync<TRequest>` and `PatchHandlerAsync<TRequest, TResponse>`

These endpoints are based on the `EndpointHandlerAsync` class, which provides the following methods:

- `Configure`: this virtual method is called when the endpoint is configured. You can override it to add your custom configuration.
- `HandleAsync`: this abstract method is called when the endpoint is invoked. You must override it to implement your custom logic.

```csharp
public record Hello() : GetHandlerAsync("/hello")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    protected override async Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return Results.Ok("Hello World!");
    }
}
```

If you specify the `TRequest` generic type, the endpoint will automatically bind the request to the specified type using `AsParametersAttribute`.

```csharp
public record struct Request(string Name);

public record Hello() : GetHandlerAsync<Request>("/hello/{name}")
{
    protected override async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return Results.Ok($"Hello {request.Name}!");
    }
}
```

This is the same as:

```csharp
public record struct Request([FromRoute] string Name);

public record Hello() : GetHandlerAsync<Request>("/hello/{name}")
{
    protected override async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return Results.Ok($"Hello {request.Name}!");
    }
}
```

In the `Request` record you can specify the parameter name using the `FromRouteAttribute`, `FromQueryAttribute`, `FromHeaderAttribute`, `FromBodyAttribute` or `FromServicesAttribute` attributes.

And if you specify the `TResponse` generic type, the endpoint will automatically return the response to the specified type.

```csharp
public record struct Request(string Name);

public record struct Response(string Message);

public record Hello() : GetHandlerAsync<Request, Response>("/hello/{name}")
{
    protected override async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        return new Response($"Hello {request.Name}!");
    }
}
```
