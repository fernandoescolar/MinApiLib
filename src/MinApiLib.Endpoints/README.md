# MinApiLib.Endpoints

[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints)](https://www.nuget.org/packages/MinApiLib.Endpoints)


This package contains extensions to create endpoints using records and one file per endpoint.

## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.Endpoints
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.Endpoints
```

## Usage

To create endpoints, you can use the `MapEndpoints` extension method to map all endpoints in the assembly:

```csharp
global using MinApiLib.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();
app.Run();
```

By default, it wil map the classes in the current executing assembly, but you can change this behavior by adding the `Assembly` parameter:

```csharp
app.MapEndpoints(typeof(SomeObject).Assembly);
```

Then, you can create endpoints:

```csharp
public record Hello() : Get("/hello")
{
    protected override IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}
```

And test it with curl:

```bash
$ curl "http://localhost:5000/hello"
"Hello World!"%
```

This endpoint accepts asynchrony and cancellation:

```csharp
public record Hello() : GetAsync("/hello")
{
    protected override async Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        return Results.Ok("Hello World!");
    }
}
```

This endpoint accepts a request object:

```csharp
public record struct Request(string Name);

public record Hello() : GetAsync<Request>("/hello")
{
    protected override async Task<IResult> HandleAsync(Request request, CancellationToken CancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        return Results.Ok($"Hello {request.Name}!");
    }
}
```

You can test it with curl:

```bash
$ curl "http://localhost:5000/hello?name=fer"
"Hello fer!"%
```

And you can decorate the request object with attributes:

```csharp
public record struct Request([FromQuery] string Name);
```

This endpoint accepts also a response object:

```csharp
public record struct Request(string Name, CancellationToken CancellationToken);

public record struct Response(string Message);

public record Hello() : GetAsync<Request, Response>("/hello")
{
    protected override async Task<Response> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        var response = new Response($"Hello {request.Name}!");
        return response;
    }
}
```

You can test it with curl:

```bash
$ curl "http://localhost:5000/hello?name=fer"
{"message":"Hello fer!"}%
```

And you can configure any endpoint overriding the `Configure` method:

```csharp
public record Hello() : GetEndpoint("/hello")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    protected override IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}
```


Here is a list of all the available endpoints:

- `Get`
- `Get<TRequest>`
- `Get<TRequest, TResponse>`
- `GetAsync`
- `GetAsync<TRequest>`
- `GetAsync<TRequest, TResponse>`
- `Post`
- `Post<TRequest>`
- `Post<TRequest, TResponse>`
- `PostAsync`
- `PostAsync<TRequest>`
- `PostAsync<TRequest, TResponse>`
- `Put`
- `Put<TRequest>`
- `Put<TRequest, TResponse>`
- `PutAsync`
- `PutAsync<TRequest>`
- `PutAsync<TRequest, TResponse>`
- `Delete`
- `Delete<TRequest>`
- `Delete<TRequest, TResponse>`
- `DeleteAsync`
- `DeleteAsync<TRequest>`
- `DeleteAsync<TRequest, TResponse>`
- `Patch`
- `Patch<TRequest>`
- `Patch<TRequest, TResponse>`
- `PatchAsync`
- `PatchAsync<TRequest>`
- `PatchAsync<TRequest, TResponse>`