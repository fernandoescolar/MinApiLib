namespace Example.VerticalSlice.Api.Features.GetBeersByBrewery;

public record Handler() : GetHandlerAsync<Request>("/breweries/{id}/beers")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetBeersByBrewery")
                .WithTags("Beers")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var brewery = await req.Database.Breweries.FindAsync(new object[] { (int)req.BreweryId }, cancellationToken);
        if (brewery is null)
        {
            return Results.NotFound();
        }

        var total = await req.Database.Beers.CountAsync(b => b.Brewery.Id == (int)req.BreweryId, cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var beers = await req.Database
                                .Beers
                                .Where(b => b.Brewery.Id == (int)req.BreweryId)
                                .OrderBy(b => b.Name)
                                .Skip((req.Page - 1) * req.PageSize)
                                .Take(req.PageSize)
                                .Select(b => new ResponseItem
                                {
                                    Id = b.Id,
                                    Name = b.Name,
                                    BreweryId = b.Brewery.Id,
                                    BreweryName = b.Brewery.Name,
                                    StyleId = b.Style.Id,
                                    StyleName = b.Style.Name,
                                })
                                .ToListAsync(cancellationToken);

        var resourceList = new Response(beers, total, req.Page, req.PageSize) { BreweryId = req.BreweryId };

        return Results.Ok(resourceList);
    }
}
