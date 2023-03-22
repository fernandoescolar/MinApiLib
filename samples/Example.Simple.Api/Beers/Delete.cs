namespace Example.Simple.Api.Beers;

public record DeleteBeer() : Delete("/beers/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName(nameof(DeleteBeer))
                .WithTags("Beers");

    public async Task<IResult> HandleAsync(BeerDbContext db, HashedId id, CancellationToken cancellationToken)
    {
        var beer = await db.Beers.FindAsync(new object[] { (int)id }, cancellationToken);
        if (beer is null)
        {
            return Results.NotFound();
        }

        db.Beers.Remove(beer);
        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
