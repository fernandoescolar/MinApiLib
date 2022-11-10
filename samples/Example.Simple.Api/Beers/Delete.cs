namespace Example.Simple.Api.Beers;

public record Delete() : DeleteEndpoint("/beers/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("DeleteBeer")
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
