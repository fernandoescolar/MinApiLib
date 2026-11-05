namespace Example.VerticalSlice.Api.Features.GetBeers;

public record Handler() : GetHandlerAsync<Request>("/beers")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("GetBeers")
                .WithTags("Beers")
                .WithValidation();

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var total = await req.Database.Beers.CountAsync(cancellationToken);
        if (total == 0)
        {
            return Results.NoContent();
        }

        var beers = await req.Database
                                .Beers
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

        var resourceList = new Response(beers, total, req.Page, req.PageSize);

        return Results.Ok(resourceList);
    }
}
