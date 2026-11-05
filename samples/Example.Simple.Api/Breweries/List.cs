namespace Example.Simple.Api.Breweries;

public record GetBreweries() : Get("/breweries")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BreweryList>(StatusCodes.Status200OK)
                .Produces<BreweryList>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName(nameof(GetBreweries))
                .WithTags("Breweries");

    public async Task<IResult> HandleAsync(BeerDbContext _db, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return Results.BadRequest("page must be >= 1 and pageSize between 1 and 100");
        }

        var total = await _db.Breweries.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var breweries = await _db.Breweries
                             .OrderBy(b => b.Name)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .Select(b => new BreweryListItem
                                 {
                                     Id = b.Id,
                                     Name = b.Name,
                                     City = b.City,
                                     Country = b.Country,
                                 })
                             .ToListAsync(cancellationToken);

        var resourceList = new BreweryList(breweries, total, page, pageSize);

        return Results.Ok(resourceList);
    }
}
