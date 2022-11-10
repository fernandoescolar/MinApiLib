namespace Example.VerticalSlice.Api.Features.DeleteBeer;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromRoute] HashedId Id
);
