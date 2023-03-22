namespace Example.VerticalSlice.Api.Features.DeleteBeer;

public record Handler() : DeleteHandlerAsync<Request>("/beers/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("DeleteBeer")
                .WithTags("Beers");

    protected override async Task<IResult> HandleAsync(Request req, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var beer = await req.Database.Beers.FindAsync(new object[] { (int)req.Id }, cancellationToken);
        if (beer is null)
        {
            return Results.NotFound();
        }

        req.Database.Beers.Remove(beer);
        await req.Database.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
