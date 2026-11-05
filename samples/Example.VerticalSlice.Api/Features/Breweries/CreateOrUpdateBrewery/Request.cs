namespace Example.VerticalSlice.Api.Features.CreateOrUpdateBrewery;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromBody] RequestBody Body,
    [FromRoute] HashedId Id
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
