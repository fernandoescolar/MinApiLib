namespace Example.VerticalSlice.Api.Features.CreateOrUpdateBeer;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromBody] RequestBody Body,
    [FromRoute] HashedId Id
);

public class RequestBody
{
    public string Name { get; set; }

    public required HashedId BreweryId { get; set; }

    public required HashedId StyleId { get; set; }
}

public class RequestValidator : AbstractValidator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Body.Name).NotEmpty().Length(3, 100);
    }
}
