---
name: minapilib-validation
description: Add input validation to a MinApiLib vertical slice with FluentValidation (MinApiLib.FluentValidation), or migrate from the deprecated MinApiLib.Validation. Use when a slice accepts a body, query or route input that must be checked, or when validation seems to be ignored.
---

# Validate a slice

Use `MinApiLib.FluentValidation`. `MinApiLib.Validation` is deprecated: its README explains how to migrate a project that still uses it.

## Setup, once per project

```csharp
// _Imports.cs
global using FluentValidation;
global using MinApiLib.FluentValidation;

// Program.cs
builder.Services.AddValidatorsFromAssembly();
```

## In the slice

```csharp
// Request.cs
public record struct Request(
    [FromServices] AppDbContext Database,
    [FromBody] RequestBody Body,
    [FromQuery] int Page = 1
);

public class RequestBody
{
    public string Name { get; set; }

    public required HashedId OwnerId { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 100);
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
    }
}

// Handler.cs, in Configure
.WithValidation();
```

An invalid request gets a 400 with a `{ "Body.Name": ["message"] }` dictionary, and `.WithValidation()` also declares that 400 in OpenAPI.

## Rules

- **The validator targets `Request`, the type the handler receives.** `WithValidation()` looks for a validator among the handler's parameters. If it finds none, requests pass unchecked and it only logs a warning, which is what an `AbstractValidator<RequestBody>` causes. That usage is obsolete: if you see the warning, fix the validator.
- **Every slice that receives input validates it,** including paging values in query strings: `RuleFor(x => x.PageSize).InclusiveBetween(1, 100)`.
- **`HashedId` in the body:** an invalid value is already rejected with a 400. Mark the property `required` to reject a missing one too; otherwise it arrives as `0`.
- **Shape goes in the validator, business rules in the handler.** Anything that needs the database, such as existence or uniqueness, goes in `HandleAsync`, so a valid hash pointing to a missing entity returns the error there.
- Declare any extra 400 the handler returns with `.Produces(StatusCodes.Status400BadRequest)`.
- Calling `.WithValidation()` on a group validates only the endpoints with a validator, and skips the others silently. Prefer calling it on each slice.

## Verify

Add a test that sends invalid input and expects 400 (see `minapilib-tests`). This is what catches a missing validator.
