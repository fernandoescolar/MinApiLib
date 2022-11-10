namespace Example.Simple.Api.Beers;

public record Get() : GetEndpoint("/beers/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BeerDetail>(StatusCodes.Status200OK)
                .Produces<BeerDetail>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetBeer")
                .WithTags("Beers");

    public async Task<IResult> HandleAsync(BeerDbContext db, HashedId id, CancellationToken cancellationToken)
    {
        var beer = await db.Beers
                            .Include(nameof(Beer.Brewery))
                            .Include(nameof(Beer.Style))
                            .FirstOrDefaultAsync(b => b.Id == (int)id, cancellationToken);

        if (beer is null)
        {
            return Results.NotFound();
        }

        var resource = (BeerDetail)beer;

        return Results.Ok(resource);
    }
}
