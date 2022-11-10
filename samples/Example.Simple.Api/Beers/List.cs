namespace Example.Simple.Api.Beers;

public record List() : GetEndpoint("/beers")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BeerList>(StatusCodes.Status200OK)
                .Produces<BeerList>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("GetBeers")
                .WithTags("Beers");

    public async Task<IResult> HandleAsync(BeerDbContext _db, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var total = await _db.Beers.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var beers = await _db.Beers
                        .Select(b => new BeerListItem
                        {
                            Id = b.Id,
                            Name = b.Name,
                            BreweryId = b.Brewery.Id,
                            BreweryName = b.Brewery.Name,
                            StyleId = b.Style.Id,
                            StyleName = b.Style.Name,
                        })
                        .OrderBy(b => b.Name)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken);

        var resourceList = new BeerList(beers, total, page, pageSize);

        return Results.Ok(resourceList);
    }
}
