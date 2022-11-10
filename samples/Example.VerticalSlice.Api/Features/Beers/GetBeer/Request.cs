namespace Example.VerticalSlice.Api.Features.GetBeer;

public record struct Request(
    [FromServices]BeerDbContext Database,
    [FromRoute] HashedId Id
);
