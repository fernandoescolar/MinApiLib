namespace Example.Simple.Api.Beers;

public record GetBeers() : Get("/beers")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BeerList>(StatusCodes.Status200OK)
                .Produces<BeerList>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName(nameof(GetBeers))
                .WithTags("Beers");

    public async Task<IResult> HandleAsync(BeerDbContext _db, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return Results.BadRequest("page must be >= 1 and pageSize between 1 and 100");
        }

        var total = await _db.Beers.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var beers = await _db.Beers
                        .OrderBy(b => b.Name)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(b => new BeerListItem
                        {
                            Id = b.Id,
                            Name = b.Name,
                            BreweryId = b.Brewery.Id,
                            BreweryName = b.Brewery.Name,
                            StyleId = b.Style.Id,
                            StyleName = b.Style.Name,
                        })
                        .ToListAsync(cancellationToken);

        var resourceList = new BeerList(beers, total, page, pageSize);

        return Results.Ok(resourceList);
    }
}
