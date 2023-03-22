namespace Example.VerticalSlice.Api.Features.GetBeer;

public record Handler() : GetHandlerAsync<Request>("/beers/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
                .ProducesHypermedia<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("GetBeer")
                .WithTags("Beers");

    protected override async Task<IResult> HandleAsync(Request query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var beer = await query.Database.Beers
                                       .Include(nameof(Beer.Brewery))
                                       .Include(nameof(Beer.Style))
                                       .FirstOrDefaultAsync(b => b.Id == (int)query.Id, cancellationToken);

        if (beer is null)
        {
            return Results.NotFound();
        }

        var resource = (Response)beer;

        return Results.Ok(resource);
    }
}
