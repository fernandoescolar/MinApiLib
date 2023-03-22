[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Validation)](https://www.nuget.org/packages/MinApiLib.Validation)

# MinApiLib.Validation

This package contains extensions to use data annotations in your endpoints. It uses the [MiniValidation](https://github.com/DamianEdwards/MiniValidation) library.


## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.Validation
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.Validation
```

## Usage

To use the validation, you can use the `WithValidation` extension method:

```csharp
global using MinApiLib.Validation;
```

Now configure the validation filter in your endpoint:

```csharp
public record struct Request(
    [FromBody] RequestBody Body
);

public class RequestBody
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }
}

public record CreateThing() : PostEndpoint<Request>("things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces<Response>(StatusCodes.Status201Created)
                .WithName("CreateThings")
                .WithTags("things")
                .WithValidation(); // invokes .ProducesValidationError()

    protected override async Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        // async stuff
        return Results.Created($"/things/{created.Id}", created);
    }
}
```

It will validate the request body and return a 400 status code response if the validation fails:

```bash
$ curl -X POST -H "Content-Type: application/json" -d '{"name":"John Doe"}' "http://localhost:5000/things"
{"City":["The City field is required."],"Country":["The Country field is required."]}%
```