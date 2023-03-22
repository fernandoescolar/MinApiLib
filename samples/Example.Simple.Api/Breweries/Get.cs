namespace Example.Simple.Api.Breweries;

public record GetBrewery() : Get("/breweries/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BreweryDetail>(StatusCodes.Status200OK)
                .Produces<BreweryDetail>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName(nameof(GetBrewery))
                .WithTags("Breweries");

    public async Task<IResult> HandleAsync(BeerDbContext db, HashedId id, CancellationToken cancellationToken)
    {
        var brewery = await db.Breweries.FindAsync(new object[] { (int)id }, cancellationToken);
        if (brewery is null)
        {
            return Results.NotFound();
        }

        var resource = (BreweryDetail)brewery;

        return Results.Ok(resource);
    }
}
