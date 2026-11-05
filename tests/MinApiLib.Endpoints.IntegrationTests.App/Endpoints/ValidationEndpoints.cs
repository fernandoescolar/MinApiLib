using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MinApiLib.Endpoints.IntegrationTests.App.Endpoints.Things3;

public record struct Request(
    [FromBody] RequestBody Body
);

public class RequestBody
{
    public string Name { get; set; }

    public string City { get; set; }

    public string Country { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 150);
        RuleFor(x => x.Body.City).NotEmpty();
        RuleFor(x => x.Body.Country).NotEmpty();
    }
}

public record CreateThing() : PostHandlerAsync<Request>("things")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces<RequestBody>(StatusCodes.Status201Created)
                .WithName("CreateThings")
                .WithTags("things")
                .WithValidation();

    protected override Task<IResult> HandleAsync(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Results.Created($"/things/{1}", request.Body));
    }
}
