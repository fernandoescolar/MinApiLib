namespace Example.VerticalSlice.Api.Features.GetBrewery;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromRoute] HashedId Id
);
