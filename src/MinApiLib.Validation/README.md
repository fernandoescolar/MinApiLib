[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Validation)](https://www.nuget.org/packages/MinApiLib.Validation)

# MinApiLib.Validation

> **Deprecated.** Use [MinApiLib.FluentValidation](../MinApiLib.FluentValidation/README.md) instead. This package no longer receives fixes, and its types are marked `[Obsolete]`.

## Known problems

The filter validates data annotations only on a request member named `Body`. It skips validation without any error if the member has another name, or if the body is received directly as a handler parameter. It doesn't check query, route or header values.

## Migrating

Replace the package:

```bash
dotnet remove package MinApiLib.Validation
dotnet add package MinApiLib.FluentValidation
```

Update the usings, and register the validators:

```csharp
// global using MinApiLib.Validation;
global using FluentValidation;
global using MinApiLib.FluentValidation;
// ...
builder.Services.AddValidatorsFromAssembly();
```

Move each annotation to a validator for the request. `WithValidation()` stays the same in the endpoint.

```csharp
// Before
public record struct Request([FromBody] RequestBody Body);

public class RequestBody
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }
}

// After
public record struct Request([FromBody] RequestBody Body);

public class RequestBody
{
    public string Name { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 150);
    }
}
```

The `400` response keeps the same shape, with the errors per property, but the keys include the path of the property, such as `Body.Name`. An endpoint that calls `WithValidation()` needs a validator: without one, its requests aren't validated, and a warning is logged.
