using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Validation;

namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Things3;

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

public record CreateThing() : PostAsync<Request>("things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("CreateThings")
                .WithTags("things")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        // async stuff
        return Results.Created($"/things/{1}", request);
    }
}