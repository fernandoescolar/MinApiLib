namespace Example.VerticalSlice.Api.Features.DeleteBrewery;

public record Handler() : DeleteEndpoint<Request>("/breweries/{id}")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("DeleteBrewery")
                .WithTags("Breweries");

    protected override async Task<IResult> OnHandleAsync(Request req, CancellationToken cancellationToken)
    {
        var brewery = await req.Database.Breweries.FindAsync(new object[] { (int)req.Id }, cancellationToken);
        if (brewery is null)
        {
            return Results.NotFound();
        }

        req.Database.Breweries.Remove(brewery);
        await req.Database.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
