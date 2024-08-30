[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.FluentValidation)](https://www.nuget.org/packages/MinApiLib.FluentValidation)

# MinApiLib.FluentValidation

This package contains extensions to use validation in your endpoints. It uses the [FluentValidation](https://docs.fluentvalidation.net/en/latest/) library.


## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.FluentValidation
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.FluentValidation
```

## Usage

To use the validation, you can use the `WithValidation` extension method:

```csharp
global using MinApiLib.FluentValidation;
```

And to create a validatior for your request, you can use the `AbstractValidator` class from the `FluentValidation` namespace:

```csharp
global using FluentValidation;
```

Now configure the validation filter in your endpoint:

```csharp
public readonly record struct Request(
    [FromBody] RequestBody Body
);

public readonly record struct RequestBody(
    public string Name,
    public string City,
    public string Country
);

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body).NotNull();
        RuleFor(x => x.Body.Name).NotEmpty();
        // ...
    }
}
```

Then, in your endpoint, use the `WithValidation` or  `WithValidation` extension method:

```csharp
public record CreateThing() : PostEndpoint<Request>("things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces<Response>(StatusCodes.Status201Created)
                .WithName("CreateThings")
                .WithTags("things")
                .WithValidation(); // <------------
                // .WithValidation<Request>(); // <------------ is also valid

    protected override async Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        // async stuff
        return Results.Created($"/things/{created.Id}", created);
    }
}
```

To make the validation work you need to register your validators in the DI container a `IValidator<T>`:

```csharp
services.AddValidation();
services.AddSingleton<IValidator<Request>, RequestValidator>();
```

Or you can use the `AddValidatorsFromAssembly` extension method to register all validators in the assembly:

```csharp
services.AddValidatorsFromAssembly();
// or
services.AddValidatorsFromAssembly(typeof(RequestValidator).Assembly);
```