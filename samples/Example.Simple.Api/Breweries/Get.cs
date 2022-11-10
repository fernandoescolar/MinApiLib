namespace Example.Simple.Api.Breweries;

public record Get() : GetEndpoint("/breweries/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<BreweryDetail>(StatusCodes.Status200OK)
                .Produces<BreweryDetail>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetBrewery")
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
