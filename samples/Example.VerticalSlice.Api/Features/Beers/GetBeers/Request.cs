namespace Example.VerticalSlice.Api.Features.GetBeers;

public record struct Request(
    [FromServices] BeerDbContext Database,
    [FromQuery] int Page = 1,
    [FromQuery] int PageSize = 10
);
