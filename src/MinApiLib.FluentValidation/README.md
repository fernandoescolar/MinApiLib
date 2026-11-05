[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.FluentValidation)](https://www.nuget.org/packages/MinApiLib.FluentValidation)

# MinApiLib.FluentValidation

Validate requests with the [FluentValidation](https://docs.fluentvalidation.net/en/latest/) library, including body, query, route and header values.

## Installation

```bash
dotnet add package MinApiLib.FluentValidation
```

## Usage

```csharp
global using FluentValidation;
global using MinApiLib.FluentValidation;
```

Register the validators of the assembly as transient services. This also registers the filter:

```csharp
builder.Services.AddValidatorsFromAssembly();
// or
builder.Services.AddValidatorsFromAssembly(typeof(RequestValidator).Assembly);
```

Write a validator for the **request type** and call `WithValidation()` in the endpoint:

```csharp
public record struct Request(
    [FromBody] RequestBody Body,
    [FromQuery] int Page = 1
);

public class RequestBody
{
    public string Name { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 100);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
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

An invalid request gets a `400` with the errors per property, and the handler doesn't run. `WithValidation()` also declares that `400` in the endpoint metadata.

## What gets validated

| Call | Validates | If there is no validator |
|---|---|---|
| `WithValidation()` on an endpoint | The first handler parameter with a registered `IValidator<T>` | The endpoint isn't validated, and a warning is logged |
| `WithValidation<T>()` on an endpoint | The handler parameter of type `T` | The endpoint isn't validated, and a warning is logged |
| `WithValidation()` on a group | In each endpoint, the first parameter with a validator | The endpoint isn't validated |

- **Calling `WithValidation()` on an endpoint without a validator is obsolete,** and it will throw in a future version. The warning is logged when the endpoint is built, on the first request at the latest. Check your logs for it, and cover each validation with a test.
- **The validator must target the type the handler receives.** With `[AsParameters]` handlers, that's `Request`: an `AbstractValidator<RequestBody>` isn't associated with them.
- If the filter isn't registered, `WithValidation()` throws an `InvalidOperationException`, with or without a validator.
- The group overload doesn't declare the `400` in the metadata, because it applies to endpoints that may not have a validator.
- Validators receive the request's `CancellationToken`.

## Manual registration

To register validators one by one, add the filter and your validators:

```csharp
builder.Services.AddValidationFilter();
builder.Services.AddSingleton<IValidator<Request>, RequestValidator>();
```

`AddValidationFilter` accepts the lifetime of the filter, which is `Singleton` by default. Validators are resolved from the request scope, so they can be scoped.

If the project references `FluentValidation.DependencyInjectionExtensions`, its `AddValidatorsFromAssembly` has the same name as the one in this package. Use only one of them per file, or call the method through its class.

## Upgrading

`AddValidation()` is obsolete: use `AddValidationFilter()`. ASP.NET Core 10 has its own `AddValidation()`, and calls to it were ambiguous. So on `net10.0`, the obsolete method requires the lifetime, as in `AddValidation(ServiceLifetime.Singleton)`, and `AddValidation()` without arguments is the one from ASP.NET Core, which doesn't register this filter.
