namespace Example.Simple.Api.Breweries;

public record Delete() : DeleteEndpoint("/breweries/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("DeleteBrewery")
                .WithTags("Breweries");

    public async Task<IResult> HandleAsync(BeerDbContext db, HashedId id, CancellationToken cancellationToken)
    {
        var brewery = await db.Breweries.FindAsync(new object[] { (int)id }, cancellationToken);
        if (brewery is null)
        {
            return Results.NotFound();
        }

        db.Breweries.Remove(brewery);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
