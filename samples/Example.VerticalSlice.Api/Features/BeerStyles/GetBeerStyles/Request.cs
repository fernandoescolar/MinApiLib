namespace Example.VerticalSlice.Api.Features.GetBeerStyles;

public record struct Request(
    [FromServices] BeerDbContext Database
);
