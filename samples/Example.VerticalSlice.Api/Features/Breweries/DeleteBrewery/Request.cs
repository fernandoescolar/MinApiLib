namespace Example.VerticalSlice.Api.Features.DeleteBrewery;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromRoute, Required] HashedId Id
);